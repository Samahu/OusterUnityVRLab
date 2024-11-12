using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;


public class LoadMainScene : MonoBehaviour
{
    private XRNode controllerNode = XRNode.RightHand;
    private InputDevice controller;
    private bool triggerPressed = false;

    void Start()
    {
        controller = InputDevices.GetDeviceAtXRNode(controllerNode);
    }

    public void OnButtonClick()
    {
        Debug.Log("Loading Main Scene!");
        SceneManager.LoadScene("MainScene");
    }

    void Update()
    {
        if (controller.TryGetFeatureValue(CommonUsages.menuButton, out bool triggerValue))
        {
            if (triggerValue && !triggerPressed)
            {
                OnButtonClick();
                triggerPressed = true;
            }
        }
    }
}
