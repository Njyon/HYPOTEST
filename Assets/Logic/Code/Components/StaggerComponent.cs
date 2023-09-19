using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaggerComponent : RecourceBase
{
    GameCharacter gameCharacter;
	Ultra.Timer staggerTimer;
	bool refillStagger = false;

	public bool IsStaggerable 
	{ 
		get 
		{  
			if (CurrentValue <= 0) return true;
			return false; 
		}
	}

	public StaggerComponent(GameCharacter gameCharacter, float staggerTime, float startValue, float maxValue = float.PositiveInfinity, float minValue = 0, float defaultValueChangePerSecond = 0) : base(startValue, maxValue, minValue, defaultValueChangePerSecond)
	{
        this.gameCharacter = gameCharacter;
		staggerTimer = new Ultra.Timer(staggerTime, true);
		this.gameCharacter.StateMachine.onStateChanged += OnStateChanged;
		this.onCurrentValueChange += OnCurrentValueChange;
		staggerTimer.onTimerFinished += OnTimerFinished;
	}

	public override void Update(float deltaTime)
	{
		base.Update(deltaTime);
		if (staggerTimer != null) staggerTimer.Update(deltaTime);
	
	}

	void OnCurrentValueChange(float newValue, float oldValue)
	{
		if (newValue <= 0 && !staggerTimer.IsRunning)
		{
			staggerTimer.Start();
		}
	}

	void OnStateChanged(IState<EGameCharacterState> newState, IState<EGameCharacterState> oldState)
	{
		if (refillStagger)
		{
			switch (newState.GetStateType())
			{
				case EGameCharacterState.Standing:
				case EGameCharacterState.Moving:
				case EGameCharacterState.Attack:
				case EGameCharacterState.Sliding:
				case EGameCharacterState.AttackRecovery:
					refillStagger = false;
					CurrentValue = MaxValue; break;
				default:
					break;
			}
		}
	}

	void OnTimerFinished()
	{
		refillStagger = true;
		// Check if Stagger can be refilled right now
		OnStateChanged(gameCharacter.StateMachine.CurrentState, null);
	}
}
