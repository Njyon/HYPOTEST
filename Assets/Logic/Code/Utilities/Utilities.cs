using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

[Flags]
public enum DebugAreas
{
    None = 0,
    Movement = 1,
    Combat = 2,
    Animation = 4,
    Camera = 8,
    AI = 16,
    Misc = 32,
}

public enum EAxis
{
	X,
	XY,
	XYZ,
	Y,
	YZ,
	Z,
}

namespace Ultra {
	public class Utilities : Singelton<Utilities> {
		public List<string> onScreenList = new List<string>();
		public List<TimedMessage> onScreenListTimed = new List<TimedMessage>();
		public GUIStyle style = new GUIStyle();
		public int debugLevel = 100;
		public DebugAreas debugAreas = (DebugAreas)(-1);
		double debugTimeSlice = new();

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
			return "[" + StringColor.Blue + className + "::" + functionCaller + StringColor.EndColor + "]" + StringColor.Teal + information + StringColor.EndColor;
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
#if UNITY_EDITOR || DEVELOPMENT_BUILD
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
#if UNITY_EDITOR || DEVELOPMENT_BUILD
			onScreenListTimed.Add(new TimedMessage(color + message + StringColor.EndColor, time, debugLevel, debugArea));
#endif
		}
#if UNITY_EDITOR || DEVELOPMENT_BUILD
		async void OnGUI() {
			Update();
			await new WaitForEndOfFrame();
			for (int i = 0; i < onScreenList.Count; i++) {
				GUI.Label(new Rect(0, 0 + i * style.fontSize, 1000f, 1000f), onScreenList[i], style);
			}
			// List is every second tick cleaned, don't know why
			if (onScreenList.Count > 0) {
				int j = 0;
				for (int i = 0; i < onScreenListTimed.Count; i++) {
					if (onScreenListTimed[i].DebugLevel <= this.debugLevel && (onScreenListTimed[i].DebugArea & this.debugAreas) == onScreenListTimed[i].DebugArea)
					{
						j++;	
						GUI.Label(new Rect(0, 0 + (onScreenList.Count + j) * style.fontSize, 1000f, 1000f), onScreenListTimed[i].Message, style);
					}
					onScreenListTimed[i].Time -= Time.deltaTime;
					if (onScreenListTimed[i].Time <= 0) onScreenListTimed.RemoveAt(i);
				}
			}
			onScreenList.Clear();
		}
#endif
		public static bool IsNearlyEqual(float a, float b, float epsilon)
		{
			return (a >= b - epsilon && a <= b + epsilon);
			
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
		public static float Remap(float value, float fromA, float toA, float fromB, float toB)
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

		public static void DrawCapsule(Vector3 position, Quaternion orientation, float height, float radius, Color color, float time = 1f, int debugLevel = 100, DebugAreas debugArea = DebugAreas.Misc, bool drawFromBase = false)
		{
			if (debugLevel > Instance.debugLevel || (debugArea & Instance.debugAreas) != debugArea) return;

			// Clamp the radius to a half of the capsule's height
			radius = Mathf.Clamp(radius, 0, height / 2);
			Vector3 localUp = orientation * Vector3.up;
			Quaternion arcOrientation = orientation * Quaternion.Euler(0, 90, 0);

			Vector3 basePositionOffset = drawFromBase ? Vector3.zero : (localUp * (height * 0.5f));
			Vector3 baseArcPosition = position + localUp * radius - basePositionOffset;
			DrawArc(180, 360, baseArcPosition, orientation, radius, color, time);
			DrawArc(180, 360, baseArcPosition, arcOrientation, radius, color, time);

			float cylinderHeight = height - (radius * 2.0f);
			DrawCylinder(baseArcPosition, orientation, cylinderHeight, radius, color, time, true);

			Vector3 topArcPosition = baseArcPosition + localUp * cylinderHeight;

			DrawArc(0, 180, topArcPosition, orientation, radius, color, time);
			DrawArc(0, 180, topArcPosition, arcOrientation, radius, color, time);
		}

		public static void DrawCylinder(Vector3 position, Quaternion orientation, float height, float radius, Color color, float time, bool drawFromBase = true)
		{
			Vector3 localUp = orientation * Vector3.up;
			Vector3 localRight = orientation * Vector3.right;
			Vector3 localForward = orientation * Vector3.forward;

			Vector3 basePositionOffset = drawFromBase ? Vector3.zero : (localUp * height * 0.5f);
			Vector3 basePosition = position - basePositionOffset;
			Vector3 topPosition = basePosition + localUp * height;

			Quaternion circleOrientation = orientation * Quaternion.Euler(90, 0, 0);

			Vector3 pointA = basePosition + localRight * radius;
			Vector3 pointB = basePosition + localForward * radius;
			Vector3 pointC = basePosition - localRight * radius;
			Vector3 pointD = basePosition - localForward * radius;

			Debug.DrawRay(pointA, localUp * height, color, time);
			Debug.DrawRay(pointB, localUp * height, color, time);
			Debug.DrawRay(pointC, localUp * height, color, time);
			Debug.DrawRay(pointD, localUp * height, color, time);

			DrawCircle(basePosition, circleOrientation, radius, 32, color, time);
			DrawCircle(topPosition, circleOrientation, radius, 32, color, time);
		}

		public static void DrawCircle(Vector3 position, Quaternion rotation, float radius, int segments, Color color, float time)
		{
			// If either radius or number of segments are less or equal to 0, skip drawing
			if (radius <= 0.0f || segments <= 0)
			{
				return;
			}

			// Single segment of the circle covers (360 / number of segments) degrees
			float angleStep = (360.0f / segments);

			// Result is multiplied by Mathf.Deg2Rad constant which transforms degrees to radians
			// which are required by Unity's Mathf class trigonometry methods

			angleStep *= Mathf.Deg2Rad;

			// lineStart and lineEnd variables are declared outside of the following for loop
			Vector3 lineStart = Vector3.zero;
			Vector3 lineEnd = Vector3.zero;

			for (int i = 0; i < segments; i++)
			{
				// Line start is defined as starting angle of the current segment (i)
				lineStart.x = Mathf.Cos(angleStep * i);
				lineStart.y = Mathf.Sin(angleStep * i);
				lineStart.z = 0.0f;

				// Line end is defined by the angle of the next segment (i+1)
				lineEnd.x = Mathf.Cos(angleStep * (i + 1));
				lineEnd.y = Mathf.Sin(angleStep * (i + 1));
				lineEnd.z = 0.0f;

				// Results are multiplied so they match the desired radius
				lineStart *= radius;
				lineEnd *= radius;

				// Results are multiplied by the rotation quaternion to rotate them 
				// since this operation is not commutative, result needs to be
				// reassigned, instead of using multiplication assignment operator (*=)
				lineStart = rotation * lineStart;
				lineEnd = rotation * lineEnd;

				// Results are offset by the desired position/origin 
				lineStart += position;
				lineEnd += position;

				// Points are connected using DrawLine method and using the passed color
				Debug.DrawLine(lineStart, lineEnd, color, time);
			}
		}

		public static void DrawArc(float startAngle, float endAngle, Vector3 position, Quaternion orientation, float radius, Color color, float time, bool drawChord = false, bool drawSector = false, int arcSegments = 32)
		{
			float arcSpan = Mathf.DeltaAngle(startAngle, endAngle);

			// Since Mathf.DeltaAngle returns a signed angle of the shortest path between two angles, it 
			// is necessary to offset it by 360.0 degrees to get a positive value
			if (arcSpan <= 0)
			{
				arcSpan += 360.0f;
			}

			// angle step is calculated by dividing the arc span by number of approximation segments
			float angleStep = (arcSpan / arcSegments) * Mathf.Deg2Rad;
			float stepOffset = startAngle * Mathf.Deg2Rad;

			// stepStart, stepEnd, lineStart and lineEnd variables are declared outside of the following for loop
			float stepStart = 0.0f;
			float stepEnd = 0.0f;
			Vector3 lineStart = Vector3.zero;
			Vector3 lineEnd = Vector3.zero;

			// arcStart and arcEnd need to be stored to be able to draw segment chord
			Vector3 arcStart = Vector3.zero;
			Vector3 arcEnd = Vector3.zero;

			// arcOrigin represents an origin of a circle which defines the arc
			Vector3 arcOrigin = position;

			for (int i = 0; i < arcSegments; i++)
			{
				// Calculate approximation segment start and end, and offset them by start angle
				stepStart = angleStep * i + stepOffset;
				stepEnd = angleStep * (i + 1) + stepOffset;

				lineStart.x = Mathf.Cos(stepStart);
				lineStart.y = Mathf.Sin(stepStart);
				lineStart.z = 0.0f;

				lineEnd.x = Mathf.Cos(stepEnd);
				lineEnd.y = Mathf.Sin(stepEnd);
				lineEnd.z = 0.0f;

				// Results are multiplied so they match the desired radius
				lineStart *= radius;
				lineEnd *= radius;

				// Results are multiplied by the orientation quaternion to rotate them 
				// since this operation is not commutative, result needs to be
				// reassigned, instead of using multiplication assignment operator (*=)
				lineStart = orientation * lineStart;
				lineEnd = orientation * lineEnd;

				// Results are offset by the desired position/origin 
				lineStart += position;
				lineEnd += position;

				// If this is the first iteration, set the chordStart
				if (i == 0)
				{
					arcStart = lineStart;
				}

				// If this is the last iteration, set the chordEnd
				if (i == arcSegments - 1)
				{
					arcEnd = lineEnd;
				}

				Debug.DrawLine(lineStart, lineEnd, color, time);
			}

			if (drawChord)
			{
				Debug.DrawLine(arcStart, arcEnd, color, time);
			}
			if (drawSector)
			{
				Debug.DrawLine(arcStart, arcOrigin, color, time);
				Debug.DrawLine(arcEnd, arcOrigin, color, time);
			}
		}

		// Draw a triangle defined by three points
		public static void DrawTriangle(Vector3 pointA, Vector3 pointB, Vector3 pointC, Color color)
		{
			// Connect pointA and pointB
			Debug.DrawLine(pointA, pointB, color);

			// Connect pointB and pointC
			Debug.DrawLine(pointB, pointC, color);

			// Connect pointC and pointA
			Debug.DrawLine(pointC, pointA, color);
		}

		public static void DrawTriangle(Vector3 pointA, Vector3 pointB, Vector3 pointC, Vector3 offset, Quaternion orientation, Color color)
		{
			pointA = offset + orientation * pointA;
			pointB = offset + orientation * pointB;
			pointC = offset + orientation * pointC;

			DrawTriangle(pointA, pointB, pointC, color);
		}
		public static void DrawTriangle(float length, Vector3 center, Quaternion orientation, Color color)
		{
			float radius = length / Mathf.Cos(30.0f * Mathf.Deg2Rad) * 0.5f;
			Vector3 pointA = new Vector3(Mathf.Cos(330.0f * Mathf.Deg2Rad), Mathf.Sin(330.0f * Mathf.Deg2Rad), 0.0f) * radius;
			Vector3 pointB = new Vector3(Mathf.Cos(90.0f * Mathf.Deg2Rad), Mathf.Sin(90.0f * Mathf.Deg2Rad), 0.0f) * radius;
			Vector3 pointC = new Vector3(Mathf.Cos(210.0f * Mathf.Deg2Rad), Mathf.Sin(210.0f * Mathf.Deg2Rad), 0.0f) * radius;

			DrawTriangle(pointA, pointB, pointC, center, orientation, color);
		}
		public static void DrawTriangle(Vector3 origin, Quaternion orientation, float baseLength, float height, Color color)
		{
			Vector3 pointA = Vector3.right * baseLength * 0.5f;
			Vector3 pointC = Vector3.left * baseLength * 0.5f;
			Vector3 pointB = Vector3.up * height;

			DrawTriangle(pointA, pointB, pointC, origin, orientation, color);
		}
		public static void DrawQuad(Vector3 pointA, Vector3 pointB, Vector3 pointC, Vector3 pointD, Color color, float time = 0)
		{
			// Draw lines between the points
			Debug.DrawLine(pointA, pointB, color, time);
			Debug.DrawLine(pointB, pointC, color, time);
			Debug.DrawLine(pointC, pointD, color, time);
			Debug.DrawLine(pointD, pointA, color, time);
		}
		// Draw a rectangle defined by its position, orientation and extent
		public static void DrawRectangle(Vector3 position, Quaternion orientation, Vector2 extent, Color color, float time = 0)
		{
			Vector3 rightOffset = Vector3.right * extent.x * 0.5f;
			Vector3 upOffset = Vector3.up * extent.y * 0.5f;

			Vector3 offsetA = orientation * (rightOffset + upOffset);
			Vector3 offsetB = orientation * (-rightOffset + upOffset);
			Vector3 offsetC = orientation * (-rightOffset - upOffset);
			Vector3 offsetD = orientation * (rightOffset - upOffset);

			DrawQuad(position + offsetA,
						position + offsetB,
						position + offsetC,
						position + offsetD,
						color,
						time);
		}
		public static void DrawBox(Vector3 position, Quaternion orientation, Vector3 size, Color color, float time = 0, int debugLevel = 100, DebugAreas debugArea = DebugAreas.Misc)
		{
			if (debugLevel > Instance.debugLevel || (debugArea & Instance.debugAreas) != debugArea) return;

			Vector3 offsetX = orientation * Vector3.right * size.x * 0.5f;
			Vector3 offsetY = orientation * Vector3.up * size.y * 0.5f;
			Vector3 offsetZ = orientation * Vector3.forward * size.z * 0.5f;

			Vector3 pointA = (-offsetX + offsetY) + position;
			Vector3 pointB = (offsetX + offsetY) + position;
			Vector3 pointC = (offsetX - offsetY) + position;
			Vector3 pointD = (-offsetX - offsetY) + position;

			DrawRectangle(position - offsetZ, orientation, new Vector2(size.x, size.y), color, time);
			DrawRectangle(position + offsetZ, orientation, new Vector2(size.x, size.y), color, time);

			Debug.DrawLine(pointA - offsetZ, pointA + offsetZ, color, time);
			Debug.DrawLine(pointB - offsetZ, pointB + offsetZ, color, time);
			Debug.DrawLine(pointC - offsetZ, pointC + offsetZ, color, time);
			Debug.DrawLine(pointD - offsetZ, pointD + offsetZ, color, time);
		}
		public static Collider[] OverlapCapsule(Vector3 position, float capsulHeight, float radius, int layerMask = -5, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.UseGlobal) { 
			Vector3 point1 = GetCapsuleSphere(position, capsulHeight, radius);
			Vector3 point2 = GetCapsuleSphere(position, -capsulHeight, radius);
			return Physics.OverlapCapsule(point1, point2, radius, layerMask, triggerInteraction);
		}
		public static Vector3 GetCapsuleSphere(Vector3 position, float hightDirection, float radius) {
			Vector3 value = position + Vector3.up * ((hightDirection / 2) - Math.Sign(hightDirection) * radius);
			return value;
		}
		public static bool CapsulCast(Vector3 capsulCenter, float capsulHeight, float radius, Vector3 direction, out RaycastHit hit, Color debugColor, int debugLevel = 100, DebugAreas debugAreas = DebugAreas.Misc, int layerMask = -5, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.UseGlobal) {
			// IF Application is Playing check is needed to be able to use the funtion out of playmodus
			if (Application.isPlaying) DrawCapsule(capsulCenter, Quaternion.identity, capsulHeight, radius, debugColor.WithAlpha(0.2f), 0f, debugLevel, debugAreas);
			bool value = Physics.CapsuleCast(GetCapsuleSphere(capsulCenter, capsulHeight, radius), GetCapsuleSphere(capsulCenter, -capsulHeight, radius), radius, direction, out hit, direction.magnitude, layerMask, triggerInteraction);
			if (Application.isPlaying) 
			{
				if (value) DrawArrow(capsulCenter, hit.point - capsulCenter, Vector3.Distance(capsulCenter, hit.point), debugColor, 0, debugLevel, debugAreas);
				else DrawCapsule(capsulCenter + direction, Quaternion.identity, capsulHeight, radius, debugColor.WithAlpha(0.5f), 0f, debugLevel, debugAreas);
			}
			return value;
		}

		public static RaycastHit[] CapsulCastAll(Vector3 capsulCenter, float capsulHeight, float radius, Vector3 direction, Color debugColor, int debugLevel = 100, DebugAreas debugAreas = DebugAreas.Misc, int layerMask = -5, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.UseGlobal)
		{
			// IF Application is Playing check is needed to be able to use the funtion out of playmodus
			if (Application.isPlaying) DrawCapsule(capsulCenter, Quaternion.identity, capsulHeight, radius, debugColor.WithAlpha(0.2f), 0f, debugLevel, debugAreas);
			RaycastHit[] value = Physics.CapsuleCastAll(GetCapsuleSphere(capsulCenter, capsulHeight, radius), GetCapsuleSphere(capsulCenter, -capsulHeight, radius), radius, direction, direction.magnitude, layerMask, triggerInteraction);
			if (Application.isPlaying)
			{
				foreach (RaycastHit hit in value)
				{
					if (hit.collider != null) DrawArrow(capsulCenter, hit.point - capsulCenter, Vector3.Distance(capsulCenter, hit.point), debugColor, 0, debugLevel, debugAreas);
				}
				DrawCapsule(capsulCenter + direction, Quaternion.identity, capsulHeight, radius, debugColor.WithAlpha(0.5f), 0f, debugLevel, debugAreas);
			}

			return value;
		}

		public static float GetAngleBetweenVectors(Vector3 a, Vector3 b)
		{
			float angle = Vector3.Angle(a, b);
			Vector3 cross = Vector3.Cross(a, b);
			if (cross.y < 0 && angle > 180f)
			{
				angle = 360f - angle; // Korrektur nur wenn der Winkel > 180 Grad ist
			}
			return angle;
		}

		public static Vector3 IgnoreAxis(Vector3 dir, EAxis axis)
		{
			switch (axis)
			{
				case EAxis.X:
					dir.x = 0;
					break;	
				case EAxis.XY: 
					dir.x = 0;
					dir.y = 0;
					break;
				case EAxis.XYZ:
					dir.x = 0;
					dir.y = 0;
					dir.z = 0;
					break;
				case EAxis.Y:
					dir.y = 0;
					break; 
				case EAxis.YZ:
					dir.y = 0;
					dir.z = 0;
					break;
				case EAxis.Z:
					dir.z = 0;
					break;
				default: return dir;
			}
			return dir;
		}
		public static bool IsPointInBoundingBox(Vector3 pointToCheck, Vector3 boundsCenter, Vector3 boundsRanges)
		{
			Bounds bounds = new Bounds(boundsCenter, boundsRanges * 2); // Erstelle Bounding Box

			return bounds.Contains(pointToCheck); // Überprüfe, ob der Punkt in der Bounding Box liegt
		}

		public void DebugPrintTimeAndElapsedSinceLast(string message)
		{
#if UNITY_EDITOR || DEVELOPMENT_BUILD
			var newTimeSlice = Time.realtimeSinceStartupAsDouble;
			var timeSliceDelta = math.abs(debugTimeSlice - newTimeSlice);
			Debug.Log(DebugLogString("Utilities", "DebugPrintTimeAndElapsedSinceLast", message + "| Delta: " + timeSliceDelta + ". New TimeSlice: " + newTimeSlice + ". OldTimeSlice: " + debugTimeSlice));
			debugTimeSlice = newTimeSlice;
#endif
		}

		public static Color RandomColor(float alpha = 1f)
		{
			return new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), alpha);
		}

		public static Quaternion MirrorQuaternion(Quaternion q, Vector3 axis)
		{
			Quaternion inverseQuaternion = Quaternion.Inverse(q);
			Quaternion mirroredQuaternion = Quaternion.FromToRotation(axis, -axis) * inverseQuaternion * Quaternion.FromToRotation(-axis, axis);

			return mirroredQuaternion;
		}
	}
}