using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace KasperDev.Dialogue.Example.Ex01
{
    public class Player : BasePlayer
    {
        [SerializeField] private int money = 10;
        [SerializeField] private int health = 70;
        [SerializeField] private bool didWeTalk = false;

        public int Money { get => money; }
        public int Health { get => health; }
        public bool DidWeTalk { get => didWeTalk; set => didWeTalk = value; }

        public UnityAction OnChangedMoney;
        public UnityAction OnChangedHealth;

        protected new void Awake()
        {
            base.Awake();  
        }

        private void Start()
        {
            OnChangedMoney?.Invoke();
            OnChangedHealth?.Invoke();
        }

        protected new void Update()
        {
            base.Update();   
        }

        protected new void FixedUpdate()
        {
            base.FixedUpdate();
        }

        public void ModifyMoeny(int value)
        {
            money += value;
            OnChangedMoney?.Invoke();
        }

        public void ModifyHealth(int value)
        {
            health += value;
            OnChangedHealth?.Invoke();
        }
    }
}