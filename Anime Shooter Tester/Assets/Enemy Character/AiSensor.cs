using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class AiSensor : MonoBehaviour
{
    public float distance = 10;
    public float angle = 30;
    public float height = 1.0f;
    public Color meshColor = Color.red;
    public int scanFrequency = 30;
    public LayerMask layers;
    public LayerMask occlusionLayers;

    public List<GameObject> Objects
    {
        get
        {
            objects.RemoveAll(obj => !obj);
            return objects;
        }
    }

    private List<GameObject> objects = new List<GameObject>();
    private Collider[] colliders = new Collider[50];
    private Mesh mesh;
    private int count;
    private float scanInterval;
    private float scanTimer;
    private float distanceSq;

    private GameObject wedge;

    void Start()
    {
        scanInterval = 1.0f / scanFrequency;
        distanceSq = distance * distance;

        // Create runtime wedge mesh object
        wedge = new GameObject("AiSensor Mesh");
        wedge.transform.parent = transform;
        wedge.transform.localPosition = Vector3.zero;
        wedge.transform.localRotation = Quaternion.identity;

        MeshFilter meshFilter = wedge.AddComponent<MeshFilter>();
        mesh = CreateWedgeMesh();
        meshFilter.mesh = mesh;

        MeshRenderer meshRenderer = wedge.AddComponent<MeshRenderer>();
        Material mat = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
        mat.color = new Color(meshColor.r, meshColor.g, meshColor.b, 0.3f);
        meshRenderer.material = mat;

        meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        meshRenderer.receiveShadows = false;

        wedge.layer = LayerMask.NameToLayer("Ignore Raycast"); // Optional: avoid interfering with detection
    }

    void Update()
    {
        scanTimer -= Time.deltaTime;
        if (scanTimer < 0)
        {
            scanTimer += scanInterval;
            Scan();
        }

        // Optional: update mesh if parameters change
        if (Application.isPlaying)
        {
            mesh = CreateWedgeMesh();
            wedge.GetComponent<MeshFilter>().mesh = mesh;
        }
    }

    private void Scan()
    {
        count = Physics.OverlapSphereNonAlloc(transform.position, distance, colliders, layers, QueryTriggerInteraction.Collide);
        objects.Clear();
        for (int i = 0; i < count; ++i)
        {
            GameObject obj = colliders[i].gameObject;
            if (IsInSight(obj))
            {
                objects.Add(obj);
            }
        }
    }

    public bool IsInSight(GameObject obj)
    {
        Vector3 origin = transform.position;
        Vector3 dest = obj.transform.position;
        Vector3 direction = dest - origin;

        if (direction.y > height || direction.y < 0)
            return false;

        direction.y = 0;
        if (direction.sqrMagnitude > distanceSq)
            return false;

        float deltaAngle = Vector3.Angle(direction, transform.forward);
        if (deltaAngle > angle)
            return false;

        origin.y += height / 2;
        dest.y = origin.y;
        if (Physics.Linecast(origin, dest, occlusionLayers))
            return false;

        return true;
    }

    private Mesh CreateWedgeMesh()
    {
        Mesh mesh = new Mesh();

        int segments = 10;
        int numTriangles = (segments * 4) + 2 + 2;
        int numVertices = numTriangles * 3;

        Vector3[] vertices = new Vector3[numVertices];
        int[] triangles = new int[numVertices];

        Vector3 bottomCenter = Vector3.zero;
        Vector3 bottomLeft = Quaternion.Euler(0, -angle, 0) * Vector3.forward * distance;
        Vector3 bottomRight = Quaternion.Euler(0, angle, 0) * Vector3.forward * distance;

        Vector3 topCenter = bottomCenter + Vector3.up * height;
        Vector3 topRight = bottomRight + Vector3.up * height;
        Vector3 topLeft = bottomLeft + Vector3.up * height;

        int vert = 0;

        // Sides
        vertices[vert++] = bottomCenter; vertices[vert++] = bottomLeft; vertices[vert++] = topLeft;
        vertices[vert++] = topLeft; vertices[vert++] = topCenter; vertices[vert++] = bottomCenter;

        vertices[vert++] = bottomCenter; vertices[vert++] = topCenter; vertices[vert++] = topRight;
        vertices[vert++] = topRight; vertices[vert++] = bottomRight; vertices[vert++] = bottomCenter;

        float currentAngle = -angle;
        float deltaAngle = (angle * 2) / segments;
        for (int i = 0; i < segments; ++i)
        {
            bottomLeft = Quaternion.Euler(0, currentAngle, 0) * Vector3.forward * distance;
            bottomRight = Quaternion.Euler(0, currentAngle + deltaAngle, 0) * Vector3.forward * distance;
            topRight = bottomRight + Vector3.up * height;
            topLeft = bottomLeft + Vector3.up * height;

            // Far side
            vertices[vert++] = bottomLeft; vertices[vert++] = bottomRight; vertices[vert++] = topRight;
            vertices[vert++] = topRight; vertices[vert++] = topLeft; vertices[vert++] = bottomLeft;

            // Top
            vertices[vert++] = topCenter; vertices[vert++] = topLeft; vertices[vert++] = topRight;

            // Bottom
            vertices[vert++] = bottomCenter; vertices[vert++] = bottomRight; vertices[vert++] = bottomLeft;

            currentAngle += deltaAngle;
        }

        for (int i = 0; i < numVertices; ++i) triangles[i] = i;

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        return mesh;
    }

    private void OnValidate()
    {
        mesh = CreateWedgeMesh();
        scanInterval = 1.0f / scanFrequency;
        distanceSq = distance * distance;

        if (Application.isPlaying && wedge != null)
        {
            wedge.GetComponent<MeshFilter>().mesh = mesh;
        }
    }

    public int Filter(GameObject[] buffer, string layerName)
    {
        int layer = LayerMask.NameToLayer(layerName);
        int count = 0;
        foreach (var obj in Objects)
        {
            if (obj.layer == layer)
            {
                buffer[count++] = obj;
                if (buffer.Length == count)
                    break;
            }
        }
        return count;
    }
}
