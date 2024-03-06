using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasGroupAwakeSetter : MonoBehaviour
{
	[SerializeField] CanvasGroup targetCanvasGroup;
	[SerializeField] float startAlpha;

	void Awake()
	{
		if (targetCanvasGroup == null) 
			targetCanvasGroup = GetComponent<CanvasGroup>();

		targetCanvasGroup.alpha = startAlpha;
	}
}
