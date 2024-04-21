using UnityEngine;
using UnityEngine.UI;

namespace EpicToonFX
{
    public class ETFXButtonScript : MonoBehaviour
    {
        public GameObject Button;

        public float buttonsX;
        public float buttonsY;
        public float buttonsSizeX;
        public float buttonsSizeY;
        public float buttonsDistance;

        private ETFXFireProjectile effectScript; // A variable used to access the list of projectiles
        private Text MyButtonText;
        private string projectileParticleName; // The variable to update the text component of the button
        private ETFXProjectileScript projectileScript;

        private void Start()
        {
            effectScript = GameObject.Find("ETFXFireProjectile").GetComponent<ETFXFireProjectile>();
            getProjectileNames();
            MyButtonText = Button.transform.Find("Text").GetComponent<Text>();
            MyButtonText.text = projectileParticleName;
        }

        private void Update()
        {
            MyButtonText.text = projectileParticleName;
//		print(projectileParticleName);
        }

        public void getProjectileNames() // Find and diplay the name of the currently selected projectile
        {
            // Access the currently selected projectile's 'ProjectileScript'
            projectileScript = effectScript.projectiles[effectScript.currentProjectile]
                .GetComponent<ETFXProjectileScript>();
            projectileParticleName =
                projectileScript.projectileParticle
                    .name; // Assign the name of the currently selected projectile to projectileParticleName
        }

        public bool overButton() // This function will return either true or false
        {
            var button1 = new Rect(buttonsX, buttonsY, buttonsSizeX, buttonsSizeY);
            var button2 = new Rect(buttonsX + buttonsDistance, buttonsY, buttonsSizeX, buttonsSizeY);

            if (button1.Contains(new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y)) ||
                button2.Contains(new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y)))
                return true;
            return false;
        }
    }
}