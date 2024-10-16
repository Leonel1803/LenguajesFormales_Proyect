using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingHealthBar : MonoBehaviour
{
    // Start is called before the first frame update
    public Slider slider;

    private void Awake()
    {
        UnityEngine.Debug.Log(gameObject);
        slider = gameObject.GetComponent<Slider>();
    //   // slider = this.transform.parent.GetComponent<Slider>();
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
