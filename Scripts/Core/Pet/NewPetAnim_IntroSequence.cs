using System.Collections;
using System.Collections.Generic;
using Febucci.UI;
using UnityEngine;

namespace Core.Pet
{
    public class NewPetAnim_IntroSequence : MonoBehaviour
    {
        [SerializeField] private TypewriterByCharacter intro_ui;

        public void Init(string welcomeString)
        {
            gameObject.SetActive(true);

            intro_ui.ShowText(welcomeString);
            intro_ui.StartShowingText();
        }
    }
}