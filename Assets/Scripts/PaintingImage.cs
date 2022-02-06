using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PaintingImage : ScriptableObject
{
    public Texture2D image;
    public Color[] palette;
}
