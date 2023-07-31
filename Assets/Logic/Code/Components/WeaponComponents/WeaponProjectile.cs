using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class WeaponProjectile : MonoBehaviour
{
	public delegate void OnProjectileHit(GameObject other);
	public OnProjectileHit onProjectileHit;

	[SerializeField] float speed = 5f;
	[SerializeField] float aliveTime = 5f;
	Vector3 direction;
	ColliderHitScript colliderScript;
	CapsuleCollider capsuleCollider;
	GameCharacter gameCharacterOwner;
	bool hit = false;
	float moveDistance = 0.5f;
	bool initilized = false;
	Ultra.Timer timer;

	public Vector3 Direction { get { return direction; } }
	public GameCharacter GameCharacterOwner { get { return gameCharacterOwner; } }

	public void Initialize(GameCharacter Owner, Vector3 direction)
	{
		gameCharacterOwner = Owner;
		this.direction = direction;
		initilized = true;

		colliderScript = GetComponent<ColliderHitScript>();
		capsuleCollider = GetComponent<CapsuleCollider>();
		colliderScript.onOverlapEnter += OnOverlapEnter;
		timer = new Ultra.Timer();
		timer.onTimerFinished += OnTimerFinished;
		timer.Start(aliveTime);
	}

	private void Awake()
	{

	}

	// Update is called once per frame
	void Update()
    {
		timer.Update(Time.deltaTime);
		if (!hit && initilized)
		{
			RaycastHit hit;
			if (Physics.Raycast(transform.position + Direction.normalized * (moveDistance * 2), Direction.normalized, out hit, (speed * Time.deltaTime) + moveDistance))
			{
				OnOverlapEnter(hit.collider);
				if (this.hit)
				{
					return;
				}
			}
			transform.position = transform.position + Direction.normalized * (speed * Time.deltaTime);
		}
    }

	private void OnDestroy()
	{
		if (colliderScript != null) colliderScript.onOverlapEnter -= OnOverlapEnter;
		if (timer != null) timer.onTimerFinished -= OnTimerFinished;
	}

	void OnOverlapEnter(Collider other)
	{
		if (other.transform == this.transform) return;
		GameCharacter otherChracter = other.GetComponent<GameCharacter>();
		if (otherChracter != null && otherChracter == gameCharacterOwner) return;

		if (hit) return; 
		hit = true;
		transform.position = transform.position + Direction.normalized * moveDistance;
		transform.rotation = Quaternion.LookRotation(Direction, Vector3.up);
		capsuleCollider.enabled = false;

		if (onProjectileHit != null) onProjectileHit(other.gameObject);
		transform.parent = other.transform;
		timer.AddTime(1);

	}

	void OnTimerFinished()
	{
		Destroy(this.gameObject);
	}
}
