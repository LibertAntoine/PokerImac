using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using MathNet.Numerics.Integration;

public class LogicGame : MonoBehaviour
{
    private int nbJoueur;

    private int nbJoueurManche;
    private int LastRelance = -1;
    private int _currentRelance = 0;

    private int PointWinner = 0;
    private ArrayList Classement;

    private int currentTour;
    private LogicPerso currentPlayer;
    private LogicPerso reelPlayer;
    public GameObject persoPrefab;
    private bool game = true;
    private bool _tourReelPlayer = false;


    private int minMise = 10;

    private LogicPot pot;
    private LogicPioche pioche;

    private Vector4[][] placeJoueur =
    {
        new []{ new Vector4(-3.02f,-0.09f,0f,89.6f)},
        new []{ new Vector4(-3f,-0.11f,0f,89.6f), new Vector4(2.9f,0.31f,0f,-84.74f)},
        new []{ new Vector4(-2.83f,-0.91f,0f,109.29f), new Vector4(-2f,1.6f,0f,21.3f), new Vector4(2.98f,-0.08f,0f,-90.68f)},
        new []{ new Vector4(-2.53f,-1.3f,0f,119.8f), new Vector4(-2.64f,1.38f,0f,39.04f), new Vector4(2.16f,1.66f,0f,-37.22f), new Vector4(2.51f,-1.31f,0f,-135.83f)},
        new []{ new Vector4(-2.49f,-1.22f,0f,128.03f), new Vector4(-2.62f,1.33f,0f,42.01f), new Vector4(1.56f,1.85f,0f,-13.89f), new Vector4(2.96f,0.25f,0f,-86.26f), new Vector4(2f,-1.69f,0f,-157.89f)},
    };

    private Vector4[] placeCards =
    {
            new Vector4(-0.74f,0.13f,0f,0f), new Vector4(-0.37f,0.13f,0f,0f), new Vector4(0f,0.13f,0f,0f), new Vector4(0.37f,0.13f,0f,0f), new Vector4(0.74f,0.13f,0f,0f)
    };

    // Start is called before the first frame update
    void Start()
    {
        pot = this.GetComponentInChildren<LogicPot>();
        pioche = this.GetComponentInChildren<LogicPioche>();

        nbJoueur = PlayerPrefs.GetInt("NbJoueur");
        //PlayerPrefs.GetInt("Bluff");
        //PlayerPrefs.GetInt("Decision");

        currentTour = 1;
        
        StartCoroutine("CreatePersonnage"); 
    }

    private IEnumerator CreatePersonnage()
    {
        
        GameObject parent = this.transform.Find("Personnages").gameObject;
        Vector4 v4;

        reelPlayer = ((GameObject) Instantiate(Resources.Load("Prefab/Perso" + Random.Range(1, 6)), new Vector3(0.17f, -1.91f, 0f), Quaternion.identity, parent.transform)).GetComponent<LogicPerso>();
        reelPlayer.gameObject.transform.Rotate(0, 0, 180);
        reelPlayer.SetReel(true);
        currentPlayer = reelPlayer;

        for (int i = 0; i < nbJoueur - 1; i++)
        {
            yield return new WaitForSeconds(0.3f);
            v4 = placeJoueur[nbJoueur - 2][i];
            
            LogicPerso perso = ((GameObject)Instantiate(Resources.Load("Prefab/Perso" + Random.Range(1, 7)), new Vector3(v4.x, v4.y, v4.z), Quaternion.identity, parent.transform)).GetComponent<LogicPerso>();
            perso.gameObject.transform.Rotate(0, 0, v4.w);
            currentPlayer.setNext(perso);
            perso.setPrevious(currentPlayer);
            currentPlayer = perso;
        }
        currentPlayer.setNext(reelPlayer);
        reelPlayer.setPrevious(currentPlayer);

        int nb = Random.Range(1, nbJoueur); // Select first player;
        for (int i = 1; i < nb; i++)
        {
            currentPlayer.next();
        }
        currentPlayer.SetBlind(true);

        StartCoroutine("PlayTour");
    }

    public IEnumerator PlayTour()
    {
        yield return new WaitForSeconds(1f);
        switch (currentTour)
        {
            case 1:
                yield return new WaitForSeconds(1f);
                this.InitManche();
                break;
            case 2:
                yield return new WaitForSeconds(1f);
                pot.addCard(pioche.Piocher(placeCards[0],Quaternion.identity, 0.05f, pot.gameObject, true, new Vector3(1.5f, 1.5f, 1)));
                yield return new WaitForSeconds(1f);
                pot.addCard(pioche.Piocher(placeCards[1], Quaternion.identity, 0.05f, pot.gameObject, true, new Vector3(1.5f, 1.5f, 1)));
                yield return new WaitForSeconds(1f);
                pot.addCard(pioche.Piocher(placeCards[2], Quaternion.identity, 0.05f, pot.gameObject, true, new Vector3(1.5f, 1.5f, 1)));
                yield return new WaitForSeconds(1f);
                this.FindBlind();
                StartCoroutine("PlayJoueurs");
                break;
            case 3:
                yield return new WaitForSeconds(1f);
                pot.addCard(pioche.Piocher(placeCards[3], Quaternion.identity, 0.05f, pot.gameObject, true, new Vector3(1.5f, 1.5f, 1)));
                yield return new WaitForSeconds(1f);
                this.FindBlind();
                StartCoroutine("PlayJoueurs");
                break;
            case 4:
                yield return new WaitForSeconds(1f);
                pot.addCard(pioche.Piocher(placeCards[4], Quaternion.identity, 0.05f, pot.gameObject, true, new Vector3(1.5f, 1.5f, 1)));
                yield return new WaitForSeconds(1f);
                this.FindBlind();
                StartCoroutine("PlayJoueurs");
                break;
            case 5:
                StartCoroutine("FinishManche");
                break;
            default:
                break;
        }
    }

    private void FindBlind()
    {
        while (!currentPlayer.haveBlind())
            currentPlayer = currentPlayer.next();
    }

    private void MoveBlind()
    {
        if (!currentPlayer.haveBlind())
            this.FindBlind();
        currentPlayer.SetBlind(false);
        currentPlayer = currentPlayer.previous();
        currentPlayer.SetBlind(true);
    }

    private void InitManche()
    {
        MoveBlind();
        nbJoueurManche = nbJoueur;
        LastRelance = 0;
        pioche.Melanger();
        IEnumerator Distrib = pioche.GetComponent<LogicPioche>().Distribuer(currentPlayer, this);
        StartCoroutine(Distrib);
        currentPlayer.Relancer(minMise);
        currentPlayer = currentPlayer.next();
        currentPlayer.Relancer(minMise*2);
        currentPlayer = currentPlayer.next();
    }

    public IEnumerator PlayJoueurs()
    {
        while (LastRelance +1 != nbJoueurManche)
        {
            if (!currentPlayer.isCoucher())
            {
                if (!currentPlayer.isReel())
                {
                    yield return new WaitForSeconds(Random.Range(2, 4));
                }
                currentPlayer.Jouer();
                while (_tourReelPlayer)
                {
                    yield return new WaitForSeconds(1f);
                }
            }
            currentPlayer = currentPlayer.next();
        } 
        currentTour++;
        this.FindBlind();
        IEnumerator Ramasse = pot.ramasseMise(currentPlayer, nbJoueurManche);
        StartCoroutine(Ramasse);
    }

    private IEnumerator FinishManche()
    {
        // TODO : ne prend pas en compte les tapis.
        int maxPoint = 0;
        int nbGagnant = 1;
        this.FindBlind();
        do
        {
            if (currentPlayer.pointManche() == maxPoint) { nbGagnant++; }
            if (currentPlayer.pointManche() > maxPoint) {maxPoint = currentPlayer.pointManche(); nbGagnant = 1;}
            currentPlayer.Devoile();
            currentPlayer = currentPlayer.next();
        } while (!currentPlayer.haveBlind());
        pot.finishManche(maxPoint, nbGagnant);
        currentTour = 1;
        if (nbJoueur == 1)
            this.WinGame();
        yield return new WaitForSeconds(5f);
        pot.DestroyPotCards();
        do
        {
            currentPlayer.finishManche();
            currentPlayer = currentPlayer.next();
        } while (!currentPlayer.haveBlind());
        StartCoroutine("PlayTour");
    }

    public void tourReelPlayer(bool value)
    {
        _tourReelPlayer = value;
    }

    public void LoseGame()
    {
        SceneManager.LoadScene(4);
    }

    public void WinGame()
    {
        SceneManager.LoadScene(3);
    }

    public void DeleteJoueur(LogicPerso player)
    {
        player.next().setPrevious(player.previous());
        player.previous().setNext(player.next());
        Destroy(player);
    }

    // GETTERS //
    public int CurrentRelance() { return _currentRelance; }
    public int CurrentTour() { return currentTour; }
    public LogicPerso CurrentPlayer() { return currentPlayer; }

    // SETTERS //
    public void removeJoueurManche() {nbJoueurManche--;}
    public void incrLastRelance(){ LastRelance++; }
    public void videCurrentRelance()
    {
        _currentRelance = 0;
        LastRelance = -1;
    }
    public void nextPlayer()
    {
        currentPlayer = currentPlayer.next();
    }

    public void SetCurrentRelance(int value)
    {
        _currentRelance = value;
        LastRelance = 0;
    }

    
}
