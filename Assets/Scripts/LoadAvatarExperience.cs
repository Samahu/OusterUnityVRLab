using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class LoadAvatarExperience : MonoBehaviour
{
    public void OnButtonClick()
    {
        Debug.Log("Loading avatar experience scene!");
        SceneManager.LoadScene("AvatarExperience");
    }
}
