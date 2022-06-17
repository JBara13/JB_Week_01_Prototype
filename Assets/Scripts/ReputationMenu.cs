using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ReputationMenu : MonoBehaviour
{
    public GameObject reputationMenu, npcProfile;

    private Encounter encounter;

    public List<Slider> npcReputations;
    public List<Image> npcReputationImages, handleSprites, happinessbarFills;

    public Sprite happyHandle, neutralHandle, upsetHandle;

    private void Start()
    {
        encounter = GetComponent<Encounter>();

        npcReputations = new List<Slider>();
        npcReputationImages = new List<Image>();

        SpawnNPCProfiles();
        UpdateNPCProfiles();
    }

    private void Update()
    {
        
    }

    void SpawnNPCProfiles()
    {
        foreach(NPCBrain npc in encounter.potentialEncounterList)
        {
            GameObject newProfile = Instantiate(npcProfile, reputationMenu.transform.position, Quaternion.identity);

            newProfile.transform.SetParent(reputationMenu.transform);

            Vector3 resetScale = Vector3.one;

            newProfile.transform.localScale = resetScale;

            //Image npcImage = newProfile.transform.GetChild(0).GetComponent<Image>();
            //npcImage.sprite = npc.spriteAnimation.animationFrames[0];

            TextMeshProUGUI npcName = newProfile.transform.GetChild(1).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            npcName.text = npc.npcName;
            
            TextMeshProUGUI npcAlignment = newProfile.transform.GetChild(3).GetComponent<TextMeshProUGUI>();
            npcAlignment.text = npc.npcAlignment;

            Slider npcHappiness = newProfile.transform.GetChild(2).GetComponent<Slider>();
            npcHappiness.value =  Mathf.Clamp(npc.playerReputation, 0f, 100f);

            npcReputations.Add(npcHappiness);

            Image handle = newProfile.transform.GetChild(2).transform.GetChild(2).transform.GetChild(0).GetComponent<Image>();
            handleSprites.Add(handle);

            Image happinessBarFill = newProfile.transform.GetChild(2).transform.GetChild(1).transform.GetChild(0).GetComponent<Image>();
            happinessbarFills.Add(happinessBarFill);

            Image npcHappinessImage = newProfile.transform.GetChild(1).GetComponent<Image>();
            npcReputationImages.Add(npcHappinessImage);

            //newProfile.GetComponent<NPCBrain>()
        }
    }

    public void UpdateNPCProfiles()
    {

        for (int i = 0; i < npcReputations.Count; i++)
        {
            if (npcReputations[i].value != encounter.potentialEncounterList[i].playerReputation)
            {
                npcReputations[i].value = encounter.potentialEncounterList[i].playerReputation;
            }

            // Set Name Panel Colour
            if (npcReputations[i].value >= 51f)
            {
                npcReputationImages[i].color = Color.Lerp(encounter.neutralColour, encounter.happyColour, Mathf.InverseLerp(50f, 100f, npcReputations[i].value));
            }
            else if (npcReputations[i].value <= 49f)
            {
                npcReputationImages[i].color = Color.Lerp(encounter.upsetColour, encounter.neutralColour, Mathf.InverseLerp(0f, 50f, npcReputations[i].value));
            }
            else
            {
                npcReputationImages[i].color = encounter.neutralColour;
            }

            happinessbarFills[i].color = npcReputationImages[i].color;

            // Set Happiness Sprite
            if (npcReputations[i].value >= 75f)
            {
                handleSprites[i].sprite = happyHandle;
            }
            else if (npcReputations[i].value <= 25f)
            {
                handleSprites[i].sprite = upsetHandle;
            }
            else
            {
                handleSprites[i].sprite = neutralHandle;
            }
        }
    }

    //public void ToggleReputationMenu()
    //{
    //    if (!reputationParent.activeSelf)
    //    {
    //        reputationParent.SetActive(true);
    //    }
    //    else
    //    {
    //        reputationParent.SetActive(false);       
    //    }
    //}
}
