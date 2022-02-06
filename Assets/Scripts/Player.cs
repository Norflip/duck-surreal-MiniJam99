using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;

public class Player : MonoBehaviour
{
    public Color selectedColor;

    [Header("head")]
    public Transform head;
    public float mouseSensitivity = 1.2f;
    public bool invertY = false;

    [Header("launching")]
    public float throwAngle = 30.0f;
    public Transform launchPivot;
    public LineRenderer projectilePath;
    
    public float cooldown = 0.2f;

    public bool allowInput;
    public Painting painting;
    public Bird birdPrefab;

    public float minTorque = 100.0f;
    public float maxTorque = 400.0f;

    public float minScale = 0.2f;
    public float maxScale = 2.0f;
    public int scaleSteps = 9;
    public TextMeshProUGUI scaleText;

    [Header("targeting")]
    public Transform targetHitPoint;
    public LayerMask hitLayerMask;
    public float radius = 0.8f;
    public float notHitForwardDistance = 5.0f;

    [Header("movement")]
    public bool run;
    public float speed = 5.0f;
    public float headHeight = 1.0f;
    public LayerMask terrainLayer;

    [Header("rotation")]
    public float maxY;
    public float hardCap = 10.0f;

    [Header("main cam")]
    public Camera mainCamera;

    Bird inHand;
    float lastThrowTime;

    Pool<Bird> birdPool;
    [SerializeField, NaughtyAttributes.ReadOnly]
    float xAxisClamp, yAxisClamp;

    int currentScaleStep;

    private void Awake() {
        if(painting == null)
            painting = FindObjectOfType<Painting>();
        
        if(projectilePath == null)
            projectilePath = GetComponentInChildren<LineRenderer>();

        birdPool = new Pool<Bird>(birdPrefab, 12);
        Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
        
        currentScaleStep = 2;

        lastThrowTime = Time.time;
        CreateBirdInHand();
    }

    void CreateBirdInHand ()
    {
        inHand = birdPool.Get(launchPivot);
        inHand.transform.localPosition = Vector3.zero;
        inHand.body.isKinematic = true;
        inHand.launched = false;

        Collider[] colliders = inHand.GetComponentsInChildren<Collider>();
        for (int i = 0; i < colliders.Length; i++)
            colliders[i].enabled = false;

        Renderer rend = inHand.GetComponentInChildren<Renderer>();
        MaterialPropertyBlock block = new MaterialPropertyBlock();
        block.SetColor("_SelectedColor", selectedColor);
        rend.SetPropertyBlock(block);
    }

    private void Update() {

        RotateCamera();

        if(inHand != null)
        {
            inHand.transform.localPosition = Vector3.zero;
            inHand.transform.localRotation = Quaternion.identity;

            currentScaleStep += Mathf.RoundToInt(Input.mouseScrollDelta.y);
            currentScaleStep = Mathf.Clamp(currentScaleStep, 1, scaleSteps);
            scaleText.text = currentScaleStep.ToString();

            float scale = Mathf.Lerp(minScale, maxScale, (float)(currentScaleStep-1) / (float)scaleSteps);
            inHand.transform.localScale = Vector3.one * scale;


        }
        
        float enter;

        RaycastHit hit;
        Ray ray = new Ray(head.position, head.forward);
        targetHitPoint.position = head.position + head.forward * notHitForwardDistance;
        targetHitPoint.rotation = LookRot(head.forward);

        if(Physics.SphereCast(ray, radius, out hit, 1000.0f, hitLayerMask.value))
        {
            ITarget t = hit.transform.GetComponent<ITarget>();
            if(t != null)
            {
                Plane plane = t.GetPlane();
                if(plane.Raycast(ray, out enter))
                {
                    targetHitPoint.position = ray.GetPoint(enter) + plane.normal * 0.01f;
                    targetHitPoint.rotation = LookRot(-plane.normal);
                }
            }
        }

        if(lastThrowTime + cooldown < Time.time)
        {
                inHand.gameObject.SetActive(true);

            if(Input.GetMouseButtonDown(0))
            {
                lastThrowTime = Time.time;

                Vector3 launch = LaunchDirection() * LaunchForce();
                if(float.IsNaN(launch.x) || float.IsNaN(launch.y) || float.IsNaN(launch.z))
                {
                    launch = head.forward * 10.0f;
                }

                Bird bird = inHand;
                bird.transform.SetParent(null);
                Collider[] colliders = bird.GetComponentsInChildren<Collider>();
                for (int i = 0; i < colliders.Length; i++)
                    colliders[i].enabled = true;

                Rigidbody body = bird.GetComponent<Rigidbody>();
                body.position = body.transform.position = launchPivot.position;
                body.isKinematic = false;
                body.AddForce(launch, ForceMode.VelocityChange);
                body.AddTorque(Random.onUnitSphere * Random.Range(minTorque, maxTorque));
                body.velocity += Vector3.forward * speed;
                inHand.launched = true;

                inHand = null;
                CreateBirdInHand();
                inHand.gameObject.SetActive(false);
                // CREATE NEXT BIRD
            }
        }

       

        if(run)
        {
            Vector3 next = transform.position + Vector3.forward * speed * Time.deltaTime;
            next.y = headHeight;

            if(Physics.Raycast(transform.position + Vector3.up * 5.0f, Vector3.down, out hit, 10.0f, terrainLayer.value))
                next.y += hit.point.y;
            
            transform.position = next;
        }
    }

    Quaternion LookRot (Vector3 dir)
    {
        return Quaternion.LookRotation(dir, (Mathf.Abs(Vector3.Dot(dir, Vector3.up)) > 1.0f - Mathf.Epsilon) ? Vector3.right : Vector3.up);
    }

    public void SelectColor (Color color)
    {
        this.selectedColor = color;
        if(inHand != null)
        {
            Renderer rend = inHand.GetComponentInChildren<Renderer>();
            MaterialPropertyBlock block = new MaterialPropertyBlock();
            block.SetColor("_SelectedColor", selectedColor);
            rend.SetPropertyBlock(block);
        }
    }

    private void RotateCamera()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        float rotAmountX = mouseX * mouseSensitivity;
        float rotAmountY = mouseY * mouseSensitivity;

        xAxisClamp -= rotAmountY;
        yAxisClamp -= rotAmountX;

        //Vector3 targetRotCamera = transform.localRotation.eulerAngles;
        Vector3 targetRotBody = head.localRotation.eulerAngles;

        if (invertY)
            rotAmountY *= -1;

        targetRotBody.x -= rotAmountY;
        targetRotBody.z = 0;

        targetRotBody.y += rotAmountX;

        if (xAxisClamp > 90)
        {
            xAxisClamp = targetRotBody.x = 90;
        }
        else if (xAxisClamp < -90)
        {
            xAxisClamp = -90f;
            targetRotBody.x = 270f;
        }

        if (yAxisClamp > 90 - maxY)
        {
            Debug.Log("clamp >");
            yAxisClamp = Mathf.Lerp(yAxisClamp, 90 - maxY, Time.deltaTime * 0.5f);
            targetRotBody.y = -yAxisClamp;
        }
        else if (yAxisClamp < -90 + maxY)
        {
            Debug.Log("clamp <");
            yAxisClamp = Mathf.Lerp(yAxisClamp, -90 + maxY, Time.deltaTime * 0.5f);
            targetRotBody.y = -yAxisClamp;
        }

        //transform.localRotation = Quaternion.Euler(targetRotBody);
        head.rotation = Quaternion.Euler(targetRotBody);
    }

    Vector3 LaunchDirection ()
    {
        float angle = throwAngle * Mathf.Deg2Rad;
        Quaternion yrot = Quaternion.AngleAxis(transform.eulerAngles.y, Vector3.up);

        Vector3 dirxz = new Vector3(targetHitPoint.position.x - launchPivot.position.x, 0.0f, targetHitPoint.position.z - launchPivot.position.z);
        yrot = Quaternion.LookRotation(dirxz, Vector3.up);

        return (yrot * new Vector3(0.0f, Mathf.Sin(angle), Mathf.Cos(angle))).normalized;
    }

    float LaunchForce ()
    {
        float angle = throwAngle * Mathf.Deg2Rad;
        float distance = Vector3.Distance(targetHitPoint.position, transform.position);
        float yOffset = transform.position.y - targetHitPoint.position.y;

        float A = (1 / Mathf.Cos(angle));
        float B = (0.5f * -Physics.gravity.y * (distance * distance));
        float C = (distance * Mathf.Tan(angle) + yOffset);
        float D = B / C;
        float E = Mathf.Sqrt(D);

        return A * Mathf.Sqrt(B / C);
    }

    void CalculateTrajectory ()
    {
        float mass = 1.0f; // birdPrefab.GetComponent<Rigidbody>().mass;
        int steps = 32;
        projectilePath.positionCount = steps;
        Vector3[] points = new Vector3[steps];
        
        Vector3 position = launchPivot.position;
        Vector3 velocity = (LaunchDirection() * LaunchForce()) / mass;
        points[0] = position;

        for(int i = 1; i  < steps; i++)
        {
            Vector3 acc = Vector3.zero;
            acc += AddForceDummy(Physics.gravity, mass);

            position += velocity * 0.1f;
            velocity += acc * 0.1f;

            points[i] = position;
        }

        projectilePath.SetPositions(points);
    }

    Vector3 AddForceDummy(Vector3 force, float mass)
    {
        return force / mass;
    }

    private void OnDrawGizmos() {
        Gizmos.DrawLine(targetHitPoint.position, launchPivot.position);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(head.position, head.position + head.forward * 1.0f);
    }
}
