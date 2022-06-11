using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Dialogue")]
public class DialogueSystem : ScriptableObject
{
    public string dialogue;
    public string yesResponse, noResponse, idkResponse;
    public DialogueSystem yesConsequence, noConsequence, idkConsequence;

    //public bool hasEncountered, isConsequence, isCompleted;
   
}
