using Febucci.UI;
using UnityEngine;
using UnityEngine.Serialization;

namespace Core.Pet
{
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