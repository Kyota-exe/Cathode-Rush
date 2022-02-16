using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kyota
{
    public class GravityPlatform : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D rb = default;
        [SerializeField] private float gravity = -10f;
        [SerializeField] private GravityPlatform[] fallList = default;
        public bool falling = false;


        private void FixedUpdate()
        {
            if (falling) rb.velocity = new Vector2(0, gravity);
        }

        public void FallEveryPlatform()
        {
            foreach (GravityPlatform platform in fallList)
            {
                platform.falling = true;
            }
        }
    }
}
