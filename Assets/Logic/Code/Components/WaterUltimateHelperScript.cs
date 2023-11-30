using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class WaterUltimateHelperScript : MonoBehaviour
{
    public GameCharacterDetection gameCharacterDetection;
    public BoxCollider boxCollider;
	public CharacterController characterController;

	GameCharacter gameCharacter;
	float speed;
	float lifeTime;
	bool isInit = false;

	public void Awake()
	{
		if (gameCharacterDetection == null) gameCharacterDetection = GetComponent<GameCharacterDetection>();
		if (boxCollider == null) boxCollider = GetComponent<BoxCollider>();
		if (characterController == null) characterController = GetComponent<CharacterController>();
	}

	public void Init(GameCharacter owner, float speed, float lifeTime)
	{
		gameCharacter = owner;
		this.speed = speed;
		GameObject.Destroy(gameObject, lifeTime);
		lifeTime = 0;
		isInit = true;
	}

	void Update()
	{
		if (!isInit) return;

		lifeTime += Time.deltaTime;
		//characterController.Move(transform.forward * (Physics.gravity.y * Time.deltaTime));
		CollisionFlags collflag = characterController.Move(characterController.transform.forward.normalized * (speed * Time.deltaTime));
		
		if (lifeTime > 1f)
		{
			if ((collflag & CollisionFlags.CollidedSides) != 0)
			{
				// Animation? or Effects?
				GameObject.Destroy(gameObject);
			}
		}
	}

	void OnDestroy()
	{
		
	}
}
