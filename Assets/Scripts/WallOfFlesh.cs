using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace Kyota
{
    public class WallOfFlesh : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float maxDistance = 10f;
        [SerializeField] private Vector3 direction = Vector3.right;
        [SerializeField] private float playerAdjustSpeed = 2f;

        private Transform player;


        private void Start()
        {
            player = Player.instance.transform;
        }

        private void Update()
        {
            transform.position += direction * (moveSpeed * Time.deltaTime);
            if (direction == Vector3.right)
            {
                if (Mathf.Abs(player.position.x - transform.position.x) > maxDistance)
                {
                    transform.position = new Vector2(player.position.x - maxDistance, transform.position.y);
                }

                float playerY = player.position.y;
                float wallY = transform.position.y;
                float absoluteDifference = Mathf.Abs(playerY - wallY);
                transform.position += (Vector3) (playerY > wallY ? Vector2.up : Vector2.down) * (playerAdjustSpeed * absoluteDifference * Time.deltaTime);
            }
            else
            {
                if (Mathf.Abs(player.position.y - transform.position.y) > maxDistance)
                {
                    if (direction == Vector3.up) transform.position = new Vector2(transform.position.x, player.position.y - maxDistance);
                    else if (direction == Vector3.down) transform.position = new Vector2(transform.position.x, player.position.y + maxDistance);
                }
                
                float playerX = player.position.x;
                float wallX = transform.position.x;
                float absoluteDifference = Mathf.Abs(playerX - wallX);
                transform.position += (Vector3) (playerX > wallX ? Vector2.right : Vector2.left) * (playerAdjustSpeed * absoluteDifference * Time.deltaTime);
            }
        }
    }
}
