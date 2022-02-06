using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SoundCollection
{
    public Sound[] sounds;
    public Sound this[int key] => sounds[key];
    public int Length => sounds.Length;
}
