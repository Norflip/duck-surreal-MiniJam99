using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public const int Size = 32;
    // generate terrain
    int index;
    
    public void Generate (int index)
    {
        this.index = index;
        transform.position = new Vector3(0.0f, 0.0f, index * Size);
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(new Vector3(0.0f, 0.0f, index * Size), new Vector3(Size, 0, Size));
    }
}
