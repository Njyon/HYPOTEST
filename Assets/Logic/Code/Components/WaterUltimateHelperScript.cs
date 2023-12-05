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
		Vector3 prevPos = transform.position;
		Vector3 moveDir = transform.forward.normalized * (speed * Time.deltaTime);
		RaycastHit hit;
		if (Physics.Raycast(transform.position, Vector3.down, out hit, 2f, -5, QueryTriggerInteraction.Ignore))
		{
			moveDir = Vector3.ProjectOnPlane(moveDir, hit.normal);
		}else
		{
			moveDir = new Vector3(moveDir.x * 10, Physics.gravity.y, moveDir.z  * 10);
		}
		CollisionFlags collflag = characterController.Move(moveDir);

		// Weird bug fix for when wave moves to in World.Zero point
		float dist = Vector3.Distance(transform.position, prevPos);
		if (dist > moveDir.magnitude * 2f) 
			transform.position = prevPos;
		
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
