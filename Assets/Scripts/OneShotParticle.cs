using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kyota
{
    public class OneShotParticle : MonoBehaviour
    {
        private ParticleSystem ps;
        
        
        private void Start()
        {
            ps = GetComponent<ParticleSystem>();
        }

        private void Update()
        {
            if (!ps.IsAlive())
            {
                Destroy(gameObject);
            }
        }
    }
}
