using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kyota
{
    public class MusicPlayer : MonoBehaviour
    {
        [SerializeField] private AudioClip introClip = default;
        public static MusicPlayer instance;
        private AudioSource audioSource;


        private void Start()
        { 
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
                audioSource = GetComponent<AudioSource>();
                audioSource.PlayOneShot(introClip);
                audioSource.PlayDelayed(introClip.length);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
