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

    private void Awake() {
        chunks = new Chunk[2] {chunk0, chunk1};
        chunk0.Generate(0, noiseScale, noiseHeight);
        chunk1.Generate(1, noiseScale, noiseHeight);
    }

    void Update ()
    {
        int currentIndex = GetCurrentIndex();
        //Debug.Log(index + " -> " + (index % 2));

        float dz = chunks[currentIndex % 2].transform.position.z - player.position.z;
        if(dz < 0.0f && chunks[(currentIndex + 1) % 2].Index != currentIndex + 1)
            chunks[(currentIndex + 1) % 2].Generate(currentIndex + 1, noiseScale, noiseHeight);
    }

    int GetCurrentIndex ()
    {
        return Mathf.FloorToInt((player.position.z + Chunk.Size * 0.5f) / Chunk.Size);
    }
}
