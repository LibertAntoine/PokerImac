using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinAction : MonoBehaviour
{
    public void DisplayStatsWin()
    {
        SceneManager.LoadScene(5);
    }
}
