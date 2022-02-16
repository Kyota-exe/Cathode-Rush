using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kyota
{
    public class PowerJumpReset : MonoBehaviour
    { 
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.TryGetComponent(out Player player))
            {
                player.powerJumpsLeft = 0;
            }
        }
    }
}
