using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

[Icon("d_UnityEditor.GameView@2x")]
public class PlayerController : ControllerBase
{
    [HideInInspector] public PlayerInputs playerInputs;
	CameraController camController;
	PlayerGameCharacter gameCharacter;

	public CharacterSpawner spawner;
	AttackEvent holdAttack;
	float defaultHoldTime = 0.4f;

	//DebugStuff
	bool bForcedFrameRate = false;
	

	public override void BeginPosses(GameObject pawn, ScriptableCharacter characterData)
	{
		base.BeginPosses(pawn, characterData);

		SetupDefaultPlayerInputs();
		SetupGameCharacter(pawn);
		SetupCamera(pawn);

	}

	private void SetupGameCharacter(GameObject pawn)
	{
		gameCharacter = pawn.AddComponent<PlayerGameCharacter>();
		gameCharacter.CharacterData = characterData;
		gameCharacter.IsPlayerCharacter = true;
		gameCharacter.CustomAwake();
		gameCharacter.Team = HyppoliteTeam.TeamPlayer;
		GameCharacterMovementComponent movementComponent = pawn.GetComponent<GameCharacterMovementComponent>();
		if (movementComponent != null) movementComponent.SetupGameCharacter(gameCharacter);

		gameCharacter.onGameCharacterDied += OnGameCharacterDied;
	}

	protected override void OnGameCharacterDied(GameCharacter gameCharacter)
	{
		playerInputs.Default.Disable();
		this.gameCharacter?.EventComponent?.AddEvent(null);
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
		playerInputs.Default.AllDebugAreasOff.performed += ctx => AllDebugAreasOff();

		playerInputs.Default.WeaponOne.performed += ctx => EquipWeaponOne();
		playerInputs.Default.WeaponTwo.performed += ctx => EquipWeaponTwo();
		playerInputs.Default.WeaponThree.performed += ctx => EquipWeaponThree();
		playerInputs.Default.WeaponFour.performed += ctx => EquipWeaponFour();
		playerInputs.Default.NextWeapon.performed += ctx => EquipNextWeapon();
		playerInputs.Default.PreviousWeapon.performed += ctx => EquipPreviousWeapon();
		//playerInputs.Default.ScrollThrouhWeapos.performed += ctx => Scroll(ctx.ReadValue<float>());
		playerInputs.Default.Attack.performed += ctx => Attack();
		playerInputs.Default.Attack.canceled += ctx => AttackEnd();
		playerInputs.Default.DefensiveAction.performed += ctx => DefensiveAction();
		playerInputs.Default.Dodge.performed += ctx => Dodge();
		playerInputs.Default.ForceFrameRate.performed += ctx => ForceFrameRate();
		playerInputs.Default.DebugPauseGame.performed += ctx => DebugPauseGame();
		playerInputs.Default.DebugSlomo.performed += ctx => DebugSlomo();

		playerInputs.Default.DebugButton01.performed += ctx => DebugButton01();
		playerInputs.Default.DebugButton02.performed += ctx => DebugButton02();
		playerInputs.Default.DebugButton03.performed += ctx => DebugButton03();
		playerInputs.Default.DebugButton04.performed += ctx => DebugButton04();
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
		gameCharacter?.EventComponent?.AddEvent(new JumpEvent(gameCharacter));
	}
	void EquipWeaponOne()
	{
		gameCharacter?.EventComponent?.AddEvent(new WeaponSwitchEvent(gameCharacter, 0));
	}
	void EquipWeaponTwo()
	{
		gameCharacter?.EventComponent?.AddEvent(new WeaponSwitchEvent(gameCharacter, 1));
	}
	void EquipWeaponThree()
	{
		gameCharacter?.EventComponent?.AddEvent(new WeaponSwitchEvent(gameCharacter, 2));
	}
	void EquipWeaponFour()
	{
		gameCharacter?.EventComponent?.AddEvent(new WeaponSwitchEvent(gameCharacter, 3));
	}
	void EquipNextWeapon()
	{
		gameCharacter?.EventComponent?.AddEvent(new WeaponSwitchEvent(gameCharacter, 0, EWeaponSwitchType.NextWeapon));
	}
	void EquipPreviousWeapon()
	{
		gameCharacter?.EventComponent?.AddEvent(new WeaponSwitchEvent(gameCharacter, 0, EWeaponSwitchType.PreviousWeapon));
	}
	void Scroll(float axis)
	{
		if (axis > 0) EquipNextWeapon();
		else EquipPreviousWeapon();
	}
	void Attack()
	{
		CharacterEvent previousAttackEvent = GetFistEventOfType(EGameCharacterEvent.Attack, ref gameCharacter.EventComponent.previousEventsOverTimeFrame);

		float directionTreshold = 0.5f;
		Vector2 movementVector = gameCharacter.MovementInput;
		bool hasDirection = Mathf.Abs(movementVector.y) > directionTreshold;
		if (!hasDirection)
		{
			gameCharacter?.EventComponent?.AddEvent(new AttackEvent(gameCharacter, EAttackType.Default, previousAttackEvent != null ? previousAttackEvent.inputTime : -1));
		}
		else
		{
			if (movementVector.y > 0)
			{
				gameCharacter?.EventComponent?.AddEvent(new AttackEvent(gameCharacter, EAttackType.AttackUp, previousAttackEvent != null ? previousAttackEvent.inputTime : -1));
			}
			else
			{
				gameCharacter?.EventComponent?.AddEvent(new AttackEvent(gameCharacter, EAttackType.AttackDown, previousAttackEvent != null ? previousAttackEvent.inputTime : -1));
			}
		}
		holdAttack = new AttackEvent(gameCharacter, EAttackType.AttackHorizontal, previousAttackEvent != null ? previousAttackEvent.inputTime : -1, defaultHoldTime);
		gameCharacter?.EventComponent?.AddHoldEvent(holdAttack);
	}
	void AttackEnd()
	{
		gameCharacter?.EventComponent?.RemoveHoldEvent(holdAttack);
	}
	void DefensiveAction()
	{
		gameCharacter?.EventComponent?.AddEvent(new DefensiveEvent(gameCharacter));
	}
	void Dodge()
	{
		gameCharacter?.EventComponent?.AddEvent(new DodgeEvent(gameCharacter));
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
	void AllDebugAreasOff()
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

	private void ForceFrameRate()
	{
		if (bForcedFrameRate) {
			Application.targetFrameRate = - 999;
			bForcedFrameRate = false;
		}else
		{
			Application.targetFrameRate = 10;
			bForcedFrameRate = true;
		}
	}

	void DebugPauseGame()
	{
		Debug.Break();
	}

	void DebugSlomo()
	{
		if (Time.timeScale >= 1)
		{
			Time.timeScale = 0.2f;
		}
		else
		{
			Time.timeScale = 1f;
		}
	}

	void DebugButton01()
	{
		gameCharacter.Health.AddCurrentValue(-10f);
	}

	void DebugButton02()
	{
		gameCharacter.Health.AddCurrentValue(10f);

	}

	void DebugButton03()
	{
		spawner.Spawn();
	}

	void DebugButton04()
	{

	}

	CharacterEvent GetFistEventOfType(EGameCharacterEvent eventType, ref List<CharacterEvent> list)
	{
		for (int i = list.Count - 1; i >= 0; i--)
		{
			if (list[i].GetCharacterEvenetType() == eventType)
			{
				return list[i];	
			}
		}
		return null;
	}
}
