using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedMessage {
    string message;
    public string Message { get { return message; } }
    float time;
    public float Time {
        get { return time; }
        set { time = value; }
    }
    int debugLevel;
    public int DebugLevel { 
        get { return debugLevel; }
        set { debugLevel = value; }
    }
    DebugAreas debugArea;
    public DebugAreas DebugArea {
        get { return debugArea; }
        set { debugArea = value; }
	}

    public TimedMessage(string message, float time, int debugLevel = 100, DebugAreas debugArea = DebugAreas.Misc) {
        this.message = message;
        this.time = time;
        this.debugLevel = debugLevel;
        this.debugArea = debugArea;
    }
}