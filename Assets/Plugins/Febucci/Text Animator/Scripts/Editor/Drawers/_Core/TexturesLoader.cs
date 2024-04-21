using UnityEditor;
using UnityEngine;

namespace Febucci.UI
{
    public static class TexturesLoader
    {
        private const string localPath_resourcesFolder = "Text Animator";

        private static Texture aboutLogo;


        private static Texture stopIcon;

        private static Texture restartIcon;

        private static Texture saveIcon;

        private static Texture playIcon;

        private static Texture pauseIcon;

        public static Texture AboutLogo
        {
            get
            {
                if (!aboutLogo) aboutLogo = Resources.Load<Texture>(localPath_resourcesFolder + "/about_logo");
                return aboutLogo;
            }
        }

        public static Texture StopIcon
        {
            get
            {
                if (!stopIcon) stopIcon = Resources.Load<Texture>(localPath_resourcesFolder + "/stop_icon");
                return stopIcon;
            }
        }

        public static Texture RestartIcon
        {
            get
            {
                if (!restartIcon) restartIcon = Resources.Load<Texture>(localPath_resourcesFolder + "/restart_icon");
                return restartIcon;
            }
        }

        public static Texture SaveIcon
        {
            get
            {
                if (!saveIcon) saveIcon = Resources.Load<Texture>(localPath_resourcesFolder + "/save_icon");
                return saveIcon;
            }
        }

        public static Texture PlayIcon
        {
            get
            {
                if (!playIcon) playIcon = Resources.Load<Texture>(localPath_resourcesFolder + "/play_icon");
                return playIcon;
            }
        }

        public static Texture PauseIcon
        {
            get
            {
                if (!pauseIcon) pauseIcon = Resources.Load<Texture>(localPath_resourcesFolder + "/pause_icon");
                return pauseIcon;
            }
        }


        public static Texture WarningIcon => EditorGUIUtility.IconContent("Warning").image;
        public static Texture ErrorIcon => EditorGUIUtility.IconContent("Error").image;
    }
}