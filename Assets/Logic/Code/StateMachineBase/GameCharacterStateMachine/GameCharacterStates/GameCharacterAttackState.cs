using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCharacterAttackState : AGameCharacterState
{
	float initYVelocity;
	float initXVelocity;
	float lerpTimeY;
	float lerpTimeX;
	public GameCharacterAttackState(GameCharacterStateMachine stateMachine, GameCharacter gameCharacter) : base (stateMachine, gameCharacter)
	{ }

    public override void StartState(EGameCharacterState oldState)
	{
		GameCharacter.AnimController.InAttack = true;
		GameCharacter.AnimController.InterpSecondaryMotionLayerWeight(0, 10f);
		initYVelocity = GameCharacter.MovementVelocity.y;
		initXVelocity = GameCharacter.MovementVelocity.x;
		lerpTimeY = 0;
		lerpTimeX = 0;
	}

	public override EGameCharacterState GetStateType()
	{
		return EGameCharacterState.Attack;
	}

	public override EGameCharacterState UpdateState(float deltaTime, EGameCharacterState newStateRequest)
	{
		switch (newStateRequest)
		{
 			case EGameCharacterState.AttackRecovery: return EGameCharacterState.AttackRecovery;
			default: break;
		}

		return GetStateType();
	}

	public override void ExecuteState(float deltaTime)
	{
		lerpTimeY += deltaTime * GameCharacter.GameCharacterData.AirToZeroVelYInAttackSpeed;
		lerpTimeX += deltaTime * GameCharacter.GameCharacterData.AirToZeroVelXInAttackSpeed;
		float yMotion = GameCharacter.RootmotionVector.y + Mathf.Lerp(initYVelocity, 0, lerpTimeY);
		float xMotion = GameCharacter.RootmotionVector.x + Mathf.Lerp(initXVelocity, 0, lerpTimeX);
		Vector3 rootmotionVector = new Vector3(xMotion, yMotion, 0);
		GameCharacter.MovementVelocity = rootmotionVector;
	}
	
	public override void FixedExecuteState(float deltaTime)
	{
	
	}

	public override void LateExecuteState(float deltaTime)
	{
	
	}

	public override void EndState(EGameCharacterState newState)
	{
	
	}
}