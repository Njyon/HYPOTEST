using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemPool : ComponentPoolBase<ParticleSystem>
{
	GameObject goInstance;
	public GameObject GameObjectInstance { get { return goInstance; } }

	public ParticleSystemPool(GameObject instance, GameObject parent) : base(parent)
	{
		this.goInstance = instance;
	}

	public ParticleSystemPool(GameObject instance, GameObject parent, int minSize) : base(parent, minSize)
	{
		this.goInstance = instance;
	}

	public override ParticleSystem GetValue()
	{
		if (!IsTStackInit) InitStack();

		if (NoMoreTInStack)
			SpawnValue();

		ParticleSystem ps = stack.Pop();

		ps.GetComponent<ParticleSystemCallBackComponent>().onParticleSystemStopped += OnParticleSystemStopped;
		var psModule = ps.main;
		psModule.stopAction = ParticleSystemStopAction.Callback;
		ActivateValue(ps);
		return ps;
	}

	protected override void SpawnValue()
	{
		GameObject go = GameObject.Instantiate(goInstance, Parent.transform);
		go.name = ">> " + go.name;
		ParticleSystem value = go.GetComponent<ParticleSystem>();
		go.AddComponent<ParticleSystemCallBackComponent>();
		stack.Push(value);
		DeactivateValue(value);
	}

	protected override void ActivateValue(ParticleSystem value)
	{
		value.gameObject.SetActive(true);
	}

	protected override void DeactivateValue(ParticleSystem value)
	{
		value.gameObject.SetActive(false);
	}

	void OnParticleSystemStopped(GameObject go)
	{
		go.GetComponent<ParticleSystemCallBackComponent>().onParticleSystemStopped -= OnParticleSystemStopped;
		var ps = go.GetComponent<ParticleSystem>();
		ReturnValue(ps);
	}
}
