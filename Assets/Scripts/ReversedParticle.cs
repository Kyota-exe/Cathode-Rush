using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kyota
{
    public class ReversedParticle : MonoBehaviour
    {
        [SerializeField] private float startTime = 1f;
        [SerializeField] private float lifeTime = 1f;

        private ParticleSystem ps;
        private ParticleSystem.MainModule mainPS;
        private float simulationTime;
        private float lifetimeLeft;
        

        private void Start()
        {
            ps = GetComponent<ParticleSystem>();
            mainPS = ps.main;
            ps.Simulate(startTime, false, false, true);
            
            lifetimeLeft = lifeTime;
        }

        private void Update()
        {
            ps.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);
            simulationTime += mainPS.simulationSpeed * -Time.deltaTime;
            ps.Simulate(startTime + simulationTime);
            if (simulationTime > mainPS.startLifetime.constant) Destroy(gameObject);

            lifetimeLeft -= Time.deltaTime;
            if (lifetimeLeft <= 0) Destroy(gameObject);
        }
    }
}
