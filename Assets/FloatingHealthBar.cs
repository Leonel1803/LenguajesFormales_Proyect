using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class FloatingHealthBar : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Slider slider;

    public void UpdateHealthBar(float currentValue, float maxValue) 
    {
        slider.value = currentValue / maxValue;
    }

    // Update is called once per frame
    void Update()
     {

     }

}
