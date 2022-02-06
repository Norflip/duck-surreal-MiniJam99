using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Painting : MonoBehaviour, ITarget
{
    const int RT_TEXTURE_DEPTH = 16;
    const int DISPATCH_GROUP_SZ = 32;

    public Player player;
    public Frame frame;

    [Header("res")]
    public Vector2Int resolution;
    public float scale;

    [Header("camera")]
    public LayerMask layer;
    public Camera paintCamera;
    public float camOffset;
    public float camExtraThickness;

    public float Aspect => (float)resolution.x / (float)resolution.y;

    public Material material;
    public ComputeShader brush_cs;
    //public ComputeShader composite_cs;
    
    [Header("tween")]        
    public float shakeDuration = 0.1f;
    public float shakeStrength = 0.1f;

    [Header("debug")]
    [SerializeField, NaughtyAttributes.ReadOnly] RenderTexture brushTexture;
    [SerializeField, NaughtyAttributes.ReadOnly] RenderTexture resultTexture;

    public bool initializeOnStart = false;

    private void Start() {
        if(initializeOnStart)
            Initialize(resolution.x, resolution.y);
    }

    private void OnValidate() {
        SetCanvasSize();
        CenterCamera();
    }

    public void Initialize (int width, int height)
    {
        resolution = new Vector2Int(width, height);
        SetCanvasSize();

        brushTexture = new RenderTexture(width, height, RT_TEXTURE_DEPTH);
        brushTexture.enableRandomWrite = false;
        brushTexture.filterMode = FilterMode.Point;
        brushTexture.anisoLevel = 1;
        brushTexture.Create();
        
        resultTexture = new RenderTexture(width, height, 0);
        resultTexture.filterMode = FilterMode.Bilinear;
        resultTexture.enableRandomWrite = true;
        resultTexture.antiAliasing = 2;
        resultTexture.Create();
 
        paintCamera.depthTextureMode = paintCamera.depthTextureMode | DepthTextureMode.Depth;
        paintCamera.targetTexture = brushTexture;

        frame.Generate();
        CenterCamera();
    }

    public void SetCanvasSize ()
    {
        float width = scale * Aspect;
        float height = scale;
        transform.localScale = new Vector3(width, height, 1.0f);
        transform.localPosition = new Vector3(0.0f, height * 0.5f, 0.0f);
    }

    public void CenterCamera ()
    {
        if(paintCamera != null)
        {
            float width = transform.localScale.x;
            float height = transform.localScale.y;

            paintCamera.transform.position = transform.position + transform.forward * camOffset * -1;
            paintCamera.aspect = width / height;
            paintCamera.orthographic = true;
            paintCamera.orthographicSize = (height * 0.5f);// + padding;
            paintCamera.farClipPlane = camOffset + camExtraThickness;
            paintCamera.cullingMask = layer.value;
        }
    }

    public void Shake ()
    {
        transform.DOComplete();
        transform.DOShakePosition(shakeDuration, shakeStrength, 10, 90, false, true);
    }

    private void LateUpdate() {
        TriggerBrush();
    }

    public void TriggerBrush ()
    {
        RenderTexture.active = paintCamera.targetTexture = brushTexture;
        paintCamera.Render();

        Vector4 _ZBufferParams = new Vector4((paintCamera.farClipPlane / paintCamera.nearClipPlane) - 1.0f, 1.0f, 0.0f, 0.0f);        
        brush_cs.SetVector("_ZBufferParams", _ZBufferParams);

        int thrx = Mathf.CeilToInt(resolution.x / (float)DISPATCH_GROUP_SZ);
        int thry = Mathf.CeilToInt(resolution.y / (float)DISPATCH_GROUP_SZ);

        brush_cs.SetTextureFromGlobal(0, "_DepthTexture", "_LastCameraDepthTexture");


        brush_cs.SetTexture(0, "_Result", resultTexture);
        brush_cs.SetVector("_Color", (Vector4)player.selectedColor);
        brush_cs.SetInts("_Size", resolution.x, resolution.y);
        brush_cs.Dispatch(0, thrx, thry, 1);


        material.SetTexture("_MainTex", resultTexture);
        //RenderTexture.active = currentRT;
    }

    public Plane GetPlane()
    {
        return new Plane(transform.forward * -1, transform.position);
    }
}
