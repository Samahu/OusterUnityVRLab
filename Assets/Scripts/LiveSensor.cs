using UnityEngine;
using OusterSdkCSharp;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.XR;

public class LiveSensor : MonoBehaviour
{
    public string sensorURL;

    private OusterScanSource scanSource;

    public GameObject boxFilter;
    private Collider boxFilterCollider;

    private List<Vector3> filteredPoints;
    private List<Color> filteredColors;

    private List<Vector3> ghostPoints = new List<Vector3>();
    private List<Color> ghostColors = new List<Color>();

    private List<Vector3> headPoints = new List<Vector3>();
    private List<Color> headColors = new List<Color>();

    public GameObject colliderObject;

    // Cached collider for ghost gravitation (avoid GetComponent each frame)
    private Collider gravCollider;

    const float cooldownSeconds = 1f;
    private float lastAcceptTime = -100f;
    private bool lastPhysicalPressed = false;

    public Transform controllerTransform;

    void Start()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh.Clear();

        boxFilterCollider = boxFilter?.GetComponent<Collider>();
        gravCollider = colliderObject ? colliderObject.GetComponent<Collider>() : null;


        scanSource = OusterScanSource.Create(sensorURL);
        if (scanSource is null)
        {
            Debug.LogError("Failed to create scan source");
            return;
        }

        Debug.Log("Metadata: " + scanSource.GetMetadata());

        filteredPoints = new List<Vector3>(scanSource.Width * scanSource.Height);
        filteredColors = new List<Color>(scanSource.Width * scanSource.Height);
    }

    void OnDestroy()
    {
        scanSource?.Dispose();
    }

    private bool filterPoint(Vector3 point, out Vector3 tpoint)
    {
        tpoint = transform.TransformPoint(point);
        return boxFilterCollider.ClosestPoint(tpoint) != tpoint;
    }

    void GravitateGhostPoints()
    {
        if (colliderObject != null && colliderObject.activeSelf)
        {
            Collider collider = colliderObject.GetComponent<Collider>();
            // Vector3 controllerCenter = controllerTransform.position;
            Vector3 direction;
            for (int i = 0; i < ghostPoints.Count; ++i)
            {
                Vector3 point = transform.TransformPoint(ghostPoints[i]);
                if (collider.ClosestPoint(point) == point)
                {
                    // Move point slightly towards the center of the controller
                    direction = (controllerCenter - ghostPoints[i]).normalized;
                    ghostPoints[i] += direction * 0.1f;
                }
            }
        }
    }

    void Update()
    {
        // Capture snapshot of filtered points/colors when grip button is pressed
        if (GripButtonPressed())
        {
            ghostPoints.AddRange(filteredPoints);
            ghostColors.AddRange(filteredColors);
            ghostPoints.AddRange(headPoints);
            ghostColors.AddRange(headColors);
            Debug.Log($"Ghost snapshot stored: {ghostPoints.Count} points");
        }

        using var scan = scanSource.NextScan(0);
        if (scan is null)
            return;

        float[] xyz = scan.GetXYZ(filterInvalid: false);
        if (xyz == null)
        {
            return; // no new data or don't update when frozen
        }

        filteredPoints.Clear();
        filteredColors.Clear();
        headPoints.Clear();
        headColors.Clear();

        int w = scan.Width;
        int h = scan.Height;

        // inside check it is head
        Vector3 camPosition = Camera.main.transform.position;

        for (int y = 0; y < h; ++y)
        {
            for (int x = 0; x < w; ++x)
            {
                int i = y * w + x;
                Vector3 point = new Vector3(
                    xyz[3 * i + 0],
                    xyz[3 * i + 1],
                    xyz[3 * i + 2]);

                if (!filterPoint(point, out Vector3 tpoint))
                {
                    bool headPoint = (camPosition - tpoint).sqrMagnitude < (0.2f*0.2f);

                    float hue = (float)y / (h - 1); // normalize
                    Color color = Color.HSVToRGB(hue, 1f, 1f);

                    if (headPoint) {
                        headPoints.Add(point);
                        headColors.Add(color);
                    }
                    else
                    {
                        filteredPoints.Add(point);
                        filteredColors.Add(color);
                    }
                }
            }
        }

        GravitateGhostPoints();

        // Combine filtered points with ghost points
        if (ghostPoints.Count > 0)
        {
            filteredPoints.AddRange(ghostPoints);
            filteredColors.AddRange(ghostColors);
        }

        MeshFilter meshFilter = GetComponent<MeshFilter>();
        Mesh mesh = meshFilter.mesh;
        mesh.Clear();
        mesh.vertices = filteredPoints.ToArray();
        mesh.SetIndices(
            Enumerable.Range(0, filteredPoints.Count).ToArray(), 
            MeshTopology.Points, 0);
        mesh.colors = filteredColors.ToArray();
    }

    private bool GripButtonPressed()
    {
        InputDevice controller = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        bool physicalPressed = controller.TryGetFeatureValue(CommonUsages.gripButton, out bool pressed) && pressed;
        bool accept = false;
        // Rising edge && cooldown satisfied
        if (physicalPressed && !lastPhysicalPressed && (Time.time - lastAcceptTime) >= cooldownSeconds)
        {
            accept = true;
            lastAcceptTime = Time.time;
        }

        lastPhysicalPressed = physicalPressed;
        return accept;
    }
}
