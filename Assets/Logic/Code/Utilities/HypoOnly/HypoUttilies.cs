using System.Collections;
using System.Collections.Generic;
using Ultra;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ultra
{
	public class HypoUttilies : Singelton<HypoUttilies>
	{
		public static List<ScriptableCharacter> GetAllCharacters()
		{
#if UNITY_EDITOR

			List<ScriptableCharacter> dataList = new List<ScriptableCharacter>();
			string[] guids = AssetDatabase.FindAssets("t:" + typeof(ScriptableCharacter).Name);
			foreach (string guid in guids)
			{
				string path = AssetDatabase.GUIDToAssetPath(guid);
				ScriptableCharacter data = AssetDatabase.LoadAssetAtPath<ScriptableCharacter>(path);

				if (data != null)
				{
					dataList.Add(data);
				}
			}
			return dataList;
#endif
			return new List<ScriptableCharacter>();
		}
		public static GameCharacter FindCharactereNearestToDirection(Vector3 fromPosition, Vector3 direction, ref List<GameCharacter> list)
		{
			if (list == null || list.Count <= 0) return null;

			GameCharacter mostPointingObject = null;
			float smallestAngle = 360f;

			foreach (GameCharacter character in list)
			{
				float angle = Ultra.Utilities.GetAngleBetweenVectors(direction.normalized, (character.MovementComponent.CharacterCenter - fromPosition).normalized);
				if (angle < smallestAngle)
				{
					mostPointingObject = character;
					smallestAngle = angle;
				}
			}

			return mostPointingObject;
		}
		public static GameCharacter FindCHaracterNearestToDirectionWithMinAngel(Vector3 fromPosition, Vector3 direction, float minAngelAngle, ref List<GameCharacter> list)
		{
			if (list == null || list.Count <= 0) return null;

			GameCharacter mostPointingObject = null;
			float smallestAngle = minAngelAngle;

			foreach (GameCharacter character in list)
			{
				float angle = Ultra.Utilities.GetAngleBetweenVectors(direction.normalized, (character.MovementComponent.CharacterCenter - fromPosition).normalized);
				if (angle <= smallestAngle)
				{
					mostPointingObject = character;
					smallestAngle = angle;
				}
			}

			return mostPointingObject;
		}
		public static GameCharacter FindCHaracterNearestToDirectionWithMinAngel(Vector3 fromPosition, Vector3 direction, Vector3 forward, float minAngelAngle, ref List<GameCharacter> list)
		{
			if (list == null || list.Count <= 0) return null;

			GameCharacter mostPointingObject = null;
			float smallestAngle = minAngelAngle;

			foreach (GameCharacter character in list)
			{
				if (Vector3.Dot(character.MovementComponent.CharacterCenter - fromPosition, forward) >= 0)
				{
					float angle = Ultra.Utilities.GetAngleBetweenVectors(direction.normalized, (character.MovementComponent.CharacterCenter - fromPosition).normalized);
					if (angle <= smallestAngle)
					{
						mostPointingObject = character;
						smallestAngle = angle;
					}
				}
			}

			return mostPointingObject;
		}
		public static GameCharacter FindCharactereNearestToDirectionWithRange(Vector3 fromPosition, Vector3 direction, float range, ref List<GameCharacter> list)
		{
			if (list == null || list.Count <= 0) return null;

			GameCharacter mostPointingObject = null;
			float smallestAngle = 360f;

			foreach (GameCharacter character in list)
			{
				float distance = Vector3.Distance(fromPosition, character.MovementComponent.CharacterCenter);
				if (distance <= range)
				{
					float angle = Ultra.Utilities.GetAngleBetweenVectors(direction.normalized, (character.MovementComponent.CharacterCenter - fromPosition).normalized);
					if (angle < smallestAngle)
					{
						mostPointingObject = character;
						smallestAngle = angle;
					}
				}
			}

			return mostPointingObject;
		}
		public static GameCharacter FindCharactereNearestToDirectionWithRange(Vector3 fromPosition, Vector3 direction, Vector3 forward, float range, ref List<GameCharacter> list)
		{
			if (list == null || list.Count <= 0) return null;

			GameCharacter mostPointingObject = null;
			float smallestAngle = 360f;

			foreach (GameCharacter character in list)
			{
				float distance = Vector3.Distance(fromPosition, character.MovementComponent.CharacterCenter);
				if (distance <= range)
				{
					if (Vector3.Dot(character.MovementComponent.CharacterCenter - fromPosition, forward) >= 0)
					{
						float angle = Ultra.Utilities.GetAngleBetweenVectors(direction.normalized, (character.MovementComponent.CharacterCenter - fromPosition).normalized);
						if (angle < smallestAngle)
						{
							mostPointingObject = character;
							smallestAngle = angle;
						}
					}
				}
			}

			return mostPointingObject;
		}
		public static GameCharacter FindCharactereNearestToDirectionTresholdWithRange(Vector3 fromPosition, Vector3 direction, float directionTresholdAngel, Vector3 forward, float range, ref List<GameCharacter> list)
		{
			if (list == null || list.Count <= 0) return null;

			GameCharacter mostPointingObject = null;
			float smallestAngle = 360f;
			float closestRange = range;

			foreach (GameCharacter character in list)
			{
				float distance = Vector3.Distance(fromPosition, character.MovementComponent.CharacterCenter);
				if (distance <= range)
				{
					if (Vector3.Dot(character.MovementComponent.CharacterCenter - fromPosition, forward) >= 0)
					{
						float angle = Ultra.Utilities.GetAngleBetweenVectors(direction.normalized, (character.MovementComponent.CharacterCenter - fromPosition).normalized);
						if (angle <= smallestAngle)
						{
							if (angle + directionTresholdAngel < smallestAngle || distance < closestRange)
							{
								mostPointingObject = character;
								smallestAngle = angle;
								closestRange = distance;
							}
						}
					}
				}
			}

			return mostPointingObject;
		}
		public static GameCharacter FindCharactereInDirectionInRange(Vector3 fromPosition, Vector3 direction, Vector3 range, ref List<GameCharacter> list)
		{
			if (list == null || list.Count <= 0) return null;

			GameCharacter mostPointingObject = null;
			float smallestAngle = 360;

			foreach (GameCharacter character in list)
			{
				if (Ultra.Utilities.IsPointInBoundingBox(character.MovementComponent.CharacterCenter, fromPosition, range))
				{
					float angle = Ultra.Utilities.GetAngleBetweenVectors(direction.normalized, (character.MovementComponent.CharacterCenter - fromPosition).normalized);
					if (angle < smallestAngle)
					{
						mostPointingObject = character;
						smallestAngle = angle;
					}
				}
			}

			return mostPointingObject;
		}

		public static T CreateGameMode<T> () where T : GameModeBase
		{
			var gameModeObj = new GameObject();
			T gameMode = gameModeObj.AddComponent<T>();
			gameModeObj.name = gameMode.name;
			DontDestroyOnLoad(gameModeObj);
			gameMode.Create();
			return gameMode;
		}

		public static GameModeBase[] FindAllGameModes()
		{
			return FindObjectsOfType<GameModeBase>();
		}

		public static void DeleteAllGameModes()
		{
			GameModeBase[] gameModes = FindAllGameModes();
			foreach (GameModeBase gameMode in gameModes)
			{
				GameObject.Destroy(gameMode.gameObject);
			}
		}

		public static GameModeBase GetGameMode()
		{
			GameModeBase[] gameModes = FindAllGameModes();
			if (gameModes != null && gameModes.Length > 0)
			{
				return gameModes[0];
			}
			// Backup for Editor modus
			Ultra.Utilities.Instance.DebugLogOnScreen("Created Default GameMode backup!", 10f, StringColor.Red);
			Debug.Log("Created Default GameMode backup!");
			GameModeBase gamemode = CreateGameMode<StoryGameMode>();
			gamemode.GameDifficultyLevel = GameDifficultyLevel.Normal;
			if (gameModes != null)
			{
				return gamemode;
			}
			Debug.Log(Ultra.Utilities.Instance.DebugErrorString("HypoUttilies", "GetGameMode", "GameMode was null!"));
			return null;
		}

		public static PlayerGameCharacter GetPlayerGameCharacter()
		{
			return FindObjectOfType<PlayerGameCharacter>();
		}
	}
}
