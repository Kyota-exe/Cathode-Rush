using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kyota
{
    public class Void : MonoBehaviour
    {
        [SerializeField] private bool xFollow = false;
        [SerializeField] private bool teleport = false;

        private Transform player;
        private Vector2 startPosition;
        

        private void Start()
        {
            player = Player.instance.transform;
            startPosition = player.position;
        }

        private void Update()
        {
            if (!xFollow) return;
            transform.position = new Vector2(player.position.x, transform.position.y);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out Player playerComponent))
            {
                if (!teleport) playerComponent.Die();
                else player.position = startPosition;
            }
            else if (other.TryGetComponent(out Projectile projectile))
            {
                projectile.DestroyProjectile();
            }
        }
    }
}
