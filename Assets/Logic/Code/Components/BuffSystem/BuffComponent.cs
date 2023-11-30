using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EBuff
{
    Unknown,
    WaterUltimate,
}

public class BuffComponent
{
    GameCharacter gameCharacter;
    List<ABuff> buffList = new List<ABuff>();  

    public BuffComponent(GameCharacter owener)
    {
        gameCharacter = owener;

    }

    public void AddBuff(ABuff buff)
	{
        // If Buff Found remove old buff and apply new one
		ABuff foundBuff = buffList.Find((e) => { return e.GetBuffType() == buff.GetBuffType(); });
        if (foundBuff != null) 
            OnBuffFinished(foundBuff); 

		buffList.Add(buff);
        buff.onBuffFinished += OnBuffFinished;
	}

    public void Update(float deltaTime)
    {
        foreach (ABuff buff in buffList) 
        {
            buff.Update(deltaTime);
            //Ultra.Utilities.Instance.DebugLogOnScreen("Buff: " + buff.GetType().Name + " is Active", 0f, StringColor.Aqua);
        }
    }

    public void RemoveBuff(EBuff buffType)
    {
        ABuff buff = buffList.Find((e) => { return e.GetBuffType() == buffType; });
        OnBuffFinished(buff);
    }

	async void OnBuffFinished(ABuff buff)
    {
        buff.onBuffFinished -= OnBuffFinished;
        buff.BuffEnds();
        await new WaitForEndOfFrame();
        buffList.Remove(buff);
    }
}
