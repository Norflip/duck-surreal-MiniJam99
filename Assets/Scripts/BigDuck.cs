using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigDuck : MonoBehaviour
{
    public Player player;
    public float zOffset;
    public float yHeight;
    
    private void OnValidate() {
        if(player != null)
            Update();
    }

    void Update()
    {
        Vector3 playerPos = player.transform.position;
        playerPos.y = 0.0f;

        Vector3 duckPos = playerPos + Vector3.back * zOffset;
        duckPos.y = yHeight;
        transform.position = duckPos;
    }
}
