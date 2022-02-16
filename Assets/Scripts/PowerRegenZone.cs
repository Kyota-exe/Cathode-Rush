using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kyota
{
    public class PowerRegenZone : MonoBehaviour
    {
        [SerializeField] private Powerup[] powerups = default;
        
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                foreach (Powerup regeneratingPowerup in powerups)
                {
                    regeneratingPowerup.allowRegeneration = true;
                    if (!regeneratingPowerup.PowerupAlive)
                    {
                        regeneratingPowerup.StartCoroutine(regeneratingPowerup.RegenerateOrb(0));
                    }
                }
            }
        }


        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                foreach (Powerup regeneratingPowerup in powerups)
                {
                    regeneratingPowerup.allowRegeneration = false;
                    if (regeneratingPowerup.PowerupAlive) regeneratingPowerup.DestroyPowerup(false);
                    if (regeneratingPowerup.regenerationCoroutine != null)
                    {
                        regeneratingPowerup.StopCoroutine(regeneratingPowerup.regenerationCoroutine);
                    }
                }
            }
        }
    }
}
