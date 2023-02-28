using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ultra {
    public class Utilities : Singelton<Utilities> {
        public List<string> onScreenList = new List<string>();
        public List<TimedMessage> onScreenListTimed = new List<TimedMessage>();
        public GUIStyle style = new GUIStyle();


		private void Awake()
		{
            style.fontSize = 30;
            style.wordWrap = true;
        }

		private void Update()
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD 
            if (onScreenListTimed.Count > 0 && onScreenList.Count <= 0)
                DebugLogOnScreen(StringColor.Red + "Debug Log:" + StringColor.EndColor);
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
        public void DebugLogOnScreen(string message)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD 
            onScreenList.Add(message);
#endif
        }
        /// <summary>
        /// Logs a string on the screen
        /// </summary>
        /// <param name="message">message that gets logged</param>
        /// <param name="time"> hopw long the message gets logged </param>
        public void DebugLogOnScreen(string message, float time)
        {
#if UNITY_EDITOR|| DEVELOPMENT_BUILD
            onScreenListTimed.Add(new TimedMessage(message, time));
#endif
        }
        /// <summary>
        /// Logs a string on the screen
        /// </summary>
        /// <param name="message">message that gets logged</param>
        /// <param name="time"> hopw long the message gets logged </param>
        /// <param name="color"> Color of the string (NEEDS TO BE STRINGCOLOR class) </param>
        public void DebugLogOnScreen(string message, float time, string color)
		{
#if UNITY_EDITOR|| DEVELOPMENT_BUILD
            onScreenListTimed.Add(new TimedMessage(color + message + StringColor.EndColor, time));
#endif
        }
#if UNITY_EDITOR|| DEVELOPMENT_BUILD
        async void OnGUI() {
			await new WaitForEndOfFrame();
            for (int i = 0; i < onScreenList.Count; i++) {
                GUI.Label(new Rect(0, 0 + i * style.fontSize, 1000f, 1000f), onScreenList[i], style);
            }
            // List is every second tick cleaned, don't know why
            if (onScreenList.Count > 0) {
                for (int i = 0; i < onScreenListTimed.Count; i++) {
                    GUI.Label(new Rect(0, 0 + (onScreenList.Count + i) * style.fontSize, 1000f, 1000f), onScreenListTimed[i].Message, style);
                    onScreenListTimed[i].Time -= Time.deltaTime;
                    if (onScreenListTimed[i].Time <= 0) onScreenListTimed.RemoveAt(i);
                }
            }
            onScreenList.Clear();
        }
#endif
        public bool IsNearlyEqual(float a, float b, float epsilon)
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
        public bool IsNearlyEqual(Vector3 a, Vector3 b, Vector3 epsilon)
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
        public bool IsNearlyEqual(Color a, Color b, Color epsilon)
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
        public float Remap (float value, float fromA, float toA, float fromB, float toB)
		{
            return (value - fromA) / (toA - fromA) * (toB - fromB) + fromB;
		}
        /// <summary>
        /// Given two intervals "outer" and "inner" the function uniformly picks a value 
        /// that lies in the outer and not the inner interval.
        /// </summary>
        /// <returns> NEED to check if float is NaN in case of not valid ranges </returns>
        public float PickUniformlyFromSplitInterval(float outerLower, float outerUpper, float innerLower, float innerUpper)
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
        public int PickUniformlyFromSplitInterval(int outerLower, int outerUpper, int innerLower, int innerUpper)
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
        public void DrawWireSphere(Vector3 center, float radius, Color color, float duration, int quality = 3)
        {
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
    }
}
