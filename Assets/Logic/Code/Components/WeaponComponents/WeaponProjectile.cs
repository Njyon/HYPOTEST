using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponProjectile : MonoBehaviour
{
	public delegate void OnProjectileHit(WeaponProjectile projectile, Collider other);
	public delegate void OnProjectileLifeTimeEnd(WeaponProjectile projectile);
	

	bool isInit = false;
	Vector3 dir;
	float speed;
	ColliderHitScript colliderScript;
	Collider projectileCollider;
	GameCharacter gameCharacterOwner;
	Rigidbody projectileRigidBody;
	Ultra.Timer lifeTimeTimer;
	OnProjectileHit onHit;
	OnProjectileLifeTimeEnd onLifeTimeEnd;
	TrailRenderer tr;


	public void Init(GameCharacter owner, Vector3 direction, float speed, OnProjectileHit onHit, OnProjectileLifeTimeEnd onProjectileLifeTimeEnd, float lifeTime = 5f)
	{
		gameCharacterOwner = owner;
		dir = direction;
		transform.rotation = Quaternion.LookRotation(dir);
		this.speed = speed;
		if (lifeTimeTimer == null) lifeTimeTimer = new Ultra.Timer();
		lifeTimeTimer.onTimerFinished += OnTimerFinished;
		lifeTimeTimer.Start(lifeTime);
		this.onHit = onHit;
		onLifeTimeEnd = onProjectileLifeTimeEnd;

		Init_Intern();

		isInit = true;
	}

	void Init_Intern()
	{
		if (colliderScript == null)
		{
			colliderScript = gameObject.GetComponent<ColliderHitScript>();
			if (colliderScript != null ) 
				colliderScript = gameObject.AddComponent<ColliderHitScript>();
		}
		projectileCollider = gameObject.GetComponent<Collider>();
		if (tr == null) tr = GetComponent<TrailRenderer>();
		if (tr != null) tr.enabled = true;

		colliderScript.onOverlapEnter += OnOverlapEnter;
	}

	void Update()
    {
		if (!isInit) return;
		if (lifeTimeTimer != null) lifeTimeTimer.Update(Time.deltaTime);

		transform.position = transform.position + dir.normalized * (speed * Time.deltaTime);
    }

	void RemoveSubscriptions()
	{
		if (colliderScript != null) colliderScript.onOverlapEnter -= OnOverlapEnter;
		if (lifeTimeTimer != null) lifeTimeTimer.onTimerFinished -= OnTimerFinished;
	}

	public void TurnOff()
	{
		isInit = false;
		if (tr != null) tr.enabled = false;	
		RemoveSubscriptions();
	}

	void OnDestroy()
	{
		RemoveSubscriptions();
	}

	void OnOverlapEnter(Collider other)
	{
		if (!isInit) return;
		if (onHit != null)
		{
			if (onHit != null) onHit(this, other);
		}
	}

	void OnTimerFinished()
	{
		if (onLifeTimeEnd != null) onLifeTimeEnd(this);
	}
}
