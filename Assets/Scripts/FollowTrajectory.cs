using System.IO;
using TMPro;
using UnityEngine;


public class FollowTrajectory : MonoBehaviour
{
    public string trajectoryPath;
    private string[] trajectory;
    private float elapsedTime = 0f;
    private float lastUpdate = 0f;
    private int currSequence = 0;
    private Transform cam_transform;
    private Transform map_transform;
    private const float UPDATE_INTERVAL = 0.01f;

    void Start()
    {
        trajectory = File.ReadAllLines(trajectoryPath);
        cam_transform = GameObject.Find("Camera Offset").transform;
        map_transform = GameObject.Find("full_map").transform;
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime - lastUpdate > UPDATE_INTERVAL) {
            lastUpdate = elapsedTime;
            currSequence = (currSequence + 1) % trajectory.Length;
        }

        string[] entries;
        entries = trajectory[currSequence].Split(" ");
        Vector3 p1 = new(float.Parse(entries[1]), float.Parse(entries[2]), float.Parse(entries[3]));
        int nextSequence = (currSequence + 1) % trajectory.Length;
        entries = trajectory[nextSequence].Split(" ");
        Vector3 p2 = new(float.Parse(entries[1]), float.Parse(entries[2]), float.Parse(entries[3]));
        Vector3 p = Vector3.Lerp(p1, p2, (elapsedTime - lastUpdate) / UPDATE_INTERVAL);
        cam_transform.position = map_transform.TransformPoint(p);
    }
}
