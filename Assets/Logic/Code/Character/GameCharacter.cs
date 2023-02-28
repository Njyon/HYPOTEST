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
	float yGravity;
	Vector3 movementVelocity;

	public GameCharacterStateMachine StateMachine { get { return stateMachine; } }
	public CharacterController CharacterController { get { return characterController; } }
	public Vector2 MovementInput { get { return movementInput; } }
	public Vector3 Veloctiy { get { return veloctiy; } }
	public CharacterData CharacterData { get { return characterData; } }
	public Vector3 MovementVelocity { get { return movementVelocity; } set { movementVelocity = value; } }

	private void Awake()
	{
		characterData = gameObject.GetComponent<CharacterData>();
		if (!characterData) gameObject.AddComponent<CharacterData>();
		characterController = gameObject.GetComponent<CharacterController>();
		if (!characterController) gameObject.AddComponent<CharacterController>();
		stateMachine = gameObject.AddComponent<GameCharacterStateMachine>();	
	}

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
	private void AddGravityOnCharacterController()
	{
		if (characterController.isGrounded && yGravity < 0f)
		{
			yGravity = 0f;
		}

		yGravity -= CharacterData.MovmentGravity * Time.deltaTime;
		movementVelocity = new Vector3(movementVelocity.x, movementVelocity.y + yGravity, movementVelocity.z);
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

	private void Update()
	{
		CalculateVelocity();
		AddGravityOnCharacterController();
		CollisionFlags falgs = CharacterController.Move(MovementVelocity);
	}

	private void LateUpdate()
	{
		KillZOffset();
	}

	private void OnControllerColliderHit(ControllerColliderHit hit)
	{

	}
}
