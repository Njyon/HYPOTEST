using EasyButtons;
using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;


public class ForceFieldComponent : MonoBehaviour
{
	[SerializeField] Collider col;
	[SerializeField] MeshRenderer meshRenderer;
	[SerializeField] bool startActive = false;

	[Header("ForceField Animation")]
	[SerializeField] float fadeInDuration = 1f;
	[SerializeField] AnimationCurve fadeIn;
	[SerializeField] bool useFadeInAsOutReverse = true;
	[ConditionalField("useFadeInAsOutReverse", true)]
	[SerializeField] AnimationCurve fadeOut;
	[ConditionalField("useFadeInAsOutReverse", true)]
	[SerializeField] float fadeOutDuration;

	Ultra.Timer fadeTimer;
	bool isFadingIn = false;
	bool forceFieldIsOn = false;

	float FadeDuration
	{
		get
		{
			if (!isFadingIn)
			{
				if (useFadeInAsOutReverse)
					return fadeInDuration;
				else
					return fadeOutDuration;
			}
			else
			{
				return fadeInDuration;
			}
		}
	}
	AnimationCurve FadeCurve {
		get
		{
			if (!isFadingIn)
			{
				if (useFadeInAsOutReverse)
					return fadeIn;
				else
					return fadeOut;
			}
			else
			{
				return fadeIn;
			}
		} 
	}
	float Progress
	{
		get
		{
			if (!isFadingIn)
			{
				if (useFadeInAsOutReverse)
					return Ultra.Utilities.Remap(fadeTimer.GetProgress(), 0f, 1f, 1f, 0f);
			}
			return fadeTimer.GetProgress();
		}
	}

	int blendID;

	void Awake()
	{
		if (col == null)
			col = GetComponent<Collider>();

		if (meshRenderer == null)
			meshRenderer = GetComponent<MeshRenderer>();

		blendID = Shader.PropertyToID("_Blend");

		HARDReset();

		fadeTimer = new Ultra.Timer(true);

		if (startActive)
		{
			SetMaterialValues(1);
			col.enabled = true;
			forceFieldIsOn = true;
		}
	}

	public void HARDReset()
	{
		SetMaterialValues(0);
		col.enabled = false;
		forceFieldIsOn = false;	
		if (fadeTimer != null) fadeTimer.Stop();
	}

	private void SetMaterialValues(float value)
	{
		foreach (Material mat in meshRenderer.materials)
		{
			mat.SetFloat(blendID, value);
		}
	}

	void Update()
	{
		if (fadeTimer.IsRunning)
		{
			float value = FadeCurve.Evaluate(Progress);
			SetMaterialValues(value);
			fadeTimer.Update(Time.deltaTime);
		}
	}

	public async void StartForcefieldDelayed(float time)
	{
		await new WaitForSeconds(time);
		StartForceField();
	}

	public async void EndForcefieldDelayed(float time)
	{
		await new WaitForSeconds(time);
		EndForceField();
	}

	[ButtonMethod()]
	public void StartForceField()
	{
		if (!forceFieldIsOn)
		{
			isFadingIn = true;
			fadeTimer.onTimerFinished -= OnTimerFinished;
			fadeTimer.Start(FadeDuration);
			col.enabled = true;
			forceFieldIsOn = true;
		}
	}

	[ButtonMethod()]
	public void EndForceField()
	{
		if (forceFieldIsOn)
		{
			isFadingIn = false;
			fadeTimer.Start(FadeDuration);
			fadeTimer.onTimerFinished += OnTimerFinished;
			forceFieldIsOn = false;
		}
	}

	void OnTimerFinished()
	{
		fadeTimer.onTimerFinished -= OnTimerFinished;
		SetMaterialValues(0);
		col.enabled = false;
	}

}
