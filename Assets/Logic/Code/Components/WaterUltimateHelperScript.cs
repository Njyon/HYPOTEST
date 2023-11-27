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
	}

	void Update()
	{
		lifeTime += Time.deltaTime;	
		CollisionFlags collflag = characterController.Move(transform.forward * (speed * Time.deltaTime));

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
