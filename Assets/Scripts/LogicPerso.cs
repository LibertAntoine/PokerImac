using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class LogicPerso : MonoBehaviour
{
    private IA IA; 

    private double _coeffBluff;
    private int _nbJeton = 0;
    private bool _haveBlind;
    private bool _isReel;
    private bool _isCoucher = false;
    private LogicCard[] _cartes;
    private LogicGame jeu;
    private LogicPot pot;
    private int _point;
    private AleaVar _Alea;

    private MenuPlay _menu;

    private int _currentMise = 0;
    private int _currentMiseManche = 0;
    private int point = 0;

    private SpriteRenderer _grossePile;
    private SpriteRenderer _moyenPile;
    private SpriteRenderer _petitePile;
    private SpriteRenderer _grosTas;
    private SpriteRenderer _moyenTas;
    private SpriteRenderer _petitTas;
    private SpriteRenderer _deal;
    private Text _textMise;
    private Text _textJeton;
    private SpriteRenderer _PersoBras;
    private SpriteRenderer _PersoMise;
    private AudioSource jeton;
    private AudioSource toc;
    private AudioSource couche;


    LogicPerso _nextPlayer;
    LogicPerso _previousPlayer;

    // Start is called before the first frame update
    void Start()
    {
        jeu = this.GetComponentInParent<LogicGame>();
        IA = this.gameObject.transform.parent.GetComponentInChildren<IA>();
        pot = jeu.gameObject.GetComponentInChildren<LogicPot>();
        _menu = jeu.gameObject.transform.Find("Menu").GetComponent<MenuPlay>();

        jeton = this.transform.parent.gameObject.GetComponent<AudioSource>();
        toc = IA.gameObject.GetComponent<AudioSource>();
        couche = jeu.gameObject.GetComponent<AudioSource>();

        this.Initcartes();

        _grossePile = this.transform.Find("JetonPile_1").GetComponent<SpriteRenderer>();
        _moyenPile = this.transform.Find("JetonPile_2").GetComponent<SpriteRenderer>();
        _petitePile = this.transform.Find("JetonPile_3").GetComponent<SpriteRenderer>();
        _grosTas = this.transform.Find("JetonTas_1").GetComponent<SpriteRenderer>();
        _moyenTas = this.transform.Find("JetonTas_3").GetComponent<SpriteRenderer>();
        _petitTas = this.transform.Find("JetonTas_4").GetComponent<SpriteRenderer>();
        _PersoBras = this.transform.Find("PersoBras").GetComponent<SpriteRenderer>();
        _PersoMise = this.transform.Find("PersoMise").GetComponent<SpriteRenderer>();
        _deal = this.transform.Find("deal").GetComponent<SpriteRenderer>();
        _textMise = this.transform.Find("Canvas").Find("TextMise").GetComponent<Text>();
        _textJeton = this.transform.Find("Canvas").Find("TextJeton").GetComponent<Text>();
        _Alea = this.transform.parent.GetComponentInChildren<AleaVar>();

        _textMise.gameObject.SetActive(false);

       


        _grossePile.enabled = false;
        _moyenPile.enabled = false;
        _petitePile.enabled = false;
        _grosTas.enabled = false;
        _moyenTas.enabled = false;
        _petitTas.enabled = false;
        _deal.enabled = false;
        _PersoBras.enabled = false;
        _PersoMise.enabled = false;

        this.AddToNbJeton(3000);

        double bluff = PlayerPrefs.GetInt("Bluff");
        //print("coeff" + bluff);
        _coeffBluff = _Alea.LoiNormale(bluff / 100.0, 0.1);
        if (_coeffBluff < 0) _coeffBluff = 0;
        if (_coeffBluff > 1) _coeffBluff = 1;
    }

    private void Initcartes()
    {

    }

    public void Jouer()
    {
        if (_isReel)
            this.Reeljouer();
        else
            this.VirtualJouer();
    }

    void Reeljouer()
    {
        if(!_isCoucher)
        {
            double esperance = IA.CalculEsperances(this, this._cartes, pot.potCards());
            //print(esperance);
            //print("point" + _point);
            _menu.gameObject.SetActive(true);
            _menu.Launch(this);
        } 
    }

    void VirtualJouer()
    {
        if (!_isCoucher)
        {
            double esperance = IA.CalculEsperances(this, this._cartes, pot.potCards());

            int IAchoix = 0;
            bool bluffDesc = false;
            if (jeu.CurrentTour() != 1)
            {
                double bluffDecision = _Alea.LoiBeta(1.3 + 2 * _coeffBluff, 4.8);
                //print("bluff" + bluffDecision);
                if (bluffDecision > 0.5 && esperance < 0.5) esperance += 0.3;
                if (bluffDecision > 0.5 && esperance > 0.5) {esperance -= 0.3; bluffDesc = true;}
            }

            if (jeu.CurrentTour() != 1 && !bluffDesc)
            {
                double coucheDecision = _Alea.LoiBeta(4.3, 5.6 - esperance * 5);
                if (coucheDecision < 0.4) IAchoix = -1;
            }

            if (IAchoix == 0 && jeu.CurrentTour() != 1)
            {

                double aleaDecision = PlayerPrefs.GetInt("Decision");
                double newEsperance = _Alea.LoiNormale(esperance, aleaDecision / 1000.0);
                if (newEsperance < 0) newEsperance = 0;
                if (newEsperance > 1) newEsperance = 1;
                //print("newEsperance" + newEsperance);
                double pari = _Alea.LoiGamma(newEsperance);
                if (pari > 1) pari = 1;
                //print("pari" + pari);
                int misePotentiel = (int) Math.Floor((_currentMiseManche + _nbJeton) * pari /5);
                //print("miseManche" + _currentMiseManche);
                //print("misePotentiel" + misePotentiel);
                if (jeu.CurrentRelance() + _currentMiseManche < misePotentiel) IAchoix = misePotentiel - _currentMiseManche;
            }


                //print(esperance);
                //print("point" + _point);
                //int IAchoix = IA.Choisir(this);
            switch (IAchoix)
            {
                case -1: // Se coucher
                    this.Coucher();
                    
                    break;
                case 0: // Checker / Suivre
                    if (jeu.CurrentRelance() == 0)
                    {
                        //print("check");
                        this.Check();
                    }
                    else
                    {
                        //print("suivre");
                        this.Suivre();
                    }
                    break;
                default: // Miser / Relancer
                    this.Relancer(IAchoix);
                    break;
            }
        }
    }

    public void Suivre()
    {
        this.AddtoCurrentMise(jeu.CurrentRelance() - _currentMise);
        jeu.incrLastRelance();
    }

    public void Relancer(int relance)
    {
        this.AddtoCurrentMise(relance);
        jeu.SetCurrentRelance(_currentMise);
    }

    public void Check()
    {
        toc.Play();
        jeu.incrLastRelance();
    }

    public void Coucher()
    {
        couche.Play();
        _isCoucher = true;
        Destroy(_cartes[0].gameObject);
        Destroy(_cartes[1].gameObject);
        _point = 0;
        _cartes = null;
        jeu.removeJoueurManche();
    }
    

    public void finishManche()
    {
        if (_nbJeton == 0)
        {
            if (_isReel == true)
                jeu.LoseGame();
            else
                jeu.DeleteJoueur(this);
        }
        if(_isCoucher == false)
        {
            Destroy(_cartes[0].gameObject);
            Destroy(_cartes[1].gameObject);
        }
        _isCoucher = false;
        _point = 0;
        _cartes = null;
        _currentMiseManche = 0;
    }

    public void Devoile()
    {
        if (!this.isCoucher())
        {
            _cartes[0].setVisible(true);
            _cartes[1].setVisible(true);
        }
    }


    private void AddtoCurrentMise(int value)
    {
        jeton.Play();
        StartCoroutine("animMise");
        _textMise.gameObject.SetActive(true);

        this.AddToNbJeton(-value);
        _currentMise += value;
        _currentMiseManche += value;

        _grosTas.enabled = false;
        _moyenTas.enabled = false;
        _petitTas.enabled = false;

        if (_currentMise < 200)
        {
            _petitTas.enabled = true;
        } else if (_currentMise > 1000)
        {
            _grosTas.enabled = true;
        } else
        {
            _moyenTas.enabled = true;
        }
        _textMise.text = _currentMise.ToString();
    }

    public int rammasseCurrentMise()
    {
        
        _grosTas.enabled = false;
        _moyenTas.enabled = false;
        _petitTas.enabled = false;

        int ramasse = _currentMise;
        _currentMise = 0;
        _textMise.gameObject.SetActive(false);
        return ramasse;
    }

    public IEnumerator animMise()
    {
        _PersoBras.enabled = true;
        _PersoMise.enabled = true;

        yield return new WaitForSeconds(1f);

        _PersoBras.enabled = false;
        _PersoMise.enabled = false;
    }


    public void AddToNbJeton(int value)
    {
        
        _nbJeton += value;
        _grossePile.enabled = false;
        _moyenPile.enabled = false;
        _petitePile.enabled = false;


        if (_currentMise < 1000)
        {
            _petitePile.enabled = true;
        }
        else if (_currentMise > 4000)
        {
            _grossePile.enabled = true;
        }
        else
        {
            _moyenPile.enabled = true;
        }
        _textJeton.text = _nbJeton.ToString();
    }

    public int nbJeton()
    {
        return _nbJeton;
    }

    // SETTERS //
    public void setCard(LogicCard newCard)
    {

        if (_cartes == null)
        {
            LogicCard[] cartes = { newCard };
            _cartes = cartes;
        }
        else
        {
            LogicCard[] cartes = { _cartes[0], newCard };
            _cartes = cartes;
        }
    }

    public void SetReel(bool isReel)
   {
        _isReel = isReel;
   }

    public void SetBlind(bool haveBlind)
    {
        if (haveBlind)
        {
            _previousPlayer.SetDeal(true);
        } else
        {
            _previousPlayer.SetDeal(false);
        }
        _haveBlind = haveBlind;
    }


    public void SetDeal(bool deal)
    {
        _deal.enabled = deal;
    }

    public void SetCartes(LogicCard[] cartes)
    {
        _cartes = cartes;
    }

    public void setNext(LogicPerso nextPlayer)
    {
        _nextPlayer = nextPlayer;
    }

    public void setPrevious(LogicPerso previousPlayer)
    {
         _previousPlayer = previousPlayer;
    }

    public void SetCoucher(bool isCoucher)
    {
        _isCoucher = isCoucher;
    }

    public void videMise()
    {
        _currentMise = 0;
    }

    public void givePoint(int value)
    {
        if (_point < value) _point = value;
    }

    public void resetPoint()
    {
       _point = 0;
    }

    // GETTERS //
    public LogicPerso next()
    {
        return _nextPlayer;
    }

    public LogicPerso previous()
    {
        return _previousPlayer;
    }

    public bool haveBlind()
    {
        return _haveBlind;
    }

    public bool isReel()
    {
        return _isReel;
    }

    public int nbCard()
    {
        if(_cartes == null)
        {
            return 0;
        } else
        {
            return _cartes.Length;
        }
 
    }

    public int currentMise()
    {
        return _currentMise;
    }

    public bool isCoucher()
    {
        return _isCoucher;
    }

    public Vector3 misePosition()
    {
        return _grosTas.gameObject.transform.position;
    }

    public Vector3 jetonPosition()
    {
        return _grossePile.gameObject.transform.position;
    }

    public int pointManche()
    {
        return _point;
    }



    public Vector3 nextCardPosition()
    {
        Vector3 position = new Vector3(0f,0f,0f);
        float rotation = 0f;
        if (_cartes == null)
        {
            position = this.transform.Find("card1").transform.position;
        } else
        {
            position = this.transform.Find("card2").transform.position;
        }
        if (rotation < 0) rotation += 360;
        return position;
    }

    public Quaternion nextCardRotation()
    {
        return this.gameObject.transform.rotation;
    }
}
