using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Ultra;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ultra
{
	public class HypoUttilies : Singelton<HypoUttilies>
	{
		static GameModeBase gameModeBase;
		static public GameModeBase GameMode { 
			get 
			{ 
				if (gameModeBase == null)
				{
					gameModeBase = GetGameMode();
				}
				return gameModeBase;
			} 
		}

		static PlayerGameCharacter playerCharacter;

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
				if (character == null) continue;
				float angle = Ultra.Utilities.GetAngleBetweenVectors(direction.normalized, (character.MovementComponent.CharacterCenter - fromPosition).normalized);
				if (angle < smallestAngle)
				{
					mostPointingObject = character;
					smallestAngle = angle;
				}
			}

			return mostPointingObject;
		}
		public static GameCharacter FindCharactereNearestToDirection(Vector3 fromPosition, Vector3 direction, HyppoliteTeam team, ref List<GameCharacter> list)
		{
			if (list == null || list.Count <= 0) return null;

			GameCharacter mostPointingObject = null;
			float smallestAngle = 360f;

			foreach (GameCharacter character in list)
			{
				if (character == null || character.CheckForSameTeam(team)) continue;
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
				if (character == null) continue;
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
				if (character == null) continue;
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
				if (character == null) continue;
				float distance = Vector3.Distance(fromPosition, character.MovementComponent.CharacterCenter);
				if (distance <= range)
				{
					float angle = Vector3.Angle(direction.normalized, (character.MovementComponent.CharacterCenter - fromPosition).normalized);
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
				if (character == null) continue;
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
				if (character == null) continue;
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
				if (character == null) continue;
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

		public static GameCharacter FindCharactereNearestToDirectionWithRangeWithAngleTreshHold(Vector3 fromPosition, Vector3 direction, float range, ref List<GameCharacter> list, float angleTreshHold = 2f)
		{
			if (list == null || list.Count <= 0) return null;

			GameCharacter mostPointingObject = null;
			float smallestAngle = 360f;
			float closestDistance = 99999999f; 

			foreach (GameCharacter character in list)
			{
				if (character == null) continue;
				float distance = Vector3.Distance(fromPosition, character.MovementComponent.CharacterCenter);
				if (distance <= range)
				{
					Vector3 dir = (character.MovementComponent.CharacterCenter - fromPosition);
					//Ultra.Utilities.DrawArrow(fromPosition, dir, dir.magnitude, Ultra.Utilities.RandomColor(), 10f);
					float angle = Vector3.Angle(direction.normalized, dir.normalized);
					if (angle.IsNearlyEqual(smallestAngle, angleTreshHold) && distance < closestDistance)
					{
						mostPointingObject = character;
						smallestAngle = angle;
						closestDistance = distance;
					}
					else if (angle + angleTreshHold < smallestAngle)
					{
						mostPointingObject = character;
						smallestAngle = angle;
						closestDistance = distance;
					}
				}
			}

			//Ultra.Utilities.DrawWireSphere(mostPointingObject.MovementComponent.CharacterCenter, 1f, Color.green, 10f);
			return mostPointingObject;
		}

		public static T CreateGameMode<T> () where T : GameModeBase
		{
			var gameModeObj = new GameObject();
			T gameMode = gameModeObj.AddComponent<T>();
			gameModeObj.name = gameMode.name;
			DontDestroyOnLoad(gameModeObj);
			gameMode.Create();
			gameModeBase = gameMode;
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
			if (playerCharacter == null)
			{
				playerCharacter = FindObjectOfType<PlayerGameCharacter>();
			}
			return playerCharacter;
		}

		public ControllerBase GetController(string controllerName, GameObject pawn)
		{
			ControllerBase controller = null;
			switch (controllerName)
			{
				case "AIControllerBase": controller = pawn.AddComponent<AIControllerBase>(); break;
				case "PlayerController": controller = pawn.AddComponent<PlayerController>(); break;
				default:
					Ultra.Utilities.Instance.DebugErrorString("HyppoUttilies", "GetController", "Unvalid Controller name, maybe forgot to Add case?");
					break;
			}
			return controller;
		}
		public ControllerBase SpawnController(GameObject characterToControll, ScriptableCharacter characterData)
		{
			ControllerBase controller = GetController(characterData.ControllerName, characterToControll);
			if (controller == null)
			{
				Debug.LogError("Controller could not be created correctly. GameObject => " + controller.name);
				return controller;
			}
			controller.BeginPosses(characterToControll, characterData);

			//ControllerBase controllerBase = null;
			//ControllerBase[] controllers = Ultra.Utilities.GetAll<ControllerBase>().ToArray();
			//for (int i = 0; i < controllers.Length; i++)
			//{
			//	if (controllers[i].GetType().Name == characterData.ControllerName)
			//	{
			//		GameObject controller = new GameObject(characterData.ControllerName);
			//		controller.transform.position = Vector3.zero;
			//		controller.transform.rotation = Quaternion.identity;
			//		Component comp = controller.AddComponent(controllers[i].GetType());
			//		controllerBase = (ControllerBase)comp;
			//		if (controllerBase == null)
			//		{
			//			Debug.LogError("Controller could not be created correctly. GameObject => " + controller.name);
			//			return controllerBase;
			//		}
			//		controllerBase.BeginPosses(characterToControll, characterData);
			//		break;
			//	}
			//}
			return controller;
		}
	}
}
