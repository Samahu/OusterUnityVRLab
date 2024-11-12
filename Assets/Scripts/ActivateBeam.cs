using UnityEngine;
using UnityEngine.XR;


public class ActivateBeam : MonoBehaviour
{
    public GameObject beamCollider;

    public AudioSource audioData;

    private XRNode controllerNode = XRNode.RightHand;
    private InputDevice controller;
    private bool triggerPressed = false;

    void Start()
    {
        controller = InputDevices.GetDeviceAtXRNode(controllerNode);
    }

    private void activateCollider() {
        beamCollider.SetActive(true);
        beamCollider.transform.position = transform.position;
        beamCollider.transform.rotation = transform.rotation;
        beamCollider.transform.SetParent(transform);
        audioData.Play(0);
    }

    private void deactivateCollider() {
        beamCollider.SetActive(false);
        beamCollider.transform.SetParent(null);
        beamCollider.transform.position = new Vector3(0.0f, 1000.0f, 0.0f);
        audioData.Stop();
    }

    void Update()
    {
        if (controller.TryGetFeatureValue(CommonUsages.triggerButton, out bool triggerValue))
        {
            if (triggerValue && !triggerPressed)
            {
                activateCollider();
                triggerPressed = true;
            }
            else if (!triggerValue && triggerPressed)
            {
                deactivateCollider();
                triggerPressed = false;
            }
        }
    }
}
