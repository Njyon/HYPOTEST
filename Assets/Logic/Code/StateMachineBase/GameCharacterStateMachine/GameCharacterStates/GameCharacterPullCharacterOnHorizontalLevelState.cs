using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCharacterPullCharacterOnHorizontalLevelState : AGameCharacterState
{
	Vector3 enemyCordinate;
	Vector3 enemyCordinateOnlyY;
	float interpolationSpeed = 20f;
	float distenceMultiplier = 100f;
	public GameCharacterPullCharacterOnHorizontalLevelState(GameCharacterStateMachine stateMachine, GameCharacter gameCharacter) : base (stateMachine, gameCharacter)
	{ }

    public override void StartState(EGameCharacterState oldState)
	{
		if (ShouldLeaveState())
		{
			GameCharacter.RequestBestCharacterState();
			return;
		}
		GameCharacter.MovementComponent.UseGravity = false;

		enemyCordinate = GameCharacter.CombatComponent.HookedToCharacter.transform.position + GameCharacter.CombatComponent.HookedToCharacter.MovementComponent.CapsuleCollider.center;
		enemyCordinateOnlyY = new Vector3(GameCharacter.transform.position.x ,enemyCordinate.y, GameCharacter.transform.position.z);

		GameCharacter.MovementComponent.onMoveCollisionFlag += OnMoveCollisionFlag;
	}

	public override EGameCharacterState GetStateType()
	{
		return EGameCharacterState.PullCharacterOnHorizontalLevel;
	}

	public override EGameCharacterState UpdateState(float deltaTime, EGameCharacterState newStateRequest)
	{
		if (newStateRequest != EGameCharacterState.Unknown) 
			return newStateRequest;
		return GetStateType();
	}

	public override void ExecuteState(float deltaTime)
	{
		if (Ultra.Utilities.IsNearlyEqual(GameCharacter.transform.position.y + GameCharacter.MovementComponent.CapsuleCollider.center.y, enemyCordinate.y + GameCharacter.MovementComponent.CapsuleCollider.center.y, 0.1f))
		{
			GameCharacter.MovementComponent.MovementVelocity = Vector3.zero;
			GameCharacter.StateMachine.RequestStateChange(EGameCharacterState.Freez);
			return;
		}

		Ultra.Utilities.DrawWireSphere(enemyCordinateOnlyY + GameCharacter.MovementComponent.CapsuleCollider.center, 0.5f, Color.red, 2f, 100, DebugAreas.Combat);
		GameCharacter.MovementComponent.MovementVelocity = (enemyCordinateOnlyY - GameCharacter.transform.position).normalized * (interpolationSpeed * (Vector3.Distance(GameCharacter.transform.position, enemyCordinateOnlyY) * distenceMultiplier)) * deltaTime;
	}
	
	public override void FixedExecuteState(float deltaTime)
	{
	
	}

	public override void LateExecuteState(float deltaTime)
	{
	
	}

	public override void EndState(EGameCharacterState newState)
	{
		GameCharacter.MovementComponent.onMoveCollisionFlag -= OnMoveCollisionFlag;
		GameCharacter.MovementComponent.UseGravity = true;
	}

	bool ShouldLeaveState()
	{
		if (GameCharacter == null || GameCharacter.CombatComponent == null || GameCharacter.CombatComponent.HookedToCharacter == null || GameCharacter.CombatComponent.HookedToCharacter.MovementComponent == null) return true;
		return false;
	}


	void OnMoveCollisionFlag(CollisionFlags collisionFlag)
	{
		if ((collisionFlag & CollisionFlags.Above) != 0)
		{
			GameCharacter.RequestBestCharacterState();
		}else if ((collisionFlag & CollisionFlags.Below) != 0)
		{
			//GameCharacter.RequestBestCharacterState();
			// Maybe Bounce here?
		}
	}
}