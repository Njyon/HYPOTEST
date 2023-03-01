using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCharacter : MonoBehaviour
{
	GameCharacterStateMachine stateMachine;
	CharacterController characterController;
	CharacterData characterData;
	Vector2 movementInput;
	Vector3 postionLastFrame;
	Vector3 veloctiy;
	Vector3 movementVelocity;
	int currentJumpAmount = 0;
	PredictedLandingPoint predictedLandingPoint;
	bool isGrounded = false;

	public GameCharacterStateMachine StateMachine { get { return stateMachine; } }
	public CharacterController CharacterController { get { return characterController; } }
	public Vector2 MovementInput { get { return movementInput; } }
	public Vector3 Veloctiy { get { return veloctiy; } }
	public CharacterData CharacterData { get { return characterData; } }
	public Vector3 MovementVelocity { get { return movementVelocity; } set { movementVelocity = value; } }
	public int CurrentJumpAmount { get { return currentJumpAmount; } set { currentJumpAmount = value; } }
	public PredictedLandingPoint PossibleGround { get { return predictedLandingPoint; } set { predictedLandingPoint = value; } }
	public bool IsGrounded { get { return isGrounded; } }


	public void HorizontalMovementInput(float Haxis)
	{
		movementInput.x = Haxis;
	}
	public void VerticalMovmentInput(float Vaxis)
	{
		movementInput.y = Vaxis;
	}
	public Vector2 GetMovementInputDir()
	{
		Vector2 nomralizedInput = MovementInput;
		nomralizedInput.Normalize();
		return nomralizedInput;
	}
	private void AddGravityOnMovementVelocity()
	{
		if (characterController.isGrounded && MovementVelocity.y <= 0f)
		{
			var mV = MovementVelocity;
			mV.y = 0f;
			MovementVelocity = mV;
		}
		float yGravity = CalculateGravity();

		MovementVelocity = new Vector3(MovementVelocity.x, MovementVelocity.y - yGravity, MovementVelocity.z);
	}

	public float CalculateGravity()
	{
		//if (StateMachine.GetCurrentStateType() == GameCharacterState.Moving) return (CharacterData.MovmentGravity * Time.deltaTime) * 5;
		return ((MovementVelocity.y > 0) ? (CharacterData.MovmentGravity * CharacterData.GravityMultiplier) : CharacterData.MovmentGravity) * Time.deltaTime;
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

	private void Awake()
	{
		characterData = gameObject.GetComponent<CharacterData>();
		if (!characterData) gameObject.AddComponent<CharacterData>();
		characterController = gameObject.GetComponent<CharacterController>();
		if (!characterController) gameObject.AddComponent<CharacterController>();
		stateMachine = gameObject.AddComponent<GameCharacterStateMachine>();
	}

	private void Update()
	{
		CalculateVelocity();
		AddGravityOnMovementVelocity();

		RaycastHit newHit;
		Vector3 top = transform.position + Vector3.up * (CharacterController.height / 2f - characterController.radius);
		Vector3 bottom = transform.position + Vector3.down * (CharacterController.height / 2f - characterController.radius);
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

		MoveCharacter();

		if (PossibleGround != null)
		{
			Ultra.Utilities.DrawWireSphere(PossibleGround.hit.point, 0.2f, Color.blue, 0.0f, 100, DebugAreas.Movement);
		}
	}

	private void MoveCharacter()
	{
		Vector3 movementVector = GetMovmentVelocityWithDeltaTime();
		CollisionFlags falgs = CharacterController.Move(movementVector);
		Ultra.Utilities.Instance.DebugLogOnScreen("MovementVelocity: " + MovementVelocity.ToString());
		Ultra.Utilities.DrawArrow(transform.position, movementVector.normalized, movementVector.magnitude * 50f, Color.red, 0f, 50, DebugAreas.Movement);
		Vector3 moveDir = GetMovementVelocityWithoutGravity();
		Ultra.Utilities.DrawArrow(transform.position, (moveDir.normalized.magnitude <= 0) ? transform.forward : moveDir.normalized, moveDir.magnitude * Time.deltaTime * 50f, Color.blue, 0f, 50, DebugAreas.Movement);
	}

	private void LateUpdate()
	{
		KillZOffset();
	}

	private void OnControllerColliderHit(ControllerColliderHit hit)
	{
		
	}

	private void OnAnimatorMove()
	{
		
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
