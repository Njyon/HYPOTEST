using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;

public class GameCharacterMovementComponent : MonoBehaviour
{
	public delegate void OnMoveCollisionFlag(CollisionFlags collisionFlag);
	public OnMoveCollisionFlag onMoveCollisionFlag;
	public delegate void OnCharacterGroundedChanged(bool newState);
	public OnCharacterGroundedChanged onCharacterGroundedChanged;

	[SerializeField] float stepHight = 0.5f;
	[SerializeField] float maxWalkableSlopAngle = 45f;
	[SerializeField] float headBounceValue = -2f;
	[SerializeField] float capsulInterpolationSpeed = 5f;
	NullableHit predictedLandingPoint;
	NullableHit rayCastGroundHit;
	CharacterController unityMovementController;
	bool isGrounded = false;
	Vector3 postionLastFrame;
	Vector3 veloctiy;
	Vector3 movementVelocity;
	GameCharacter gameCharacter;
	Vector3 rootmotionVector;
	bool isInJump = false;
	LayerMask defaultLayerMask;
	LayerMask moveThroughCharacterLayerMask;
	int characterLayerIndex;
	float characterDefaultRadius;
	float characterDefaultHeight;
	bool useGravity = true;

	CapsuleCollider capsuleCollider;
	public CapsuleCollider CapsuleCollider { get { return capsuleCollider; } }
	public float StepHeight { get { return stepHight; } }
	public Vector3 CharacterCenter { get { return transform.position + capsuleCollider.center; } }
	public NullableHit PossibleGround { get { return predictedLandingPoint; } set { predictedLandingPoint = value; } }
	public NullableHit RayCastGroundHit { get { return rayCastGroundHit; } set { rayCastGroundHit = value; } }
	public bool IsGrounded { get { return isGrounded && !IsInJump; } }
	public Vector3 MovementVelocity { get { return movementVelocity; } set { movementVelocity = value; } }
	public Vector3 Velocity { get { return veloctiy; } }
	public float MovementSpeed { get { return Velocity.magnitude; } }
	public float MaxWalkableSlopAngle { get { return maxWalkableSlopAngle; } }
	public Vector3 RootmotionVector { get { return rootmotionVector; } }
	public float Height { get { return capsuleCollider.height; } }
	public float Radius { get { return capsuleCollider.radius; } }
	public float SlopeLimit { get { return unityMovementController.slopeLimit; } set { unityMovementController.slopeLimit = value; } }
	public CharacterController UnityMovementController { get { return unityMovementController; } }
	public float HeadBounceValue { get { return headBounceValue; } }
	public bool UseGravity { get { return useGravity; } set { useGravity = value; } }
	public bool IsInJump
	{
		get { return isInJump; }
		set
		{
			if (value)
			{
				StopCoroutine(IsJumping());
				StartCoroutine(IsJumping());
			}
			isInJump = value;

		}
	}
	private bool IsGroundedIntern { get { return isGrounded; } 
		set 
		{
			if (value == isGrounded) return;
			isGrounded = value;
			if (onCharacterGroundedChanged != null) onCharacterGroundedChanged(isGrounded);
		}
	}
	static float minDistance = 0.001f;

	void Awake()
	{
		capsuleCollider = GetComponent<CapsuleCollider>();
		unityMovementController = gameObject.AddComponent<CharacterController>();
		unityMovementController.height = capsuleCollider.height;
		unityMovementController.radius = capsuleCollider.radius;
		unityMovementController.center = capsuleCollider.center;
		unityMovementController.slopeLimit = maxWalkableSlopAngle;
		unityMovementController.stepOffset = stepHight;
		unityMovementController.skinWidth = 0.01f;

		characterLayerIndex = LayerMask.NameToLayer("Character");
		defaultLayerMask = unityMovementController.excludeLayers;
		moveThroughCharacterLayerMask = ExcludeLayerIsMask(defaultLayerMask, characterLayerIndex);
	
		characterDefaultRadius = capsuleCollider.radius;
		characterDefaultHeight = capsuleCollider.height;
	}

	void Update()
	{
		if (gameCharacter == null) return;
		/// Interpolate Character Capsul
		if (!Ultra.Utilities.IsNearlyEqual(capsuleCollider.radius, gameCharacter.CharacterRadiusTarget, 0.001f) || !Ultra.Utilities.IsNearlyEqual(unityMovementController.radius, gameCharacter.CharacterRadiusTarget, 0.001f))
		{
			float value = Mathf.Lerp(capsuleCollider.radius, gameCharacter.CharacterRadiusTarget, Time.deltaTime * capsulInterpolationSpeed);
			capsuleCollider.radius = value;
			unityMovementController.radius = value;
		}
		if (!Ultra.Utilities.IsNearlyEqual(capsuleCollider.height, gameCharacter.CharacterHeightTarget, 0.001f) || !Ultra.Utilities.IsNearlyEqual(unityMovementController.height, gameCharacter.CharacterHeightTarget, 0.001f))
		{
			float value = Mathf.Lerp(capsuleCollider.height, gameCharacter.CharacterHeightTarget, Time.deltaTime * capsulInterpolationSpeed);
			capsuleCollider.height = value;
			unityMovementController.height = value;
		}

		if (gameCharacter.IsPlayerCharacter) Ultra.Utilities.Instance.DebugLogOnScreen(StringColor.Lime + "Is Grounded = " + isGrounded.ToString() + StringColor.EndColor);
	}

	private void OnAnimatorMove()
	{
		if (gameCharacter?.Animator != null)
		{
			rootmotionVector = gameCharacter.Animator.deltaPosition * (1 / Time.deltaTime);
			if (gameCharacter.IsPlayerCharacter) Ultra.Utilities.Instance.DebugLogOnScreen(rootmotionVector.ToString(), 0, StringColor.Teal, 100, DebugAreas.Movement);
		
		}
	}

	public void SetupGameCharacter(GameCharacter character)
	{
		gameCharacter = character;
		gameCharacter.CharacterRadiusTarget = capsuleCollider.radius;
		gameCharacter.CharacterHeightTarget = capsuleCollider.height;
	}

	void RequestMove(Vector3 moveRequestVector)
	{

		if (moveRequestVector.magnitude < 0.0001f && gameCharacter.MovementInput.magnitude < 0.0001f) return;

		//if (!IsInJump && gameCharacter.StateMachine.GetCurrentStateType() == EGameCharacterState.Moving)
		//{
		//	unityMovementController.Move(moveRequestVector);
		//	RaycastHit checkForGroundBelowHit;
		//	if (Ultra.Utilities.CapsulCast(CharacterCenter, CapsuleCollider.height, CapsuleCollider.radius, Vector3.down * StepHeight, out checkForGroundBelowHit, Color.cyan, 200, DebugAreas.Movement))
		//	{
		//		Vector3 groundMovement = Vector3.down * StepHeight;
		//		unityMovementController.Move(groundMovement);
		//	}
		//}


		CollisionFlags collisionFlag = unityMovementController.Move(moveRequestVector);
		if (onMoveCollisionFlag != null && collisionFlag != CollisionFlags.None) onMoveCollisionFlag(collisionFlag);

		MovementVelocity = new Vector3(MovementVelocity.x, MovementVelocity.y, 0);

		Vector3 noZVector = new Vector3(transform.position.x, transform.position.y, 0);
		Vector3 noZDirection = noZVector - transform.position;	
		if (noZDirection.magnitude > 0.0001f) unityMovementController.Move(noZDirection);
	

		return;

		Vector3 currentPos = CharacterCenter;
		Vector3 requestedPosition = currentPos + moveRequestVector;
		Vector3 requestedDirection = requestedPosition - currentPos;
		Vector3 requestedPositionWithStepHight = requestedPosition + (Vector3.up * stepHight);
		Vector3 capsulCenter = CharacterCenter;

		Ultra.Utilities.DrawArrow(currentPos, requestedDirection, 10f, Color.green);

		RaycastHit setpHightHit;
		if (gameCharacter.MovementInput.magnitude > 0.01f && Ultra.Utilities.CapsulCast(requestedPositionWithStepHight, capsuleCollider.height, capsuleCollider.radius, Vector3.down * stepHight, out setpHightHit, Color.red, 100, DebugAreas.Movement))
		{
			if (gameCharacter.IsPlayerCharacter) Ultra.Utilities.Instance.DebugLogOnScreen(StringColor.Red + "StepHeigtHit" + StringColor.EndColor);
			Vector3 currentPosWithYDelta = CapsulCenterFromEnds(moveRequestVector, setpHightHit, Vector3.up);
			capsulCenter = MoveCapsul(moveRequestVector, currentPosWithYDelta);
		
			MoveTransfowm(currentPos, capsulCenter);
		}
		else
		{
			if (gameCharacter.IsPlayerCharacter) Ultra.Utilities.Instance.DebugLogOnScreen(StringColor.Green + "StepHeigtNoHit" + StringColor.EndColor);
			capsulCenter = MoveCapsul(moveRequestVector, capsulCenter);
			Ultra.Utilities.DrawCapsule(capsulCenter, Quaternion.identity, capsuleCollider.height, capsuleCollider.radius, Color.green);
			RaycastHit stepDownHit;
			if (!IsInJump && !IsGroundedCheck(capsulCenter, out stepDownHit))
			{
				if (Ultra.Utilities.CapsulCast(capsulCenter, capsuleCollider.height, capsuleCollider.radius, Vector3.down * stepHight, out stepDownHit, Color.green, 100, DebugAreas.Movement))
				{
					if (gameCharacter.IsPlayerCharacter) Ultra.Utilities.Instance.DebugLogOnScreen(StringColor.Red + "StepDownHit" + StringColor.EndColor);
					// ConvertHitPoint into center
					capsulCenter = CapsulCenterFromEnds(moveRequestVector, stepDownHit, Vector3.up);
					Ultra.Utilities.DrawCapsule(capsulCenter, Quaternion.identity, capsuleCollider.height, capsuleCollider.radius, Color.green.WithAlpha(0.5f), 100, DebugAreas.Movement);
				}
				else
				{
					if (gameCharacter.IsPlayerCharacter) Ultra.Utilities.Instance.DebugLogOnScreen(StringColor.Green + "StepDownNoHit" + StringColor.EndColor);
				}
			}else
			{
				if (gameCharacter.IsPlayerCharacter) Ultra.Utilities.Instance.DebugLogOnScreen(StringColor.Green + "StepDownNoHit" + StringColor.EndColor);
			}

			MoveTransfowm(currentPos, capsulCenter);
		}
	}

	Vector3 RemoveCenterPosition(Vector3 pos)
	{
		return pos - capsuleCollider.center;
	}

	private void MoveTransfowm(Vector3 currentPos, Vector3 capsulCenter)
	{
		Vector3 moveDirection = capsulCenter - currentPos;
		transform.Translate(moveDirection, Space.World);
	}

	private Vector3 MoveCapsul(Vector3 moveRequestVector, Vector3 capsulCenter, int iteration = 0)
	{
		// Min move request and max iteration check
		if (moveRequestVector.magnitude < 0.001f || iteration > 2)
		{
			Debug.Log("Movement Max iteration hit");
			MovementVelocity = new Vector3(0, Velocity.y, 0);
			return capsulCenter;
		}

		RaycastHit capsuleMoveHit;
		if (Ultra.Utilities.CapsulCast(capsulCenter, capsuleCollider.height, capsuleCollider.radius, moveRequestVector, out capsuleMoveHit, Color.blue, 100, DebugAreas.Movement))
		{
			if (gameCharacter.IsPlayerCharacter) Ultra.Utilities.Instance.DebugLogOnScreen(StringColor.Red + "MoveCapsulHit" + StringColor.EndColor);

			Vector3 newCenter = Vector3.zero;
			float hitDot = Vector3.Dot(Vector3.up, capsuleMoveHit.normal.normalized);
			// Zylinder hit
			if (hitDot == 0)
			{
				Ultra.Utilities.DrawWireSphere(capsuleMoveHit.point, 0.1f, Color.red, 10f);
				// ConvertHitPoint into center without Y information
				Vector3 capsulCenterDirection = (capsuleMoveHit.normal * capsuleCollider.radius);
				Vector3 capsulCenterWithoutY = capsuleMoveHit.point + capsulCenterDirection;
				Vector3 c = capsulCenterWithoutY - capsulCenter;
				Vector3 bDir = Vector3.down;
				// Calsulate Y position and find new CapsulCenter
				float beta = Mathf.Deg2Rad * Vector3.Angle(bDir, -c.normalized);
				float gamma = Mathf.Deg2Rad * Vector3.Angle(-bDir, -moveRequestVector.normalized);
				float zL = c.magnitude * Mathf.Sin(beta) / Mathf.Sin(gamma);
				newCenter = capsulCenter + moveRequestVector.normalized * (zL - minDistance);
			}
			// Upper Sphere hit
			else if (hitDot < 0)
			{
				newCenter = CapsulCenterFromEnds(moveRequestVector, capsuleMoveHit, Vector3.down);
			}
			// Lower Sphere hit
			else if (hitDot > 0)
			{
				newCenter = CapsulCenterFromEnds(moveRequestVector, capsuleMoveHit, Vector3.up);
			}
		
			float distance = 0.2f;
			RaycastHit normalInformationHit;
			if (Physics.Raycast(capsuleMoveHit.point + capsuleMoveHit.normal * (distance / 2), -capsuleMoveHit.normal, out normalInformationHit, distance))
			{
				Vector3 moveVector = newCenter - capsulCenter;
				float moveMagnitureDelta = moveVector.magnitude - moveRequestVector.magnitude;
				Vector3 planeDir = Vector3.ProjectOnPlane(moveRequestVector, normalInformationHit.normal);
				Vector3 newMoveRequest = planeDir * math.abs(moveMagnitureDelta);
			
				capsulCenter = newCenter;
			
				Ultra.Utilities.DrawCapsule(capsulCenter, Quaternion.identity, capsuleCollider.height, capsuleCollider.radius, Color.red.WithAlpha(0.7f), 100, DebugAreas.Movement);
			
				return MoveCapsul(newMoveRequest, capsulCenter, iteration++);
			}

			capsulCenter = newCenter;
			//MovementVelocity = new Vector3(0, Veloctiy.y, 0);
			Ultra.Utilities.DrawCapsule(capsulCenter, Quaternion.identity, capsuleCollider.height, capsuleCollider.radius, Color.blue.WithAlpha(0.5f), 100, DebugAreas.Movement);
		}
		else
		{
			if (gameCharacter.IsPlayerCharacter) Ultra.Utilities.Instance.DebugLogOnScreen(StringColor.Green + "MoveCapsulNoHit" + StringColor.EndColor);
			capsulCenter += moveRequestVector;
		}

		return capsulCenter;
	}

	private Vector3 CapsulCenterFromEnds(Vector3 moveRequestVector, RaycastHit capsuleMoveHit, Vector3 centerDirection)
	{
		Vector3 newCenter;
		// ConvertHitPoint into center without Y information
		Vector3 capsulCenterDirection = (capsuleMoveHit.normal * capsuleCollider.radius);
		Vector3 capsulCenterWithoutY = capsuleMoveHit.point + capsulCenterDirection;
		// Add Y infomation
		newCenter = capsulCenterWithoutY + centerDirection * ((capsuleCollider.height / 2) - capsuleCollider.radius);
		// Add mindistance
		newCenter = newCenter + -moveRequestVector * minDistance;
		return newCenter;
	}

	public float CalculateGravity()
	{
		return ((MovementVelocity.y > 0) ? gameCharacter.GameCharacterData.MovmentGravity * gameCharacter.GameCharacterData.GravityMultiplier : gameCharacter.GameCharacterData.MovmentGravity) * Time.deltaTime;
	}

	public void CheckIfCharacterIsGrounded()
	{
		RaycastHit groundHitCapsul;
		RaycastHit groundHitRayCast;
		if (IsGroundedCheck(CharacterCenter, out groundHitCapsul))
		{
			if (groundHitCapsul.transform.gameObject.layer == characterLayerIndex)
			{
				IsGroundedIntern = false;
				return;
			}

			Vector3 castOrigin = new Vector3(groundHitCapsul.point.x, groundHitCapsul.point.y + 1f, groundHitCapsul.point.z);
			if (Physics.Raycast(castOrigin, groundHitCapsul.point - castOrigin, out groundHitRayCast, Vector3.Distance(castOrigin, groundHitCapsul.point) + 0.1f))
				RayCastGroundHit = new NullableHit(groundHitRayCast);
			else
				RayCastGroundHit = null;

			IsGroundedIntern = true;
			PossibleGround = new NullableHit(groundHitCapsul);
			//if (MovementVelocity.y < 0) MovementVelocity = new Vector3(MovementVelocity.x, 0f, MovementVelocity.z);
		}
		else
		{
			IsGroundedIntern = false;
		}
	}

	public bool IsGroundedCheck(Vector3 center, out RaycastHit newHit)
	{
		return Ultra.Utilities.CapsulCast(center, capsuleCollider.height, capsuleCollider.radius, Vector3.down * (0.1f + minDistance), out newHit, Color.cyan.WithAlpha(0.35f), 100, DebugAreas.Movement);
	}

	public bool CheckCharacterCapsulInDirection(Vector3 center ,Vector3 direction, out RaycastHit newHit)
	{
		return Ultra.Utilities.CapsulCast(center, capsuleCollider.height, capsuleCollider.radius, direction, out newHit, Color.cyan.WithAlpha(0.35f), 100, DebugAreas.Movement);
	}

	Vector3 GetMovementVelocityWithoutGravity()
	{
		float yGravity = CalculateGravity();
		return new Vector3(MovementVelocity.x, MovementVelocity.y + yGravity, MovementVelocity.z);
	}

	public void MoveCharacter()
	{
		Vector3 movementVector = GetMovmentVelocityWithDeltaTime();
		RequestMove(movementVector);
		if (gameCharacter.IsPlayerCharacter) Ultra.Utilities.Instance.DebugLogOnScreen("MovementVelocity: " + MovementVelocity.ToString() + " MovementSpeed: " + MovementVelocity.magnitude);
		if (gameCharacter.IsPlayerCharacter) Ultra.Utilities.Instance.DebugLogOnScreen(StringColor.Lime + "Velocity: " + Velocity.ToString() + " VelocityMagnitude: " + Velocity.magnitude + StringColor.EndColor);
		Ultra.Utilities.DrawArrow(transform.position, movementVector.normalized, movementVector.magnitude * 50f, Color.red, 0f, 50, DebugAreas.Movement);
		Vector3 moveDir = GetMovementVelocityWithoutGravity();
		Ultra.Utilities.DrawArrow(transform.position, (moveDir.normalized.magnitude <= 0) ? transform.forward : moveDir.normalized, moveDir.magnitude * Time.deltaTime * 50f, Color.blue, 0f, 50, DebugAreas.Movement);
	}
	public void AddGravityOnMovementVelocity()
	{
		if (gameCharacter.StateMachine.GetCurrentStateType() == EGameCharacterState.InAir || !IsGrounded)
		{
			if (!UseGravity) return;
			float yGravity = CalculateGravity();
			MovementVelocity = new Vector3(MovementVelocity.x, MovementVelocity.y - yGravity, MovementVelocity.z);
		}
	}

	public void CalculateVelocity()
	{
		Vector3 currentPosition = transform.position;
		veloctiy = (currentPosition - postionLastFrame) / Time.deltaTime;
		postionLastFrame = currentPosition;
	}

	public float GetPossibleGroundAngle()
	{
		if (PossibleGround == null) return 0;
		return Vector3.Angle(PossibleGround.hit.normal, Vector3.up);
	}
	public Vector3 GetMovmentVelocityWithDeltaTime()
	{
		return MovementVelocity * Time.deltaTime;
	}

	private LayerMask ExcludeLayerIsMask(LayerMask mask, int layer)
	{
		int excludeLayers = mask.value;

		// Überprüfe, ob die Layer-ID bereits im excludeLayer-Array enthalten ist
		if ((excludeLayers & (1 << layer)) == 0)
		{
			// Füge die Layer-ID zum excludeLayer-Array hinzu
			excludeLayers |= (1 << layer);
			mask = (LayerMask)excludeLayers;
		}
		return mask;
	}

	public void SetLayerToDefault()
	{
		gameCharacter.Rigidbody.excludeLayers = defaultLayerMask;
		unityMovementController.excludeLayers = defaultLayerMask;
	}

	public void MoveThroughCharacterLayer()
	{
		gameCharacter.Rigidbody.excludeLayers = moveThroughCharacterLayerMask;
		unityMovementController.excludeLayers = moveThroughCharacterLayerMask;
	}

	public void ResetCharacterCapsulToDefault()
	{
		gameCharacter.CharacterRadiusTarget = characterDefaultRadius;
		gameCharacter.CharacterHeightTarget = characterDefaultHeight;
	}

	IEnumerator IsJumping()
	{
		yield return new WaitForSeconds(0.2f);
		IsInJump = false;
	}
}

public class NullableHit
{
	public RaycastHit hit;

	public NullableHit(RaycastHit hit)
	{
		this.hit = hit;
	}
}