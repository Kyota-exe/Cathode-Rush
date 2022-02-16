using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kyota
{
    public class PassthroughPlatform : MonoBehaviour
    {
        [SerializeField] private PlatformEffector2D effector = default;


        private void Update()
        {
            if (Input.GetAxisRaw("Vertical") == -1f)
            {
                effector.rotationalOffset = 180f;
            }
            else
            {
                effector.rotationalOffset = 0f;
            }
        }
    }
}
