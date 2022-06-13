using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Encounter : MonoBehaviour
{
    public List<NPCBrain> potentialEncounterList;
    public Queue<NPCBrain> encounterQueue;
    private NPCBrain[] queueArray;
    public NPCBrain currentEncounter;

    public int queueLength;

    //public SpriteRenderer npcSpriteRenderer;
    public UIAnimator uiAnimator;

    public TextMeshProUGUI npcUIText, npcNameText;

    public string playerResponse;

    public bool encountering, playerHasResponded;

    public float endEncounterTimer;
    private float endEncounterTimerReset;

    public float reputation;

    public Slider happinessMeter, responseTimerBar;

    void Start()
    {
        
        queueArray = new NPCBrain[queueLength];
        encounterQueue = new Queue<NPCBrain>();

        FillEncounterQueue();

        ResetResponse();
        //currentEncounter = encounterQueue.Peek();

        endEncounterTimerReset = endEncounterTimer;
        responseTimerBar.maxValue = endEncounterTimerReset;
    }

    void Update()
    {
        if (!encountering)
        {
            responseTimerBar.value = endEncounterTimer;

            EncounterNPC();
        }
        
        if (encountering && !playerHasResponded)
        {
            AwaitPlayerResponse();
        }
        else
        {
            responseTimerBar.value = endEncounterTimer;
            endEncounterTimer -= Time.deltaTime;

            if (endEncounterTimer <= 0f)
            {
                EndEncounter();
            }
        }

        happinessMeter.value = Mathf.Clamp(reputation, 0f, 100f);
    }

    public void FillEncounterQueue()
    {
        for (int i = 0; i < queueArray.Length; i++)
        {
            int r = Random.Range(0, potentialEncounterList.Count);

            queueArray[i] = potentialEncounterList[r];

            queueArray[i].ResetNPCChoices();

            encounterQueue.Enqueue(queueArray[i]);
        }

        foreach(NPCBrain npc in queueArray)

        Debug.Log(encounterQueue.Peek());


        EncounterNPC();
        //encountering = true;


    }

    public void EncounterNPC()
    {
        currentEncounter = encounterQueue.Peek();

        //npcSpriteRenderer.sprite = currentEncounter.npcSprite;
        uiAnimator.uiAnimation = currentEncounter.spriteAnimation;
        npcNameText.text = currentEncounter.npcName;


        if (!currentEncounter.isConsequence)
        {
            currentEncounter.ChooseDialogue();
            npcUIText.text = currentEncounter.chosenDialogue.dialogue;

        }
        else
        {
            currentEncounter.chosenDialogue = currentEncounter.consequenceDialogue;
            npcUIText.text = currentEncounter.chosenDialogue.dialogue;
        }

        encountering = true;
        
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

    public void YesResponse()
    {
        if (!playerHasResponded)
        {
            playerResponse = "yes";
            reputation += currentEncounter.chosenDialogue.yesReputationAffect;
        }
    }

    public void NoResponse()
    {
        if (!playerHasResponded)
        { 
            playerResponse = "no";
            reputation += currentEncounter.chosenDialogue.noReputationAffect;
        }
    }

    public void IDKResponse()
    {
        if (!playerHasResponded)
        {
            playerResponse = "idk";
            reputation += currentEncounter.chosenDialogue.idkReputationAffect;
        }
    }

    public void ResetResponse()
    {
        if (playerHasResponded)
        {
            playerResponse = null;
            playerHasResponded = false;
        }
    }

    public void EndEncounter()
    {
        if (!currentEncounter.chosenDialogue.isConsequence)
        {
            NPCBrain consequenceEncounter = currentEncounter;
            consequenceEncounter.isConsequence = true;
            consequenceEncounter.chosenDialogue = currentEncounter.consequenceDialogue;
            //consequenceEncounter.playerResponseString = playerResponse;

            encounterQueue.Enqueue(consequenceEncounter);

            ResetResponse();

            encounterQueue.Dequeue();
        }
        else
        {
            currentEncounter.chosenDialogue.isCompleted = true;

            ResetResponse();

            encounterQueue.Dequeue();
            //FillEncounterQueue();
            currentEncounter.ConsequenceDelivered();

        }
        endEncounterTimer = endEncounterTimerReset;

        encountering = false;
        

        //NPC Leaves
    }
}
