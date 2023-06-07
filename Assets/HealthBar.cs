using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider Slider;

    public void SetMaxHealth(int MaxHealth)
    {
        Slider.maxValue = MaxHealth;
        Slider.value = MaxHealth;
    }

    public void SetHealth(int Health)
    {
        Slider.value = Health;
    }
}