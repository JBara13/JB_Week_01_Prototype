using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBrain : MonoBehaviour
{
    public Encounter encounter;

    public List<DialogueSystem> dialogueOptions;

    public DialogueSystem chosenDialogue, consequenceDialogue;

    public string playerResponseString;

    public bool isConsequence, encounterComplete;
    void Awake()
    {
        chosenDialogue = dialogueOptions[Random.Range(0, dialogueOptions.Count)];
    }

}
