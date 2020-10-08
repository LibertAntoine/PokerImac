using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Security.Permissions;
using System.Runtime.InteropServices;
using UnityEngine.UI;

public class IA : MonoBehaviour
{
    private LogicPot pot;
    private Transform _textTableau;
    private AleaVar _Alea;
    private float[] Omega = { 133784560f, 0f, 2118760f, 0f, 0f, 1081f, 46f, 1f};

    void Start()
    {
       pot = this.GetComponentInParent<LogicPot>();
       _textTableau = this.transform.parent.parent.Find("TextTableau");
       _Alea = this.transform.parent.GetComponentInChildren<AleaVar>();


        /*
        print("Loi Beta");
        double moyenne = 0;
        double value = 0;
        double center = 0;
      for(int i = 0; i < 100; i++)
        {
            value = _Alea.LoiBeta(3.3, 4.8);
            print(value); 
            moyenne += value;
            
            if (value < 0.5 && value > -0.5) {
                center++;
            }

        }
        
        print("moyenne: " + moyenne / 100.0);
        print("center: " + center);
        */
        /*
        double moyenne = 0;
        double value;
        double center = 0;
        print("Loi Gamma");
        for (int i = 0; i < 100; i++)
        {
            value = _Alea.LoiGamma(0.1);
            moyenne += value;
            if (value < 0.5) center++;
            print("value: " + value);
        }
        print("moyenne: " + moyenne / 100.0);
        print("center" + center);*/
        
    }

    public double CalculEsperances(LogicPerso Joueur, LogicCard[] main, LogicCard[] plateau)
    {
        int[] couleurs = { 0, 0, 0, 0 };
        int[] valeurs = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        int[] quinte = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        int[][] flush = {
            new []{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            new []{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            new []{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            new []{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
        };
        LogicCard[] allCards = { main[0], main[1], plateau[0], plateau[1], plateau[2], plateau[3], plateau[4] };
        int t = 0;
        int v = 0;
        int p = 0;
        int b = 0;
        int c = 0;
        int fMax = -1;
        int qMax = -1;
        int cMax = -1;
        int bMax = -1;
        int pMax = -1;
        int vMax = -1;
        int couMax = -1;
        bool win = false;



        foreach (LogicCard card in allCards)
        {
            if (card == null) break;
            int s = card.signe();

            int co = card.color();
            couleurs[co]++;
            valeurs[s]++;
            t++;
            for (int j = -4; j < 1 && j + s < 10; j++)
            {
                if (j + s >= 0)
                {
                    flush[co][j + s]++;
                    if (flush[co][j + s] == 5) fMax = j + s;
                }
            }
            if (s == 0) {
                flush[co][9]++;
                if (flush[co][9] == 5) fMax = 9;
            }
        }

        for (int i = 0; i < 4; i++)
        {
            if (couleurs[i] == 5) couMax = 1;
        }

        for (int i = 0; i < 13; i++)
        {
            if (valeurs[i] != 0)
            {
                v++;
                vMax = i;
                for (int j = -4; j < 1 && j + i < 10; j++)
                {

                    if (j + i >= 0) {
                        quinte[j + i]++;
                        if (quinte[j + i] == 5) {
                            qMax = i;
                        }
                    }

                }
            }
            if (valeurs[i] == 2)
            {
                p++;
                pMax = i;
            }
            if (valeurs[i] == 3)
            {
                b++;
                bMax = i;
            }
            if (valeurs[i] == 4)
            {
                c++;
                cMax = i;
            }
        }

        // Probabilité quinte flush royale
        float Qr;
        int CardQr = 0;
        if (fMax == 9)
        {
            Qr = 1;
            win = true;
            Joueur.givePoint(180);
        }
        else
        {
            
            for (int couleur = 0; couleur < 4; couleur++)
            {
                if ((5 - flush[couleur][9]) < (7 - t)) CardQr += Binom(47, 7 - (t + (5 - flush[couleur][9])));
            }
            Qr = CardQr / Omega[t];
        }

        // Probabilité quinte flush
        float Qf = 0;
        int CardQf = 0;
        if (fMax > -1 || win == true)
        {
            Qf = 1;
            win = true;
            Joueur.givePoint(160 + fMax);
        }
        else
        {
            
            for (int serie = 0; serie < 9; serie++)
            {
                for (int couleur = 0; couleur < 4; couleur++)
                {
                    int x = 5 - flush[couleur][serie];
                    int r = 7 - t - x;
                    if (r < 0) CardQf += 0;
                    else CardQf += Binom(46, r);
                }
            }
            Qf = CardQf / Omega[t];
        }

        // Probabilités Carrée
        int CardCa = 0;
        float Ca;
        if (cMax > -1 || win == true)
        {
            Ca = 1;
            win = true;
            Joueur.givePoint(140 + cMax);
        }
        else
        {
            if (t - v == 0) CardCa += (13 - t) * Binom(48 - t, 3 - t) + t * Binom(47 - t, 2 - t);
            else CardCa += Binom(49 - v, 4 - v);
            Ca = CardCa / Omega[t];
        }

        // Full
        if ((pMax > -1 && bMax > -1) || win == true)
        {
            win = true;
            Joueur.givePoint(120 + cMax);
        }



        // Probabilité couleur
        float C;
        int CardC = 0;
        if (couMax == 1 || win == true)
        {
            C = 1;
            win = true;
            Joueur.givePoint(100 + vMax);
        }
        else
        {
            for (int couleur = 0; couleur < 4; couleur++)
            {
                int x = 5 - couleurs[couleur];
                int r = 7 - t - x;
                if (r < 0) CardC += 0;
                if (r == 2) CardC += Binom(13 - x, x) * Binom(39, r) + 39 * Binom(13 - x, x + 1) + Binom(13 - x, x - 2);
                if (r == 1) CardC += Binom(13 - x, x) * Binom(39, r) + 39 * Binom(13 - x, x + 1);
                if (r == 0) { CardC += Binom(13 - x, x) * Binom(39, r);}
            }
            CardC -= CardQr + CardQf;
            C = CardC / Omega[t];
        }

        // Probabilité quinte
        float Q;
        int CardQ = 0;
        int CardN = 0;
        int CardP = 0;
        int CardDp = 0;
        int CardB = 0;
        int Cn = 0;
        int Cp1 = 0;
        int Cp2 = 0;
        int Cdp1 = 0;
        int Cdp2 = 0;
        int Cdp3 = 0;
        int Cb1 = 0;
        int Cb2 = 0;
        int Cp3 = 0;
        int Cdp4 = 0;
        int Cdp5 = 0;
        int Cb3 = 0;
        int Cdp6 = 0;
        int Cb4 = 0;

        if (qMax > -1 || win == true)
        {
            Q = 1;
            win = true;
            Joueur.givePoint(80 + qMax);
        }
        else
        {
            for (int serie = 0; serie < 10; serie++)
            {
                int a = 6;
                if (serie == 9) a = 5;
                int x = 5 - quinte[serie];
                int r = 7 - t - x;

                if (r < 0) CardQ += 0;
                else
                {
                    // Cas 0
                    if (t - v == 0)
                    {
                        // Calcul des #C

                        for (int couleur = 0; couleur < 4; couleur++)
                        {
                            int y = 5 - couleurs[couleur];
                            if (y <= 7 - t)
                            {
                                Cn += (int) (Binom(7 - t, y + 2) + Binom(6 - t, y + 1) * Math.Pow(3, 6 - t - y) + Binom(7 - t, y) * Math.Pow(3, 7 - t - y));
                                Cp1 += (int)((5 - y) * Binom(7 - t, y) + (t - 5 + y) * (Binom(7 - t, y) * Math.Pow(3, 6 - t - y) + Binom(7 - t, y + 1)));
                                Cp2 += Binom(5 - t, y);
                                Cdp1 += Binom(5 - t, y);
                                Cdp2 += Binom(5 - t, y);
                                Cdp3 += Binom(4 - t, y);
                                Cb1 += (t == 5 - y) ? 1 : 0;
                                Cb2 += (t == 5 - y) ? 1 : 0;
                            }
                        }

                        //Calcul des #Q
                        CardN += (int)((Math.Pow(4, 7 - t) - Cn) * Binom(13 - a, r));
                        CardP += (int)((3 * v * (Math.Pow(4, 6 - t) - Cp1) + 6 * (6 - v)*(Math.Pow(4, 5 - t) - Cp2)) * Binom(13 - a, 6 - x - v));
                        CardDp += (int)(Binom(5 - x, 2) * 9 * (Math.Pow(4, 5 - t) - Cdp1) + Binom(x, 2) * 36 * (Math.Pow(4, 3 - t) - Cdp2) + 3 * (5 - x) * 6 * x * (Math.Pow(4, 4 - t) - Cdp3));
                        CardB += (int)(3 * v * (Math.Pow(4, 5 - t) - Cb1) + 4 * x * (Math.Pow(4, 4 - t) - Cb2));


                    } // Cas 1
                    else if (t - v == 1)
                    {
                        // Calcul des #C
                        for (int couleur = 0; couleur < 4; couleur++)
                        {
                            int y = 5 - couleurs[couleur];
                            if (y <= 7 - t)
                            {
                                Cp3 += (int)(Binom(6 - t, y + 1) * Math.Pow(3, 6 - t - y) + Binom(7 - t, y) * Math.Pow(3, 7 - t - y));
                                Cdp4 += Binom(6 - t, y);
                                Cdp5 += Binom(5 - t, y);
                                Cb3 += (t == 5 - y) ? 1 : 0;
                            }
                        }

                        //Calcul des #Q
                        CardP += (int)((Math.Pow(4, 7 - t) - Cp3) * Binom(13 - a, 6 - x - v));
                        CardDp += (int)((4 - x) * 3 * (Math.Pow(4, 6 - t) - Cdp4) + x * 6 * (Math.Pow(4, 5 - t) - Cdp5));
                        CardB += (int)(2 * (Math.Pow(4, 6 - t) - Cb3));

                    } // Cas 2
                    else if (t - v == 2)
                    {
                        // Calcul des #C
                        for (int couleur = 0; couleur < 4; couleur++)
                        {
                            int y = 5 - couleurs[couleur];
                            if (y <= 7 - t)
                            {
                                if (b == 0) Cdp6 += Binom(7 - t, y);
                                if (b == 1) Cb4 += 1;
                            }
                        }

                        //Calcul des #Q
                        if (b == 0) CardDp += (int)((Math.Pow(4, 7 - t) - Cdp6));
                        if (b == 1) CardB += (int)((Math.Pow(4, 7 - t) - Cb4));
                    }
                    
                }
            }
            CardQ = CardN + CardP + CardDp + CardB;
            Q = CardQ / Omega[t];
        }

        // Probabilités Brelan
        int CardBr = 0;
        float Br;
        if (bMax > -1 || win == true)
        {
            Br = 1;
            win = true;
            Joueur.givePoint(60 + bMax);
        }
        else
        {
            if (t - v == 0) { CardBr += (int)(3 * t * Math.Pow(4, 5 - v) * Binom(13 - v, 5 - v) + 4 * (5 - t) * Math.Pow(4, 4 - v) * Binom(13 - v, 4 - v));};
            if (t - v == 1) { CardBr += (int)(2 * Math.Pow(4, 5 - v) * Binom(13 - v, 5 - v));};
            //if (t - v == 2 && b == 1) CardBr += (int)(Math.Pow(4, 5 - v) * Binom(13 - v, 5 - v));
            Br = CardBr / Omega[t];
        }


        // Probabilités Double Paire
        int CardDpa = 0;
        float Dpa;
        if (p > 1 || win == true)
        {
            Dpa = 1;
            win = true;
            Joueur.givePoint(40 + pMax);
        }
        else
        {

            if (p == 0) { CardDpa += (int)(2 * 3 * t * Math.Pow(4, 5 - v) * Binom(13 - v, 5 - v) + 2 * 6 * (5 - t) * Math.Pow(4, 3 - v) * Binom(13 - v, 3 - v) + 6 * (5 - t) * 3 * t * Math.Pow(4, 4 - v) * Binom(13 - v, 5 - v)); }
            if (p == 1) { CardDpa += (int)(3 * t * Math.Pow(4, 5 - v) * Binom(13 - v, 5 - v) + 6 * (5 - t) * Math.Pow(4, 3 - v) * Binom(13 - v, 3 - v)); }

            //if (t - v == 2 && b == 0) CardDpa +=;
            //if (t - v == 3 && b == 0) CardBr += Math.Pow(4, 5 - v) * Binom(13 - v, 5 - v);
            Dpa = CardDpa / Omega[t];
        }


        // Probabilités Paire
        int CardPa = 0;
        float Pa;
        if (pMax > -1 || win == true)
        {
            Pa = 1;
            win = true;
            Joueur.givePoint(20 + pMax);
        }
        else
        {
            if (t - v == 0) { CardPa += (int)((3 * v * (Math.Pow(4, 6 - t) - Cp1) + (6 - v) * 6 * (Math.Pow(4, 5 - t) - Cp2)) * Binom(13 - v, 6 - v)); }
            if (t - v == 1) { CardPa += (int)((3 * v * (Math.Pow(4, 6 - t) - Cp1) + (6 - v) * 6 * (Math.Pow(4, 5 - t) - Cp2)) * Binom(13 - v, 6 - v)); }
            //if (t - v == 1) { CardPa += (int)(Binom(13, 6 - v) * (Math.Pow(4, 7 - t) - Cp3) - CardP);}

            Pa = CardPa / Omega[t];
        }

        


        if (win == false)
        {
            Joueur.givePoint(vMax);
        }

        if (Joueur.isReel())
        {
            _textTableau.Find("TextTab_1").gameObject.GetComponent<Text>().text = $"{Qr*100:0.0000}%";        
            _textTableau.Find("TextTab_2").gameObject.GetComponent<Text>().text = $"{Qf * 100:0.0000}%";
            _textTableau.Find("TextTab_3").gameObject.GetComponent<Text>().text = $"{Ca * 100:0.0000}%";
            _textTableau.Find("TextTab_4").gameObject.GetComponent<Text>().text = $"{C * 100:0.0000}%";
            _textTableau.Find("TextTab_5").gameObject.GetComponent<Text>().text = $"{Q * 100:0.0000}%";
            _textTableau.Find("TextTab_6").gameObject.GetComponent<Text>().text = $"{Br * 100:0.0000}%";
            _textTableau.Find("TextTab_7").gameObject.GetComponent<Text>().text = $"{Dpa * 100:0.0000}%";
            _textTableau.Find("TextTab_8").gameObject.GetComponent<Text>().text = $"{Pa * 100:0.0000}%";
        }


        /*
        print("stat");

        print("Quinte Flush Royale" + Qr *100 + "%");
        print("Quinte Flush" + Qf * 100 + "%");
        print("Carrée" + Ca * 100 + "%");
        print("Couleur" + C * 100 + "%");
        print("Quinte" + Q * 100 + "%");
        print("Brelan" + Br * 100 + "%");
        print("DoublePaire" + Dpa * 100 + "%");
        print("Paire" + Pa * 100 + "%");
        print("endStat");
        */

        //return (Pa+Dpa*2+Br*3+Q*4+C*5+Ca*6+Qf*7+Qr*8)/36.0;
        return (Pa + Dpa + Br + Q + C + Ca + Qf + Qr) / 8.0;
    }

    public int Binom(int a, int b) 
    {
        if (b == 0 || a == b) return 1;
        if (a < b || a < 1 || b < 0) return 0;
        return (int) (Fact(a) / (Fact(b) * Fact(a - b)));
    }

    public double Fact(double nb)
    {
        return (nb > 1) ? nb * this.Fact(nb - 1) : nb;
    }

    public int Choisir(LogicPerso player)
    {
        return 0;
    }
}
