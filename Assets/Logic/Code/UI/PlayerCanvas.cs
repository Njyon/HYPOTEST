using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCanvas : MonoBehaviour
{
	[SerializeField] Canvas canvas;

	private void Awake()
	{
		UIManager.Instance.Canvas = canvas;
	}
}
