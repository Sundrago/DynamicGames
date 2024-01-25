using System.Collections;
using System.Collections.Generic;
using Febucci.UI;
using UnityEngine;

public class NewPetAnim_1Intro : MonoBehaviour
{
    [SerializeField] private TypewriterByCharacter intro_ui;
    public void Init(string welcomeString)
    {
        gameObject.SetActive(true);
        
        intro_ui.ShowText(welcomeString);
        intro_ui.StartShowingText();
    }
}
