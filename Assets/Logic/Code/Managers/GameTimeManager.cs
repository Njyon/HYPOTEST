using Megumin.GameFramework.AI.BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedTimeManipulation
{
    public TimedTimeManipulation(string name, float time) {
        this.name = name;
        this.time = time;
    }

    string name;
    float time;

    public string Name { get { return name; } }
    public float Time { get { return time; } set { time = value; } }
}

public class GameTimeManager : Singelton<GameTimeManager>
{
    public delegate void OnTimeScaleChanged(float newTimeScale, float oldTimeScale);
    public OnTimeScaleChanged onTimeScaleChanged;

    float currentTimeMultiplier = 1f;

    Dictionary<string, float> timeManipulators = new Dictionary<string, float>();
    List<TimedTimeManipulation> timeManipulationList = new List<TimedTimeManipulation>();
	List<TimedTimeManipulation> toDeletedManiplulations = new List<TimedTimeManipulation>();

    public void ClearAllTimeManipulations()
    {
		timeManipulators.Clear();
		timeManipulationList.Clear();
		toDeletedManiplulations.Clear();

		CurrentTimeMultiplier = 1f;
	}

	public float CurrentTimeMultiplier { get { return currentTimeMultiplier; } 
        private set 
        { 
            if (currentTimeMultiplier != value)
            {
                float oldValue = currentTimeMultiplier;
                currentTimeMultiplier = value; 
                Time.timeScale = currentTimeMultiplier;

                if (onTimeScaleChanged != null) onTimeScaleChanged(currentTimeMultiplier, oldValue);
            }
        } 
    }

    void Update()
    {
        foreach(TimedTimeManipulation manipulation in timeManipulationList)
        {
            manipulation.Time -= Time.unscaledDeltaTime;
            if (manipulation.Time <= 0) toDeletedManiplulations.Add(manipulation);
		}

        foreach(TimedTimeManipulation manipulation in toDeletedManiplulations)
        {
			timeManipulationList.Remove(manipulation);
            var value = timeManipulators[manipulation.Name];
			timeManipulators.Remove(manipulation.Name);
			RemoveManipulation(value);
		}
		toDeletedManiplulations.Clear();
	}

    public void ToggleTimeManipulation(string name, float timeMultiplier)
    {
        if (timeManipulators.ContainsKey(name))
        {
            var manipulation = timeManipulators[name];
			timeManipulators.Remove(name);
			RemoveManipulation(manipulation);
          
        }else
        {
            timeManipulators.Add(name, timeMultiplier);
            AddManipulation(timeMultiplier);
		}
    }

    public void AddTimedManipulation(string name, float timeLenght, float timeManipulationMultiplier)
    {
        var manipulation = timeManipulationList.Find((e) => { return e.Name == name; });
        if (manipulation != null)
        {
            manipulation.Time = timeLenght;
        }else
        {
            manipulation = new TimedTimeManipulation(name, timeLenght);
            timeManipulationList.Add(manipulation);
            timeManipulators.Add(manipulation.Name, timeManipulationMultiplier);
			AddManipulation(timeManipulationMultiplier);
		}
    }

    public void RemoveTimedManipulation(string name)
	{
		var manipulation = timeManipulationList.Find((e) => { return e.Name == name; });
        if (manipulation != null)
        {
			timeManipulationList.Remove(manipulation);
            var multiplier = timeManipulators[manipulation.Name];
			timeManipulators.Remove(manipulation.Name);
            RemoveManipulation(multiplier);
		}
	}

    void AddManipulation(float manipulation)
	{
		CurrentTimeMultiplier *= manipulation;
	}

    void RemoveManipulation(float manipulation)
	{
        if (manipulation == 0)
        {
            CurrentTimeMultiplier = 1f;
            return;
		}
		CurrentTimeMultiplier /= manipulation;
	}

    public void AddHeavyFreezFrame()
    {
		DefaultGameModeData data = Ultra.HypoUttilies.GameMode.GetDefaultGameModeData();
        RemoveTimedManipulation(data.defaultHitFreezString);
		AddTimedManipulation(data.heavytHitFreezString, data.heavyHitFreezTime, data.heavyHitFreezTimeManipulation); 
	}
	public void AddDefaultFreezFrame()
	{
        DefaultGameModeData data = Ultra.HypoUttilies.GameMode.GetDefaultGameModeData();
        if (timeManipulators.ContainsKey(data.heavytHitFreezString)) return;
		AddTimedManipulation(data.defaultHitFreezString, data.defaultHitFreezTime, data.defaultHitFreezTimeManipulation);
	}
}
