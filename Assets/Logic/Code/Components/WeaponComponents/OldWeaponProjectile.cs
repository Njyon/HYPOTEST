using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class OldWeaponProjectile : MonoBehaviour
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
	bool flyInDirection = false;
	bool lerpToCharacter;
	float t;
	Vector3 from;
	GameCharacter toCharacter;
	Ultra.Timer timer;
	Vector3 scale;
	LineRenderer lineRenderer;

	public Vector3 Direction { get { return direction; } }
	public GameCharacter GameCharacterOwner { get { return gameCharacterOwner; } }

	public void Initialize(GameCharacter Owner, Vector3 direction)
	{
		gameCharacterOwner = Owner;
		this.direction = direction;
		flyInDirection = true;
		lineRenderer = gameObject.GetComponent<LineRenderer>();
		lineRenderer.enabled = false;

		Init();
	}

	public void Initialize(GameCharacter Owner, Vector3 from, GameCharacter toCharacter)
	{
		gameCharacterOwner = Owner;
		this.toCharacter = toCharacter;
		this.from = from;
		lerpToCharacter = true;
		t = 0;
		lineRenderer = gameObject.GetComponent<LineRenderer>();
		lineRenderer.positionCount = 2;

		Init();
	}

	private void Init()
	{
		initilized = true;
		colliderScript = GetComponent<ColliderHitScript>();
		capsuleCollider = GetComponent<CapsuleCollider>();
		colliderScript.onOverlapEnter += OnOverlapEnter;
		timer = new Ultra.Timer();
		timer.onTimerFinished += OnTimerFinished;
		timer.Start(aliveTime);
		scale = transform.localScale;
	}

	private void Awake()
	{

	}

	// Update is called once per frame
	void Update()
    {
		if (lineRenderer != null)
		{
			lineRenderer.SetPosition(0, transform.position);
			lineRenderer.SetPosition(1, gameCharacterOwner.GameCharacterData.HandROnjectPoint.position);
		}
	

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

			if (flyInDirection)
				transform.position = transform.position + Direction.normalized * (speed * Time.deltaTime);

			if (lerpToCharacter)
			{
				t += Time.deltaTime * 2f;
				transform.position = Vector3.Lerp(from, toCharacter.MovementComponent.CharacterCenter, t);
				transform.rotation = Quaternion.LookRotation((toCharacter.MovementComponent.CharacterCenter - lineRenderer.GetPosition(1)).normalized, Vector3.up);
			}
		}
	}

	private void OnDestroy()
	{
		if (colliderScript != null) colliderScript.onOverlapEnter -= OnOverlapEnter;
		if (timer != null) timer.onTimerFinished -= OnTimerFinished;
		lineRenderer.enabled = false;
	}

	void OnOverlapEnter(Collider other)
	{
		if (other.transform == this.transform) return;
		GameCharacter otherChracter = other.GetComponent<GameCharacter>();
		if (otherChracter == null || otherChracter == gameCharacterOwner) return;

		if (hit) return; 
		hit = true;
		if (flyInDirection)
		{
			transform.position = transform.position + Direction.normalized * moveDistance;
			transform.rotation = Quaternion.LookRotation(Direction, Vector3.up);
		}
		if (lerpToCharacter)
		{
 			transform.position = otherChracter.MovementComponent.CharacterCenter + -transform.forward * 1.5f;
		}
		capsuleCollider.enabled = false;

		if (onProjectileHit != null) onProjectileHit(other.gameObject);
		transform.parent = other.transform;
		transform.localScale = scale;
		timer.AddTime(1);

	}

	void OnTimerFinished()
	{
		Destroy(this.gameObject);
	}
}
