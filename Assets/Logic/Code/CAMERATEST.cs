using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CAMERATEST : MonoBehaviour
{
    public Shader shader;
    void Start()
    {
        Camera.main.RenderWithShader(shader, "RenderType");
    }
}
