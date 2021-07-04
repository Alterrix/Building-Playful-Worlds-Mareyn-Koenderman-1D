using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
   private GameManager gameManager;
    private void Start()
    {
        gameManager = GameManager.Instance;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 1f;
    }
    public void Playgame()
    {
        gameManager.LoadGameSettings(true);
        gameManager.SetGameSettings();
        SceneManager.LoadScene(1);
    }

    public void LoadGame()
    {
        gameManager.LoadGameSettings(false);
        gameManager.SetGameSettings();
        SceneManager.LoadScene(1);
    }

    public void Quitgame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
    }
}