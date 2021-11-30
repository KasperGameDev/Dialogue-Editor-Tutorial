using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dialogue.Scripts
{
    public class LanguageController : MonoBehaviour
    {
        [SerializeField] private LanguageType language;

        public static LanguageController Instance { get; private set; }
        public LanguageType Language { get => language; set => language = value; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}