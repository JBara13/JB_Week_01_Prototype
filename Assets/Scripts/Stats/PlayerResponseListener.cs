using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerResponseListener : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI[] playerAnswerText, consequenceText;

    [SerializeField] private int yesAnswerCount, noAnswerCount, idkAnswerCount, promptCount, consequenceCount;

    private void OnEnable()
    {
        Encounter.OnYesAnswer += YesResponse;
        Encounter.OnNoAnswer += NoResponse;
        Encounter.OnIDKAnswer += IDKResponse;

        Encounter.OnConsequence += Consequence;
        Encounter.OnPrompt += Prompt;
    }

    private void OnDisable()
    {
        Encounter.OnYesAnswer -= YesResponse;
        Encounter.OnNoAnswer -= NoResponse;
        Encounter.OnIDKAnswer -= IDKResponse;

        Encounter.OnConsequence -= Consequence;
        Encounter.OnPrompt -= Prompt;
    }

    public void YesResponse()
    {
        yesAnswerCount++;
        playerAnswerText[0].text = "Yes Answer Count: " + yesAnswerCount.ToString();
    }
    public void NoResponse()
    {
        noAnswerCount++;
        playerAnswerText[1].text = "No Answer Count: " + noAnswerCount.ToString();
    }
    public void IDKResponse()
    {
        idkAnswerCount++;
        playerAnswerText[2].text = "IDK Answer Count: " + idkAnswerCount.ToString();
    }

    public void Consequence()
    {
        consequenceCount++;
        consequenceText[1].text = "Consequence Count: " + consequenceCount.ToString();
    }

    public void Prompt()
    {
        promptCount++;
        consequenceText[0].text = "Prompt Count: " + promptCount.ToString();
    }
}
