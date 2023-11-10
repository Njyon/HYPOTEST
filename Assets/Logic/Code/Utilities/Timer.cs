using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ultra
{
	public class Timer
	{
		public Timer()
		{
			this.time = 0;
			isPaused = true;
			currentTime = 0;
			isFinished = false;
		}

		public Timer(float time, bool startPaused = true)
		{
			this.time = time;
			isPaused = startPaused;
			currentTime = 0;
			isFinished = false;
		}

		public Timer(bool startPaused)
		{
			this.time = -1f;
			isPaused = startPaused;
			currentTime = 0;
			isFinished = false;
		}

		public delegate void OnTimerFinished();
		public delegate void OnTimerPaused();
		public delegate void OnTimerStarted(float lengh);
		public delegate void OnTimerUpdated(float deltaTime);
		public OnTimerFinished onTimerFinished;
		public OnTimerPaused onTimerPaused;
		public OnTimerStarted onTimerStarted;
		public OnTimerUpdated onTimerUpdated;

		float time;
		float currentTime;
		bool isPaused;
		bool isFinished;
		public bool IsPaused
		{
			get { return isPaused; }
			set
			{
				isPaused = value;
				if (isPaused)
				{
					if (onTimerPaused != null) onTimerPaused();
				}
			}
		}
		public bool IsFinished { get { return isFinished; } }
		public bool IsRunning { get { return !isPaused && !isFinished; } }
		public float CurrentTime { get { return currentTime; } }
		public float Time { get { return time; } }	

		public void Start(float time)
		{
			this.time = time;
			IsPaused = false;
			isFinished = false;
			currentTime = 0;

			if (onTimerStarted != null) onTimerStarted(this.time);
		}

		public void Start()
		{
			IsPaused = false;
			isFinished = false;
			currentTime = 0;

			if (onTimerStarted != null) onTimerStarted(this.time);
		}

		public void Stop()
		{
			IsPaused = true;
		}

		public void AddTime(float value)
		{
			this.time += value;
		}

		public void Update(float deltaTime)
		{
			if (isPaused || isFinished) return;

			if (time >= 0)
			{
				if (time > currentTime)
				{
					UpdateTimer(deltaTime);
				}
				else if (time <= currentTime)
				{
					isFinished = true;
					if (onTimerFinished != null) onTimerFinished();
				}
			}else
			{
				UpdateTimer(deltaTime);
			}

		}

		private void UpdateTimer(float deltaTime)
		{
			currentTime += deltaTime;
			if (onTimerUpdated != null) onTimerUpdated(deltaTime);
		}

		/// <summary>
		/// Get Progress between 0 (start) and 1 (end)
		/// </summary>
		/// <returns></returns>
		public float GetProgess()
		{
			if (Time < 0) return 0.5f; // this Funktion does not work on Timers with no end
			return Mathf.Lerp(0, Time, CurrentTime);
		}
	}
}

