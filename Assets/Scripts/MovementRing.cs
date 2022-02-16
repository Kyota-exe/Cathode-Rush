using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kyota
{
    public class MovementRing : MonoBehaviour
    {
        [Header("Settings")] 
        [SerializeField] private Type type = Type.MoveSpeedBoost;
        [SerializeField] private float cooldown = 1f;

        [Header("Move Speed Boost")]
        [SerializeField] private float speedBoost = 8f;
        [SerializeField] private bool additive = false;

        [Header("Jump Pad/Ring")] 
        [SerializeField] private Vector2 jumpForce = new Vector2(0, 10f);
        [SerializeField] private bool cancelWithSlide = false;

        private float cooldownLeft;
        private Coroutine ringJumpCoroutine;
        
        private enum Type
        {
            MoveSpeedBoost, JumpRing
        }


        private void OnTriggerEnter2D(Collider2D other)
        {
            if (cooldownLeft <= 0 && other.TryGetComponent(out Player player))
            {
                RingMagic(player);
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (cooldownLeft <= 0 && other.transform.TryGetComponent(out Player player))
            {
                RingMagic(player);
            }
        }

        private void RingMagic(Player player)
        {
            switch (type)
            {
                case Type.MoveSpeedBoost:
                    float newSpeed = additive ? player.moveSpeed + speedBoost : speedBoost;
                    player.moveSpeed = newSpeed;
                    cooldownLeft = cooldown;
                    break;
                case Type.JumpRing:
                    if (cancelWithSlide && player.Sliding) break;
                    if (ringJumpCoroutine != null) StopCoroutine(ringJumpCoroutine);
                    ringJumpCoroutine = StartCoroutine(player.RingJumpingTimer(jumpForce));
                    break;
            }
        }

        private void Update()
        {
            cooldownLeft -= Time.deltaTime;
        }
    }
}
