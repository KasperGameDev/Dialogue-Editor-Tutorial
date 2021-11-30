using Dialogue;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Dialogue.Scripts;
using System.Collections.Generic;

public class NPC : Character
{
    [SerializeField] DialogueTalk dialogueTalk;
    [SerializeField] public bool spokeToPlayer = false;
    // Use this for initialization
    void Start()
    {
        dialogueTalk = GetComponent<DialogueTalk>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("Player"))
        {
            dialogueTalk.StartDialogue();
        }
    }
}
