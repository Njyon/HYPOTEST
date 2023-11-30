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
    float speed;

	public void Init(GameCharacter owner, GameCharacter target, WaterDropPool pool, float speed)
	{
        isInit = true;
        gameCharacter = owner;
        this.target = target;
        this.pool = pool;
        this.speed = speed;

        this.target.onGameCharacterDied += OnGameCharacterDied;
	}

    public void TurnOff()
	{
		if (target != null) target.onGameCharacterDied -= OnGameCharacterDied;
		isInit = false;
    }

	void Awake()
    {
        isInit = false;
    }

    void Update()
    {
        if (isInit && gameCharacter != null && target != null)
        {
            transform.position = Vector3.Slerp(transform.position, target.transform.position, Time.deltaTime * speed);
            if (transform.position.IsNearlyEqual(target.transform.position, 1f))
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
