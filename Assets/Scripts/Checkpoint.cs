using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kyota
{
    public class Checkpoint : MonoBehaviour
    {
        [SerializeField] public Transform newSpawnPos = default;
        [SerializeField] public bool upsideDown = false;
        [SerializeField] private GameObject[] destroyList = default;
        
        public static List<string> claimedCheckpoints = new List<string>();


        private void Start()
        {
            if (claimedCheckpoints.Contains(name))
            {
                foreach (GameObject target in destroyList) Destroy(target);
            }
        }


        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                if (!claimedCheckpoints.Contains(name))
                {
                    claimedCheckpoints.Add(name);
                }
            }
        }
    }
}
