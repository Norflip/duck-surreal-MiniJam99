using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particles : MonoBehaviour
{
    static Particles m_Instance;

    public ParticleSystem feathersPrefab;

    Pool<ParticleSystem> featherPool;
    
    private void Awake() {
        m_Instance = this;
        featherPool = new Pool<ParticleSystem>(feathersPrefab, 3);
    }

    public static void SpawnFeathers (Transform target,Vector3 pos)
    {
        m_Instance.Internal_Spawn(target, pos);
    }

    void Internal_Spawn(Transform target, Vector3 pos)
    {
        ParticleSystem particle = featherPool.Get();
        particle.transform.SetParent(target);
        particle.transform.position = pos;
        particle.Play();

        float time = particle.main.startLifetime.constant * particle.main.startLifetimeMultiplier;
        StartCoroutine(ReturnParticlesAfterTime(particle, time));
    }

    IEnumerator ReturnParticlesAfterTime (ParticleSystem system, float time)
    {
        yield return new WaitForSeconds(time);
        system.Stop();
        featherPool.Return(system);
    }
}
