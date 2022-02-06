using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Jobs;
using Unity.Collections;
using Unity.Burst;
using Unity.Jobs;
using Unity.Mathematics;

[BurstCompile]
public struct GeneratePalette : IJobParallelFor
{
    [ReadOnly]
    public NativeArray<Vector4> pixels;

    public int width;
    public int height;
    public int maxColors;

    public void Execute(int index)
    {
        
    }
}

public class palettegen : MonoBehaviour
{
    public Texture2D painting;

    
}
