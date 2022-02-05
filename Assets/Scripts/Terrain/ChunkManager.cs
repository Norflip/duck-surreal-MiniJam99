using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    public Transform player;

    Chunk[] chunks;
    int currentWorldIndex;


    int GetPlayerOnChunkIndex ()
    {
        return Mathf.FloorToInt(player.position.z / Chunk.Size);
    }

    void Update ()
    {
        int index = GetPlayerOnChunkIndex();
        if(index != currentWorldIndex)
        {

        }
    }
}
