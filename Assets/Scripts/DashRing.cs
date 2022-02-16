using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kyota
{
    public class DashRing : MonoBehaviour
    {
        [SerializeField] private bool inverted = false;
        
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out Player player))
            {
                bool rightSide = player.transform.position.x >= transform.position.x;
                player.SetDashing(true, inverted, rightSide);
            }
        }
    }
}
