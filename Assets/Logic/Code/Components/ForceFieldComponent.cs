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
					return Ultra.Utilities.Remap(fadeTimer.GetProgess(), 0f, 1f, 1f, 0f);
			}
			return fadeTimer.GetProgess();
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
	}

	public void HARDReset()
	{
		SetMaterialValues(0);
		col.enabled = false;
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

	[ButtonMethod()]
	public void StartForceField()
	{
		isFadingIn = true;
		fadeTimer.onTimerFinished -= OnTimerFinished;
		fadeTimer.Start(FadeDuration);
		col.enabled = true;
	}

	[ButtonMethod()]
	public void EndForceField()
	{
		isFadingIn = false;
		fadeTimer.Start(FadeDuration);
		fadeTimer.onTimerFinished += OnTimerFinished;
	}

	void OnTimerFinished()
	{
		fadeTimer.onTimerFinished -= OnTimerFinished;
		SetMaterialValues(0);
		col.enabled = false;
	}

}
