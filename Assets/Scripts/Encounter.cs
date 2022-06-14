using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Encounter : MonoBehaviour
{
    public List<NPCBrain> potentialEncounterList;
    public Queue<NPCBrain> encounterQueue;
    [SerializeField] private NPCBrain[] queueArray;
    public NPCBrain currentEncounter;

    public int queueLength;

    //public SpriteRenderer npcSpriteRenderer;
    public UIAnimator uiAnimator;

    public TextMeshProUGUI npcUIText, npcNameText;

    public string playerResponse;

    public bool encountering, playerHasResponded;

    public float endEncounterTimer;
    private float endEncounterTimerReset;

    private ReputationMenu reputation;

    //public float reputation;

    public Slider happinessMeter, responseTimerBar;

    int count;

    void Start()
    {
        foreach (NPCBrain npc in potentialEncounterList)
        {
            npc.playerReputation = 50f;
        }


        queueArray = new NPCBrain[queueLength];
        encounterQueue = new Queue<NPCBrain>();

        for (int i = 0; i < queueArray.Length; i++)
        {
            count = i;

            queueArray[i] = FillEncounterQueue(count, queueArray);

            queueArray[i].ResetNPCChoices();

            encounterQueue.Enqueue(queueArray[i]);

        }

        //encounterQueue.Enqueue(npc);

        ResetResponse();

        EncounterNPC();
        //currentEncounter = encounterQueue.Peek();

        endEncounterTimerReset = endEncounterTimer;
        responseTimerBar.maxValue = endEncounterTimerReset;

        reputation = GetComponent<ReputationMenu>();
        reputation.reputationParent.SetActive(false);

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

        happinessMeter.value = Mathf.Clamp(currentEncounter.playerReputation, 0f, 100f);
    }

    public NPCBrain FillEncounterQueue(int count, NPCBrain[] queueArray)
    {
        int r = Random.Range(0, potentialEncounterList.Count);

        int previousNPC = count - 1;

        //Debug.Log("previous npc count = " + previousNPC);

        if (previousNPC > -1 && queueArray[previousNPC] == potentialEncounterList[r]) 
        {
            if (potentialEncounterList[r + 1] != null)
            {
                return potentialEncounterList[r + 1];
            }
            else
            {
                return potentialEncounterList[r - 1];
            }
        }

        return potentialEncounterList[r];
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

            reputation.UpdateNPCProfiles();
        }
    }

    public void YesResponse()
    {
        if (!playerHasResponded)
        {
            playerResponse = "yes";

            foreach (DecisionEffect effect in currentEncounter.chosenDialogue.yesEffectsList)
            {
                effect.npc.playerReputation += effect.reputationEffect;
            }
        }
    }

    public void NoResponse()
    {
        if (!playerHasResponded)
        { 
            playerResponse = "no";

            foreach (DecisionEffect effect in currentEncounter.chosenDialogue.noEffectsList)
            {
                effect.npc.playerReputation += effect.reputationEffect;
            }
        }
    }

    public void IDKResponse()
    {
        if (!playerHasResponded)
        {
            playerResponse = "idk";

            foreach (DecisionEffect effect in currentEncounter.chosenDialogue.idkEffectsList)
            {
                effect.npc.playerReputation += effect.reputationEffect;
            }
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
        if (currentEncounter.chosenDialogue.hasConsequence)
        {
            EncounterHistory currentEncounterHistory = new EncounterHistory();
            currentEncounterHistory.dialogue = currentEncounter.chosenDialogue;
            currentEncounterHistory.playerResponse = playerResponse;

            NPCBrain consequenceEncounter = currentEncounter;
            consequenceEncounter.isConsequence = true;
            consequenceEncounter.chosenDialogue = currentEncounter.consequenceDialogue;

            currentEncounter.encounterHistory.Add(currentEncounterHistory);
            //consequenceEncounter.playerResponseString = playerResponse;

            encounterQueue.Enqueue(consequenceEncounter);

            ResetResponse();

            encounterQueue.Dequeue();
        }
        else
        {
            currentEncounter.chosenDialogue.isCompleted = true;

            ResetResponse();

            currentEncounter.ConsequenceDelivered();

            encounterQueue.Dequeue();
            //FillEncounterQueue();

        }
        //reputation.UpdateNPCProfiles();

        endEncounterTimer = endEncounterTimerReset;

        encountering = false;
        

        //NPC Leaves
    }
}
