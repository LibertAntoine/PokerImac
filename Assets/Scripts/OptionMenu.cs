using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OptionMenu : MonoBehaviour
{
    public GameObject[] sliders; 

    void Update()
    {
        
    }

    public void StartGame()
    {
        
        foreach (GameObject slider in sliders)
        {
            PlayerPrefs.SetInt(slider.name, slider.GetComponent<Slider_menuChoix>().value());
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
    }
}
