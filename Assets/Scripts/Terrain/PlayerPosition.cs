using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PlayerPosition : MonoBehaviour
{
    public float g_PlayerFade;
    [Range(0.0f, 32.0f)]
    public float g_PlayerRadius;

    void Update ()
    {
        float z = transform.position.z;
        Shader.SetGlobalFloat("g_PlayerWorldPositionZ", z);
        Shader.SetGlobalFloat("g_PlayerFade", g_PlayerFade);
        Shader.SetGlobalFloat("g_PlayerRadius", g_PlayerRadius);
    }
}
