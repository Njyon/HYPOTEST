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
	protected float damage;
	float currentGravityEffect;
	ColliderHitScript colliderScript;
	Collider projectileCollider;
	protected GameCharacter gameCharacterOwner;
	Rigidbody projectileRigidBody;
	Ultra.Timer lifeTimeTimer;
	OnProjectileHit onHit;
	OnProjectileLifeTimeEnd onLifeTimeEnd;
	TrailRenderer tr;

	public float gravity = 0.0f;

	public void Init(GameCharacter owner, Vector3 direction, float speed, float damage, OnProjectileHit onHit, OnProjectileLifeTimeEnd onProjectileLifeTimeEnd, float lifeTime = 5f)
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
		this.damage = damage;
		currentGravityEffect = 0;

		Init_Intern();

		isInit = true;
	}

	protected virtual void Init_Intern()
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

		currentGravityEffect += gravity * Time.deltaTime;

		transform.position = transform.position + dir.normalized * (speed * Time.deltaTime);
		if (Mathf.Abs(currentGravityEffect) > 0) transform.position = transform.position + Vector3.down * currentGravityEffect;
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
		OnHit(other);
		if (onHit != null) onHit(this, other);
	}

	protected void OnTimerFinished()
	{
		lifeTimeTimer.Stop();
		if (onLifeTimeEnd != null) onLifeTimeEnd(this);
	}

	public virtual void OnHit(Collider other) { }
}
