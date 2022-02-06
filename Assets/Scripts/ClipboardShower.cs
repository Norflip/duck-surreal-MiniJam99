using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClipboardShower : MonoBehaviour
{

    public Vector3 hidePos;
    public float smoothTime = 0.2f;
    private Vector3 refVelocity;
    private Vector3 targetPos;
    private Vector3 defaultPos;

    private void Awake()
    {
        defaultPos = transform.localPosition;
    }

    void Update()
    {
        float t = Input.GetKey(KeyCode.LeftShift) ? 1 : 0;
        targetPos = Vector3.Lerp(hidePos, defaultPos, t);
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, targetPos, ref refVelocity, smoothTime);
    }
}
