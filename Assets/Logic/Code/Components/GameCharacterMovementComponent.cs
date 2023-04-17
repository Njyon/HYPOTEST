using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class GameCharacterMovementComponent : MonoBehaviour
{
    [SerializeField] float stepHight;
	[SerializeField] float maxWalkableSlopAngle;
	PredictedLandingPoint predictedLandingPoint;
	bool isGrounded = false;
	Vector3 postionLastFrame;
	Vector3 veloctiy;
	Vector3 movementVelocity;
	GameCharacter gameCharacter;
	float slopStrengh = 1;
	float slopeLimit;
	Vector3 rootmotionVector;

	CapsuleCollider capsuleCollider;
    public CapsuleCollider CapsuleCollider { get { return capsuleCollider; } }
    public float StepHeight { get { return stepHight; } }
    public Vector3 CharacterCenter { get { return transform.position + capsuleCollider.center; } }
	public PredictedLandingPoint PossibleGround { get { return predictedLandingPoint; } set { predictedLandingPoint = value; } }
	public bool IsGrounded { get { return isGrounded; } }
	public Vector3 MovementVelocity { get { return movementVelocity; } set { movementVelocity = value; } }
	public Vector3 Veloctiy { get { return veloctiy; } }
	public float MovementSpeed { get { return Veloctiy.magnitude; } }
	public float SlopStrengh { get { return slopStrengh; } set { slopStrengh = value; } }
	public float MaxWalkableSlopAngle { get { return maxWalkableSlopAngle; } }
	public Vector3 RootmotionVector { get { return rootmotionVector; } }
	public float Height { get { return capsuleCollider.height; } }
	public float Radius { get { return capsuleCollider.radius; } }
	public float SlopeLimit { get { return slopeLimit; } set { slopeLimit = value; } }

	static float minDistance = 1.001f;

	void Awake()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();

		/// Set sloplimit to max so we can use Slope strengh to walk over anything at first end slide when slop strengh hits 0
		slopeLimit = 90f;
	}

    void Update()
	{
		Ultra.Utilities.Instance.DebugLogOnScreen(StringColor.Lime + "Is Grounded = " + isGrounded.ToString() + StringColor.EndColor);
	}

	private void OnAnimatorMove()
	{
		rootmotionVector = gameCharacter.Animator.deltaPosition * (1 / Time.deltaTime);
	}

	public void SetupGameCharacter(GameCharacter character)
	{
		gameCharacter = character;
	}

	void RequestMove(Vector3 moveRequestVector)
    {
		if (moveRequestVector.magnitude < 0.001f) return;

        Vector3 currentPos = CharacterCenter;
		Vector3 requestedPosition = currentPos + moveRequestVector;
		Vector3 requestedDirection = requestedPosition - currentPos;
		Vector3 requestedPositionWithStepHight = requestedPosition + (Vector3.up * stepHight);
		Vector3 capsulCenter = CharacterCenter;

		Ultra.Utilities.DrawArrow(currentPos, requestedDirection, 10f, Color.green);

		RaycastHit setpHightHit;
		if (Ultra.Utilities.CapsulCast(requestedPositionWithStepHight, capsuleCollider.height, capsuleCollider.radius, Vector3.down * stepHight, out setpHightHit, Color.red, 100, DebugAreas.Movement))
		{
			Ultra.Utilities.Instance.DebugLogOnScreen(StringColor.Red + "StepHeigtHit" + StringColor.EndColor);
			// ConvertHitPoint into center
			Vector3 capsulBottomCenter = setpHightHit.point + (setpHightHit.normal * (capsuleCollider.radius + minDistance));
			capsulCenter = capsulBottomCenter + Vector3.up * ((capsuleCollider.height / 2) - capsuleCollider.radius);
			Ultra.Utilities.DrawCapsule(capsulCenter, Quaternion.identity, capsuleCollider.height, capsuleCollider.radius, Color.green.WithAlpha(0.5f));
			
			float yPosDelta = Mathf.Abs(capsulCenter.y - requestedPositionWithStepHight.y);
			// add stephight to currentpos for capsulcast
			Vector3 currentPosWithYDelta = currentPos + Vector3.up * yPosDelta;
			capsulCenter = MoveCapsul(moveRequestVector, currentPosWithYDelta);
			
			MoveTransfowm(currentPos, capsulCenter);
		}
		else
		{
			Ultra.Utilities.Instance.DebugLogOnScreen(StringColor.Green + "StepHeigtNoHit" + StringColor.EndColor);
			capsulCenter = MoveCapsul(moveRequestVector, capsulCenter);
			Ultra.Utilities.DrawCapsule(capsulCenter, Quaternion.identity, capsuleCollider.height, capsuleCollider.radius, Color.green);
			//RaycastHit stepDownHit;
			//if (!IsGroundedCheck(capsulCenter, out stepDownHit))
			//{
			//	if (Ultra.Utilities.CapsulCast(capsulCenter, capsuleCollider.height, capsuleCollider.radius, Vector3.down * stepHight, out stepDownHit, Color.green, 100, DebugAreas.Movement))
			//	{
			//		Ultra.Utilities.Instance.DebugLogOnScreen(StringColor.Red + "StepDownHit" + StringColor.EndColor);
			//		// ConvertHitPoint into center
			//		Vector3 capsulBottomCenter = stepDownHit.point + (stepDownHit.normal * (capsuleCollider.radius + minDistance));
			//		capsulCenter = capsulBottomCenter + Vector3.up * ((capsuleCollider.height / 2) - capsuleCollider.radius);
			//		Ultra.Utilities.DrawCapsule(capsulCenter, Quaternion.identity, capsuleCollider.height, capsuleCollider.radius, Color.green.WithAlpha(0.5f), 100, DebugAreas.Movement);
			//	}
			//	else
			//	{
			//		Ultra.Utilities.Instance.DebugLogOnScreen(StringColor.Green + "StepDownNoHit" + StringColor.EndColor);
			//	}
			//}else
			//{
			//	Ultra.Utilities.Instance.DebugLogOnScreen(StringColor.Green + "StepDownNoHit" + StringColor.EndColor);
			//}
		
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

	private Vector3 MoveCapsul(Vector3 moveRequestVector, Vector3 capsulCenter)
	{
		if (moveRequestVector.magnitude < 0.001f) return capsulCenter;

		RaycastHit capsuleMoveHit;
		if (Ultra.Utilities.CapsulCast(capsulCenter, capsuleCollider.height, capsuleCollider.radius, moveRequestVector, out capsuleMoveHit, Color.blue, 100, DebugAreas.Movement))
		{
			Ultra.Utilities.Instance.DebugLogOnScreen(StringColor.Red + "MoveCapsulHit" + StringColor.EndColor);

			Vector3 newCenter = Vector3.zero;
			float hitDot = Vector3.Dot(Vector3.up, capsuleMoveHit.normal.normalized);
			//Debug.Log("Dot =" + hitDot);
			// Zylinder hit
			if (hitDot == 0)
			{
				Ultra.Utilities.DrawWireSphere(capsuleMoveHit.point, 0.1f, Color.red, 10f);
				// ConvertHitPoint into center without Y information
				Vector3 capsulCenterDirection = (capsuleMoveHit.normal * (capsuleCollider.radius + minDistance));
				Vector3 capsulCenterWithoutY = capsuleMoveHit.point + capsulCenterDirection;
				Vector3 c = capsulCenterWithoutY - capsulCenter;
				Vector3 bDir = Vector3.down;
				// Calsulate Y position and find new CapsulCenter
				float beta = Vector3.Angle(bDir, -c.normalized);
				float gamma = Vector3.Angle(-bDir, -moveRequestVector.normalized);
				float zL = c.magnitude * math.sin(beta) / math.sin(gamma);
				Debug.Log("zl =" + zL + " beta = " + beta + " gamma = " + gamma);
				newCenter = capsulCenter + moveRequestVector.normalized * zL;
			}
			// Upper Sphere hit
			else if (hitDot > 0)
			{
				newCenter = CapsulCenterFromEnds(moveRequestVector, capsuleMoveHit, Vector3.down);
			}
			// Lower Sphere hit
			else if (hitDot < 0)
			{
				newCenter = CapsulCenterFromEnds(moveRequestVector, capsuleMoveHit, Vector3.up);
			}
			

			//float distance = 0.2f;
			//RaycastHit normalInformationHit;
			//if (Physics.Raycast(capsuleMoveHit.point + capsuleMoveHit.normal * (distance / 2), -capsuleMoveHit.normal, out normalInformationHit, distance))
			//{
			//	Vector3 moveVector = newCenter - capsulCenter;
			//	float moveMagnitureDelta = moveVector.magnitude - moveRequestVector.magnitude;
			//	Vector3 planeDir = Vector3.ProjectOnPlane(moveRequestVector, normalInformationHit.normal);
			//	Vector3 newMoveRequest = planeDir * moveMagnitureDelta;
			//
			//	return MoveCapsul(newMoveRequest, capsulCenter);
			//}

			capsulCenter = newCenter;

			MovementVelocity = new Vector3(0, Veloctiy.y, 0);
			Ultra.Utilities.DrawCapsule(capsulCenter, Quaternion.identity, capsuleCollider.height, capsuleCollider.radius, Color.blue.WithAlpha(0.5f), 100, DebugAreas.Movement);

		}
		else
		{
			Ultra.Utilities.Instance.DebugLogOnScreen(StringColor.Green + "MoveCapsulNoHit" + StringColor.EndColor);
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
		newCenter = capsulCenterWithoutY + centerDirection * (capsuleCollider.height / 2);
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
		RaycastHit newHit;
		if (IsGroundedCheck(CharacterCenter, out newHit))
		{
			Vector3 castOrigin = new Vector3(newHit.point.x, newHit.point.y + 1f, newHit.point.z);
			if (Physics.Raycast(castOrigin, newHit.point - castOrigin, out newHit, Vector3.Distance(castOrigin, newHit.point) + 0.1f))
				PossibleGround = new PredictedLandingPoint(newHit);
			isGrounded = true;
			if (MovementVelocity.y < 0) MovementVelocity = new Vector3(MovementVelocity.x, 0f, MovementVelocity.z);
		}
		else
		{
			isGrounded = false;
		}
	}

	private bool IsGroundedCheck(Vector3 center, out RaycastHit newHit)
	{
		return Ultra.Utilities.CapsulCast(center, capsuleCollider.height, capsuleCollider.radius, Vector3.down * (0.1f + minDistance), out newHit, Color.cyan.WithAlpha(0.35f), 100, DebugAreas.Movement);
	}

	Vector3 GetMovementVelocityWithoutGravity()
	{
		float yGravity = CalculateGravity();
		return new Vector3(MovementVelocity.x, MovementVelocity.y + yGravity, MovementVelocity.z);
	}

	public void CalculateSlopLimit()
	{
		if (GetPossibleGroundAngle() > maxWalkableSlopAngle)
		{
			if (slopStrengh > 0)
			{
				slopStrengh = slopStrengh - Time.deltaTime * gameCharacter.GameCharacterData.SlopStrenghDecrease;
				if (slopStrengh < 0) slopStrengh = 0;
			}
		}
		else
		{
			if (slopStrengh < 1)
			{
				slopStrengh = slopStrengh + Time.deltaTime * gameCharacter.GameCharacterData.SlopStrenghIncrease;
				if (slopStrengh > 1) slopStrengh = 1;
			}
		}
		slopeLimit = Unity.Mathematics.math.remap(0, 1, maxWalkableSlopAngle, 90, slopStrengh);
	}

	public void MoveCharacter()
	{
		Vector3 movementVector = GetMovmentVelocityWithDeltaTime();
		RequestMove(movementVector);
		Ultra.Utilities.Instance.DebugLogOnScreen("MovementVelocity: " + MovementVelocity.ToString() + "MovementSpeed: " + MovementVelocity.magnitude);
		//Ultra.Utilities.DrawArrow(transform.position, movementVector.normalized, movementVector.magnitude * 50f, Color.red, 0f, 50, DebugAreas.Movement);
		Vector3 moveDir = GetMovementVelocityWithoutGravity();
		//Ultra.Utilities.DrawArrow(transform.position, (moveDir.normalized.magnitude <= 0) ? transform.forward : moveDir.normalized, moveDir.magnitude * Time.deltaTime * 50f, Color.blue, 0f, 50, DebugAreas.Movement);
	}
	public void AddGravityOnMovementVelocity()
	{
		if (gameCharacter.StateMachine.GetCurrentStateType() == EGameCharacterState.InAir || !IsGrounded)
		{
			if (gameCharacter.StateMachine.GetCurrentStateType() == EGameCharacterState.Attack) return;
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

}

public class PredictedLandingPoint
{
	public RaycastHit hit;

	public PredictedLandingPoint(RaycastHit hit)
	{
		this.hit = hit;
	}
}