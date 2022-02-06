using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Sound
{
    public AudioClip clip;
    
    [Space(10.0f)]
    public bool randomizePitch;
    [NaughtyAttributes.ShowIf("randomizePitch")]
    public float minPitch;
    [NaughtyAttributes.ShowIf("randomizePitch")]
    public float maxPitch;
    
    [NaughtyAttributes.HideIf("randomizePitch")]
    public float pitch;
}

public class AudioRunner : MonoBehaviour
{
    public static AudioRunner Instance => m_instance;
    static AudioRunner m_instance;

    public AudioSource prefab;    
    Pool<AudioSource> sourcePool;

    private void Awake() {
        m_instance = this;
        sourcePool = new Pool<AudioSource>(prefab, 8);
    }

    public static void Play2D (SoundCollection collection, System.Action<AudioSource> callback = null) => Play2D(collection[Random.Range(0, collection.Length)], callback);
    public static void Play2D (Sound sound, System.Action<AudioSource> callback = null)
    {
        m_instance.Internal_Play2D(sound, callback);
    }

    public static void Play3D (SoundCollection collection, Vector3 position, System.Action<AudioSource> callback = null) => Play3D(collection[Random.Range(0, collection.Length)], position, callback);
    public static void Play3D (Sound sound, Vector3 position, System.Action<AudioSource> callback = null)
    {
        m_instance.Internal_Play3D(sound, position, callback);
    }

    void Internal_Play2D(Sound sound, System.Action<AudioSource> callback = null)
    {
        AudioSource src = sourcePool.Get();
        callback?.Invoke(src);
        src.spatialBlend = 0.0f;
        src.clip = null;
        src.clip = sound.clip;
        src.pitch = sound.randomizePitch? Random.Range(sound.minPitch, sound.maxPitch) : sound.pitch;

        if(src.pitch == 0.0f)
        {
            Debug.LogWarning("PITCH IS ZERO DUDE");
        }
        
        src.Stop();
        src.Play();

        StartCoroutine(ReturnSourceAfterTime(src, sound.clip.length * 1.1f));
    }

    void Internal_Play3D(Sound sound, Vector3 position, System.Action<AudioSource> callback = null)
    {

        
        AudioSource src = sourcePool.Get();
        callback?.Invoke(src);
        src.spatialBlend = 1.0f;
        src.clip = null;
        src.clip = sound.clip;
        src.pitch = sound.randomizePitch? Random.Range(sound.minPitch, sound.maxPitch) : sound.pitch;

        if(src.pitch == 0.0f)
        {
            Debug.LogWarning("PITCH IS ZERO DUDE");
        }

        src.transform.position = position;
        src.Stop();
        src.Play();

        StartCoroutine(ReturnSourceAfterTime(src, sound.clip.length * 1.1f));
    }

    IEnumerator ReturnSourceAfterTime(AudioSource source, float time)
    {
        yield return new WaitForSeconds(time);

        source.Stop();
        if(source.gameObject.activeInHierarchy)
            sourcePool.Return(source);

    }
}
