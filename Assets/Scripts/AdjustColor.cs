using UnityEngine;

public class AdjustColor : MonoBehaviour
{
    public GameObject colliderObject;

    private Color[] originalColors;

    void Start()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        Mesh mesh = meshFilter.sharedMesh;
        originalColors = mesh.colors;

        Color[] colors = new Color[originalColors.Length];
        for (int i = 0; i < colors.Length; ++i)
            colors[i] = Color.gray;

        mesh.colors = colors;
    }

    void OnDestroy() {
        // restore saved colors
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        Mesh mesh = meshFilter.sharedMesh;
        mesh.colors = originalColors;
    }

    void Update()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        Mesh mesh = meshFilter.sharedMesh;
        Vector3[] vertices = mesh.vertices;
        Color[] colors = mesh.colors;

        if (colliderObject != null && colliderObject.activeSelf)
        {
            Collider collider = colliderObject.GetComponent<Collider>();
            for (int i = 0; i < colors.Length; ++i)
            {
                Vector3 point = transform.TransformPoint(vertices[i]);
                if (collider.ClosestPoint(point) == point) {
                    colors[i] = originalColors[i];
                }
            }
        }

        mesh.colors = colors;
    }
}
