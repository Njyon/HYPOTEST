using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Flags]
public enum DebugAreas
{
    None = 0,
    Movement = 1,
    Combat = 2,
    Animation = 4,
    Camera = 8,
    Misc = 16,
}

namespace Ultra {
    public class Utilities : Singelton<Utilities> {
        public List<string> onScreenList = new List<string>();
        public List<TimedMessage> onScreenListTimed = new List<TimedMessage>();
        public GUIStyle style = new GUIStyle();
        public int debugLevel = 100;
        public DebugAreas debugAreas = (DebugAreas)(-1);


		void Awake()
		{
            style.fontSize = 30;
            style.wordWrap = true;
        }

        void Update()
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD 
            if (onScreenListTimed.Count > 0 && onScreenList.Count <= 0)
			{
                //DebugLogOnScreen(StringColor.Red + "Debug Log:" + StringColor.EndColor);
			}
#endif
        }

        /// <summary>
        /// Retruns the default colored and formatet debug string
        /// </summary>
        /// <param name="className"></param>
        /// <param name="functionCaller"></param>
        /// <param name="information"></param>
        /// <returns></returns>
        public string DebugLogString(string className, string functionCaller, string information)
        {
            return "[" + StringColor.Teal + className + "::" + functionCaller + StringColor.EndColor + "]" + StringColor.Blue + information + StringColor.EndColor;
        }
            /// <summary>
            /// Debugs a error message on the screen for 10 seconds
            /// </summary>
            /// <param name="className"></param>
            /// <param name="functionCaller"></param>
            /// <param name="information"></param>
            /// <returns></returns>
            public string DebugErrorString(string className, string functionCaller, string information)
        {
            string info = "[" + StringColor.Orange + className + "::" + functionCaller + StringColor.EndColor + "]" + StringColor.Red + information + StringColor.EndColor;
            DebugLogOnScreen(info, 10);
            Debug.LogError(info);
            return info;
        }
        /// <summary>
        /// Logs a string on the screen
        /// </summary>
        /// <param name="message">message that gets logged</param>
        public void DebugLogOnScreen(string message, int debugLevel = 100, DebugAreas debugArea = DebugAreas.Misc)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (debugLevel > this.debugLevel || (debugArea & this.debugAreas) != debugArea) return;
            onScreenList.Add(message);
#endif
        }
        /// <summary>
        /// Logs a string on the screen
        /// </summary>
        /// <param name="message">message that gets logged</param>
        /// <param name="time"> hopw long the message gets logged </param>
        public void DebugLogOnScreen(string message, float time, int debugLevel = 100, DebugAreas debugArea = DebugAreas.Misc)
        {
#if UNITY_EDITOR|| DEVELOPMENT_BUILD
            onScreenListTimed.Add(new TimedMessage(message, time, debugLevel, debugArea));
#endif
        }
        /// <summary>
        /// Logs a string on the screen
        /// </summary>
        /// <param name="message">message that gets logged</param>
        /// <param name="time"> hopw long the message gets logged </param>
        /// <param name="color"> Color of the string (NEEDS TO BE STRINGCOLOR class) </param>
        public void DebugLogOnScreen(string message, float time, string color, int debugLevel = 100, DebugAreas debugArea = DebugAreas.Misc)
		{
#if UNITY_EDITOR|| DEVELOPMENT_BUILD
            onScreenListTimed.Add(new TimedMessage(color + message + StringColor.EndColor, time, debugLevel, debugArea));
#endif
        }
#if UNITY_EDITOR|| DEVELOPMENT_BUILD
        async void OnGUI() {
            Update();
			await new WaitForEndOfFrame();
            for (int i = 0; i < onScreenList.Count; i++) {
                GUI.Label(new Rect(0, 0 + i * style.fontSize, 1000f, 1000f), onScreenList[i], style);
            }
            // List is every second tick cleaned, don't know why
            if (onScreenList.Count > 0) {
                for (int i = 0; i < onScreenListTimed.Count; i++) {
                    if (onScreenListTimed[i].DebugLevel <= this.debugLevel || (onScreenListTimed[i].DebugArea & this.debugAreas) == onScreenListTimed[i].DebugArea)
					{
                        GUI.Label(new Rect(0, 0 + (onScreenList.Count + i) * style.fontSize, 1000f, 1000f), onScreenListTimed[i].Message, style);
                        onScreenListTimed[i].Time -= Time.deltaTime;
                        if (onScreenListTimed[i].Time <= 0) onScreenListTimed.RemoveAt(i);
                    }
                }
            }
            onScreenList.Clear();
        }
#endif
        public static bool IsNearlyEqual(float a, float b, float epsilon)
		{
            if (a >= b - epsilon && a <= b + epsilon)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static bool IsNearlyEqual(Vector3 a, Vector3 b, Vector3 epsilon)
        {
            if (IsNearlyEqual(a.x, b.x, epsilon.x) && IsNearlyEqual(a.y, b.y, epsilon.y) && IsNearlyEqual(a.z, b.z, epsilon.z))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
		public static bool IsNearlyEqual(Vector3 a, Vector3 b, float epsilon)
		{
			if (IsNearlyEqual(a.x, b.x, epsilon) && IsNearlyEqual(a.y, b.y, epsilon) && IsNearlyEqual(a.z, b.z, epsilon))
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		public static bool IsNearlyEqual(Color a, Color b, Color epsilon)
		{
            if (IsNearlyEqual(a.r, b.r, epsilon.r) && IsNearlyEqual(a.g, b.g, epsilon.g) && IsNearlyEqual(a.b, b.b, epsilon.b) && IsNearlyEqual(a.a, b.a, epsilon.a))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static float Remap (float value, float fromA, float toA, float fromB, float toB)
		{
            return (value - fromA) / (toA - fromA) * (toB - fromB) + fromB;
		}
        /// <summary>
        /// Given two intervals "outer" and "inner" the function uniformly picks a value 
        /// that lies in the outer and not the inner interval.
        /// </summary>
        /// <returns> NEED to check if float is NaN in case of not valid ranges </returns>
        public static float PickUniformlyFromSplitInterval(float outerLower, float outerUpper, float innerLower, float innerUpper)
        {
            float firstSectionLength = Mathf.Max(0.0f, innerLower - outerLower);
            float secondSectionLength = Mathf.Max(0.0f, outerUpper - innerUpper);

            if (firstSectionLength + secondSectionLength <= 0)
                return float.NaN;

            float xFromLength = UnityEngine.Random.Range(0.0f, firstSectionLength + secondSectionLength);

            if (xFromLength < firstSectionLength)
            {
                return outerLower + xFromLength;
            }
            else
            {
                return innerUpper + xFromLength - firstSectionLength;
            }
        }
        /// <summary>
        /// Given two intervals "outer" and "inner" the function uniformly picks a value 
        /// that lies in the outer and not the inner interval.
        /// </summary>
        /// <returns> NEED to check if int is int.MinValue in case of not valid ranges </returns>
        public static int PickUniformlyFromSplitInterval(int outerLower, int outerUpper, int innerLower, int innerUpper)
        {
            int firstSectionLength = Mathf.Max(0, innerLower - outerLower);
            int secondSectionLength = Mathf.Max(0, outerUpper - innerUpper);

            if (firstSectionLength + secondSectionLength <= 0)
                return int.MinValue;
            
            int xFromLength = UnityEngine.Random.Range(0, firstSectionLength + secondSectionLength);

            if (xFromLength < firstSectionLength)
            {
                return outerLower + xFromLength;
            }
            else
            {
                return innerUpper + xFromLength - firstSectionLength + 1;
            }
        }

        /// <summary>
        ///   Draw a wire sphere
        /// </summary>
        /// <param name="center"> </param>
        /// <param name="radius"> </param>
        /// <param name="color"> </param>
        /// <param name="duration"> </param>
        /// <param name="quality"> Define the quality of the wire sphere, from 1 to 10 </param>
        public static void DrawWireSphere(Vector3 center, float radius, Color color, float duration, int debugLevel = 100, DebugAreas debugArea = DebugAreas.Misc, int quality = 3)
        {
            if (debugLevel > Instance.debugLevel || (debugArea & Instance.debugAreas) != debugArea) return;

            quality = Mathf.Clamp(quality, 1, 10);

            int segments = quality << 2;
            int subdivisions = quality << 3;
            int halfSegments = segments >> 1;
            float strideAngle = 360F / subdivisions;
            float segmentStride = 180F / segments;

            Vector3 first;
            Vector3 next;
            for (int i = 0; i < segments; i++)
            {
                first = (Vector3.forward * radius);
                first = Quaternion.AngleAxis(segmentStride * (i - halfSegments), Vector3.right) * first;

                for (int j = 0; j < subdivisions; j++)
                {
                    next = Quaternion.AngleAxis(strideAngle, Vector3.up) * first;
                    UnityEngine.Debug.DrawLine(first + center, next + center, color, duration);
                    first = next;
                }
            }

            Vector3 axis;
            for (int i = 0; i < segments; i++)
            {
                first = (Vector3.forward * radius);
                first = Quaternion.AngleAxis(segmentStride * (i - halfSegments), Vector3.up) * first;
                axis = Quaternion.AngleAxis(90F, Vector3.up) * first;

                for (int j = 0; j < subdivisions; j++)
                {
                    next = Quaternion.AngleAxis(strideAngle, axis) * first;
                    UnityEngine.Debug.DrawLine(first + center, next + center, color, duration);
                    first = next;
                }
            }
        }

        public static IEnumerable<T> GetAll<T>() where T : class
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.IsSubclassOf(typeof(T)))
                .Select(type => Activator.CreateInstance(type) as T);
        }

        public static void DrawArrow(Vector3 startPoint, Vector3 direction, float length, Color color, float time = 0f, int debugLevel = 100, DebugAreas debugArea = DebugAreas.Misc)
        {
            if (debugLevel > Instance.debugLevel || (debugArea & Instance.debugAreas) != debugArea) return;
            if (direction == Vector3.zero) return;

            length = Math.Clamp(length, 0.1f, 999999f);
            // Draw line
            Debug.DrawRay(startPoint, direction * length, color, time);

            // Draw arrowhead
            Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + 45, 0) * new Vector3(0, 0, 1);
            Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - 45, 0) * new Vector3(0, 0, 1);
            Debug.DrawRay(startPoint + direction * length, right * 0.25f * length, color, time);
            Debug.DrawRay(startPoint + direction * length, left * 0.25f * length, color, time);
        }

        public static Vector3[] CalculateTrijactoryPoints(int segments, float duration, Vector3 startPosition, Vector3 velocity, Vector3 gracity)
		{
			Vector3[] points = new Vector3[segments];
            float timeStep = duration / segments;
            Vector3 position = startPosition;
            Vector3 internVel = velocity;
            Vector3 gravity = gracity;

            for (int i = 0; i < segments; i++)
            {
                float time = timeStep * i;
                Vector3 displacement = internVel * time + 0.5f * gravity * time * time;
                Vector3 newPosition = position + displacement;
                Vector3 newVelocity = internVel + gravity * time;
                internVel = newVelocity;
                position = newPosition;
                points[i] = position;
            }

            return points;
        }

		public static double SigmoidInterpolation(double startValue, double endValue, double t, double steepness = 1.0)
		{
			// Transform t to a value between 0 and 1
			double normalizedT = Math.Max(0.0, Math.Min(1.0, t));

			// Compute the sigmoid function with the given steepness
			double sigmoid = 1.0 / (1.0 + Math.Exp(-steepness * (normalizedT - 0.5)));

			// Interpolate between the start and end values using the sigmoid as a weight
			return startValue + (endValue - startValue) * sigmoid;
		}

		public static float SigmoidInterpolation(float startValue, float endValue, float t, float steepness = 1.0f)
		{
			// Transform t to a value between 0 and 1
			float normalizedT = Math.Max(0.0f, Math.Min(1.0f, t));

			// Compute the sigmoid function with the given steepness
			float sigmoid = 1.0f / (1.0f + (float)Math.Exp(-steepness * (normalizedT - 0.5f)));

			// Interpolate between the start and end values using the sigmoid as a weight
			return startValue + (endValue - startValue) * sigmoid;
		}
	}
}
