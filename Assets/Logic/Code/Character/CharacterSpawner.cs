using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterSpawner : MonoBehaviour
{
	GameObject characterToSpawn;
	// only for inspector override
	[HideInInspector] public ScriptableCharacter spawnableCharacterData;
	[HideInInspector] public int index = 0;

	void Start()
	{
		SpawnCharacterIntoWorld();
		SpawnController();
	}

	private void SpawnController()
	{
		ControllerBase[] controllers = Ultra.Utilities.GetAll<ControllerBase>().ToArray();
		for (int i = 0; i < controllers.Length; i++)
		{
			if (controllers[i].GetType().Name == spawnableCharacterData.ControllerName)
			{
				GameObject controller = new GameObject(spawnableCharacterData.ControllerName);
				controller.transform.position = Vector3.zero;
				controller.transform.rotation = Quaternion.identity;
				Component comp = controller.AddComponent(controllers[i].GetType());
				ControllerBase controllerBase = (ControllerBase)comp;
				if (!controllerBase)
				{
					Debug.LogError("Controller could not be created correctly. GameObject => " + controller.name);
					return;
				}
				controllerBase.BeginPosses(characterToSpawn);
				break;
			}
		}
	}

	private void SpawnCharacterIntoWorld()
	{
		if (transform.childCount > 0)
		{
			characterToSpawn = transform.GetChild(0).gameObject;
			characterToSpawn.transform.parent = null;
		}
		else
		{
			if (spawnableCharacterData == null)
			{
				Debug.LogError("CharacterSpawner chould not spawn character because spawnableChracterData was null");
				return;
			}

			characterToSpawn = GameObject.Instantiate(spawnableCharacterData.CharacterPrefab, transform);
			SnapToGround();
			characterToSpawn.transform.parent = null;
		}
	}

	public void SnapToGround()
	{
		if (transform.childCount <= 0) return;
		GameObject go = transform.GetChild(0).gameObject;
		CharacterController characterController = go.GetComponent<CharacterController>();
		CapsuleCollider capsuleCollider = go.GetComponent<CapsuleCollider>();
		if (!characterController && !capsuleCollider) return;
		Vector3 p1 = transform.position + (characterController ? characterController.center + Vector3.up : capsuleCollider.center + Vector3.up) * (characterController ? -characterController.height : -capsuleCollider.height) * 0.5F;
		Vector3 p2 = p1 + Vector3.up * (characterController ? characterController.height : capsuleCollider.height);
		float capsulLenght = Vector3.Distance(p1, p2);
		RaycastHit hit;
		if (Physics.CapsuleCast(p1, p2, (characterController ? characterController.radius : capsuleCollider.radius), Vector3.down, out hit, 100000))
		{
			transform.position = hit.point + Vector3.up * ((characterController ? characterController.height : capsuleCollider.height) / 2f);
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawIcon(transform.position, "SpanwerImage", true);
	}
}
