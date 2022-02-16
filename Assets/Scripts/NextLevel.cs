using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kyota
{
    public class NextLevel : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            SceneLoader.LoadNextScene();
        }
    }
}
