using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kyota
{
    public class Snapper : MonoBehaviour
    {
        [SerializeField] private Animator animator = default;
        [SerializeField] private bool harmful = false;
        
        
        // Initial Trigger
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                animator.SetTrigger("close");
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.transform.TryGetComponent(out Player player) && harmful)
            {
                player.Die();
            }
        }
    }
}
