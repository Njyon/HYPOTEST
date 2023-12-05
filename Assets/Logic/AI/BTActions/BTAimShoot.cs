using Megumin.GameFramework.AI;
using Megumin.GameFramework.AI.BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;

[Category("Action")]
public class BTAimShoot : BTHyppoliteActionNodeBase
{
	[Header("AimShootData")]
	public float aimTime = 1f;

	Ultra.Timer aimTimer = new Ultra.Timer();
	LineRenderer lr;
	WeaponObjData weaponObjData;

	protected override void OnEnter(object options = null)
	{
		if (aimTimer.IsRunning) return;
		base.OnEnter(options);

		GameCharacter.CombatComponent.AimPositionCheck.Value = false;

		aimTimer.Start(aimTime);
		aimTimer.onTimerFinished += OnTimerFinished;

		weaponObjData = GameCharacter.CombatComponent.CurrentWeapon.SpawnedWeapon.GetComponent<WeaponObjData>();

		if (lr == null)
		{
			lr = GameObject.Instantiate(GameAssets.Instance.laserLineRenderer, GameCharacter.CreateHolderChild("RangeWeaponLineRendererHolder").transform);
		}
		if (weaponObjData != null)
		{
			lr.gameObject.SetActive(true);
			lr.SetPosition(0, weaponObjData.weaponTip.transform.position);
			lr.SetPosition(1, weaponObjData.weaponTip.transform.position);
		}

	}

	protected override Status OnTick(BTNode from, object options = null)
	{
		if (aimTimer != null) aimTimer.Update(Time.deltaTime);

		if (weaponObjData != null && lr != null) lr.SetPosition(0, weaponObjData.weaponTip.transform.position);
		Vector3 dir = TargetGameCharacter.MovementComponent.CharacterCenter - weaponObjData.weaponTip.transform.position;
		RaycastHit[] hits = Physics.RaycastAll(weaponObjData.weaponTip.transform.position, dir.normalized, dir.magnitude, -5, QueryTriggerInteraction.Ignore);
		RaycastHit finalHit = new RaycastHit();
		foreach (RaycastHit hit in hits)
		{
			if (hit.collider.transform.parent != null)
			{
				Transform parent = hit.collider.transform.parent;
				while (parent.parent != null)
				{
					parent = parent.parent;
				}

				if (parent.gameObject == TargetGameCharacter)
				{
					finalHit = hit;
					break;
				}else if (parent.gameObject.layer == GameCharacter.CharacterLayer)
				{
					continue;
				}
				else
				{
					finalHit = hit;
					break;
				}
			}else
			{
				if (hit.collider.gameObject == TargetGameCharacter)
				{
					finalHit = hit;
					break;
				}
				else if (hit.collider.gameObject.layer == GameCharacter.CharacterLayer)
				{
					continue;
				}
				else
				{
					finalHit = hit;
					break;
				}
			}
		}

		lr.SetPosition(1, Vector3.Lerp(lr.GetPosition(0), finalHit.point, aimTimer.GetProgess()));

		if (aimTimer.IsFinished)
			return Status.Succeeded;

		return Status.Running;
	}

	protected override void OnExit(Status result, object options = null)
	{
		base.OnExit(result, options);

		switch (result) 
		{
			case Status.Failed:
			case Status.Succeeded:
				lr.gameObject.SetActive(false);
				// something?
				break;
			default: break;
		}
	}

	void OnTimerFinished()
	{
		GameCharacter.CombatComponent.AimPositionCheck.Value = true;
		GameCharacter.CombatComponent.AimPositionCheck.Position = TargetGameCharacter.MovementComponent.CharacterCenter;
		// Do Effects?
	}
}
