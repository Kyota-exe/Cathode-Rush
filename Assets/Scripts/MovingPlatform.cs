using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Kyota
{
    public class MovingPlatform : MonoBehaviour
    {
        [SerializeField] private float speed = 3f;
        [SerializeField] private Transform[] waypointTransforms = default;

        private Vector2[] waypoints;
        private int targetIndex = 1;
        private bool incrementIndex = true;
        
        private bool playerAttached = false;
        private Vector2 previousPos;
 

        private void Start()
        {
            waypoints = (from waypointTransform in waypointTransforms select (Vector2) waypointTransform.position).ToArray();
            transform.position = waypoints[0];
        }

        private void FixedUpdate()
        {
            previousPos = transform.position;
            
            transform.position = Vector2.MoveTowards(transform.position,
                waypoints[targetIndex], speed * Time.deltaTime);

            if ((Vector2) transform.position ==  waypoints[targetIndex])
            {
                if (targetIndex + 1 == waypoints.Length) incrementIndex = false;
                else if (targetIndex == 0) incrementIndex = true;
                targetIndex = incrementIndex ? targetIndex + 1 : targetIndex - 1;
            }
            
            if (playerAttached)
            {
                Player.instance.rb.velocity += (Vector2) transform.position - previousPos;
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.transform.CompareTag("Player"))
            {
                playerAttached = true;
            }
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            if (other.transform.CompareTag("Player"))
            {
                playerAttached = false;
            }
        }
    }
}
