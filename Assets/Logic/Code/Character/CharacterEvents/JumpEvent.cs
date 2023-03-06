using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpEvent : CharacterEvent
{
	public JumpEvent(GameCharacter gameCharacter, float time = 0.2F) : base(gameCharacter, time)
	{ }

	public override bool CanBeExecuted()
	{
		if (gameCharacter.CurrentJumpAmount < gameCharacter.CharacterData.MaxJumps) return true;
		return false;
	}

	public override void StartEvent()
	{
		var jumpVelocity = Mathf.Sqrt(-2 * -gameCharacter.CharacterData.MovmentGravity * gameCharacter.CharacterData.JumpForce);
		gameCharacter.MovementVelocity = new Vector3(gameCharacter.MovementVelocity.x, jumpVelocity, gameCharacter.MovementVelocity.z);
		gameCharacter.CurrentJumpAmount++;
		gameCharacter.IsInJump = true;
		if (gameCharacter.StateMachine.GetCurrentStateType() != EGameCharacterState.InAir) gameCharacter.StateMachine.ForceStateChange(EGameCharacterState.InAir, true);
		Ultra.Utilities.Instance.DebugLogOnScreen("JumpVel: " + jumpVelocity, 2f, 200, DebugAreas.Movement);
	}

	public override void StopEvent()
	{

	}

	public override void Update(float deltaTime)
	{

	}
}
