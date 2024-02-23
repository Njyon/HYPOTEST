using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EBuff
{
    Unknown,
    WaterUltimate,
    ForceAim,
    NoGravity,
    OnHitEffect,
    OnHitShaderEffect,
	HoldInAirAfterStartFalling,
}

public class BuffComponent
{
    GameCharacter gameCharacter;
    List<ABuff> buffList = new List<ABuff>();
    List<ABuff> removeBuffList = new List<ABuff>();

    public BuffComponent(GameCharacter owener)
    {
        gameCharacter = owener;

    }

    public bool IsBuffActive(EBuff buffType)
    {
		ABuff foundRemoveBuff = removeBuffList.Find((e) => { return e.GetBuffType() == buffType; });
        if (foundRemoveBuff != null) return false;
		
        ABuff foundBuff = buffList.Find((e) => { return e.GetBuffType() == buffType; });
		return foundBuff != null;
	}

    public void AddTimeToActiveBuff(EBuff buffType, float addDuration)
	{
		ABuff foundBuff = buffList.Find((e) => { return e.GetBuffType() == buffType; });
        foundBuff.DurationTimer.AddTime(addDuration);
	}

    public void ResetDurationOffActiveBuff(EBuff buffType)
	{
		ABuff foundBuff = buffList.Find((e) => { return e.GetBuffType() == buffType; });
        foundBuff.DurationTimer.Start();
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

    public void LateUpdate(float deltaTime)
    {
		foreach (ABuff removeBuff in removeBuffList)
		{
			buffList.Remove(removeBuff);
		}
		removeBuffList.Clear();
	}

    public void RemoveBuff(EBuff buffType)
    {
        ABuff buff = buffList.Find((e) => { return e.GetBuffType() == buffType; });
        OnBuffFinished(buff);
    }

	void OnBuffFinished(ABuff buff)
    {
        if (buff == null) return;

        buff.onBuffFinished -= OnBuffFinished;
        buff.INTERN_BuffEnds();
		ABuff foundRemoveBuff = removeBuffList.Find((e) => { return e.ID == buff.ID; });
        if (foundRemoveBuff == null)
		    removeBuffList.Add(buff);
    }
}
