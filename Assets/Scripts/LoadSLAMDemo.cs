using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class LoadSLAMScene : MonoBehaviour
{
    public void OnButtonClick()
    {
        Debug.Log("Loading SLAM demo!");
        SceneManager.LoadScene("SLAM Demo");
    }
}
