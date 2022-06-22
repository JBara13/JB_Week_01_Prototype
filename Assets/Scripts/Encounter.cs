using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Encounter : MonoBehaviour
{
    public List<NPCBrain> potentialEncounterList;
    public List<NPCBrain> nextEncounterList;
    public LinkedList<NPCBrain> encounterQueue;
    //public Queue<NPCBrain> encounterQueue;
    [SerializeField] private NPCBrain[] queueArray;
    public NPCBrain currentEncounter, consequenceEncounter;

    public int queueLength, turnsRemaining;

    //public SpriteRenderer npcSpriteRenderer;
    public UIAnimator uiAnimator;

    public Image npcNameImage;

    public TextMeshProUGUI npcUIText, npcNameText, turnsRemainingUI, gameOverText, respondingText;

    public GameObject gameOverPanel, nextButton, playerAnswerPanel;

    public string playerResponse;

    public bool encountering, playerHasResponded, displayTurnsRemaining;
    public bool gameOver, gameWon;

    public float turnsRemainingDisplayTimer;
    private float turnsRemainingDisplayTimerReset;

    private ReputationMenu reputation;

    public Color happyColour, upsetColour, neutralColour;

    void Start()
    {
        nextEncounterList = new List<NPCBrain>();
        nextEncounterList.AddRange(potentialEncounterList);

        reputation = GetComponent<ReputationMenu>();
        reputation.UpdateNPCProfiles();

        displayTurnsRemaining = true;
        gameOverPanel.SetActive(false);
        nextButton.SetActive(false);

        foreach (NPCBrain npc in potentialEncounterList)
        {
            npc.ResetNPCChoices();
            npc.ResetDialogue();
        }

        encounterQueue = new LinkedList<NPCBrain>();

        for (int i = 0; i < queueLength; i++)
        {
            if (encounterQueue.Count >= 1f)
            { 
                SetNextEncounter();
            }

            encounterQueue.AddLast(FillEncounterQueue());
        }

        turnsRemainingDisplayTimerReset = turnsRemainingDisplayTimer;


        
        ResetResponse();

        EncounterNPC();


    }

    void Update()
    {
        if (!encountering)
        {
            //responseTimerBar.value = endEncounterTimer;

            EncounterNPC();
        }

        if (!currentEncounter.isConsequence)
        {
            //responseTimerBar.maxValue = endEncounterTimerReset;

            //{
            //    AwaitPlayerResponse();
            //}
            //else
            if (encountering && playerHasResponded)
            {
                reputation.UpdateNPCProfiles();

                foreach (NPCBrain npc in potentialEncounterList)
                {
                    if (npc.playerReputation <= 0f)
                    {
                        //encounterQueue.AddFirst(npc);

                        npc.isLoseEncounter = true;
                    }
                }

                respondingText.text = "Responding";

                nextButton.SetActive(true);
                playerAnswerPanel.SetActive(false);


                //responseTimerBar.maxValue = endEncounterTimerReset;
                //responseTimerBar.value = endEncounterTimer;
                //endEncounterTimer -= Time.deltaTime;

                //if (endEncounterTimer <= 0f)
                //{
                //}
            }
        }
        else
        {
            respondingText.text = "Consequence";

            nextButton.SetActive(true);
            playerAnswerPanel.SetActive(false);

            //responseTimerBar.maxValue = consequenceTimerReset;
            //responseTimerBar.value = consequenceTimer;
            //consequenceTimer -= Time.deltaTime;

            //if (consequenceTimer <= 0f)
            //{
            //}
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
            LoseGame();
        }

        //happinessMeter.value = Mathf.Clamp(currentEncounter.playerReputation, 0f, 100f);
        if (currentEncounter.playerReputation >= 51f)
        {
            npcNameImage.color = Color.Lerp(neutralColour, happyColour, Mathf.InverseLerp(51f, 100f, currentEncounter.playerReputation));
        }
        else if (currentEncounter.playerReputation <= 49f)
        {
            npcNameImage.color = Color.Lerp(upsetColour, neutralColour, Mathf.InverseLerp(0f, 49f, currentEncounter.playerReputation));
        }
        else npcNameImage.color = neutralColour;
    }

    public NPCBrain FillEncounterQueue()
    {
        int r = Random.Range(0, nextEncounterList.Count);

        return nextEncounterList[r];
    }

    void SetNextEncounter()
    {
        if (nextEncounterList.Contains(encounterQueue.Last.Value))
        {
            nextEncounterList.Remove(encounterQueue.Last.Value);
        }

        if (nextEncounterList.Count <= 0)
        {
            nextEncounterList.AddRange(potentialEncounterList);
        }
    }

    public void EncounterNPC()
    {
        ResetResponse();

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


        if (!currentEncounter.isConsequence && !currentEncounter.isLoseEncounter && !gameWon)
        {
            if (currentEncounter.dialogueOptions.Count > 0)
            {
                currentEncounter.ChooseDialogue();
            }
            else
            {
                Debug.Log(currentEncounter.name + " is out of things to ask, skipping");

                encounterQueue.RemoveFirst();
                encountering = false;
            }
            npcUIText.text = currentEncounter.chosenDialogue.dialogue;

            respondingText.text = "Inquiring";

        }
        else
        {
            ResetResponse();
            //currentEncounter.chosenDialogue = currentEncounter.consequenceDialogue;
            npcUIText.text = currentEncounter.chosenDialogue.dialogue;
        }

        encountering = true;
        
    }

    //public void AwaitPlayerResponse()
    //{
    //    if (playerResponse != null)
    //    {
    //        if (playerResponse == "yes")
    //        {
    //            //consequenceEncounter.consequenceDialogue = currentEncounter.chosenDialogue.yesConsyesequence;
    //            npcUIText.text = currentEncounter.chosenDialogue.yesResponse;
    //            playerHasResponded = true;
    //        }

    //        if (playerResponse == "no")
    //        {
    //            //consequenceEncounter.consequenceDialogue = currentEncounter.chosenDialogue.noConsequence;
    //            npcUIText.text = currentEncounter.chosenDialogue.noResponse;
    //            playerHasResponded = true;

    //        }

    //        if (playerResponse == "idk")
    //        {
    //            //consequenceEncounter.consequenceDialogue = currentEncounter.chosenDialogue.idkConsequence;
    //            npcUIText.text = currentEncounter.chosenDialogue.idkResponse;
    //            playerHasResponded = true;

    //        }
    //    }
    //}

    public void YesResponse()
    {
        if (!playerHasResponded && !currentEncounter.isConsequence)
        {
            playerResponse = "yes";

            npcUIText.text = currentEncounter.chosenDialogue.yesResponse;


            foreach (DecisionEffect effect in currentEncounter.chosenDialogue.yesEffectsList)
            {
                effect.npc.playerReputation += effect.reputationEffect;
            }

            if (currentEncounter.chosenDialogue.hasConsequence)
            {
                int r = Random.Range(0, currentEncounter.chosenDialogue.yesEffectsList.Count);
                consequenceEncounter = currentEncounter.chosenDialogue.yesEffectsList[r].npc;
                consequenceEncounter.chosenDialogue = currentEncounter.chosenDialogue.yesEffectsList[r].consequenceDialogueList[Random.Range(0, currentEncounter.chosenDialogue.yesEffectsList[r].consequenceDialogueList.Count)];
            }

            playerHasResponded = true;
        }
    }

    public void NoResponse()
    {
        if (!playerHasResponded && !currentEncounter.isConsequence)
        { 
            playerResponse = "no";

            npcUIText.text = currentEncounter.chosenDialogue.noResponse;

            foreach (DecisionEffect effect in currentEncounter.chosenDialogue.noEffectsList)
            {
                effect.npc.playerReputation += effect.reputationEffect;
            }

            if (currentEncounter.chosenDialogue.hasConsequence)
            {
                int r = Random.Range(0, currentEncounter.chosenDialogue.noEffectsList.Count);
                consequenceEncounter = currentEncounter.chosenDialogue.noEffectsList[r].npc;
                consequenceEncounter.chosenDialogue = currentEncounter.chosenDialogue.noEffectsList[r].consequenceDialogueList[Random.Range(0, currentEncounter.chosenDialogue.noEffectsList[r].consequenceDialogueList.Count)];
                //consequenceEncounter.isConsequence = true;
            }

            playerHasResponded = true;

        }
    }

    public void IDKResponse()
    {
        if (!playerHasResponded && !currentEncounter.isConsequence)
        {
            playerResponse = "idk";

            npcUIText.text = currentEncounter.chosenDialogue.idkResponse;


            foreach (DecisionEffect effect in currentEncounter.chosenDialogue.idkEffectsList)
            {
                effect.npc.playerReputation += effect.reputationEffect;
            }

            if (currentEncounter.chosenDialogue.hasConsequence)
            {
                int r = Random.Range(0, currentEncounter.chosenDialogue.idkEffectsList.Count);
                consequenceEncounter = currentEncounter.chosenDialogue.idkEffectsList[r].npc;
                consequenceEncounter.chosenDialogue = currentEncounter.chosenDialogue.idkEffectsList[r].consequenceDialogueList[Random.Range(0, currentEncounter.chosenDialogue.idkEffectsList[r].consequenceDialogueList.Count)];
                //consequenceEncounter.isConsequence = true;
            }

            playerHasResponded = true;
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
        if (!currentEncounter.isConsequence)
        {
            turnsRemaining--;
        }

        if (currentEncounter.chosenDialogue.hasConsequence)
        {
            EncounterHistory currentEncounterHistory = new EncounterHistory();
            currentEncounterHistory.dialogue = currentEncounter.chosenDialogue;
            currentEncounterHistory.playerResponse = playerResponse;
            
            foreach (DecisionEffect effect in currentEncounter.chosenDialogue.yesEffectsList)
            {
                currentEncounterHistory.affectedNPCs.Add(effect.npc);
            }

            currentEncounterHistory.consequenceNPC = consequenceEncounter;
            
            currentEncounter.encounterHistory.Add(currentEncounterHistory);

            foreach (NPCBrain npc in potentialEncounterList)
            {
                if (npc.dialogueOptions.Contains(currentEncounter.chosenDialogue))
                {
                    npc.dialogueOptions.Remove(currentEncounter.chosenDialogue);

                }
            }


            consequenceEncounter.isConsequence = true;

            encounterQueue.RemoveFirst();

            if (encounterQueue.Count <= 0)
            {
                for (int i = 0; i < queueLength; i++)
                {
                    if (encounterQueue.Count >= 1f)
                    {
                        SetNextEncounter();
                    }

                    encounterQueue.AddLast(FillEncounterQueue());
                }
            }
            //encounterQueue.AddFirst(consequenceEncounter);
            encounterQueue.AddAfter(encounterQueue.First, consequenceEncounter);

            //encounterQueue.Dequeue();
        }
        else
        {
            foreach (NPCBrain npc in potentialEncounterList)
            {
                if (npc.dialogueOptions.Contains(currentEncounter.chosenDialogue))
                {
                    npc.dialogueOptions.Remove(currentEncounter.chosenDialogue);

                }
            }

            currentEncounter.ConsequenceDelivered();

            encounterQueue.RemoveFirst();

        }

        //endEncounterTimer = endEncounterTimerReset;
        //consequenceTimer = consequenceTimerReset;
        //responseBarText.text = null;
        nextButton.SetActive(false);
        playerAnswerPanel.SetActive(true);

        if (turnsRemaining <= 0 && !encounterQueue.First.Value.isConsequence)
        {
            //player wins
            gameWon = true;
            gameOverPanel.SetActive(true);
            playerAnswerPanel.SetActive(false);
            turnsRemainingUI.enabled = false;
            gameOverText.text = "You lived to the end of your reign! A worthy King!";

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
        playerAnswerPanel.SetActive(false);
        turnsRemainingUI.enabled = false;
    }
}
