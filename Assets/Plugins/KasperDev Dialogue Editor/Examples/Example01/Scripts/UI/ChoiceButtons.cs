using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KasperDev.Dialogue.Example.Ex01
{
    public class ChoiceButtons : MonoBehaviour
    {
        [Header("Buttons")]
        [SerializeField] private Button choiceButton01;
        [SerializeField] private Button choiceButton02;
        [SerializeField] private Button choiceButton03;

        [Header("KeyCode")]
        [SerializeField] private KeyCode choiceButtonKey01 = KeyCode.Alpha1;
        [SerializeField] private KeyCode choiceButtonKey02 = KeyCode.Alpha2;
        [SerializeField] private KeyCode choiceButtonKey03 = KeyCode.Alpha3;

        void Update()
        {
            if (Input.GetKeyDown(choiceButtonKey01) && choiceButton01.gameObject.activeSelf)
            {
                choiceButton01.onClick.Invoke();
            }
            if (Input.GetKeyDown(choiceButtonKey02) && choiceButton02.gameObject.activeSelf)
            {
                choiceButton02.onClick.Invoke();
            }
            if (Input.GetKeyDown(choiceButtonKey03) && choiceButton03.gameObject.activeSelf)
            {
                choiceButton03.onClick.Invoke();
            }
        }
    }
}
