using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecourceBase
{
	public delegate void ValueChange(float newValue, float oldValue);
	public ValueChange onCurrentValueChange;
	public ValueChange onMaxValueChange;
	public ValueChange onMinValueChange;
	public ValueChange onValueChangePerSecondChange;

	float currentValue;
	float maxValue;
	float minValue;
	float valueChangePerSecond;

	Ultra.Timer maxValueTimer;
	Ultra.Timer minValueTimer;
	Ultra.Timer valueChangePerSecondTimer;

	float maxValueAfterTimer;
	float minValueAfterTimer;
	float valueChangePerSecondAfterTimer;

	public float CurrentValue { 
		get { return currentValue; } 
		protected set
		{
			value = Mathf.Clamp(value, MinValue, MaxValue);

			if (currentValue == value) return;
			float oldestValue = currentValue;
			currentValue = value;
			if (onCurrentValueChange != null) onCurrentValueChange(currentValue, oldestValue);
		}
	}
	public float MaxValue { 
		get { return maxValue; }
		protected set
		{ 
			if (maxValue == value ) return;
			float oldMaxValue = maxValue;
			maxValue = value; 
			if (onMaxValueChange != null) onMaxValueChange(maxValue, oldMaxValue);
		}
	}
	public float MinValue { 
		get { return minValue; }
		protected set
		{
			if (minValue == value) return;
			float oldMinValue = minValue;
			minValue = value;
			if (onMinValueChange != null) onMinValueChange(minValue, oldMinValue);
		}
	}
	public float ValueChangePerSecond {
		get { return valueChangePerSecond; }
		protected set
		{
			if (valueChangePerSecond == value) return;
			float oldValueChangePerSecond = valueChangePerSecond;
			valueChangePerSecond = value;
			if (onValueChangePerSecondChange != null) onValueChangePerSecondChange(valueChangePerSecond, oldValueChangePerSecond);
		}
	}
	public bool MaxValueTimerRunning { get { return maxValueTimer.IsRunning; } }
	public bool MinValueTimerRunning { get { return minValueTimer.IsRunning; } }
	public bool ValueChangePerSecondTimerRunning { get { return valueChangePerSecondTimer.IsRunning; } }


	public RecourceBase(float startValue, float maxValue = Mathf.Infinity, float minValue = 0, float defaultValueChangePerSecond = 0)
	{
		this.currentValue = startValue;
		this.maxValue = maxValue;
		this.minValue = minValue;
		this.valueChangePerSecond = defaultValueChangePerSecond;
		maxValueTimer = new Ultra.Timer();
		minValueTimer = new Ultra.Timer();
		valueChangePerSecondTimer = new Ultra.Timer();
		maxValueTimer.onTimerFinished += OnMaxValueTimerTimerFinished;
		minValueTimer.onTimerFinished += OnMinValueTimerTimerFinished;
		valueChangePerSecondTimer.onTimerFinished += OnValueChangePerSecondTimerFinished;
	}

	~RecourceBase()
	{
		if (maxValueTimer != null) maxValueTimer.onTimerFinished -= OnMaxValueTimerTimerFinished;
		if (minValueTimer != null) minValueTimer.onTimerFinished -= OnMinValueTimerTimerFinished;
		if (valueChangePerSecondTimer != null) valueChangePerSecondTimer.onTimerFinished -= OnValueChangePerSecondTimerFinished;
	}

	public virtual void Update(float deltaTime)
	{
		AddCurrentValue(ValueChangePerSecond * deltaTime);
		
		if (maxValueTimer.IsRunning)
		{
			maxValueTimer.Update(deltaTime);
		}
		if (minValueTimer.IsRunning)
		{
			minValueTimer.Update(deltaTime);
		}
		if (valueChangePerSecondTimer.IsRunning)
		{
			valueChangePerSecondTimer.Update(deltaTime);
		}
	}

	public virtual void AddCurrentValue(float value)
	{
		CurrentValue += value;
	}

	public void ChangeMaxValue(float value, float changeForTime = 0, float valueAfterTime = 0)
	{
		MaxValue = value;
		if (changeForTime > 0)
		{
			maxValueTimer.Start(changeForTime);
			maxValueAfterTimer = valueAfterTime;
		}
	}

	public bool MaxValueAddTimerTime(float time)
	{
		maxValueTimer.AddTime(time);
		return maxValueTimer.IsRunning;
	}

	public bool MinValueAddTimerTime(float time)
	{
		minValueTimer.AddTime(time);
		return minValueTimer.IsRunning;
	}

	public bool ValueChangePerSecondAddTimerTime(float time)
	{
		valueChangePerSecondTimer.AddTime(time);
		return valueChangePerSecondTimer.IsRunning;
	}

	public void ChangeMinValue(float value, float changeForTime = 0, float valueAfterTime = 0)
	{
		MinValue = value;
		if (changeForTime > 0)
		{
			minValueTimer.Start(changeForTime);
			minValueAfterTimer = valueAfterTime;
		}
	}

	public void ChangeValueChangePerSecond(float value, float changeForTime = 0, float valueAfterTime = 0)
	{
		ValueChangePerSecond = value;
		if (changeForTime > 0)
		{
			valueChangePerSecondTimer.Start(changeForTime);
			valueChangePerSecondAfterTimer = valueAfterTime;
		}
	}

	void OnMaxValueTimerTimerFinished()
	{
		MaxValue = maxValueAfterTimer;
	}

	void OnMinValueTimerTimerFinished()
	{
		MinValue = minValueAfterTimer;
	}

	void OnValueChangePerSecondTimerFinished()
	{
		ValueChangePerSecond = valueChangePerSecondAfterTimer;
	}
}
