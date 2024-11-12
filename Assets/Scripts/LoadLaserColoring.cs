using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class LoadLaserColoring : MonoBehaviour
{
    public void OnButtonClick()
    {
        Debug.Log("Loading laser coloring scene!");
        SceneManager.LoadScene("LaserColoring");
    }
}
