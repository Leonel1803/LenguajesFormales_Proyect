using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class FloatingHealthBar : MonoBehaviour
{
    // Start is called before the first frame update
    public Slider slider;

    private void Awake()
    {
        slider = transform.parent.parent.GetComponentInChildren<Slider>();
       // slider = this.transform.parent.GetComponent<Slider>();
        UnityEngine.Debug.Log(slider);
    }
    public void UpdateHealthBar(float currentValue, float maxValue)
    {
        slider.value = currentValue / maxValue;
    }

    // Update is called once per frame
    void Update()
    {

    }

}
