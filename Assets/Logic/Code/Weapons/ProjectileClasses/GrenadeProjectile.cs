using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeProjectile : WeaponProjectile
{
	[SerializeField] ParticleSystem explosionEffect;
	[SerializeField] float explisionRadius = 1f;
	[SerializeField] float upForce = 5f;

	public override void OnHit(Collider other)
	{
		if (other.gameObject == gameCharacterOwner.gameObject) return;

		GameObject.Instantiate(explosionEffect.gameObject, transform.position, transform.rotation);
		
		Collider[] colliders = Physics.OverlapSphere(transform.position, explisionRadius, gameCharacterOwner.CharacterLayer, QueryTriggerInteraction.Ignore);
		foreach (Collider collider in colliders)
		{
			GameCharacter gc = collider.GetComponent<GameCharacter>();
			if (gc == null || gc.gameObject == gameCharacterOwner.gameObject) continue;

			if (gc.CombatComponent.CanRequestFlyAway())
			{
				gc.CombatComponent.RequestFlyAway(1f);
				gc.MovementComponent.MovementVelocity = Vector3.up * upForce;
			}
			
		}
		Ultra.Utilities.DrawWireSphere(transform.position, explisionRadius, Color.red, 1f, 100);
		OnTimerFinished();
	}
}
