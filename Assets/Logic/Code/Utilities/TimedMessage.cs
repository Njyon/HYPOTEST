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
    public TimedMessage(string message, float time) {
        this.message = message;
        this.time = time;
    }
}