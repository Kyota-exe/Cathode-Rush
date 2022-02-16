using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Kyota
{
    public class UpsideDownManager : MonoBehaviour
    {
        [SerializeField] private GameObject normalSide = default;
        [SerializeField] private GameObject upsideDownSide = default;
        [SerializeField] private Volume volume = default;
        public static UpsideDownManager instance;

        public static bool normalSided = true;
        

        private void Awake()
        {
            instance = this;
        }

        public void ToggleSide()
        {
            SwitchSide(!normalSided);
        }

        public void SwitchSide(bool normal)
        {
            normalSide.SetActive(normal);
            upsideDownSide.SetActive(!normal);
            volume.enabled = !normal;
            normalSided = normal;
        }
    }
}
