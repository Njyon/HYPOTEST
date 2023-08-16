using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ACameraEffect
{
	CameraController cameraController = null;
	bool isPaused = false;
	bool isFinished = false;
	Ultra.Timer timer = null;

	public CameraController CameraController { get { return cameraController; } }
	public bool IsPaused { get { return isPaused; } }
	public bool IsFinished { get { return isFinished; } }
	public Ultra.Timer Timer { get { return timer; } }


	public ACameraEffect(CameraController controller, float effectDuration = 1f, bool isPaused = false)
	{
		this.cameraController = controller;
		this.isPaused = isPaused;
		timer = new Ultra.Timer(effectDuration, isPaused);
	}

	public abstract void DoEffect();
	public abstract void Update(float deltaTime);
	public abstract void CleanUp();

	public virtual void PauseEffect()
	{
		isPaused = true;
	}
	public virtual void ResumeEffect()
	{
		isPaused = false;
	}
	public virtual void EndEffect()
	{
		isFinished = true;
		CleanUp();
	}
}
