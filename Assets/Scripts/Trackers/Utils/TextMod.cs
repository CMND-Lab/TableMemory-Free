using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]

public class TextMod : MonoBehaviour
{

    char leftArrow = '\u21E6';
    char rightArrow = '\u21E8';
    public TextMesh distractorStimuliText;


   

    void Awake()
    {
        distractorStimuliText = gameObject.GetComponent<TextMesh>();
        distractorStimuliText.text = leftArrow.ToString();
    }

}
