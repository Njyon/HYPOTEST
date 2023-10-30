using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New StyleRanking", menuName = "Assets/StyleRanking")]
public class StyleRankingScriptableObject : ScriptableObject
{
	public string Name = "newStyleRank";
	public float PointsToLevelUp = 100f;
	public float PointDecreasePerSecond = 20f;
	public float PointDecreaseStopTime = 0.5f;
	public float musicVolumeTarget = 0f;
	public Sprite StyleImage;
}
