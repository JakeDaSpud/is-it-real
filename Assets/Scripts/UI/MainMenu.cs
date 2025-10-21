using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    void Start()
    {
        AudioManager.Instance.PlayMusic(AudioManager.Music.MENU);
    }

    public void PlayGame()
    {
        SceneManager.LoadSceneAsync("PlayerMovementTest");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
