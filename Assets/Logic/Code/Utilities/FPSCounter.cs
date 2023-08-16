using System.Collections;
using UnityEngine;

public class FPSCounter : MonoBehaviour
{
	public float updateInterval = 0.5f; // Intervall, in dem die FPS aktualisiert werden

	private float accumulatedFrames = 0f;
	private float accumulatedTime = 0f;
	private float fps = 0f;
	private GUIStyle style;

	private void Start()
	{
		style = new GUIStyle();
		style.fontSize = 24;
		style.normal.textColor = Color.white;

		StartCoroutine(UpdateFPS());
	}

	private IEnumerator UpdateFPS()
	{
		while (true)
		{
			float frameTime = Time.deltaTime;
			accumulatedFrames++;
			accumulatedTime += frameTime;

			if (accumulatedTime >= updateInterval)
			{
				fps = accumulatedFrames / accumulatedTime;
				accumulatedFrames = 0;
				accumulatedTime = 0;

				yield return new WaitForSeconds(updateInterval);
			}

			yield return null;
		}
	}

	private void OnGUI()
	{
		GUI.Label(new Rect(10, 10, 200, 50), "FPS: " + fps.ToString("F1"), style);
	}
}
