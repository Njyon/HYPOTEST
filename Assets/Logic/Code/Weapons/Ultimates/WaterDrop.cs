using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using UnityEditor.Rendering;
using UnityEngine;

public class WaterDrop : MonoBehaviour
{
    bool isInit = false;
    GameCharacter gameCharacter = null;
    GameCharacter target = null;
    WaterDropPool pool = null;
    TrailRenderer tr;
    Vector3 startPos;
    float t;
    float speed;

    public TrailRenderer TrailRenderer { get { return tr; } }

	public void Init(GameCharacter owner, GameCharacter target, WaterDropPool pool, float speed)
	{
        isInit = true;
        gameCharacter = owner;
        this.target = target;
        this.pool = pool;
        this.speed = speed;
        startPos = transform.position;
        t = 0;

        this.target.onGameCharacterDied += OnGameCharacterDied;
	}

    public void TurnOff()
	{
		if (target != null) target.onGameCharacterDied -= OnGameCharacterDied;
		isInit = false;
    }

	void Awake()
	{
        tr = GetComponent<TrailRenderer>();
		isInit = false;
		startPos = transform.position;
		t = 0;
	}

    void Update()
    {
        if (isInit && gameCharacter != null && target != null)
        {
            t += Time.deltaTime * speed;
            transform.position = Vector3.Slerp(startPos, target.MovementComponent.CharacterCenter, t);
            if (t >= 1)
            {
                pool.ReturnValue(this);
                target.DoDamage(gameCharacter, 10f);
            }
        }
    }

	void OnGameCharacterDied(GameCharacter gameCharacter)
    {
        pool.ReturnValue(this);
    }
}
