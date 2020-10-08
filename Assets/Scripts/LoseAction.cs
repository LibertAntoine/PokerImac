using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoseAction : MonoBehaviour
{

    public void DisplayStatsLose()
    {
        SceneManager.LoadScene(5);
    }
}
