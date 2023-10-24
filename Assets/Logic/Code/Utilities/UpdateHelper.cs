using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateHelper : MonoBehaviour
{
    public delegate void eventAction();
    public eventAction onAwake;
    public eventAction onStart;
    public eventAction onUpdate;
    public eventAction onLateUpdate;
    public eventAction onFixedUpdate;

	void Awake()
	{
		if (onAwake != null) onAwake(); 
	}

	void Start()
    {
        if (onStart != null) onStart();
    }

    void Update()
    {
        if (onUpdate != null) onUpdate();   
    }

	void LateUpdate()
	{
	    if (onLateUpdate != null) onLateUpdate();	
	}

	void FixedUpdate()
	{
		if (onFixedUpdate != null) onFixedUpdate();	
	}
}
