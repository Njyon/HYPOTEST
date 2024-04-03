using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;

public class EnviromentDamageComponent : MonoBehaviour
{
	[SerializeField] float damageAmount = 10f;
	[SerializeField] float damageToEnemyAmount = 100f;
	[SerializeField] bool resetGameCharacterToSpawnPoint = false;

	public async void DoDamage(GameCharacter gameCharacter)
	{
		if (resetGameCharacterToSpawnPoint)
		{
 			gameCharacter.StateMachine.ForceStateChange(EGameCharacterState.Standing, true);
			gameCharacter.MovementComponent.MovementVelocity = Vector3.zero;
			gameCharacter.MovementComponent.MovementOverride = Vector3.zero;
			gameCharacter.MovementComponent.enabled = false;
			gameCharacter.MovementComponent.IsInJump = false;
			gameCharacter.MovementComponent.MovementOverrideTime = 0.15f;
			gameCharacter.PluginStateMachine.AddPluginState(EPluginCharacterState.MovementOverride);
			gameCharacter.AnimController.ResetAnimStatesHARD();
			gameCharacter.DoDamage(null, damageAmount);
			await new WaitForSeconds(0.15f);
			gameCharacter.transform.position = gameCharacter.RespawnObj.transform.position;
			gameCharacter.MovementComponent.enabled = true;
		}
		else
		{
			if (gameCharacter.IsPlayerCharacter)
			{
				gameCharacter.DoDamage(null, damageAmount);
			}else
			{
				gameCharacter.DoDamage(null, damageToEnemyAmount);
			}
		}
	}
}
