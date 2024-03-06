using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

[Icon("d_UnityEditor.GameView@2x")]
public class PlayerController : ControllerBase
{
	[HideInInspector] public PlayerInputs playerInputs;
	CameraController camController;
	PlayerGameCharacter gameCharacter;

	AttackEvent holdAttack;
	float defaultHoldTime = 0.4f;

	//DebugStuff
	bool bForcedFrameRate = false;

	private void Awake()
	{
		int lol = 1;
		int z = lol;
	}

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
		LoadingChecker.Instance.Tasks.Add(Task.Run(async () => {
			while (!gameCharacter.IsInitialized)
			{
				await Task.Yield();
			}
		}));
		gameCharacter.CharacterData = characterData;
		gameCharacter.IsPlayerCharacter = true;
		gameCharacter.CustomAwake();
		gameCharacter.Team = HyppoliteTeam.TeamPlayer;
		GameCharacterMovementComponent movementComponent = pawn.GetComponent<GameCharacterMovementComponent>();
		if (movementComponent != null) movementComponent.SetupGameCharacter(gameCharacter);

		gameCharacter.onGameCharacterDied += OnGameCharacterDied;
		gameCharacter.onGameCharacterRespawnes += OnGameCharacterRespawnes;
	}

	protected override void OnGameCharacterDied(GameCharacter gameCharacter)
	{
		playerInputs.Default.Disable();
		this.gameCharacter?.EventComponent?.AddEvent(null);
	}

	void OnGameCharacterRespawnes(GameCharacter gameCharacter)
	{
		playerInputs.Default.Enable();
	}

	async void SetupCamera(GameObject pawn)
	{
		await new WaitUntil(() => { return Camera.main != null; });
		camController = Camera.main?.GetComponent<CameraController>();
		if (!camController) camController = Camera.main?.gameObject.AddComponent<CameraController>();
		if (camController) camController.OnPosses(pawn);
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
		playerInputs.Default.DefensiveAction.canceled += ctx => DefensiveActionEnd();
		playerInputs.Default.Dodge.performed += ctx => Dodge();
		playerInputs.Default.GapCloser.performed += ctx => GapCloser();
		playerInputs.Default.Ultimate.performed += ctx => Ultimate();

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

		gameCharacter?.PluginStateMachine?.AddPluginState(EPluginCharacterState.HoldAttack);

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
		gameCharacter?.PluginStateMachine?.RemovePluginState(EPluginCharacterState.HoldAttack);
		gameCharacter?.EventComponent?.RemoveHoldEvent(holdAttack);
	}
	void GapCloser()
	{
		CharacterEvent previousAttackEvent = GetFistEventOfType(EGameCharacterEvent.Attack, ref gameCharacter.EventComponent.previousEventsOverTimeFrame);
		gameCharacter?.EventComponent?.AddEvent(new AttackEvent(gameCharacter, EAttackType.GapCloser, previousAttackEvent != null ? previousAttackEvent.inputTime : -1));
	}
	void DefensiveAction()
	{
		gameCharacter?.EventComponent?.AddEvent(new DefensiveEvent(gameCharacter));
		gameCharacter?.PluginStateMachine?.AddPluginState(EPluginCharacterState.DefensiveActionHold);
	}
	void DefensiveActionEnd()
	{
		gameCharacter?.PluginStateMachine?.RemovePluginState(EPluginCharacterState.DefensiveActionHold);
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

	void Ultimate()
	{
		CharacterEvent previousAttackEvent = GetFistEventOfType(EGameCharacterEvent.Attack, ref gameCharacter.EventComponent.previousEventsOverTimeFrame);
		gameCharacter?.EventComponent?.AddEvent(new AttackEvent(gameCharacter, EAttackType.Ultimate, previousAttackEvent != null ? previousAttackEvent.inputTime : -1));
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
        if (gameCharacter != null)
		{
			gameCharacter.onGameCharacterDied -= OnGameCharacterDied;
			gameCharacter.onGameCharacterRespawnes -= OnGameCharacterRespawnes;
		}

		if (playerInputs != null) playerInputs.Disable();
		playerInputs = null;
	}

	private void ForceFrameRate()
	{
		if (bForcedFrameRate)
		{
			Application.targetFrameRate = 150;
			bForcedFrameRate = false;
		}
		else
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

		GameTimeManager.Instance.ToggleTimeManipulation("Debug", 0.2f);
		return;
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
		foreach (ScriptableWeapon sWeapon in gameCharacter.CombatComponent.Weapons)
		{
			if (sWeapon == null || sWeapon.Weapon == null) continue;
			sWeapon.Weapon.UltCharge = sWeapon.MaxUltChargeAmount;
		} 
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
