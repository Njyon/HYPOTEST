using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : ControllerBase
{
    [HideInInspector] public PlayerInputs playerInputs;
	CameraController camController;
	GameCharacter gameCharacter;

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

		playerInputs.Default.WeaponOne.performed += ctx => EquipWeaponOne();
		playerInputs.Default.WeaponTwo.performed += ctx => EquipWeaponTwo();
		playerInputs.Default.WeaponThree.performed += ctx => EquipWeaponThree();
		playerInputs.Default.WeaponFour.performed += ctx => EquipWeaponFour();
		playerInputs.Default.NextWeapon.performed += ctx => EquipNextWeapon();
		playerInputs.Default.PreviousWeapon.performed += ctx => EquipPreviousWeapon();
		playerInputs.Default.ScrollThrouhWeapos.performed += ctx => Scroll(ctx.ReadValue<float>());
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
		gameCharacter?.EventComponent.AddEvent(new JumpEvent(gameCharacter));
	}
	void EquipWeaponOne()
	{
		gameCharacter?.EventComponent.AddEvent(new WeaponSwitchEvent(gameCharacter, 0));
	}
	void EquipWeaponTwo()
	{
		gameCharacter?.EventComponent.AddEvent(new WeaponSwitchEvent(gameCharacter, 1));
	}
	void EquipWeaponThree()
	{
		gameCharacter?.EventComponent.AddEvent(new WeaponSwitchEvent(gameCharacter, 2));
	}
	void EquipWeaponFour()
	{
		gameCharacter?.EventComponent.AddEvent(new WeaponSwitchEvent(gameCharacter, 3));
	}
	void EquipNextWeapon()
	{
		gameCharacter?.EventComponent.AddEvent(new WeaponSwitchEvent(gameCharacter, 0, EWeaponSwitchType.NextWeapon));
	}
	void EquipPreviousWeapon()
	{
		gameCharacter?.EventComponent.AddEvent(new WeaponSwitchEvent(gameCharacter, 0, EWeaponSwitchType.PreviousWeapon));
	}
	void Scroll(float axis)
	{
		if (axis > 0) EquipNextWeapon();
		else EquipPreviousWeapon();
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
