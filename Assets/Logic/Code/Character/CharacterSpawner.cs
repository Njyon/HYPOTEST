using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.UIElements;

public class CharacterSpawner : MonoBehaviour
{
	GameObject characterToSpawn;
	// only for inspector override
	[HideInInspector] public ScriptableCharacter spawnableCharacterData;
	[HideInInspector] public int index = 0;
	[SerializeField] bool autoSpawn = true;

	void Start()
	{
		if (autoSpawn)
		{
			Spawn();
		}
		else
		{
			characterToSpawn = transform.GetChild(0).gameObject;
			Destroy(characterToSpawn);
		}
	}

	public void Spawn()
	{
		SpawnCharacterIntoWorld();
		SpawnController();
	}

	private void SpawnController()
	{
		Ultra.HypoUttilies.Instance.SpawnController(characterToSpawn, spawnableCharacterData);
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
		CapsuleCollider capsuleCollider = go.GetComponent<CapsuleCollider>();
		if (!capsuleCollider) return;
		RaycastHit hit;
		if (Ultra.Utilities.CapsulCast(go.transform.position + capsuleCollider.center, capsuleCollider.height, capsuleCollider.radius, Vector3.down * 100000, out hit, Color.red, 100))
		{
			transform.position = hit.point + (Vector3.up * (capsuleCollider.height / 2f) - capsuleCollider.center);
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawIcon(transform.position, "SpanwerImage", true);
	}
}