using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DecisionEffect
{
    public NPCBrain npc;
    public float reputationEffect;

    public List<DialogueSystem> consequenceDialogueList;

}
