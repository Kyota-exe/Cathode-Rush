using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Kyota
{
    public class SceneLoader : MonoBehaviour
    {
        private static string previousScene;
        

        private void Awake()
        {
            SpeedrunTimer.timerOn = true;
            if (previousScene != SceneManager.GetActiveScene().name)
            {
                Checkpoint.claimedCheckpoints.Clear();
                UpsideDownManager.normalSided = true;
            }
        }

        public void StartGame()
        {
            LoadScene("Level 1");
            SpeedrunTimer.currentTime = 0f;
        }

        public static void ReloadCurrentScene()
        {
            LoadScene(SceneManager.GetActiveScene().name);
        }

        public static void LoadNextScene()
        {
            LoadScene(SceneManager.GetSceneByBuildIndex
                (SceneManager.GetActiveScene().buildIndex + 1).name);
        }

        public static void LoadScene(string sceneName)
        {
            previousScene = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(sceneName);
        }

        private void Start()
        { 
            if (SceneManager.GetActiveScene().name == "Level 1")
            {
                SpeedrunTimer.timerOn = true;
            }
        }
    }
}
