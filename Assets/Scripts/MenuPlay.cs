using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuPlay : MonoBehaviour
{

    private GameObject miseButton;
    private GameObject relanceButton;
    private GameObject suivreButton;
    private GameObject checkButton;
    private Slider slider;
    private LogicPerso currentPlayer;
    private LogicGame jeu;

    void Start()
    {
        miseButton = this.gameObject.transform.Find("Miser").gameObject;
        relanceButton = this.gameObject.transform.Find("Relancer").gameObject;
        suivreButton = this.gameObject.transform.Find("Suivre").gameObject;
        checkButton = this.gameObject.transform.Find("Check").gameObject;
        jeu = this.gameObject.transform.parent.GetComponent<LogicGame>();
        slider = this.gameObject.transform.Find("Slider").gameObject.GetComponent<Slider>();
        this.gameObject.SetActive(false);
    }

    public void Relance()
    {
        currentPlayer.Relancer((int) (slider.value - currentPlayer.currentMise()));
        this.Stop();
    }

    public void Suivre()
    {
        currentPlayer.Suivre();
        this.Stop();
    }

    public void Check()
    {
        currentPlayer.Check();
        this.Stop();
    }

    public void Coucher()
    {
        currentPlayer.Coucher();
        this.Stop();
    }

    public void Launch(LogicPerso player)
    {
        jeu.tourReelPlayer(true);
        int currentRelance = jeu.CurrentRelance();
        currentPlayer = player;
        slider.minValue = currentRelance + 1;
        slider.maxValue = player.nbJeton() + currentRelance;
        slider.value = currentRelance + 1;

        if(currentRelance == 0)
        {
            miseButton.SetActive(true);
            relanceButton.SetActive(false);
            suivreButton.SetActive(false);
            checkButton.SetActive(true);
        } else
        {
            miseButton.SetActive(false);
            relanceButton.SetActive(true);
            suivreButton.SetActive(true);
            checkButton.SetActive(false);
        }

        StartCoroutine("TimerMenu");
    }
           
    public void Stop()
    {
        jeu.tourReelPlayer(false);
        this.gameObject.SetActive(false);
        StopCoroutine("TimerMenu");
    }

    private IEnumerator TimerMenu()
    {
        Text textTimer = this.transform.Find("Timer").GetComponent<Text>();
        int time = 90;
        while (time > 0)
        {
            textTimer.text = time.ToString();
            yield return new WaitForSeconds(1f);
            time--;
        }
        this.Coucher();
        this.Stop();
    }


}
