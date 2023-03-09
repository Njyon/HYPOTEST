using System;
using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;

public class GameCharacter : MonoBehaviour
{
	GameCharacterStateMachine stateMachine;
	CharacterController characterController;
	CharacterData characterData;
	Animator animator;
	AnimationController animController;
	Vector2 movementInput;
	Vector3 postionLastFrame;
	Vector3 veloctiy;
	Vector3 movementVelocity;
	int currentJumpAmount = 0;
	PredictedLandingPoint predictedLandingPoint;
	bool isGrounded = false;
	float maxWalkableSlopAngle;
	float slopStrengh = 1;
	bool isInJump = false;
	Vector3 lastDir;

	public GameCharacterStateMachine StateMachine { get { return stateMachine; } }
	public CharacterController CharacterController { get { return characterController; } }
	public Vector2 MovementInput { get { return movementInput; } }
	public Vector3 Veloctiy { get { return veloctiy; } }
	public CharacterData CharacterData { get { return characterData; } }
	public Vector3 MovementVelocity { get { return movementVelocity; } set { movementVelocity = value; } }
	public int CurrentJumpAmount { get { return currentJumpAmount; } set { currentJumpAmount = value; } }
	public PredictedLandingPoint PossibleGround { get { return predictedLandingPoint; } set { predictedLandingPoint = value; } }
	public bool IsGrounded { get { return isGrounded; } }
	public float MaxWalkableSlopAngle { get { return maxWalkableSlopAngle; } }
	public float SlopStrengh { get { return slopStrengh; } set { slopStrengh = value; } }
	public Animator Animator { get { return animator; } }
	public float MovementSpeed { get { return Veloctiy.magnitude; } }
	public bool IsInJump { get { return isInJump; } 
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

	public void HorizontalMovementInput(float Haxis)
	{
		movementInput.x = Haxis;
	}
	public void VerticalMovmentInput(float Vaxis)
	{
		movementInput.y = Vaxis;
	}
	public Vector2 GetHorizontalMovementInputDir()
	{
		return new Vector2(MovementInput.x, 0f);
	}
	private void AddGravityOnMovementVelocity()
	{
		if (StateMachine.GetCurrentStateType() == EGameCharacterState.InAir || !IsGrounded)
		{
			float yGravity = CalculateGravity();
			MovementVelocity = new Vector3(MovementVelocity.x, MovementVelocity.y - yGravity, MovementVelocity.z);
		}
	}
	public float CalculateGravity()
	{
		return ((MovementVelocity.y > 0) ? CharacterData.MovmentGravity * CharacterData.GravityMultiplier : CharacterData.MovmentGravity) * Time.deltaTime;
	}
	Vector3 GetMovementVelocityWithoutGravity()
	{
		float yGravity = CalculateGravity();
		return new Vector3(MovementVelocity.x, MovementVelocity.y + yGravity, MovementVelocity.z);
	}
	private void KillZOffset()
	{
		Vector3 pos = transform.position;
		/// Kill Z so character is always on Z 0
		pos.z = 0;
		transform.position = pos;
	}
	private void CalculateVelocity()
	{
		Vector3 currentPosition = transform.position;
		veloctiy = (currentPosition - postionLastFrame) / Time.deltaTime;
		postionLastFrame = currentPosition;
	}
	public Vector3 GetMovmentVelocityWithDeltaTime()
	{
		return MovementVelocity * Time.deltaTime;
	}
	public float GetPossibleGroundAngle()
	{
		if (PossibleGround == null) return 0;
		return Vector3.Angle(PossibleGround.hit.normal, Vector3.up);
	}

	private void Awake()
	{
		characterData = gameObject.GetComponent<CharacterData>();
		if (!characterData) gameObject.AddComponent<CharacterData>();
		characterController = gameObject.GetComponent<CharacterController>();
		if (!characterController) gameObject.AddComponent<CharacterController>();
		stateMachine = gameObject.AddComponent<GameCharacterStateMachine>();
		animator = gameObject.GetComponent<Animator>();
		if (animator == null) Debug.LogError("GameObject: " + name + " Does not have an Animator Attached!");
		animController = new AnimationController(this);

		// Set Default Data
		maxWalkableSlopAngle = CharacterController.slopeLimit;
		lastDir = transform.right;
		/// Set sloplimit to max so we can use Slope strengh to walk over anything at first end slide when slop strengh hits 0
		CharacterController.slopeLimit = 90f;

		animController.Start();
	}

	private void Update()
	{
		CalculateVelocity();
		AddGravityOnMovementVelocity();
		CheckIfCharacterIsGrounded();
		MoveCharacter();
		CalculateSlopLimit();
		RotateCharacterInVelocityDirection();

		animController.Update(Time.deltaTime);

		if (PossibleGround != null)
		{
			Ultra.Utilities.DrawWireSphere(PossibleGround.hit.point, 0.2f, Color.blue, 0.0f, 100, DebugAreas.Movement);
		}
		Ultra.Utilities.Instance.DebugLogOnScreen("CurrentCharacterState: " + StateMachine.GetCurrentStateType().ToString(), 0f, StringColor.Teal, 100, DebugAreas.Movement);
		Ultra.Utilities.Instance.DebugLogOnScreen("Current Ground Angle: " + GetPossibleGroundAngle(), 0f, StringColor.Teal, 200, DebugAreas.Misc);
	}

	private void RotateCharacterInVelocityDirection()
	{
		if (MovementVelocity.normalized.x != 0) lastDir = new Vector3(MovementVelocity.x, 0, 0);
		if (lastDir == Vector3.zero) return;
		Quaternion targetRot = Quaternion.LookRotation(lastDir.normalized, Vector3.up);
		targetRot = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * characterData.RoationSpeed);
		Vector3 dir = transform.rotation * Vector3.forward;
		Vector3 cross = Vector3.Cross(lastDir.normalized, dir);
		float sign = Mathf.Sign(cross.y);
		if (Ultra.Utilities.IsNearlyEqual(cross, Vector3.zero, 0.2f))
		{
			sign = 0f;
		}
		animController.RotationTrarget = sign;
		//// Need to rotate Character
		//targetRot.eulerAngles = new Vector3(targetRot.eulerAngles.x, targetRot.eulerAngles.y + 90, targetRot.eulerAngles.z);
		transform.rotation = targetRot;
	}

	private void CalculateSlopLimit()
	{
		if (GetPossibleGroundAngle() > maxWalkableSlopAngle)
		{
			if (slopStrengh > 0)
			{
				slopStrengh = slopStrengh - Time.deltaTime * CharacterData.SlopStrenghDecrease;
				if (slopStrengh < 0) slopStrengh = 0;
			}
		}
		else
		{
			if (slopStrengh < 1)
			{
				slopStrengh = slopStrengh + Time.deltaTime * CharacterData.SlopStrenghIncrease;
				if (slopStrengh > 1) slopStrengh = 1;
			}
		}
		CharacterController.slopeLimit = Unity.Mathematics.math.remap(0, 1, maxWalkableSlopAngle, 90, slopStrengh);
	}

	private void CheckIfCharacterIsGrounded()
	{
		RaycastHit newHit;
		Vector3 top = transform.position + Vector3.up * (CharacterController.height / 2f - characterController.radius);
		top += characterController.center;
		Vector3 bottom = transform.position + Vector3.down * (CharacterController.height / 2f - characterController.radius);
		bottom += characterController.center;
		//if (Physics.Raycast(transform.position, Vector3.down, out newHit, (CharacterController.height / 2) + CharacterController.skinWidth + 0.1f))
		if (Physics.CapsuleCast(top, bottom, characterController.radius, Vector3.down, out newHit, 0.1f))
		{
			PossibleGround = new PredictedLandingPoint(newHit);
			isGrounded = true;
		}
		else
		{
			isGrounded = false;
		}
	}

	private void MoveCharacter()
	{
		Vector3 movementVector = GetMovmentVelocityWithDeltaTime();
		CollisionFlags falgs = CharacterController.Move(movementVector);
		Ultra.Utilities.Instance.DebugLogOnScreen("MovementVelocity: " + MovementVelocity.ToString() + "MovementSpeed: " + MovementVelocity.magnitude);
		Ultra.Utilities.DrawArrow(transform.position, movementVector.normalized, movementVector.magnitude * 50f, Color.red, 0f, 50, DebugAreas.Movement);
		Vector3 moveDir = GetMovementVelocityWithoutGravity();
		Ultra.Utilities.DrawArrow(transform.position, (moveDir.normalized.magnitude <= 0) ? transform.forward : moveDir.normalized, moveDir.magnitude * Time.deltaTime * 50f, Color.blue, 0f, 50, DebugAreas.Movement);
	}

	private void LateUpdate()
	{
		KillZOffset();
		animController.LateUpdate();
	}

	private void OnControllerColliderHit(ControllerColliderHit hit)
	{
		
	}

	private void OnAnimatorMove()
	{
		
	}

	IEnumerator IsJumping()
	{
		yield return new WaitForSeconds(1f);
		IsInJump = false;
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


