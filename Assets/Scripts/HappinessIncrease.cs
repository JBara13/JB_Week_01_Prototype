using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HappinessIncrease : MonoBehaviour
{

    public Slider happinessMeter;

    public GameObject button;


    public void OnButtonClick()
    {
        happinessMeter.value += 10;
    }

}