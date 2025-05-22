using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeAlpha : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        

        Invoke("cc", 1.0f);
    }
    void cc()
    {
        var img = GetComponent<Image>();
        img.color = new Color(img.color.r, img.color.g, img.color.b, 0.5f);
    }

}
