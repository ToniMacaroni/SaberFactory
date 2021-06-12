using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace SaberFactory.Editor
{
    class UnityProjectSetup : EditorWindow
    {
        [MenuItem("Window/Setup Saber Factory")]
        public static void ShowWindow()
        {
            GetWindow<UnityProjectSetup>(false, "Project Setup");
        }

        private void OnGUI()
        {
            GUILayout.Label("Following stuff needs to be set up!");

            bool needsFixing = false;

            if (!PlayerSettings.virtualRealitySupported)
            {
                AddFixButton("Fix virtual reality support", FixVrSupport);
                needsFixing = true;
            }

            if (PlayerSettings.stereoRenderingPath != StereoRenderingPath.SinglePass)
            {
                AddFixButton("Fix stereo rendering path", FixStereoRenderingPath);
                needsFixing = true;
            }

            if (PlayerSettings.colorSpace != ColorSpace.Linear)
            {
                AddFixButton("Fix color space", FixColorSpace);
                needsFixing = true;
            }

            if (!needsFixing)
            {
                Close();
            }
        }

        private void AddFixButton(string text, Action callback)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(text);
            if (GUILayout.Button("Fix")) callback?.Invoke();
            GUILayout.EndHorizontal();
        }

        private void FixVrSupport()
        {
            PlayerSettings.virtualRealitySupported = true;
        }

        private void FixStereoRenderingPath()
        {
            PlayerSettings.stereoRenderingPath = StereoRenderingPath.SinglePass;
        }

        private void FixColorSpace()
        {
            PlayerSettings.colorSpace = ColorSpace.Linear;
        }
    }
}
