using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogicCard : MonoBehaviour
{

    private int _value;
    private int _color;
    private int _signe;
    private SpriteRenderer Face;
    private SpriteRenderer Dos;

    // Start is called before the first frame update
    public void Start()
    {
        Face =  this.transform.Find("Face").GetComponent<SpriteRenderer>();
        Dos = this.transform.Find("Dos").GetComponent<SpriteRenderer>();
    }

    public void setVisible(bool isVisible)
    {
        if (isVisible == true)
        {
            if (Face.enabled == false) Face.enabled = true;
            if (Dos.enabled == true) Dos.enabled = false;
        } else
        {
            if (Face.enabled == true) Face.enabled = false;
            if (Dos.enabled == false) Dos.enabled = true;
        }
    }

    public void setValue(int value)
    {
        _value = value;
        _color = (value - 1) / 13;
        _signe = (value - 1) % 13;
    }

    public int signe()
    {
        return _signe;
    }

    public int color()
    {
        return _color;
    }
}
