using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kyota
{
    public class Projectile : MonoBehaviour
    {
        private SpriteRenderer spriteRenderer;

        
        private void Start()
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        private void Update()
        {
            if (!spriteRenderer.isVisible) DestroyProjectile();
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.transform.parent.TryGetComponent(out Powerup powerup))
            {
                powerup.DestroyPowerup(true);
            }
            else if (other.transform.TryGetComponent(out GravityPlatform gravityPlatform))
            {
                gravityPlatform.FallEveryPlatform();
            }
            DestroyProjectile();
        }

        public void DestroyProjectile()
        {
            Destroy(gameObject);
        }
    }
}
