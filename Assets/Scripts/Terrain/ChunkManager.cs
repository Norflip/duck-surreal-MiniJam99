using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    public Transform player;
    public Chunk chunk0;
    public Chunk chunk1;

    public float noiseScale = 0.1f;
    public float noiseHeight = 2.0f;

    Chunk[] chunks;
    int currentWorldIndex;

    private void Awake() {
        currentWorldIndex = GetPlayerOnChunkIndex();
        chunks = new Chunk[2] {chunk0, chunk1};
        chunk0.Generate(0, noiseScale, noiseHeight);
        chunk1.Generate(1, noiseScale, noiseHeight);
    }

    void Update ()
    {
        int index = GetPlayerOnChunkIndex();
        if(index != currentWorldIndex)
        {
            int activeChunk = index % 2;
            
        }
    }

    int GetPlayerOnChunkIndex ()
    {
        return Mathf.FloorToInt(player.position.z / Chunk.Size);
    }
}
