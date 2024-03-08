using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCharacterDodgeState : AGameCharacterState
{
	Vector3 targetPosition;
	Vector3 startPosition;
	Ultra.Timer iFrameTimer;
	Ultra.Timer minDodgeTimeTimer;
	float minDodgeTime = 0.2f;
	public GameCharacterDodgeState(GameCharacterStateMachine stateMachine, GameCharacter gameCharacter) : base (stateMachine, gameCharacter)
	{
		minDodgeTimeTimer = new Ultra.Timer();	
		iFrameTimer = new Ultra.Timer(gameCharacter.GameCharacterData.IFrameTime, true);
		iFrameTimer.onTimerStarted += OnTimerStarted;
		iFrameTimer.onTimerFinished += OnTimerFinished;
	}

    public override void StartState(EGameCharacterState oldState)
	{
		startPosition = GameCharacter.transform.position;
		Vector3 dir = Mathf.Abs(GameCharacter.MovementInput.magnitude) > 0 ? GameCharacter.MovementInput.normalized : GameCharacter.transform.forward;
		targetPosition = GameCharacter.transform.position + dir * GameCharacter.GameCharacterData.DodgeDistance;

		GameCharacter.MovementComponent.onMoveCollisionFlag += OnMoveCollisionFlag;
		Ultra.Utilities.DrawArrow(GameCharacter.transform.position, Vector3.up, 2f, Color.green, 2f, 100, DebugAreas.Combat);
		Ultra.Utilities.DrawArrow(GameCharacter.transform.position, dir, GameCharacter.GameCharacterData.DodgeDistance, Color.cyan, 2f, 100, DebugAreas.Combat);
		minDodgeTimeTimer.Start(minDodgeTime);
		iFrameTimer.Start();

		GameCharacter.AnimController.ResetAnimStatesHARD();
		GameCharacter.AnimController.InDodge = true;

		var ps = GameCharacter.DodgeParticleSystemPool.GetValue();
		ps.transform.position = GameCharacter.MovementComponent.CharacterCenter;
		ps.transform.rotation = Quaternion.LookRotation(dir.IgnoreAxis(EAxis.YZ).normalized, Vector3.up);
		GameCharacter.AnimController.InterpSecondaryMotionLayerWeight(0, 10f);
	}

	public override EGameCharacterState GetStateType()
	{
		return EGameCharacterState.Dodge;
	}

	public override EGameCharacterState UpdateState(float deltaTime, EGameCharacterState newStateRequest)
	{
		switch (newStateRequest)
		{
			default: break;
		}

		if (minDodgeTimeTimer.IsFinished)
		{
			if (newStateRequest != EGameCharacterState.Unknown)
				return newStateRequest;
		}
	
		return GetStateType();
	}

	public override void ExecuteState(float deltaTime)
	{
		minDodgeTimeTimer.Update(deltaTime);
		iFrameTimer.Update(deltaTime);

		if (Vector3.Distance(GameCharacter.transform.position, startPosition) >= GameCharacter.GameCharacterData.DodgeDistance)
		{
			GameCharacter.RequestBestCharacterState();
			Ultra.Utilities.DrawArrow(GameCharacter.transform.position, Vector3.up, 2f, Color.magenta, 2f, 100, DebugAreas.Combat);
			return;
		} else if (GameCharacter.transform.position.IgnoreAxis(EAxis.Z).IsNearlyEqual(targetPosition.IgnoreAxis(EAxis.Z), 0.1f)) {
			GameCharacter.RequestBestCharacterState();
			Ultra.Utilities.DrawArrow(GameCharacter.transform.position, Vector3.up, 2f, Color.red, 2f, 100, DebugAreas.Combat);
			return;
		}

		GameCharacter.MovementComponent.MovementVelocity = (targetPosition - GameCharacter.transform.position).normalized * (GameCharacter.GameCharacterData.DodgeSpeed); /** (Vector3.Distance(GameCharacter.transform.position, targetPosition)));*/
	}

	public override void FixedExecuteState(float deltaTime)
	{
	
	}

	public override void LateExecuteState(float deltaTime)
	{
	
	}

	public override void EndState(EGameCharacterState newState)
	{
		if (GameCharacter != null)
		{
			GameCharacter.MovementComponent.MovementVelocity = Vector3.zero;
			GameCharacter.AnimController.InDodge = false;
			GameCharacter.AnimController.InterpSecondaryMotionLayerWeight(1, 10f);
			GameCharacter.MovementComponent.onMoveCollisionFlag -= OnMoveCollisionFlag;

			GameCharacter.BuffComponent.AddBuff(new NoGravityBuff(GameCharacter, Ultra.HypoUttilies.GameMode.GetDefaultGameModeData().AfterCombatNoGravityTime));

			if (GameCharacter.MovementComponent.MovementVelocity.y < 0) GameCharacter.MovementComponent.MovementVelocity = new Vector3(GameCharacter.MovementComponent.MovementVelocity.x, 0, GameCharacter.MovementComponent.MovementVelocity.z);
		}

		if (iFrameTimer.IsRunning)
		{
			iFrameTimer.Stop();
			OnTimerFinished();
		}
	}

	void OnMoveCollisionFlag(CollisionFlags collisionFlag)
	{
		if ((collisionFlag & CollisionFlags.Sides) != 0 || (collisionFlag & CollisionFlags.Above) != 0 || (collisionFlag & CollisionFlags.Below) != 0)
		{
			GameCharacter.RequestBestCharacterState();
			Ultra.Utilities.DrawArrow(GameCharacter.transform.position, Vector3.up, 2f, Color.cyan, 2f, 100, DebugAreas.Combat);
		}
	}

	void OnTimerStarted(float lengh)
	{
		if (GameCharacter != null && GameCharacter.PluginStateMachine != null)
			GameCharacter.PluginStateMachine.AddPluginState(EPluginCharacterState.IFrame);
	}

	void OnTimerFinished()
	{
		if (GameCharacter != null && GameCharacter.PluginStateMachine != null)
			GameCharacter.PluginStateMachine.RemovePluginState(EPluginCharacterState.IFrame);
	}
}