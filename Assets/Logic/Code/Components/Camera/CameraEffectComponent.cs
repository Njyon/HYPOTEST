using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraEffectComponent
{
    CameraController cameraController;

    List<ACameraEffect> effects = new List<ACameraEffect>();
    public List<ACameraEffect> Effects {  get { return effects; } }

	public CameraEffectComponent(CameraController controller)
    {
        this.cameraController = controller;
    }

	public void Init()
    {
        
    }

    public void AddCameraEffect(ACameraEffect effect)
    {
        effects.Add(effect);
    }

    public void Update(float deltaTime)
    {
        List<ACameraEffect> finishedList = new List<ACameraEffect>();
        foreach (ACameraEffect effect in Effects)
        {
            effect.Update(deltaTime);
            if (effect.IsFinished) finishedList.Add(effect);
        }
        foreach (ACameraEffect effect in finishedList)
        {
            Effects.Remove(effect);
        }
    }
}
