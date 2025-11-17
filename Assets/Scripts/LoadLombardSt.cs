using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class LoadLombardSt : MonoBehaviour
{
    public void OnButtonClick()
    {
        Debug.Log("Loading Lombard St!");
        SceneManager.LoadScene("Lombard St");
    }
}
