using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManaBar : MonoBehaviour
{
    public Slider Slider;

    public void SetMaxStamina(int MaxStamina)
    {
        Slider.maxValue = MaxStamina;
        Slider.value = MaxStamina;
    }

    public void SetStamina(int Stamina)
    {
        Slider.value = Stamina;
    }
}