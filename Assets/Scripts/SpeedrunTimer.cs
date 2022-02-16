using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

namespace Kyota
{
    public class SpeedrunTimer : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI timerText = default;
        [SerializeField] private TextMeshProUGUI milliTimerText = default;

        private Canvas canvas;
        public static float currentTime = 0f;
        public static bool timerOn = false;
        private static bool canvasEnabled = false;


        private void Start()
        {
            canvas = GetComponent<Canvas>();
            canvas.enabled = canvasEnabled;
            /*if (SceneManager.GetActiveScene().buildIndex + 1 >= SceneManager.sceneCountInBuildSettings)
            {
                canvas.enabled = true;
            }
            else */if (SceneManager.GetActiveScene().buildIndex == 0)
            {
                EnableCanvas(false);
            }
        }

        public void EnableCanvas(bool enable)
        {
            canvas.enabled = enable;
            canvasEnabled = enable;
        }
        
        private void Update()
        {
            if (timerOn)
            {
                currentTime += Time.unscaledDeltaTime * 1000;
            }

            try
            {
                string newText = TimeSpan.FromMilliseconds(currentTime).ToString();
                newText = newText.Remove(newText.Length - 4).Remove(0, 3);
                timerText.text = newText.Remove(newText.Length - 4);
                milliTimerText.text = newText.Substring(newText.Length - 4);
            }
            catch
            {
                // ignored
            }
        }
    }
}
