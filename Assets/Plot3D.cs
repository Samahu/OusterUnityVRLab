using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class Plot3d : MonoBehaviour
{
    public GameObject squarePrefab;
    public Material material;   // The material used for the mesh (make sure it supports color)

    public string filePath = "C:\\Users\\unaal\\data\\rgb_0100.pcd";

    private Matrix4x4[] transforms;
    private MaterialPropertyBlock propertyBlock;  // Property block for additional material properties

    // public Color32[] colors;

    public static Vector3[] ReadPCDFile(string filePath)
    {
        List<Vector3> points = new List<Vector3>();

        using (StreamReader reader = new StreamReader(filePath))
        {
            string line;
            bool headerParsed = false;
            int numPoints = 0;

            while ((line = reader.ReadLine()) != null)
            {
                if (!headerParsed)
                {
                    if (line.StartsWith("POINTS"))
                    {
                        numPoints = int.Parse(line.Split(' ')[1]);
                        headerParsed = true;
                    }
                }
                else
                {
                    string[] values = line.Split(' ');
                    if (values.Length >= 3)
                    {
                        float x = float.Parse(values[0]);
                        float y = float.Parse(values[1]);
                        float z = float.Parse(values[2]);
                        // TODO: parse intensity / color
                        points.Add(new Vector3(x, z, y));

                        if (points.Count % 1e4 == 0)
                            Debug.Log("loaded so far: "+ points.Count); 

                        if (points.Count >= 1e6)
                            break;
                    }
                }
            }
        }

        return points.ToArray();
    }
    void Start()
    {
        Vector3[] points = ReadPCDFile(filePath);
        transforms = new Matrix4x4[points.Length];
        propertyBlock = new MaterialPropertyBlock();

        // colors = new Color32[points.Length];
        Vector3 scale = Vector3.one * 0.05f;
        for (int i = 0; i < points.Length; i++) {
            transforms[i] = Matrix4x4.TRS(points[i], Quaternion.identity, scale);
            // colors[i] = new Color32(64, 128, 192, 255);

            Color instanceColor = new Color(
                UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
            // Set the color in the MaterialPropertyBlock for this instance
            propertyBlock.SetColor("_Color", Color.red);

        }
    }

    void OnRenderObject() {
        Graphics.DrawMeshInstanced(squarePrefab.GetComponent<MeshFilter>().sharedMesh, 0,
                                   material, transforms, transforms.Length, propertyBlock);
    }

    void Update() {
        OnRenderObject();
    }
}