using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kyota
{
    public class MainMenuLogic : MonoBehaviour
    {
        [SerializeField] private AudioClip highlightSound = default;
        [SerializeField] private AudioClip checkboxSound = default;
        [SerializeField] private Canvas mainMenuCanvas = default;
        [SerializeField] private Canvas creditsCanvas = default;
        private AudioSource audioSource;


        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
        }

        public void PlayHighlightSound()
        {
            audioSource.PlayOneShot(highlightSound);
        }

        public void PlayCheckboxSound(bool value)
        {
            if (value) audioSource.PlayOneShot(checkboxSound);
        }

        public void OnCreditsClick(bool enableCredits)
        {
            mainMenuCanvas.enabled = !enableCredits;
            creditsCanvas.enabled = enableCredits;
        }
    }
}
