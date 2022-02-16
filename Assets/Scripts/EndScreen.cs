using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Kyota
{
    public class EndScreen : MonoBehaviour
    {
        private void Start()
        {
            SpeedrunTimer.timerOn = false;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                Destroy(MusicPlayer.instance.gameObject);
                SceneManager.LoadScene(0);
            }
        }
    }
}
