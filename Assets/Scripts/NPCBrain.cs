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

    public DialogueSystem chosenDialogue, consequenceDialogue, loseDialogue;

    public string playerResponseString;

    public bool isConsequence, isLoseEncounter;

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
        playerReputation = 50f;
        isLoseEncounter = false;
        encounterHistory.Clear();
    }

    public void ConsequenceDelivered()
    {
        isConsequence = false;
    }
}
