using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EncounterHistory
{
    public DialogueSystem dialogue;
    public string playerResponse;

    public List<NPCBrain> affectedNPCs = new List<NPCBrain>();
    public NPCBrain consequenceNPC;
}
