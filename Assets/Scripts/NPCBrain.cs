using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "NPC")]
public class NPCBrain : ScriptableObject
{
    public string npcName;
    
    //public Sprite npcSprite;

    public SpriteAnimation spriteAnimation;

    //public Encounter encounter;

    public List<DialogueSystem> dialogueOptions;

    public DialogueSystem chosenDialogue, consequenceDialogue;

    public string playerResponseString;

    public bool isConsequence;

    public float playerReputation;

    public List<EncounterHistory> encounterHistory;

    public string loseScenarioText;


    public void ChooseDialogue()
    {
        chosenDialogue = dialogueOptions[Random.Range(0, dialogueOptions.Count)];
    }

    public void ResetNPCChoices()
    {
        chosenDialogue = null;
        consequenceDialogue = null;
        playerResponseString = null;
        isConsequence = false;
    }

    public void ConsequenceDelivered()
    {
        isConsequence = false;
    }
}
