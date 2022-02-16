using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kyota
{
    public class Powerup : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private int powerJumpsCount = 1;
        [SerializeField] private bool regenerate = false;
        [SerializeField] private float regenerateTime = 2f;
        [SerializeField] private float particleTime = 0.4f;
        [SerializeField] private Addon addon = Addon.None;

        [Header("References")]
        [SerializeField] private GameObject explosionParticle = default;
        [SerializeField] private ParticleSystem[] reversedExplosionParticles = default;

        [Header("Audio")] 
        [SerializeField] private AudioClip powerupPopSound = default;
        [SerializeField] private float powerupPopVolume = 1f;

        private Player player;
        private GameObject colliderChild;
        public Coroutine regenerationCoroutine;
        public bool allowRegeneration = false;
        public bool PowerupAlive => colliderChild.activeSelf;

        public enum Addon
        {
            None, UpsideDown, Gravity, Teleport
        }


        private void Start()
        {
            player = Player.instance;
            colliderChild = transform.GetChild(0).gameObject;
        }

        public void DestroyPowerup(bool shot)
        {
            if (shot)
            {
                player.powerJumpsLeft = Math.Min(player.powerJumpsLeft + powerJumpsCount, player.powerJumpCap);
                player.PlayPowerMeterSound(Math.Min(player.powerJumpsLeft - 1, player.powerJumpCap - 1));
                
                switch (addon)
                {
                    case Addon.UpsideDown:
                        UpsideDownManager.instance.ToggleSide();
                        break;
                    case Addon.Gravity:
                        player.gravityReversed = !player.gravityReversed;
                        break;
                    case Addon.Teleport:
                        player.OnTeleportPowerupShot(transform.position);
                        break;
                }
            }

            AudioSource.PlayClipAtPoint(powerupPopSound, transform.position, powerupPopVolume);
            
            Instantiate(explosionParticle, transform.position, Quaternion.identity);

            if (regenerate)
            {
                colliderChild.SetActive(false);
                if (allowRegeneration)
                {
                    regenerationCoroutine = StartCoroutine(RegenerateOrb(regenerateTime));
                }
            }
            else Destroy(gameObject);
        }

        public IEnumerator RegenerateOrb(float waitTime)
        {
            if (waitTime > 0) yield return new WaitForSeconds(waitTime);
            foreach (ParticleSystem ps in reversedExplosionParticles)
            {
                if (allowRegeneration) Instantiate(ps, transform.position, Quaternion.identity);
                else yield break;
            }
            yield return new WaitForSeconds(particleTime);
            if (allowRegeneration) colliderChild.SetActive(true);
        }
    }
}
