using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Kyota
{
    public class LavaPool : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D lavaBall = default;
        [SerializeField] private float minInterval = 0.5f;
        [SerializeField] private float maxInterval = 1f;
        [SerializeField] private Transform[] spawnPositions = default;
        [SerializeField] private int maxRandomTries = 5;
        [SerializeField] private float speed = 4f;
        [SerializeField] private float lifetime = 1f;
        private float timeLeft;
        private int previousSpawnPosIndex;


        private void Start()
        {
            timeLeft = Random.Range(minInterval, maxInterval);
        }

        private void Update()
        {
            timeLeft -= Time.deltaTime;
            if (timeLeft <= 0)
            {
                timeLeft = Random.Range(minInterval, maxInterval);
                
                int tries = 0;
                int spawnPosIndex;
                do
                {
                    tries++;
                    spawnPosIndex = Random.Range(0, spawnPositions.Length);
                } while (spawnPosIndex == previousSpawnPosIndex && tries < maxRandomTries);
                previousSpawnPosIndex = spawnPosIndex;
                
                Rigidbody2D rb = Instantiate(lavaBall, spawnPositions[spawnPosIndex].position, Quaternion.identity);
                
                rb.AddForce(Vector2.up * speed, ForceMode2D.Impulse);
                rb.transform.parent = transform.root;
                Destroy(rb.gameObject, lifetime);
            }
        }
    }
}
