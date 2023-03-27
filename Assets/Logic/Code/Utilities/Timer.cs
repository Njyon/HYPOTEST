using System.Collections;
using System.Collections.Generic;

namespace Ultra
{
	public class Timer
	{
		public Timer(float time, bool startPaused)
		{
			this.time = time;
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

		public void Start(float time)
		{
			this.time = time;
			IsPaused = false;
			isFinished = false;
			currentTime = 0;
		}

		public void Start()
		{
			IsPaused = false;
			isFinished = false;
			currentTime = 0;
		}

		public void Update(float deltaTime)
		{
			if (isPaused || isFinished) return;

			if (currentTime > time)
			{
				currentTime += deltaTime;
				if (onTimerUpdated != null) onTimerUpdated(deltaTime);
			}
			else if (currentTime <= time)
			{
				isFinished = true;
				if (onTimerFinished != null) onTimerFinished();
			}
		}
	}
}

