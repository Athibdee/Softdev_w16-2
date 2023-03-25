using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void Findgame ()
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2);
        SceneManager.LoadScene("Join game");
    }

    public void Hostgame()
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2);
        SceneManager.LoadScene("Host game");
    }

    public void Backgame()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void Settings()
    {
        SceneManager.LoadScene("Settings") ;
    }

    public void Creditgame()
    {
        SceneManager.LoadScene("IntroScene");
    }
}
