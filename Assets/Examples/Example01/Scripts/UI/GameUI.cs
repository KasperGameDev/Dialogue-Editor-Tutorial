using KasperDev.ModularComponents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KasperDev.Dialogue.Example.Ex01
{
    public class GameUI : MonoBehaviour
    {
        [SerializeField] private Text moneyText;
        [SerializeField] private Text healthText;

        [SerializeField] private IntReference moneyReference;
        [SerializeField] private FloatReference healthReference;


        private void Update()
        {
            moneyText.text = $"Money: {moneyReference.Value}";
            healthText.text = $"Health: {healthReference.Value}";
        }
    }
}
