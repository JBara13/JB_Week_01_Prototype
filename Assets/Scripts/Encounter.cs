using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Encounter : MonoBehaviour
{
    public List<NPCBrain> potentialEncounterList;
    public LinkedList<NPCBrain> encounterQueue;
    //public Queue<NPCBrain> encounterQueue;
    [SerializeField] private NPCBrain[] queueArray;
    public NPCBrain currentEncounter, consequenceEncounter;

    public int queueLength, turnsRemaining;

    //public SpriteRenderer npcSpriteRenderer;
    public UIAnimator uiAnimator;

    public TextMeshProUGUI npcUIText, npcNameText, turnsRemainingUI, gameOverText;
    public GameObject gameOverPanel;

    public string playerResponse;

    public bool encountering, playerHasResponded, displayTurnsRemaining;
    public bool gameOver;

    public float endEncounterTimer, turnsRemainingDisplayTimer, gameOverTimer, consequenceTimer;
    private float endEncounterTimerReset, turnsRemainingDisplayTimerReset, consequenceTimerReset;

    private ReputationMenu reputation;

    //public float reputation;

    public Slider happinessMeter, responseTimerBar;

    int count;

    void Start()
    {
        reputation = GetComponent<ReputationMenu>();
        reputation.reputationParent.SetActive(false);

        displayTurnsRemaining = true;
        gameOverPanel.SetActive(false);

        foreach (NPCBrain npc in potentialEncounterList)
        {
            npc.ResetNPCChoices();
        }


        //queueArray = new NPCBrain[queueLength];
        encounterQueue = new LinkedList<NPCBrain>();
        //encounterQueue = new Queue<NPCBrain>();

        for (int i = 0; i < queueLength; i++)
        {

            encounterQueue.AddLast(FillEncounterQueue());

        }

        endEncounterTimerReset = endEncounterTimer;
        consequenceTimerReset = consequenceTimer;
        turnsRemainingDisplayTimerReset = turnsRemainingDisplayTimer;


        
        ResetResponse();

        EncounterNPC();


    }

    void Update()
    {
        

        if (!encountering)
        {
            responseTimerBar.value = endEncounterTimer;

            EncounterNPC();
        }

        if (!currentEncounter.isConsequence)
        {
            responseTimerBar.maxValue = endEncounterTimerReset;

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
        }
        else
        {
            responseTimerBar.maxValue = consequenceTimerReset;
            responseTimerBar.value = consequenceTimer;
            consequenceTimer -= Time.deltaTime;

            if (consequenceTimer <= 0f)
            {
                EndEncounter();
            }
        }

        if (displayTurnsRemaining)
        {
            turnsRemainingUI.enabled = true;
            if (turnsRemaining > 1)
            {
                turnsRemainingUI.text = "Your reign will end in " + turnsRemaining.ToString() + " turns";
            }
            else if (turnsRemaining == 1)
            {
                turnsRemainingUI.text = "Your reign will end after this turn";
            }
            else
            {
                turnsRemainingUI.text = "Congratulations!";
            }

            turnsRemainingDisplayTimer -= Time.deltaTime;

            if (turnsRemainingDisplayTimer <= 2f)
            {
                turnsRemainingUI.alpha = Mathf.Lerp(0f, 1f, turnsRemainingDisplayTimer);
            }

            if (turnsRemainingDisplayTimer <= 0f)
            {
                
                displayTurnsRemaining = false;
            }
        }
        else
        {
            turnsRemainingDisplayTimer = turnsRemainingDisplayTimerReset;
            turnsRemainingUI.enabled = false;
        }

        if (gameOver)
        {
            gameOverTimer -= Time.deltaTime;

            if (gameOverTimer <= 0f)
            {
                LoseGame();
            }
        }

        happinessMeter.value = Mathf.Clamp(currentEncounter.playerReputation, 0f, 100f);
    }

    public NPCBrain FillEncounterQueue()
    {
        int r = Random.Range(0, potentialEncounterList.Count);

        return potentialEncounterList[r];
    }

    public void EncounterNPC()
    {
        foreach (NPCBrain npc in potentialEncounterList)
        {
            if (npc.isLoseEncounter)
            {
                npc.chosenDialogue = npc.loseDialogue;
                encounterQueue.AddFirst(npc);
                gameOverText.text = npc.loseScenarioText;
                gameOver = true;
            }
        }

        //currentEncounter = encounterQueue.Peek();
        currentEncounter = encounterQueue.First.Value;

        //npcSpriteRenderer.sprite = currentEncounter.npcSprite;
        uiAnimator.uiAnimation = currentEncounter.spriteAnimation;
        npcNameText.text = currentEncounter.npcName;

        //if (currentEncounter.isLoseEncounter)
        //{
        //    currentEncounter.chosenDialogue = currentEncounter.loseDialogue;
        //    LoseGame();
        //}


        if (!currentEncounter.isConsequence && !currentEncounter.isLoseEncounter)
        {
            currentEncounter.ChooseDialogue();
            npcUIText.text = currentEncounter.chosenDialogue.dialogue;

        }
        else
        {
            //currentEncounter.chosenDialogue = currentEncounter.consequenceDialogue;
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
                //consequenceEncounter.consequenceDialogue = currentEncounter.chosenDialogue.yesConsyesequence;
                npcUIText.text = currentEncounter.chosenDialogue.yesResponse;
                playerHasResponded = true;
            }

            if (playerResponse == "no")
            {
                //consequenceEncounter.consequenceDialogue = currentEncounter.chosenDialogue.noConsequence;
                npcUIText.text = currentEncounter.chosenDialogue.noResponse;
                playerHasResponded = true;

            }

            if (playerResponse == "idk")
            {
                //consequenceEncounter.consequenceDialogue = currentEncounter.chosenDialogue.idkConsequence;
                npcUIText.text = currentEncounter.chosenDialogue.idkResponse;
                playerHasResponded = true;

            }

            reputation.UpdateNPCProfiles();

            foreach (NPCBrain npc in potentialEncounterList)
            {
                if (npc.playerReputation <= 0f)
                {
                    //encounterQueue.AddFirst(npc);
                    
                    npc.isLoseEncounter = true;
                }
            }
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

            int r = Random.Range(0, currentEncounter.chosenDialogue.yesEffectsList.Count);
            consequenceEncounter = currentEncounter.chosenDialogue.yesEffectsList[r].npc;
            consequenceEncounter.chosenDialogue = currentEncounter.chosenDialogue.yesEffectsList[r].consequenceDialogueList[Random.Range(0, currentEncounter.chosenDialogue.yesEffectsList[r].consequenceDialogueList.Count)];
            consequenceEncounter.isConsequence = true;
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

            int r = Random.Range(0, currentEncounter.chosenDialogue.noEffectsList.Count);
            consequenceEncounter = currentEncounter.chosenDialogue.noEffectsList[r].npc;
            consequenceEncounter.chosenDialogue = currentEncounter.chosenDialogue.noEffectsList[r].consequenceDialogueList[Random.Range(0, currentEncounter.chosenDialogue.noEffectsList[r].consequenceDialogueList.Count)];
            consequenceEncounter.isConsequence = true;
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

            int r = Random.Range(0, currentEncounter.chosenDialogue.idkEffectsList.Count);
            consequenceEncounter = currentEncounter.chosenDialogue.idkEffectsList[r].npc;
            consequenceEncounter.chosenDialogue = currentEncounter.chosenDialogue.idkEffectsList[r].consequenceDialogueList[Random.Range(0, currentEncounter.chosenDialogue.idkEffectsList[r].consequenceDialogueList.Count)];
            consequenceEncounter.isConsequence = true;
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
            
            foreach (DecisionEffect effect in currentEncounter.chosenDialogue.yesEffectsList)
            {
                currentEncounterHistory.affectedNPCs.Add(effect.npc);
            }


            //NPCBrain consequenceEncounter = currentEncounter;
            //consequenceEncounter.isConsequence = true;
            //consequenceEncounter.chosenDialogue = currentEncounter.consequenceDialogue;

            currentEncounterHistory.consequenceNPC = consequenceEncounter;
            
            //consequenceEncounter.playerResponseString = playerResponse;

            //encounterQueue.Enqueue(consequenceEncounter);
            currentEncounter.encounterHistory.Add(currentEncounterHistory);

            ResetResponse();

            encounterQueue.RemoveFirst();
            //encounterQueue.AddFirst(consequenceEncounter);
            encounterQueue.AddAfter(encounterQueue.First, consequenceEncounter);

            //encounterQueue.Dequeue();
        }
        else
        {
            currentEncounter.chosenDialogue.isCompleted = true;

            ResetResponse();

            currentEncounter.ConsequenceDelivered();

            encounterQueue.RemoveFirst();
            //FillEncounterQueue();

        }
        //reputation.UpdateNPCProfiles();

        endEncounterTimer = endEncounterTimerReset;
        consequenceTimer = consequenceTimerReset;

        turnsRemaining--;

        if (turnsRemaining <= 0)
        {
            //player wins
            gameOverPanel.SetActive(true);
            gameOverText.text = "You lived to the end of your reign";

            //run win game stuff
        }
        else
        {
            turnsRemainingUI.text = "Your reign will end in " + turnsRemaining.ToString() + " turns";
        }

        //if (gameOver)
        //{
        //    LoseGame();
        //}

        displayTurnsRemaining = true;
        encountering = false;
        

        //NPC Leaves
    }

    public void LoseGame()
    {
        //enable lose panel
        gameOverPanel.SetActive(true);
    }
}
