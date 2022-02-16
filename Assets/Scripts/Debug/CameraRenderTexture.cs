using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Kyota
{
    public class CameraRenderTexture : MonoBehaviour
    {
        [SerializeField] private Vector2 size = new Vector2(1280, 720);
        [SerializeField] private RawImage rawImage = default;
        private RenderTexture renderTexture;
        
        
        private void Start()
        {
            renderTexture = new RenderTexture((int) size.x, (int) size.y, 0);
            Camera inputCamera = GetComponent<Camera>();
            inputCamera.targetTexture = renderTexture;
            inputCamera.aspect = size.x / size.y;
            rawImage.texture = renderTexture;
        }
    }
}
