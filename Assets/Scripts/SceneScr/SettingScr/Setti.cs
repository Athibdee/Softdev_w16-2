using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Setti : MonoBehaviour
{
    public void OK()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void Contact()
    {
        Application.OpenURL("https://unity-devlog.blogspot.com/");
    }

}
