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

        private void Awake()
        {
            Player player = FindObjectOfType<Player>();
            if (player != null)
            {
                player.OnChangedMoney += () =>
                {
                    moneyText.text = $"Money: {player.Money}";
                };

                player.OnChangedHealth += () =>
                {
                    healthText.text = $"Health: {player.Health}";
                };
            }
        }
    }
}
