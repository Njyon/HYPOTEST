using System.Collections;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;

public class BazookaMissileProjectile : WeaponProjectile
{
	[SerializeField] ParticleSystem explosionEffect;
	[SerializeField] int cameraShakeIndex = 0;
	[SerializeField] float afterTimeExplosionRadius = 1f;
	[SerializeField] float kickAwayStrenght = 15f;
	GameCharacter target;

	[Header("MOVEMENT")]
	[SerializeField] private float _speed = 15;
	[SerializeField] private float _rotateSpeed = 95;

	[Header("PREDICTION")]
	[SerializeField] private float _maxDistancePredict = 100;
	[SerializeField] private float _minDistancePredict = 5;
	[SerializeField] private float _maxTimePrediction = 5;
	private Vector3 _standardPrediction, _deviatedPrediction;

	[Header("DEVIATION")]
	[SerializeField] private float _deviationAmount = 50;
	[SerializeField] private float _deviationSpeed = 2;


	protected override void Init_Intern()
	{
		base.Init_Intern();
		rigidBody = GetComponent<Rigidbody>();
	}

	public override bool OnHit(Collider other)
	{
		if (other.isTrigger) return false;
		if (other.gameObject == gameCharacterOwner.gameObject) return false;
		var projectile = other.GetComponent<WeaponProjectile>();
		if (projectile != null) return false;

		if (other.transform.parent != null)
		{
			Transform parent = other.transform.parent;
			if (parent != null)
			{
				while (parent.parent != null)
				{
					parent = parent.parent;
				}
			}
			if (parent.gameObject == gameCharacterOwner.gameObject) return false;
		}
		
		

		OnTimerFinished();

		return true;
	}

	protected override void OnTimerFinished()
	{
		Collider[] colliders = Physics.OverlapSphere(transform.position, afterTimeExplosionRadius, gameCharacterOwner.CharacterLayer, QueryTriggerInteraction.Ignore);
		foreach (Collider collider in colliders)
		{
			GameCharacter gc = collider.GetComponent<GameCharacter>();
			if (gc == null || gc.gameObject == gameCharacterOwner.gameObject) continue;

			gc.DoDamage(gameCharacterOwner, damage, true);
			if (gc.CombatComponent.CanRequestFlyAway())
			{
				gc.CombatComponent.RequestFlyAway(1f);
				gc.MovementComponent.MovementVelocity = (gc.MovementComponent.MovementVelocity - rigidBody.position).normalized * kickAwayStrenght;
			}
		}

		if (hitEffects != null && hitEffects.Count > 0)
		{
			SoundEffect exposionEffect = hitEffects[Random.Range(0, hitEffects.Count)];
			if (exposionEffect != null)
				SoundManager.Instance.PlaySound(exposionEffect);
		}

		GameObject.Instantiate(explosionEffect.gameObject, transform.position, transform.rotation);
		CameraController.Instance?.ShakeCamerea(cameraShakeIndex);
		base.OnTimerFinished();
	}

	public void SetTarget(GameCharacter target)
	{
		this.target = target;
	}

	private void FixedUpdate()
	{
		if (rigidBody == null) OnTimerFinished();

		rigidBody.velocity = transform.forward * _speed;

		if (target == null || target.MovementComponent == null) return;

		var leadTimePercentage = Mathf.InverseLerp(_minDistancePredict, _maxDistancePredict, Vector3.Distance(transform.position, target.MovementComponent.CharacterCenter));

		PredictMovement(leadTimePercentage);
		AddDeviation(leadTimePercentage);
		RotateRocket();
	}

	private void PredictMovement(float leadTimePercentage)
	{
		var predictionTime = Mathf.Lerp(0, _maxTimePrediction, leadTimePercentage);

		_standardPrediction = target.MovementComponent.CharacterCenter + target.MovementComponent.MovementVelocity * predictionTime;
	}

	private void AddDeviation(float leadTimePercentage)
	{
		var deviation = new Vector3(Mathf.Cos(Time.time * _deviationSpeed), 0, 0);

		var predictionOffset = transform.TransformDirection(deviation) * _deviationAmount * leadTimePercentage;

		_deviatedPrediction = _standardPrediction + predictionOffset;
	}

	private void RotateRocket()
	{
		var heading = _deviatedPrediction - transform.position;

		var rotation = Quaternion.LookRotation(heading);
		rigidBody.MoveRotation(Quaternion.RotateTowards(transform.rotation, rotation, _rotateSpeed * Time.deltaTime));
	}

	protected override void Move()
	{
		//
		//Vector3 direction = (target != null ? target.MovementComponent.CharacterCenter : rigidBody.position + gameCharacterOwner.transform.forward * 100) - rigidBody.position;
		//direction.Normalize();
		//Quaternion rot = Quaternion.LookRotation(direction);
		//rigidBody.MoveRotation(Quaternion.RotateTowards(transform.rotation, rot, rotateSpeed * Time.deltaTime));
		////Vector3 amountToRotate = Vector3.Cross(direction, transform.right) * Vector3.Angle(transform.right, direction);
		////rigidBody.angularVelocity = -amountToRotate * rotateSpeed;
		//rigidBody.velocity = transform.right * speed;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawLine(transform.position, _standardPrediction);
		Gizmos.color = Color.green;
		Gizmos.DrawLine(_standardPrediction, _deviatedPrediction);
	}
}
