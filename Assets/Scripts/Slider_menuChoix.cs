using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slider_menuChoix : MonoBehaviour
{
    private Text textMin, textMax, textValeur;
    private Slider slider;

    void Start()
    {
        textMin = this.gameObject.transform.Find("Minimum").GetComponent<Text>();
        textMax = this.gameObject.transform.Find("Maximum").GetComponent<Text>();
        textValeur = this.gameObject.transform.Find("Valeur").GetComponent<Text>();
        slider = this.gameObject.GetComponent<Slider>();
    }

    public int value()
    {
        return (int) slider.value;
    }



    void Update()
    {
        textMin.text = slider.minValue.ToString();
        textMax.text = slider.maxValue.ToString();
        textValeur.text = slider.value.ToString();
    }
}
