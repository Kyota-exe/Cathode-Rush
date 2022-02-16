using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Kyota
{
    public class ExtraJumpsText : MonoBehaviour
    {
        private TextMeshProUGUI textComponent;
        private Player player;

        
        private void Start()
        {
            textComponent = GetComponent<TextMeshProUGUI>();
            player = Player.instance;
        }

        private void Update()
        {
            textComponent.text = player.powerJumpsLeft.ToString();
        }
    }
}
