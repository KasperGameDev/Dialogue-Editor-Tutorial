using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueEditor.Dialogue.Scripts;
public class StartDialogueTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<DialogueTalk>().StartDialogue();
    }
}
