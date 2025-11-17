using System.IO;
using UnityEngine;
using Pcx;

public class UpdatePointCloud : MonoBehaviour
{
    PlyImporter plyImporter;
    FileInfo[] fileList;
    private float elapsedTime = 0f;
    private float lastUpdate = 0f;
    private int currSequence = 0;

    Mesh[] meshes;

    public static FileInfo[] GetFileList(string directoryPath, string extension)
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);
        FileInfo[] files = directoryInfo.GetFiles("*." + extension);
        return files;
    }

    void Start()
    {
        plyImporter = new PlyImporter();
        fileList = GetFileList("C:\\Users\\unaal\\data\\rgb_demo\\", "ply");
        meshes = new Mesh[fileList.Length];
        for (int i = 0; i < fileList.Length; ++i){
            meshes[i] = plyImporter.ImportAsMesh(fileList[i].FullName);
        }
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;

        if (elapsedTime - lastUpdate > 0.1f) {
            lastUpdate = elapsedTime;
            currSequence = (currSequence + 1) % fileList.Length;
            GetComponent<MeshFilter>().sharedMesh = meshes[currSequence];
        }
    }
}
