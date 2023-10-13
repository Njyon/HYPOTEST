using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCharacterDodgeState : AGameCharacterState
{
	Vector3 targetPosition;
	Vector3 startPosition;
	Ultra.Timer minDodgeTimeTimer;
	float minDodgeTime = 0.2f;
	public GameCharacterDodgeState(GameCharacterStateMachine stateMachine, GameCharacter gameCharacter) : base (stateMachine, gameCharacter)
	{
		minDodgeTimeTimer = new Ultra.Timer();	
	}

    public override void StartState(EGameCharacterState oldState)
	{
		startPosition = GameCharacter.transform.position;
		Vector3 dir = GameCharacter.MovementInput.x > 0 ? new Vector3(GameCharacter.MovementInput.x, 0, 0) : GameCharacter.transform.forward;
		targetPosition = GameCharacter.transform.position + dir * GameCharacter.GameCharacterData.DodgeDistance;

		GameCharacter.MovementComponent.onMoveCollisionFlag += OnMoveCollisionFlag;
		Ultra.Utilities.DrawArrow(GameCharacter.transform.position, Vector3.up, 2f, Color.green, 2f, 100, DebugAreas.Combat);
		minDodgeTimeTimer.Start(minDodgeTime);

		GameCharacter.AnimController.InDodge = true;
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


		if (Vector3.Distance(GameCharacter.transform.position, startPosition) >= GameCharacter.GameCharacterData.DodgeDistance)
		{
			GameCharacter.RequestBestCharacterState();
			Ultra.Utilities.DrawArrow(GameCharacter.transform.position, Vector3.up, 2f, Color.magenta, 2f, 100, DebugAreas.Combat);
			return;
		} else if (GameCharacter.transform.position.IgnoreAxis(EAxis.YZ).IsNearlyEqual(targetPosition.IgnoreAxis(EAxis.YZ), 0.1f)) {
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
		GameCharacter.MovementComponent.MovementVelocity = Vector3.zero;
		GameCharacter.AnimController.InDodge = false;
	}

	void OnMoveCollisionFlag(CollisionFlags collisionFlag)
	{
		if ((collisionFlag & CollisionFlags.Sides) != 0)
		{
			GameCharacter.RequestBestCharacterState();
			Ultra.Utilities.DrawArrow(GameCharacter.transform.position, Vector3.up, 2f, Color.cyan, 2f, 100, DebugAreas.Combat);
		}
	}
}