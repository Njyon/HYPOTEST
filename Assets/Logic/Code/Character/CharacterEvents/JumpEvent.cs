using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpEvent : CharacterEvent
{
	public JumpEvent(GameCharacter gameCharacter, float time = 0.1F) : base(gameCharacter, time)
	{ }

	public override bool CanBeExecuted()
	{
		switch (gameCharacter.StateMachine.GetCurrentStateType())
		{
			case EGameCharacterState.Attack: case EGameCharacterState.Freez: return false;
			default: break;
		}
		if (gameCharacter.CurrentJumpAmount < gameCharacter.GameCharacterData.MaxJumps) return true;
		return false;
	}

	public override EGameCharacterEvent GetGameCharacterEvenetType()
	{
		return EGameCharacterEvent.Jump;
	}

	public override void StartEvent()
	{
		var jumpVelocity = Mathf.Sqrt(-2 * -gameCharacter.GameCharacterData.MovmentGravity * gameCharacter.GameCharacterData.JumpForce);
		gameCharacter.MovementComponent.MovementVelocity = new Vector3(gameCharacter.MovementComponent.MovementVelocity.x, jumpVelocity, gameCharacter.MovementComponent.MovementVelocity.z);
		gameCharacter.CurrentJumpAmount++;
		gameCharacter.MovementComponent.IsInJump = true;
		if (gameCharacter.StateMachine.GetCurrentStateType() != EGameCharacterState.InAir) gameCharacter.StateMachine.ForceStateChange(EGameCharacterState.InAir, true);
		gameCharacter.AnimController.Jump();
		if (gameCharacter.IsPlayerCharacter) Ultra.Utilities.Instance.DebugLogOnScreen("JumpVel: " + jumpVelocity, 2f, 200, DebugAreas.Movement);

		gameCharacter.AnimController.InAttack = false;
		gameCharacter.AnimController.InAimBlendTree = false;
		gameCharacter.AnimController.InFreez = false;
		gameCharacter.AnimController.HoldAttack = false;
		gameCharacter.AnimController.TriggerAttack = false;
		gameCharacter.AnimController.InDefensiveAction = false;
		gameCharacter.AnimController.InCombat3Blend = false;
	}
}
