using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.InteropServices;

public class LogicPioche : MonoBehaviour
{
  
    private List<int> piocheCards;
    private AudioSource distrib;
    private AudioSource carte;
    
    // Start is called before the first frame update
    void Start()
    {
        this.Melanger();
        distrib = this.GetComponent<AudioSource>();
        carte = this.transform.Find("PiocheGrosse").gameObject.GetComponent<AudioSource>();
    }

    public void Melanger()
    {
        piocheCards = new List<int>();
        for (int i = 1; i <= 52; i++) piocheCards.Add(i);
    }

    public IEnumerator Distribuer(LogicPerso player, LogicGame jeu) 
    {
        LogicCard newCard;
        distrib.Play(3);
        //print("distrib");
        while (player.nbCard() != 2)
        {
            
            yield return new WaitForSeconds(0.1f);
            newCard = this.Piocher(player.nextCardPosition(), player.nextCardRotation(), 0.2f, player.gameObject, player.isReel(), new Vector3(9,9,1));
            player.setCard(newCard);
            player = player.next();
        };
        IEnumerator Play = jeu.PlayJoueurs();
        StartCoroutine(Play);
    }

    public LogicCard Piocher(Vector3 finalPosition, Quaternion rotation, float vitesse, GameObject parent, bool isVisible, Vector3 size)
    {
       int value = piocheCards[UnityEngine.Random.Range(1, piocheCards.Count)];
        carte.Play();
       piocheCards.Remove(value);
       LogicCard newCard = ((GameObject) Instantiate(Resources.Load("Prefab/Cards/Card_" + value.ToString()), this.gameObject.transform.position, Quaternion.identity, parent.transform)).GetComponent<LogicCard>();
       newCard.Start();
       newCard.gameObject.transform.localScale = size;
       newCard.setVisible(isVisible);
       newCard.setValue(value);
       IEnumerator moveCard = MoveCard(newCard, vitesse, finalPosition, rotation);
       StartCoroutine(moveCard);
       return newCard;
    }

    public void Bruler()
    {

    }

    public IEnumerator MoveCard(LogicCard card, float vitesse, Vector4 finalPosition, Quaternion rotation)
    {
        Transform TransformCard = card.gameObject.transform;
        while (
            Math.Abs(TransformCard.position.x - finalPosition.x) > vitesse ||
            Math.Abs(TransformCard.position.y - finalPosition.y) > vitesse ||
            Math.Abs(TransformCard.position.z - finalPosition.z) > vitesse
        )
        {
            yield return new WaitForSeconds(0.1f);
            if (Math.Abs(TransformCard.position.x - finalPosition.x) > vitesse / 2f) { if (TransformCard.position.x - finalPosition.x < 0) TransformCard.Translate(vitesse, 0, 0); else TransformCard.Translate(-vitesse, 0, 0); }
            if (Math.Abs(TransformCard.position.y - finalPosition.y) > vitesse / 2f) { if (TransformCard.position.y - finalPosition.y < 0) TransformCard.Translate(0, vitesse, 0); else TransformCard.Translate(0, -vitesse, 0); }
            if (Math.Abs(TransformCard.position.z - finalPosition.z) > vitesse / 2f) { if (TransformCard.position.z - finalPosition.z < 0) TransformCard.Translate(0, 0, vitesse); else TransformCard.Translate(0, 0, -vitesse); }           
        }
        TransformCard.position = finalPosition;
        TransformCard.Rotate(rotation.eulerAngles);

    } 


}
