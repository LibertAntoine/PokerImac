using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Runtime.InteropServices;

/*
public class PointClassement : IComparer
{
    bool IComparer.Compare(Object x, Object y)
    {
        return (x.getPoint < y.getPoint) ? x : y;
    }
}*/

public class LogicPot : MonoBehaviour
{
    private LogicGame jeu;

    private LogicCard[] _potCards = {null, null, null, null, null};

    private int potContent = 0;
    private Vector3 potPosition = new Vector3(-0.02f, 1.44f, 0f);

    private SpriteRenderer _grosTas;
    private SpriteRenderer _moyenTas;
    private SpriteRenderer _petitTas;
    private Text _textPot;
    private AudioSource jeton;


    void Start()
    {
        jeu = this.GetComponentInParent<LogicGame>();

        _grosTas = this.transform.Find("JetonTas_1").GetComponent<SpriteRenderer>();
        _moyenTas = this.transform.Find("JetonTas_3").GetComponent<SpriteRenderer>();
        _petitTas = this.transform.Find("JetonTas_4").GetComponent<SpriteRenderer>();
        _textPot = this.transform.Find("TextPot").Find("TextPot").GetComponent<Text>();


        _grosTas.enabled = false;
        _moyenTas.enabled = false;
        _petitTas.enabled = false;
        _textPot.gameObject.SetActive(false);

        jeton = this.GetComponent<AudioSource>();
        
    }


    void initPotCard()
    {
        for(int i = 0; i < 5; i++) _potCards[i] = null;
    }

    public void addCard(LogicCard card)
    {
        int i = 0;
        while(_potCards[i] != null) i++;
        _potCards[i] = card;
    }

    public void DestroyPotCards()
    {
        print(_potCards[0]);
        print(_potCards[0].gameObject);
        Destroy(_potCards[0].gameObject);
        Destroy(_potCards[1].gameObject);
        Destroy(_potCards[2].gameObject);
        Destroy(_potCards[3].gameObject);
        Destroy(_potCards[4].gameObject);
        _potCards[0] = null;
        _potCards[1] = null;
        _potCards[2] = null;
        _potCards[3] = null;
        _potCards[4] = null;
        _grosTas.enabled = false;
        _moyenTas.enabled = false;
        _petitTas.enabled = false;
        _textPot.gameObject.SetActive(false);
        potContent = 0;
    }


    public IEnumerator ramasseMise(LogicPerso player, int nbJoueurManche)
    {
        //print("rammase");
        jeton.Play();
        do
        {
            if (player.currentMise() != 0)
            {
                IEnumerator move = moveJeton(player.misePosition(), potPosition, player.rammasseCurrentMise());
                StartCoroutine(move);
            }
            player = player.next();
        } while (!player.haveBlind());
        jeu.videCurrentRelance();
        yield return new WaitForSeconds(2f);
        IEnumerator Play = jeu.PlayTour();
        StartCoroutine(Play);
    }

    public void finishManche(int maxPoint, int nbGagnant)
    {
        jeton.Play();
        _textPot.gameObject.SetActive(false);
        int distrib = potContent / nbGagnant;
        print("distrib" + distrib);
        print("maxPoint" + maxPoint);
        print("nbGagnant" + nbGagnant);
        do
        {
            if (jeu.CurrentPlayer().pointManche() == maxPoint)
            {
                IEnumerator Ramasse = this.moveJetonJoueur(potPosition, jeu.CurrentPlayer().jetonPosition(), distrib, jeu.CurrentPlayer());
                StartCoroutine(Ramasse);
            }
            jeu.nextPlayer();
        } while (!jeu.CurrentPlayer().haveBlind());

    }

    private IEnumerator moveJeton(Vector3 startPosition, Vector3 finalPosition, int mise)
    {
        GameObject pot;
        float vitesse = 0.2f;
        if (mise < 200)
        {
            pot = (GameObject) Instantiate(Resources.Load("Prefab/PotPetit"), startPosition, Quaternion.identity, this.transform);
        }
        else if (mise > 1000)
        {
            pot = (GameObject)Instantiate(Resources.Load("Prefab/PotGros"), startPosition, Quaternion.identity, this.transform);
        }
        else
        {
            pot = (GameObject)Instantiate(Resources.Load("Prefab/PotMoyen"), startPosition, Quaternion.identity, this.transform);
        }
        pot.transform.localScale = new Vector3(0.136f, 0.136f, 0.136f);
        while (
            Math.Abs(pot.transform.position.x - finalPosition.x) > vitesse ||
            Math.Abs(pot.transform.position.y - finalPosition.y) > vitesse ||
            Math.Abs(pot.transform.position.z - finalPosition.z) > vitesse
        )
        {
            yield return new WaitForSeconds(0.1f);
            if (Math.Abs(pot.transform.position.x - finalPosition.x) > vitesse / 2f) { if (pot.transform.position.x - finalPosition.x < 0) pot.transform.Translate(vitesse, 0, 0); else pot.transform.Translate(-vitesse, 0, 0); }
            if (Math.Abs(pot.transform.position.y - finalPosition.y) > vitesse / 2f) { if (pot.transform.position.y - finalPosition.y < 0) pot.transform.Translate(0, vitesse, 0); else pot.transform.Translate(0, -vitesse, 0); }
            if (Math.Abs(pot.transform.position.z - finalPosition.z) > vitesse / 2f) { if (pot.transform.position.z - finalPosition.z < 0) pot.transform.Translate(0, 0, vitesse); else pot.transform.Translate(0, 0, -vitesse); }
        }
        Destroy(pot);
        this.addToPot(mise);
    }

    private IEnumerator moveJetonJoueur(Vector3 startPosition, Vector3 finalPosition, int mise, LogicPerso player)
    {
        GameObject pot;
        float vitesse = 0.2f;
        if (mise < 200)
        {
            pot = (GameObject)Instantiate(Resources.Load("Prefab/PotPetit"), startPosition, Quaternion.identity, this.transform);
        }
        else if (mise > 1000)
        {
            pot = (GameObject)Instantiate(Resources.Load("Prefab/PotGros"), startPosition, Quaternion.identity, this.transform);
        }
        else
        {
            pot = (GameObject)Instantiate(Resources.Load("Prefab/PotMoyen"), startPosition, Quaternion.identity, this.transform);
        }
        pot.transform.localScale = new Vector3(0.136f, 0.136f, 0.136f);
        while (
            Math.Abs(pot.transform.position.x - finalPosition.x) > vitesse ||
            Math.Abs(pot.transform.position.y - finalPosition.y) > vitesse ||
            Math.Abs(pot.transform.position.z - finalPosition.z) > vitesse
        )
        {
            yield return new WaitForSeconds(0.1f);
            if (Math.Abs(pot.transform.position.x - finalPosition.x) > vitesse / 2f) { if (pot.transform.position.x - finalPosition.x < 0) pot.transform.Translate(vitesse, 0, 0); else pot.transform.Translate(-vitesse, 0, 0); }
            if (Math.Abs(pot.transform.position.y - finalPosition.y) > vitesse / 2f) { if (pot.transform.position.y - finalPosition.y < 0) pot.transform.Translate(0, vitesse, 0); else pot.transform.Translate(0, -vitesse, 0); }
            if (Math.Abs(pot.transform.position.z - finalPosition.z) > vitesse / 2f) { if (pot.transform.position.z - finalPosition.z < 0) pot.transform.Translate(0, 0, vitesse); else pot.transform.Translate(0, 0, -vitesse); }
        }
        Destroy(pot);
        player.AddToNbJeton(mise);
    }



    private void addToPot(int value)
    {
        potContent += value;

        _grosTas.enabled = false;
        _moyenTas.enabled = false;
        _petitTas.enabled = false;

        if (potContent < 1000)
        {
            _petitTas.enabled = true;
        } else if(potContent > 4000)
        {
            _grosTas.enabled = true;
        } else
        {
            _moyenTas.enabled = true;
        }
        _textPot.gameObject.SetActive(true);
        _textPot.text = potContent.ToString();
    }

    // GETTERS //
    public LogicCard[] potCards() { return _potCards; }
}
