using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextMise : MonoBehaviour
{
    public Text posJeton;

    void Update()
    {
        Vector3 posMise = Camera.main.WorldToScreenPoint(this.transform.position);
        posJeton.transform.position = posMise;
    }
}
