using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class ACameraModification
{
	GameCharacter gameCharacter;
	CameraController cameraController;

	protected GameCharacter GameCharacter {  get { return gameCharacter; } }
	protected CameraController CameraController { get { return cameraController; } }

	public virtual void Init(GameCharacter gameCharacter, CameraController cameraController)
	{
		this.gameCharacter = gameCharacter;
		this.cameraController = cameraController;
	}

	public abstract void DoOperation();
}
