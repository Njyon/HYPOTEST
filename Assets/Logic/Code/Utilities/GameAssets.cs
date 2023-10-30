using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Megumin.GameFramework.AI.BehaviorTree;

public class GameAssets : MonoBehaviour
{
	private static GameAssets instance;
	public static GameAssets Instance
	{
		get
		{
			if (instance == null)
			{
				//  Path: "Assets/Resources/Prefab/GameAssets"
				instance = (Instantiate(Resources.Load("Prefab/GameAssets")) as GameObject).GetComponent<GameAssets>();
				instance.name = ">> " + instance.name;
			}
			return instance;
		}
	}

	// List of Objects below
	public Material debugMaterial;
	public GameObject ThrowSpear;
	public GameObject characterDetection;
	public List<StyleRankingScriptableObject> styleRanks = new List<StyleRankingScriptableObject>();
	public GameObject EnemyInfo;
	public List<BehaviorTreeAsset_1_1> BehaviorTrees = new List<BehaviorTreeAsset_1_1>();
	public GameObject DefaultAttackFeedback;
	public List<AudioClip> MusicTracks = new List<AudioClip>();
	public AudioSource MusicObject;
}