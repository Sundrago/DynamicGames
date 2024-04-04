using Febucci.UI;
using UnityEngine;

namespace DynamicGames.Pet
{
    /// <summary>
    ///     Manages the introduction sequence for a new pet animation.
    /// </summary>
    public class NewPetAnimIntroSequence : MonoBehaviour
    {
        [SerializeField] private TypewriterByCharacter introTypewriter;

        public void Init(string welcomeString)
        {
            gameObject.SetActive(true);
            introTypewriter.ShowText(welcomeString);
            introTypewriter.StartShowingText();
        }
    }
}