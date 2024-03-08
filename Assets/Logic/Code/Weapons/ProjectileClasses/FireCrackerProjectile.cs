using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireCrackerProjectile : WeaponProjectile
{
	[SerializeField] ParticleSystem explosionEffect;
	[SerializeField] float downForce = 5f;
	[SerializeField] int cameraShakeIndex = 0;
	[SerializeField] float afterTimeExplosionRadius = 1f;

	public override bool OnHit(Collider other)
	{
		if (other.isTrigger) return false;
		if (other.gameObject == gameCharacterOwner.gameObject) return false;

		GameCharacter gc = other.GetComponent<GameCharacter>();
		if (gc == null || gc.gameObject == gameCharacterOwner.gameObject) return false;

		gc.DoDamage(gameCharacterOwner, damage, true);
		if (gc.CombatComponent.CanRequestFlyAway())
		{
			gc.CombatComponent.RequestFlyAway(1f);
			gc.MovementComponent.MovementVelocity = Vector3.down * downForce;
			//gc.BuffComponent.AddBuff(new HoldInAirAfterStartFallingBuff(gc, 5f));
		}

		GameObject.Instantiate(explosionEffect.gameObject, transform.position, transform.rotation);
		CameraController.Instance?.ShakeCamerea(cameraShakeIndex);
		return true;
	}

	protected override void OnTimerFinished()
	{
		bool didHit = false;

		Collider[] colliders = Physics.OverlapSphere(transform.position, afterTimeExplosionRadius, gameCharacterOwner.CharacterLayer, QueryTriggerInteraction.Ignore);
		foreach (Collider collider in colliders)
		{
			if (OnHit(collider)) 
				 if (!didHit) 
					didHit = true;
		}

		if (!didHit)
		{
			GameObject.Instantiate(explosionEffect.gameObject, transform.position, transform.rotation);
			CameraController.Instance?.ShakeCamerea(cameraShakeIndex);
		}

		base.OnTimerFinished();
	}
}
