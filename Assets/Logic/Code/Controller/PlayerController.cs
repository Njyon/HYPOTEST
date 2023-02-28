using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : ControllerBase
{
    [HideInInspector] public PlayerInputs playerInputs;
	CameraController camController;
	GameCharacter gameCharacter;

	public override void BeginPosses(GameObject pawn)
	{
		base.BeginPosses(pawn);

		SetupDefaultPlayerInputs();
		SetupCamera(pawn);
		SetupGameCharacter(pawn);
	}

	private void SetupGameCharacter(GameObject pawn)
	{
		gameCharacter = pawn.AddComponent<GameCharacter>();
	}

	private void SetupCamera(GameObject pawn)
	{
		camController = Camera.main.GetComponent<CameraController>();
		if (!camController) camController = Camera.main.gameObject.AddComponent<CameraController>();
		camController.OnPosses(pawn);
	}

	public override void EndPosses()
	{
		base.EndPosses();
	}

	void SetupDefaultPlayerInputs()
	{
		playerInputs = new PlayerInputs();
		playerInputs.Enable();

		playerInputs.Default.HorizontalInput.performed += ctx => HorizontalInput(ctx.ReadValue<float>());
		playerInputs.Default.VerticalInput.performed += ctx => VerticalInput(ctx.ReadValue<float>());
		playerInputs.Default.HorizontalInput.canceled += ctx => HorizontalInput(ctx.ReadValue<float>());
		playerInputs.Default.VerticalInput.canceled += ctx => VerticalInput(ctx.ReadValue<float>());
		playerInputs.Default.Jump.performed += ctx => Jump();
	}

	void HorizontalInput(float axis)
	{
		gameCharacter?.HorizontalMovementInput(axis);
	}

	void VerticalInput(float axis)
	{
		gameCharacter?.VerticalMovmentInput(axis);
	}

	void Jump()
	{

	}

	private void OnDisable()
	{
		if (playerInputs != null) playerInputs.Disable();
	}

	private void OnEnable()
	{
		if (playerInputs != null) playerInputs.Enable(); 
	}

	private void OnDestroy()
	{
		if (playerInputs != null) playerInputs.Disable();
		playerInputs = null;
	}
}
