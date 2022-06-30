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

    public float queueLength, turnsRemaining, happyNPCs, happyNPCsToWin, turnsAmount;

    //public SpriteRenderer npcSpriteRenderer;
    public UIAnimator uiAnimator;

    public Image npcNameImage;

    public TextMeshProUGUI npcUIText, npcNameText, turnsRemainingUI, gameOverText, respondingText, kingSpeechText, happyNPCsText;

    public GameObject gameOverPanel, nextButton, playerAnswerPanel, kingSpeechBubble, npcSpeechBubble;

    public string playerResponse;

    public bool encountering, playerHasResponded, displayTurnsRemaining;
    public bool gameOver, gameWon, textActivated;

    public float turnsRemainingDisplayTimer, turnsRemainingColourMix;
    private float turnsRemainingDisplayTimerReset;

    private ReputationMenu reputation;

    public Color happyColour, upsetColour, neutralColour, startTurnsColour;

    ScrollingText scrollingText;

    void Start()
    {
        nextEncounterList = new List<NPCBrain>();
        nextEncounterList.AddRange(potentialEncounterList);

        reputation = GetComponent<ReputationMenu>();
        reputation.UpdateNPCProfiles();

        displayTurnsRemaining = true;
        gameOverPanel.SetActive(false);
        nextButton.SetActive(false);
        kingSpeechBubble.SetActive(false);

        scrollingText = GetComponent<ScrollingText>();

        turnsAmount = turnsRemaining;

        foreach (NPCBrain npc in potentialEncounterList)
        {
            npc.ResetNPCChoices();
            npc.ResetDialogue();
        }

        encounterQueue = new LinkedList<NPCBrain>();

        FillEncounterQueue();

        turnsRemainingDisplayTimerReset = turnsRemainingDisplayTimer;
        happyNPCsText.text = happyNPCs + " Civilians will spread your legacy";

        textActivated = false;
        encountering = false;

        ResetResponse();

        //EncounterNPC();


    }

    void Update()
    {
        if (!encountering && !gameOver && !gameWon)
        {
            EncounterNPC();
        }

        if (encountering && !gameOver && !gameWon)
        {
            if ((turnsRemainingDisplayTimer >= turnsRemainingDisplayTimerReset / 2 && turnsRemainingDisplayTimer < turnsRemainingDisplayTimerReset))
            {
                npcSpeechBubble.SetActive(false);
                uiAnimator.uiDisplay.enabled = false;
                playerAnswerPanel.SetActive(false);
                textActivated = false;
                //scrollingText.DeactivateText();

            }
            else if (turnsRemainingDisplayTimer <= turnsRemainingDisplayTimerReset / 3)
            {
                uiAnimator.uiDisplay.enabled = true;
            }

            if (turnsRemainingDisplayTimer == turnsRemainingDisplayTimerReset)
            {
                if (!textActivated)
                {
                    //scrollingText.SetText(0);
                    scrollingText.ActivateText();
                    textActivated = true;
                }

                npcSpeechBubble.SetActive(true);

            }

            if (!currentEncounter.isConsequence)
            {
                if (turnsRemainingDisplayTimer <= 0f || turnsRemainingDisplayTimer == turnsRemainingDisplayTimerReset)
                {
                    playerAnswerPanel.SetActive(true);
                }

                if (playerHasResponded)
                {
                    reputation.UpdateNPCProfiles();

                    if (currentEncounter.playerReputation >= 51f)
                    {
                        npcNameImage.color = Color.Lerp(neutralColour, happyColour, Mathf.InverseLerp(51f, 100f, currentEncounter.playerReputation));
                    }
                    else if (currentEncounter.playerReputation <= 49f)
                    {
                        npcNameImage.color = Color.Lerp(upsetColour, neutralColour, Mathf.InverseLerp(0f, 49f, currentEncounter.playerReputation));
                    }
                    else npcNameImage.color = neutralColour;

                    happyNPCs = 0;

                    foreach (NPCBrain npc in potentialEncounterList)
                    {
                        if (npc.playerReputation <= 0f)
                        {
                            npc.isLoseEncounter = true;
                        }

                        if (npc.playerReputation >= 75f)
                        {
                            happyNPCs += 1;
                        }

                        if (happyNPCs >= happyNPCsToWin)
                        {
                            gameWon = true;
                        }
                    }

                    happyNPCsText.text = happyNPCs + " Civilians will spread your legacy";

                    respondingText.text = "Responding";

                    nextButton.SetActive(true);
                    playerAnswerPanel.SetActive(false);
                }
            }
            else
            {
                respondingText.text = "Consequence";

                if (!currentEncounter.chosenDialogue.hasConsequence)
                {
                    nextButton.SetActive(true);
                    playerAnswerPanel.SetActive(false);
                }
                else
                {
                    nextButton.SetActive(false);
                    //playerAnswerPanel.SetActive(true);
                }

            }
        }

        if (displayTurnsRemaining)
        {

            turnsRemainingUI.enabled = true;

            if (!currentEncounter.isConsequence)
            {
                if (turnsRemaining > 1)
                {
                    turnsRemainingUI.text = "Your life will end" + "\n" + "in " + turnsRemaining.ToString() + " turns";
                }
                else if (turnsRemaining == 1)
                {
                    turnsRemainingUI.text = "You will die of old age" + "\n" + "after this turn";
                }
                else
                {
                    turnsRemainingUI.text = "You have died of natural causes";
                    gameOver = true;
                }
            }
            else
            {
                turnsRemainingUI.text = "The " + currentEncounter.npcName + " is here" + "\n" +"regarding your decision";
            }

            turnsRemainingColourMix = turnsRemaining / turnsAmount;

            turnsRemainingUI.color = Color.Lerp(upsetColour, startTurnsColour, turnsRemainingColourMix);

            Debug.Log(turnsRemainingColourMix);

            turnsRemainingDisplayTimer -= Time.deltaTime;

            if (turnsRemainingDisplayTimer >= turnsRemainingDisplayTimerReset - 2f)
            {
                turnsRemainingUI.alpha = Mathf.Lerp(0f, 1f, turnsRemainingDisplayTimerReset - turnsRemainingDisplayTimer);

            }

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


        if (gameOver && npcSpeechBubble.activeSelf)
        {
            LoseGame();
        }

    }

    public void FillEncounterQueue()
    {
        for (int i = 0; i < queueLength; i++)
        {
            if (encounterQueue.Count >= 1f)
            {
                SetNextEncounter();
            }

            encounterQueue.AddLast(AddToEncounterQueue());
        }
        Debug.Log("encounter queue filled");
    }

    public NPCBrain AddToEncounterQueue()
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
                gameOverText.text = npc.loseScenarioText + "\n" + "Game Over!";
                gameOver = true;
            }
        }

        if (encounterQueue.Count <= 2)
        {
            FillEncounterQueue();
            EncounterNPC();
        }
        
        if (encounterQueue.First.Value.dialogueOptions.Count <= 0)
        {
            encounterQueue.RemoveFirst();
            EncounterNPC();

        }
        

        currentEncounter = encounterQueue.First.Value;

        uiAnimator.uiAnimation = currentEncounter.spriteAnimation;

        npcNameText.text = currentEncounter.npcName;


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
                EncounterNPC();
            }

            respondingText.text = "Inquiring";

            //playerAnswerPanel.SetActive(true);
        }
        else
        {
            ResetResponse();
            //currentEncounter.chosenDialogue = currentEncounter.consequenceDialogue;
        }

        if (currentEncounter.chosenDialogue != null)
        {
            //npcUIText.text = currentEncounter.chosenDialogue.dialogue;
            scrollingText.FillStringArray();
            //scrollingText.DeactivateText();
            scrollingText.SetText(0);

        }

        encountering = true;
        
    }

    public void YesResponse()
    {
        if (!playerHasResponded && currentEncounter.chosenDialogue.hasConsequence)
        {
            playerResponse = "yes";

            //npcUIText.text = currentEncounter.chosenDialogue.yesResponse;
            //scrollingText.DeactivateText();
            textActivated = false;

            scrollingText.SetText(1);

            if (!textActivated)
            {
                scrollingText.ActivateText();
                textActivated = true;
            }

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

            kingSpeechBubble.SetActive(true);
            kingSpeechText.text = "I'll allow it";

            playerHasResponded = true;
        }
    }

    public void NoResponse()
    {
        if (!playerHasResponded && !currentEncounter.isConsequence)
        { 
            playerResponse = "no";

            //npcUIText.text = currentEncounter.chosenDialogue.noResponse;
            //scrollingText.DeactivateText();
            textActivated = false;

            scrollingText.SetText(2);

            if (!textActivated)
            {
                scrollingText.ActivateText();
                textActivated = true;
            }

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

            kingSpeechBubble.SetActive(true);
            kingSpeechText.text = "You are denied";

            playerHasResponded = true;

        }
    }

    public void IDKResponse()
    {
        if (!playerHasResponded && !currentEncounter.isConsequence)
        {
            playerResponse = "idk";

            //npcUIText.text = currentEncounter.chosenDialogue.idkResponse;
            //scrollingText.DeactivateText();
            textActivated = false;

            scrollingText.SetText(3);

            if (!textActivated)
            {
                scrollingText.ActivateText();
                textActivated = true;
            }


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

            kingSpeechBubble.SetActive(true);
            kingSpeechText.text = "Leave me be!";

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

            currentEncounter.chosenDialogue.isCompleted = true;

            foreach (NPCBrain npc in potentialEncounterList)
            {
                for (int i = 0; i < npc.dialogueOptions.Count; i++)
                if (npc.dialogueOptions[i].isCompleted)
                {
                    npc.dialogueOptions.Remove(npc.dialogueOptions[i]);

                }
            }


            consequenceEncounter.isConsequence = true;

            encounterQueue.RemoveFirst();

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
        //playerAnswerPanel.SetActive(true);

        if (gameWon) //turnsRemaining <= 0  && !encounterQueue.First.Value.isConsequence
        {
            WinGame();
        }

        if (turnsRemaining <= 0)
        {
            gameOverText.text = "You have died of old age, you will quickly be forgotten.";

            LoseGame();
        }

        //if (gameOver)
        //{
        //    LoseGame();
        //}
        turnsRemainingUI.alpha = 0f;
        displayTurnsRemaining = true;
        kingSpeechBubble.SetActive(false);
        textActivated = false;
        
        //scrollingText.SetText(0);
        encountering = false;
        

        //NPC Leaves
    }

    public void WinGame()
    {
        gameOverPanel.SetActive(true);
        playerAnswerPanel.SetActive(false);
        gameOverText.text = "You have built a strong legacy! LONG LIVE THE KING!";
        displayTurnsRemaining = false;
        currentEncounter = null;
        npcSpeechBubble.SetActive(false);
        uiAnimator.gameObject.SetActive(false);

        displayTurnsRemaining = false;

        for (int i = 0; i < encounterQueue.Count; i++)
        {
            encounterQueue.RemoveFirst();
        }
    }

    public void LoseGame()
    {
        //enable lose panel
        gameOverPanel.SetActive(true);
        playerAnswerPanel.SetActive(false);

        currentEncounter = null;
        npcSpeechBubble.SetActive(false);
        uiAnimator.gameObject.SetActive(false);
        displayTurnsRemaining = false;

        for (int i = 0; i < encounterQueue.Count; i++)
        {
            encounterQueue.RemoveFirst();
        }
    }


}
