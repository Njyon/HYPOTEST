using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemPool : ComponentPoolBase<ParticleSystem>
{
	ParticleSystem partilceInstance;
	public ParticleSystem ParticleInstance { get { return partilceInstance; } }

	public ParticleSystemPool(ParticleSystem instance, GameObject parent) : base(parent)
	{
		this.partilceInstance = instance;
	}

	public ParticleSystemPool(ParticleSystem instance, GameObject parent, int minSize) : base(parent, minSize)
	{
		this.partilceInstance = instance;
	}

	public override ParticleSystem GetValue()
	{
		if (!IsTStackInit || ElementsInStackAreNull()) InitStack();

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
		ParticleSystem value = GameObject.Instantiate(partilceInstance, Parent.transform);
		value.gameObject.name = ">> " + value.gameObject.name;
		value.gameObject.AddComponent<ParticleSystemCallBackComponent>();
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

	protected override void DestroyElement(ParticleSystem element)
	{
		if (element != null && element.gameObject != null)
			GameObject.Destroy(element.gameObject);
	}
}
