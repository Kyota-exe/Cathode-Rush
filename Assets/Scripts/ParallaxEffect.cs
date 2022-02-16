using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kyota
{
    public class ParallaxEffect : MonoBehaviour
    {
        [SerializeField] private Vector2 multiplier = new Vector2(0.5f, 0.9f);
        
        private Transform mainCamera;
        private Vector2 lastCameraPos;
        private float textureUnitSizeX;
        private float textureUnitSizeY;


        private void Start()
        {
            mainCamera = Camera.main.transform;
            lastCameraPos = mainCamera.position;
            transform.position = lastCameraPos;
            
            Sprite sprite = GetComponent<SpriteRenderer>().sprite;
            textureUnitSizeX = sprite.texture.width / sprite.pixelsPerUnit;
            textureUnitSizeY = sprite.texture.height / sprite.pixelsPerUnit;
        }

        private void LateUpdate()
        {
            Vector2 delta = (Vector2) mainCamera.position - lastCameraPos;
            transform.position += (Vector3) (delta * multiplier);
            lastCameraPos = mainCamera.position;

            if (Mathf.Abs(mainCamera.position.x - transform.position.x) >= textureUnitSizeX)
            {
                float offsetX = (mainCamera.position.x - transform.position.x) % textureUnitSizeX;
                transform.position = new Vector3(mainCamera.position.x + offsetX, transform.position.y);
            }

            if (Mathf.Abs(mainCamera.transform.position.y - transform.position.y) >= textureUnitSizeY)
            {
                float offsetY = (mainCamera.position.y - transform.position.y) % textureUnitSizeY;
                transform.position = new Vector3(mainCamera.position.y + offsetY, transform.position.y);
            }
        }
    }
}
