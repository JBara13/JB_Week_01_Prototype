using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ReputationMenu : MonoBehaviour
{
    public GameObject reputationMenu, npcProfile, reputationParent;

    private Encounter encounter;

    public List<Slider> npcReputations;

    private void Start()
    {
        encounter = GetComponent<Encounter>();

        npcReputations = new List<Slider>();

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

            Image npcImage = newProfile.transform.GetChild(0).GetComponent<Image>();
            npcImage.sprite = npc.spriteAnimation.animationFrames[0];

            TextMeshProUGUI npcName = newProfile.transform.GetChild(1).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            npcName.text = npc.npcName;

            Slider npcHappiness = newProfile.transform.GetChild(2).GetComponent<Slider>();
            npcHappiness.value =  Mathf.Clamp(npc.playerReputation, 0f, 100f);

            npcReputations.Add(npcHappiness);

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
        }
    }

    public void ToggleReputationMenu()
    {
        if (!reputationParent.activeSelf)
        {
            reputationParent.SetActive(true);
        }
        else
        {
            reputationParent.SetActive(false);       
        }
    }
}
