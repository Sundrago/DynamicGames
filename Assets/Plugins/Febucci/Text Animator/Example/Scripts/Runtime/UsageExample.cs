using UnityEngine;
using UnityEngine.Assertions;

namespace Febucci.UI.Examples
{
    //Prevents this example to show up in the inspector, since it should be used only in the example scene (and so, not annoy you after you understand how this works)
    [AddComponentMenu("")]
    public class UsageExample : MonoBehaviour
    {
        public TypewriterByCharacter textAnimatorPlayer;

        [TextArea(3, 50)] [SerializeField] private string textToShow = " ";

        private void Awake()
        {
            Assert.IsNotNull(textAnimatorPlayer, $"Text Animator Player component is null in {gameObject.name}");
        }

        private void Start()
        {
            ShowText();
        }

        public void ShowText()
        {
            textAnimatorPlayer.ShowText(textToShow);
        }
    }
}