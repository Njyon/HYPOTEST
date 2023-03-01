using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ACameraState : IState<CameraStates>
{
	CameraStateMachine stateMachine;
	public CameraStateMachine StateMachine { get { return stateMachine; } }
	CameraController camController;
	public CameraController CameraController { get { return camController; } }
	GameCharacter gameCharacter;
	public GameCharacter GameCharacter { get { return gameCharacter; } }

	public ACameraState(CameraStateMachine stateMachine, CameraController camController, GameCharacter gameCharacter)
	{
		this.stateMachine = stateMachine;
		this.camController = camController;
		this.gameCharacter = gameCharacter;
	}

	public abstract void EndState(CameraStates newState);
	public abstract void ExecuteState(float deltaTime);
	public abstract void FixedExecuteState(float deltaTime);
	public abstract void LateExecuteState(float deltaTime);
	public abstract CameraStates GetStateType();
	public abstract void StartState(CameraStates oldState);
	public abstract CameraStates UpdateState(float deltaTime, CameraStates newStateRequest);
}
