using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnviromentDamageComponent : MonoBehaviour
{
	[SerializeField] float damageAmount = 10f;
	[SerializeField] float damageToEnemyAmount = 100f;
	[SerializeField] bool resetGameCharacterToSpawnPoint = false;

	[Header("SoundEffect")]
	public List<SoundEffect> soundEffects = new List<SoundEffect>();

	public async void DoDamage(GameCharacter gameCharacter)
	{
		if (soundEffects != null && soundEffects.Count > 0)
		{
			SoundEffect dodgeSound = soundEffects[Random.Range(0, soundEffects.Count)];
			if (dodgeSound != null)
				SoundManager.Instance.PlaySound(dodgeSound);
		}

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
