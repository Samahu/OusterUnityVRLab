using UnityEngine;
using OusterSdkCSharp;

public class LiveSensor : MonoBehaviour
{
    OusterScanSource scanSource;

    void Start()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        Mesh mesh = meshFilter.sharedMesh;

        Color[] colors = new Color[mesh.vertexCount];
        Vector3[] vertices = new Vector3[mesh.vertexCount];
        for (int i = 0; i < colors.Length; ++i) {
            colors[i] = Color.gray;
            vertices[i] = Vector3.zero;
        }

        mesh.colors = colors;
        mesh.vertices = vertices;

        scanSource = OusterScanSource.Create("169.254.101.54");
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

    void Update()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        Mesh mesh = meshFilter.sharedMesh;
        Vector3[] vertices = mesh.vertices;
        Color[] colors = mesh.colors;

        var scan = scanSource.NextScan(0);
        if (scan is null)
            return;

        float[] xyz = scan.GetXYZ(filterInvalid: false);
        byte[,] reflectivity = scan.GetField<byte>("REFLECTIVITY");
        int w = scan.Width;
        int h = scan.Height;
        scan.Dispose();

        if (xyz == null || reflectivity == null)
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
                float intensity = reflectivity[y, x] / 255.0f;
                colors[i] = new Color(intensity, intensity, intensity);
                vertices[i] = new Vector3(
                    xyz[3 * i + 0],
                    xyz[3 * i + 1],
                    xyz[3 * i + 2]);
            }
        }

        mesh.colors = colors;
        mesh.vertices = vertices;
    }

}
