using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Dialogue")]
public class DialogueSystem : ScriptableObject
{
    [TextArea(3, 10)] // for inspector
    public string dialogue;
    [TextArea(3, 10)] // for inspector
    public string yesResponse, noResponse, idkResponse;
    public DialogueSystem yesConsequence, noConsequence, idkConsequence;

    //public float yesReputationAffect, noReputationAffect, idkReputationAffect;

    public List<DecisionEffect> yesEffectsList, noEffectsList, idkEffectsList;

    public bool hasEncountered, hasConsequence, isCompleted;
   
}
