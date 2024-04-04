using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using System.Linq;
using static GameCharacterMovementComponent;
using UnityEngine.Profiling;

public class GameCharacterMovementComponent : MonoBehaviour
{
	public delegate void OnMoveCollisionFlag(CollisionFlags collisionFlag);
	public OnMoveCollisionFlag onMoveCollisionFlag;
	public delegate void OnCharacterGroundedChanged(bool newState);
	public OnCharacterGroundedChanged onCharacterGroundedChanged;
	public OnCharacterGroundedChanged onCharacterIgnoreGravityChanged;
	public OnCharacterGroundedChanged onCharacterIgnoreDeltaTimeChanged;
	public delegate void OnCharacteroMoveEvent();
	public OnCharacteroMoveEvent onCharacterGroundReset;
	public OnCharacteroMoveEvent onCharacterFinishedJumping;

	/// <summary>
	/// Endless Timer
	/// </summary>
	Ultra.Timer inAirTimer;

	[SerializeField] float stepHight = 0.5f;
	[SerializeField] float maxWalkableSlopAngle = 45f;
	[SerializeField] float headBounceValue = -2f;
	[SerializeField] float capsulInterpolationSpeed = 5f;
	NullableHit predictedLandingPoint;
	NullableHit rayCastGroundHit;
	CharacterController unityMovementController;
	bool isGrounded = false;
	Vector3 postionLastFrame;
	Vector3 fixedPostionLastFrame;
	Vector3 veloctiy;
	Vector3 fixedVeloctiy;
	Vector3 movementVelocity;
	GameCharacter gameCharacter;
	Vector3 rootmotionVector;
	bool isInJump = false;
	LayerMask defaultLayerMask;
	LayerMask moveThroughCharacterLayerMask;
	int characterLayer;
	float characterDefaultRadius;
	float characterDefaultHeight;
	bool useGravity = true;
	float movementOverrideTime;
	Vector3 movementOverride;
	float variableGravityMultiplierOverTime = 1;
	Coroutine gravityCoroutine;
	bool ignoreGravity = false;
	bool ignoreDeltaTime = false;
	bool overrideMovementByHookedCharacter = false;

	/// <summary>
	/// Endless Timer
	/// </summary>
	public Ultra.Timer InAirTimer { get { return inAirTimer; } }
	CapsuleCollider capsuleCollider;
	public CapsuleCollider CapsuleCollider { get { return capsuleCollider; } }
	public float StepHeight { get { return stepHight; } }
	public Vector3 CharacterCenter { get { return transform.position + (capsuleCollider != null ? capsuleCollider.center : Vector3.zero); } }
	public NullableHit PossibleGround { get { return predictedLandingPoint; } set { predictedLandingPoint = value; } }
	public NullableHit RayCastGroundHit { get { return rayCastGroundHit; } set { rayCastGroundHit = value; } }
	public bool IsGrounded { get { return isGrounded && !IsInJump; } }
	public Vector3 MovementVelocity 
	{ 
		get { return movementVelocity; } 
		set { movementVelocity = value; } 
	}
	public Vector3 Velocity { get { return veloctiy; } }
	public Vector3 FixedVelocity { get { return fixedVeloctiy; } }
	public float MovementSpeed { get { return Velocity.magnitude; } }
	public float MaxWalkableSlopAngle { get { return maxWalkableSlopAngle; } }
	public Vector3 RootmotionVector { get { return rootmotionVector; } }
	public float Height { get { return capsuleCollider.height; } }
	public float Radius { get { return capsuleCollider.radius; } }
	public float SlopeLimit { get { return unityMovementController.slopeLimit; } set { unityMovementController.slopeLimit = value; } }
	public CharacterController UnityMovementController { get { return unityMovementController; } }
	public float HeadBounceValue { get { return headBounceValue; } }
	public bool UseGravity { get { return useGravity; } set { useGravity = value; } }
	public float MovementOverrideTime { get { return movementOverrideTime; } set { movementOverrideTime = value; } }
	public Vector3 MovementOverride { get { return movementOverride; } set { movementOverride = value; } }
	public float VariableGravityMultiplierOverTime { get { return variableGravityMultiplierOverTime; } set { variableGravityMultiplierOverTime = value; } }
	public int CharacterLayer { get { return characterLayer; } }
	public bool OverrideMovementByHookedCharacter { get { return overrideMovementByHookedCharacter; } set { overrideMovementByHookedCharacter = value; } }
	public bool IgnoreGravity { 
		get { return ignoreGravity; }
		set 
		{ 
			ignoreGravity = value;
			if (onCharacterIgnoreGravityChanged != null) onCharacterIgnoreGravityChanged(ignoreGravity);
		}
	}	
	public bool IgnoreDeltaTime
	{
		get { return ignoreDeltaTime; }
		set 
		{ 
			ignoreDeltaTime = value;
			if (onCharacterIgnoreDeltaTimeChanged != null) onCharacterIgnoreDeltaTimeChanged(ignoreGravity);
		}
	}
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
			if (!value && isInJump)
			{
				if (onCharacterFinishedJumping != null) onCharacterFinishedJumping();
			}
			isInJump = value;

		}
	}
	private bool IsGroundedIntern { get { return isGrounded; } 
		set 
		{
			if (isGrounded != value)
			{
				isGrounded = value;

				if (!isGrounded)
					InAirTimer.Start();
				else
				{
					GroundReset();
					InAirTimer.Stop();
				}


				if (onCharacterGroundedChanged != null) onCharacterGroundedChanged(isGrounded);
			}
			//if (isGrounded)
			//	GroundReset();


		}
	}
	static float minDistance = 0.001f;

	void OnEnable()
	{
		unityMovementController.enabled = true;
	}

	void OnDisable()
	{
		unityMovementController.enabled = false;
	}

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

		characterLayer = LayerMask.NameToLayer("Character");
		moveThroughCharacterLayerMask = ExcludeLayerIsMask(defaultLayerMask, characterLayer);
		defaultLayerMask = moveThroughCharacterLayerMask;// unityMovementController.excludeLayers;

		characterDefaultRadius = capsuleCollider.radius;
		characterDefaultHeight = capsuleCollider.height;

		inAirTimer = new Ultra.Timer(true);
	}

	void Update()
	{
		if (gameCharacter == null) return;
		if (inAirTimer != null) inAirTimer.Update(Time.deltaTime);
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

	private void FixedUpdate()
	{
		Vector3 fixedPosition = transform.position;
		fixedVeloctiy = (fixedPosition - fixedPostionLastFrame) / Time.fixedDeltaTime;
		fixedPostionLastFrame = fixedPosition;
	}

	public void SetupGameCharacter(GameCharacter character)
	{
		gameCharacter = character;
		gameCharacter.CharacterRadiusTarget = capsuleCollider.radius;
		gameCharacter.CharacterHeightTarget = capsuleCollider.height;
		SetLayerToDefault();
	}

	void RequestMove(Vector3 moveRequestVector)
	{
		if (OverrideMovementByHookedCharacter) return;
		if (Mathf.Abs(gameCharacter.transform.position.z) > 0)
		{
			Vector3 noZVector = new Vector3(transform.position.x, transform.position.y, 0);
			Vector3 noZDirection = noZVector - transform.position;
			noZDirection = noZDirection.normalized * 0.1f;
			if (noZDirection.magnitude > 0.0001f) unityMovementController.Move(noZDirection);
			if (Mathf.Abs(gameCharacter.transform.position.z).IsNearlyEqual(0, 0.5f))
			{
				gameCharacter.transform.position = new Vector3(gameCharacter.transform.position.x, gameCharacter.transform.position.y, 0);
			}
		}
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

		// Move Hooked Characters
		foreach (GameCharacter gc in gameCharacter.CombatComponent.HookedCharacters)
		{
			if (gc.MovementComponent.OverrideMovementByHookedCharacter)
			{
				collisionFlag = gc.MovementComponent.UnityMovementController.Move(moveRequestVector);
				if (gc.MovementComponent.onMoveCollisionFlag != null && collisionFlag != CollisionFlags.None) gc.MovementComponent.onMoveCollisionFlag(collisionFlag);
			}
		}

		MovementVelocity = new Vector3(MovementVelocity.x, MovementVelocity.y, 0);

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
					Ultra.Utilities.DrawCapsule(capsulCenter, Quaternion.identity, capsuleCollider.height, capsuleCollider.radius, Color.green.WithAlpha(0.5f), 0f, 100, DebugAreas.Movement);
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
			
				Ultra.Utilities.DrawCapsule(capsulCenter, Quaternion.identity, capsuleCollider.height, capsuleCollider.radius, Color.red.WithAlpha(0.7f), 0f, 100, DebugAreas.Movement);
			
				return MoveCapsul(newMoveRequest, capsulCenter, iteration++);
			}

			capsulCenter = newCenter;
			//MovementVelocity = new Vector3(0, Veloctiy.y, 0);
			Ultra.Utilities.DrawCapsule(capsulCenter, Quaternion.identity, capsuleCollider.height, capsuleCollider.radius, Color.blue.WithAlpha(0.5f), 0f, 100, DebugAreas.Movement);
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
		float gravity = 0;

		if (IgnoreGravity) return gravity;

		if ((InCombatState() || gameCharacter.CombatComponent.AttackTimer.IsRunning) && gameCharacter.AnimController.GetUpMovementCurve == 0)
		{
			float value = gameCharacter.GameCharacterData.GravitationOverTime.Evaluate(gameCharacter.MovementComponent.InAirTimer.CurrentTime);
			gameCharacter.MovementComponent.MovementVelocity = new Vector3(gameCharacter.MovementComponent.MovementVelocity.x, Mathf.Clamp(gameCharacter.MovementComponent.MovementVelocity.y, ShouldNOTClampGravity() ? int.MinValue : -value, ShouldNOTClampGravity() ? int.MaxValue : value), gameCharacter.MovementComponent.MovementVelocity.z);
		}
		if (VariableGravityMultiplierOverTime < 1)
		{
			gravity = gameCharacter.GameCharacterData.MovmentGravity * VariableGravityMultiplierOverTime;
		}
		else if (MovementVelocity.y > 0)
		{
			gravity = gameCharacter.GameCharacterData.MovmentGravity * gameCharacter.GameCharacterData.GravityMultiplier;
		}
		else
			gravity = gameCharacter.GameCharacterData.MovmentGravity;

		return gravity * Time.deltaTime;
	}

	private bool ShouldNOTClampGravity()
	{
		switch (gameCharacter.StateMachine.GetCurrentStateType())
		{
			case EGameCharacterState.FlyAway:
			case EGameCharacterState.Dodge:
				return true;
			default:
				break;
		}

		return gameCharacter.MovementComponent.IsInJump;
	}

	private bool InCombatState()
	{
		if (gameCharacter.PluginStateMachine.IsPluginStatePlugedIn(EPluginCharacterState.IgnoreGravityRuleState))
			return true;
		else
			return false;


		switch (gameCharacter.StateMachine.GetCurrentStateType())
		{
			case EGameCharacterState.Attack: case EGameCharacterState.AttackRecovery:
				return true;
			default:
				return false;
		}
	}

	public void CheckIfCharacterIsGrounded()
	{
		RaycastHit groundHitCapsul;
		RaycastHit groundHitRayCast;
		if (IsGroundedCheck(CharacterCenter, out groundHitCapsul))
		{
			if (groundHitCapsul.transform.gameObject.layer == characterLayer)
			{
				IsGroundedIntern = false;
				return;
			}

			Vector3 castOrigin = new Vector3(groundHitCapsul.point.x, groundHitCapsul.point.y + 1f, groundHitCapsul.point.z);
			if (Physics.Raycast(castOrigin, groundHitCapsul.point - castOrigin, out groundHitRayCast, Vector3.Distance(castOrigin, groundHitCapsul.point) + 0.1f, gameCharacter.IgnoreCharacterLayer, QueryTriggerInteraction.UseGlobal))
				RayCastGroundHit = new NullableHit(groundHitRayCast);
			else
				RayCastGroundHit = null;

			PossibleGround = new NullableHit(groundHitCapsul);
			IsGroundedIntern = true;

			Ultra.Utilities.DrawCapsule(CharacterCenter, Quaternion.identity, CapsuleCollider.height, CapsuleCollider.radius, Color.red, 10f, 300, DebugAreas.Movement);

			//if (MovementVelocity.y < 0) MovementVelocity = new Vector3(MovementVelocity.x, 0f, MovementVelocity.z);
		}
		else
		{
			IsGroundedIntern = false;
		}
	}

	public bool IsGroundedCheck(Vector3 center, out RaycastHit newHit)
	{
		return Ultra.Utilities.CapsulCast(center, capsuleCollider.height, capsuleCollider.radius, Vector3.down * (0.1f + minDistance), out newHit, Color.cyan.WithAlpha(0.35f), 100, DebugAreas.Movement, gameCharacter.DefaultLayer, QueryTriggerInteraction.Ignore);
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

		if (gameCharacter.PluginStateMachine.IsPluginStatePlugedIn(EPluginCharacterState.MovementOverride))
			movementVector = movementOverride; 

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
			if (MovementVelocity.y <= -gameCharacter.GameCharacterData.MaxGravityPull && yGravity > 0) yGravity = 0; // maybe use interpolation or something to make it more dynamic
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
		return MovementVelocity * (IgnoreDeltaTime ? 1f : Time.deltaTime);
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
		//gameCharacter.Rigidbody.excludeLayers = defaultLayerMask;
		unityMovementController.excludeLayers = defaultLayerMask;
	}

	public void MoveThroughCharacterLayer()
	{
		//gameCharacter.Rigidbody.excludeLayers = moveThroughCharacterLayerMask;
		unityMovementController.excludeLayers = moveThroughCharacterLayerMask;
	}

	public void ResetCharacterCapsulToDefault()
	{
		gameCharacter.CharacterRadiusTarget = characterDefaultRadius;
		gameCharacter.CharacterHeightTarget = characterDefaultHeight;
	}

	public void DeactiveStepup()
	{
		unityMovementController.stepOffset = 0;
	}

	public void ActivateStepup()
	{
		unityMovementController.stepOffset = stepHight;
	}

	public void RequestMovementOverride(float overrideTime = 1f)
	{
		if (CanMovementOverride())
		{
			MovementOverrideTime = overrideTime;
			gameCharacter.PluginStateMachine.AddPluginState(EPluginCharacterState.MovementOverride);
		}
	}

	public void EnemyStep()
	{
		InAirTimer.Start();
		GroundReset();
	}

	public void GroundReset()
	{
		gameCharacter.CurrentJumpAmount = 0;
		if (onCharacterGroundReset != null) onCharacterGroundReset();
	}

	public bool CanMovementOverride()
	{
		return true;
	}

	public void StopGravityInterp()
	{
		if (gravityCoroutine != null)
			StopCoroutine(gravityCoroutine);
	}

	public void InterpGravityUp(float time = 1f)
	{
		if (gameCharacter.gameObject.activeInHierarchy)
		{
			if (gravityCoroutine != null) StopCoroutine(gravityCoroutine);
			gravityCoroutine = StartCoroutine(InterpGravity(time));
		}
	}

	public void CheckIfACharacterIsToCloseToMoveTo(out bool isValidHit, out RaycastHit validHit)
	{
		float lenght = CheckAction() ? gameCharacter.CombatComponent.CurrentWeapon.CurrentAction.Action.GetStopMovingRange() : 1f;
		Vector3 currentDir = Vector3.zero;
		if (gameCharacter.MovementInput.x != 0)
		{
			currentDir = new Vector3(gameCharacter.MovementInput.x, 0, 0);
		}
		else
		{
			currentDir = gameCharacter.transform.forward;
		}
		currentDir = currentDir * lenght;
		RaycastHit[] hits = Ultra.Utilities.CapsulCastAll(gameCharacter.MovementComponent.CharacterCenter, gameCharacter.MovementComponent.Height, gameCharacter.MovementComponent.Radius, currentDir, Color.red, 100, DebugAreas.Combat, gameCharacter.CharacterLayer, QueryTriggerInteraction.Ignore);

		isValidHit = false;
		validHit = new();
		foreach (RaycastHit hit in hits)
		{
			if (hit.collider == null) continue;
			if (hit.collider.gameObject == gameCharacter.gameObject) continue;
			if (hit.collider.transform.parent == gameCharacter.transform) continue;
			isValidHit = true;
			validHit = hit;
			break;
		}
	}

	bool CheckAction()
	{
		return (gameCharacter.CombatComponent.CurrentWeapon != null && gameCharacter.CombatComponent.CurrentWeapon.CurrentAction != null && gameCharacter.CombatComponent.CurrentWeapon.CurrentAction.Action != null);
	}

	/// <summary>
	///  Only use in GameCharacterNoGravityPluginState or when u know what u are doing!
	///  c++ friend class behaviour would be better but dont know of any in C#
	/// </summary>
	/// <param name="state"></param>
	public void DONTUSEChangeIgnoreGravity(bool state)
	{
		ignoreGravity = state;
	}

	public void IgnoreGravityForTime(float t)
	{
		StopCoroutine(IgnoreGravityOverTime(t));
		StartCoroutine(IgnoreGravityOverTime(t));
	}

	IEnumerator IgnoreGravityOverTime(float t)
	{
		IgnoreGravity = true;
		yield return new WaitForSecondsRealtime(t);
		IgnoreGravity = false;
	}

	IEnumerator InterpGravity(float time)
	{
		//variableGravityMultiplierOverTime = 0f;
		float currentTime = 0f;
		while (currentTime < time)
		{
			yield return new WaitForEndOfFrame();
			currentTime += Time.deltaTime;
			//float value = Ultra.Utilities.Remap(currentTime, 0f, time, 0f, 1f);
			//variableGravityMultiplierOverTime = gameCharacter.GameCharacterData.GravityInterpCurve.Evaluate(value);
			float value = Ultra.Utilities.Remap(currentTime, 0f, time, 0f, 1f);
			variableGravityMultiplierOverTime = Mathf.Lerp(variableGravityMultiplierOverTime, 1, value);
		}
	}

	IEnumerator IsJumping()
	{
		if (gameCharacter.CharacterData.CharacterAnimationData.Jumps.Count > 0)
		{
			int jumpIndex = gameCharacter.CurrentJumpAmount - 1;
			jumpIndex = jumpIndex % gameCharacter.CharacterData.CharacterAnimationData.Jumps.Count;
			float animLenght = gameCharacter.CharacterData.CharacterAnimationData.Jumps[jumpIndex].length;
			yield return new WaitForSeconds(animLenght + 0.1f);
			IsInJump = false;
		}
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