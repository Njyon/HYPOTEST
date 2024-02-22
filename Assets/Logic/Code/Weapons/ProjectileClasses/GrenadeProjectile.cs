using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;

public class GrenadeProjectile : WeaponProjectile
{
	[SerializeField] ParticleSystem explosionEffect;
	[SerializeField] float explosionRadius = 1f;
	[SerializeField] float upForce = 5f;
	[SerializeField] int cameraShakeIndex = 2;

	public override bool OnHit(Collider other)
	{
		if (other.gameObject == gameCharacterOwner.gameObject) return false;

		Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius, gameCharacterOwner.CharacterLayer, QueryTriggerInteraction.Ignore);
		foreach (Collider collider in colliders)
		{
			GameCharacter gc = collider.GetComponent<GameCharacter>();
			if (gc == null || gc.gameObject == gameCharacterOwner.gameObject) continue;

			gc.DoDamage(gameCharacterOwner, damage, true);
			if (gc.CombatComponent.CanRequestFlyAway())
			{
				gc.CombatComponent.RequestFlyAway(1f);
				gc.MovementComponent.MovementVelocity = Vector3.up * upForce;
				gc.BuffComponent.AddBuff(new HoldInAirAfterStartFallingBuff(gc, 5f));
			}
			
		}
		Ultra.Utilities.DrawWireSphere(transform.position, explosionRadius, Color.red, 1f, 100);

		GameObject.Instantiate(explosionEffect.gameObject, transform.position, transform.rotation);
		OnTimerFinished();
		CameraController.Instance?.ShakeCamerea(cameraShakeIndex);
		return true;
	}
}
