using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // create one instance of the gamemanager at all times
    #region singleton 
    private static GameManager instance = null;
    public static GameManager Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            instance = this;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        DontDestroyOnLoad(gameObject);
    }
    #endregion 

    [HideInInspector] public IniFile gameSettings { get; private set; }// get private settings but cant overwrite unless in this class

    [HideInInspector] public int seed = -1;

    #region const strings
    public const string Sseed = "seed";
    #endregion

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

    }
   
    private void Start()
    {
        gameSettings = new IniFile("random dungeon generator"); //create a file with the given name
    }

    public void LoadGameSettings(bool newGame)
    {
        //put the saved value of the seed in the variable
        seed = Convert.ToInt32(gameSettings.Read(Sseed, "-1"));
        //if default value or trying to start new game 
        if (seed == -1 || newGame)
        {
            //create random max 200
            seed = UnityEngine.Random.Range(0, 200);
        }
    }

    public void SetGameSettings()
    {
        //every time a function of random gets called it uses the same seed
        UnityEngine.Random.InitState(seed);
    }

    public void SaveGameSettings()
    {
        //write the current seed to the file
        gameSettings.Write(Sseed, seed.ToString());
    }
}
