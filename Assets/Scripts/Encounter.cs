using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Encounter : MonoBehaviour
{
    public List<NPCBrain> potentialEncounterList;
    public List<NPCBrain> encounterQueue;

    public NPCBrain currentEncounter;

    public int queueLength;

    public TextMeshProUGUI npcUIText;

    public string playerResponse;

    public bool encountering, playerHasResponded;

    public float endEncounterTimer, endEncounterTimerReset;

    void Start()
    {
        FillEncounterQueue();
    }

    void Update()
    {
        if (!encountering)
        {
            EncounterNPC();
        }
        
        if (encountering && !playerHasResponded)
        {
            AwaitPlayerResponse();
        }
        else
        {
            endEncounterTimer -= Time.deltaTime;

            EndEncounter();
        }
    }

    public void FillEncounterQueue()
    {
        if (encounterQueue.Count < queueLength)
        {
            for (int i = 0; i < queueLength; i++)
            {
                int rand = Random.Range(0, potentialEncounterList.Count);
                if (!potentialEncounterList[rand].encounterComplete && !potentialEncounterList[rand].isConsequence)
                {
                    encounterQueue.Add(potentialEncounterList[rand]);
                }
            }
        }
    }

    public void EncounterNPC()
    {
        currentEncounter = encounterQueue[0];

        // instantiate npc prefab?

        if (!currentEncounter.isConsequence)
        {
            npcUIText.text = currentEncounter.chosenDialogue.dialogue;
        }
        else
        {
            npcUIText.text = currentEncounter.consequenceDialogue.dialogue;
        }
    }

    public void AwaitPlayerResponse()
    {
        if (playerResponse != null)
        {
            if (playerResponse == "yes")
            {
                currentEncounter.consequenceDialogue = currentEncounter.chosenDialogue.yesConsequence;
                npcUIText.text = currentEncounter.chosenDialogue.yesResponse;
                playerHasResponded = true;
            }

            if (playerResponse == "no")
            {
                currentEncounter.consequenceDialogue = currentEncounter.chosenDialogue.noConsequence;
                npcUIText.text = currentEncounter.chosenDialogue.noResponse;
                playerHasResponded = true;

            }

            if (playerResponse == "idk")
            {
                currentEncounter.consequenceDialogue = currentEncounter.chosenDialogue.idkConsequence;
                npcUIText.text = currentEncounter.chosenDialogue.idkResponse;
                playerHasResponded = true;

            }
        }
    }

    public string YesResponse()
    {
        return playerResponse = "yes";
    }

    public string NoResponse()
    {
        return playerResponse = "no";
    }

    public string IDKResponse()
    {
        return playerResponse = "idk";
    }

    public string ResetResponse()
    {
        return playerResponse = null;
    }

    public void EndEncounter()
    {
        if (!currentEncounter.isConsequence)
        {
            currentEncounter.isConsequence = true;
            encounterQueue.Add(currentEncounter);

            encounterQueue.Remove(currentEncounter);
        }
        else
        {
            currentEncounter.encounterComplete = true;
        }
        endEncounterTimer = endEncounterTimerReset;

        encountering = false;

        //NPC Leaves
    }
}
