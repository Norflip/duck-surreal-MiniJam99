using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour
{
    [NaughtyAttributes.Layer]
    public int paintingLayer;
    public float timeToPaint = 0.2f;
    [NaughtyAttributes.Layer]
    public int layerAfterPainted;

    public ParticleSystem collisionParticles;
    public AudioClip collisionClip;
    
    public float collisionNormalForce = 500.0f;
    
    public int hits = 0;

    Rigidbody body;

    private void Awake() {
        body = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision other) {
        if(other.gameObject.layer == paintingLayer)
        {
            Painting painting = other.gameObject.GetComponent<Painting>();
            painting.Shake();
            body.AddForce(painting.PaintPlane.normal * collisionNormalForce);

            //painting.TriggerBrush();

            //gameObject.layer = layerAfterPainted;
        
            StartCoroutine(SetLayer());
            hits++;
        }
    }

    IEnumerator SetLayer ()
    {
        //yield return new WaitForSeconds(timeToPaint);
        yield return new WaitForEndOfFrame();

        gameObject.layer = layerAfterPainted;

    }
    
}
