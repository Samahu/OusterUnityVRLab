using UnityEngine;
using OusterSdkCSharp;

public class LiveSensor : MonoBehaviour
{
    public GameObject boxFilter;

    public string sensorURL = "169.254.101.54";

    private OusterScanSource scanSource;

    private Collider collider;

    void Start()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        Mesh mesh = meshFilter.sharedMesh;
        collider = boxFilter?.GetComponent<Collider>();

        Color[] colors = new Color[mesh.vertexCount];
        Vector3[] vertices = new Vector3[mesh.vertexCount];
        for (int i = 0; i < colors.Length; ++i) {
            colors[i] = Color.gray;
            vertices[i] = Vector3.zero;
        }
        mesh.colors = colors;
        mesh.vertices = vertices;

        scanSource = OusterScanSource.Create(sensorURL);
        if (scanSource is null)
        {
            Debug.LogError("Failed to create scan source");
            return;
        }

        Debug.Log("Metadata: " + scanSource.GetMetadata());
    }

    void OnDestroy()
    {
        scanSource?.Dispose();
    }

    (byte, byte) getMinMax(byte[,] reflectivity)
    {
        byte minReflectivity = byte.MaxValue;
        byte maxReflectivity = byte.MinValue;

        for (int y = 0; y < reflectivity.GetLength(0); ++y)
        {
            for (int x = 0; x < reflectivity.GetLength(1); ++x)
            {
                byte value = reflectivity[y, x];
                if (value < minReflectivity) minReflectivity = value;
                if (value > maxReflectivity) maxReflectivity = value;
            }
        }

        return (minReflectivity, maxReflectivity);
    }

    private bool filterPoint(Vector3 point)
    {
        Vector3 tpoint = transform.TransformPoint(point);
        return collider.ClosestPoint(tpoint) != tpoint;
    }

    void Update()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        Mesh mesh = meshFilter.sharedMesh;
        Vector3[] vertices = mesh.vertices;
        Color[] colors = mesh.colors;

        using var scan = scanSource.NextScan(0);
        if (scan is null)
            return;

        float[] xyz = scan.GetXYZ(filterInvalid: false);
        int w = scan.Width;
        int h = scan.Height;

        if (xyz == null || h == 0)
        {
            return; // no new data this frame
        }

        var maxPoints = Mathf.Min(vertices.Length, xyz.Length / 3);

        for (int y = 0; y < h; ++y)
        {
            for (int x = 0; x < w; ++x)
            {
                int i = y * w + x;
                if (i >= maxPoints)
                    break;

                Vector3 point = new Vector3(
                        xyz[3 * i + 0],
                        xyz[3 * i + 1],
                        xyz[3 * i + 2]);
                float hue = (float)y / (h - 1); // normalize
                colors[i] = Color.HSVToRGB(hue, 1f, 1f);
                Vector3 tpoint = transform.TransformPoint(point);
                vertices[i] = filterPoint(point) ? Vector3.zero : point;
            }
        }

        mesh.colors = colors;
        mesh.vertices = vertices;
    }

}
