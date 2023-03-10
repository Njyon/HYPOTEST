using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : ControllerBase
{
    [HideInInspector] public PlayerInputs playerInputs;
	CameraController camController;
	GameCharacter gameCharacter;
	EventComponent eventComponent = new EventComponent();

	public override void BeginPosses(GameObject pawn, ScriptableCharacter characterData)
	{
		base.BeginPosses(pawn, characterData);

		SetupDefaultPlayerInputs();
		SetupGameCharacter(pawn);
		SetupCamera(pawn);
	}

	private void SetupGameCharacter(GameObject pawn)
	{
		gameCharacter = pawn.AddComponent<GameCharacter>();
		gameCharacter.CharacterData = characterData;
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
		playerInputs.Default.DebugLevelUp.performed += ctx => DebugUp();
		playerInputs.Default.DebugLevelDown.performed += ctx => DebugDown();
		playerInputs.Default.AllDebugAreasOn.performed += ctx => AllDebugAreasOn();
		playerInputs.Default.AllDebugAreasOff.performed += ctx => AlDebugAreasOff();
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
		if (gameCharacter == null) return;
		eventComponent.AddEvent(new JumpEvent(gameCharacter));
	}

	void DebugUp()
	{
		Ultra.Utilities.Instance.debugLevel += 100;
	}
	void DebugDown()
	{
		Ultra.Utilities.Instance.debugLevel -= 100;
	}
	void AllDebugAreasOn()
	{
		Ultra.Utilities.Instance.debugAreas = (DebugAreas)(-1);
	}
	void AlDebugAreasOff()
	{
		Ultra.Utilities.Instance.debugAreas = 0;
	}

	private void Update()
	{
		eventComponent?.Update(Time.deltaTime);
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
