using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Happiness : MonoBehaviour
{
    public Slider slider;
    public static float happinessValue = 100f;
    

    public void UpdateHappiness(float happiness)
    {
        happinessValue += happiness;
        slider.value = happinessValue / 100f;
    }
}