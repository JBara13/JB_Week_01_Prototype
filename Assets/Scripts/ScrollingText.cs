using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScrollingText : MonoBehaviour
{
    Encounter encounter;

    public float textSpeed = 0.01f;

    public string[] itemInfo;
    private TextMeshProUGUI scrollingText;
    private int currentDisplayingText = 0;

    //public bool textActivated;

    private void Start()
    {
        encounter = GetComponent<Encounter>();
        scrollingText = encounter.npcUIText;

        itemInfo = new string[4];
    }

    public void ActivateText()
    {
        StartCoroutine(AnimateText());
        //textActivated = true;
    }

    public void FillStringArray()
    {
        int dialogueLength = encounter.currentEncounter.chosenDialogue.dialogue.Length;

        itemInfo[0] = encounter.currentEncounter.chosenDialogue.dialogue;
        itemInfo[1] = encounter.currentEncounter.chosenDialogue.yesResponse;
        itemInfo[2] = encounter.currentEncounter.chosenDialogue.noResponse;
        itemInfo[3] = encounter.currentEncounter.chosenDialogue.idkResponse;

        //SetText(0);
    }

    public int SetText(int count)
    {
        return currentDisplayingText = count;
    }

    IEnumerator AnimateText()
    {
        for (int i = 0; i < itemInfo[currentDisplayingText].Length + 1; i++)
        {
            scrollingText.text = itemInfo[currentDisplayingText].Substring(0, i);
            yield return new WaitForSeconds(textSpeed);
        }
    }

    //public void DeactivateText()
    //{

    //    StopCoroutine(AnimateText());
    //    textActivated = false;
        
    //}
}
