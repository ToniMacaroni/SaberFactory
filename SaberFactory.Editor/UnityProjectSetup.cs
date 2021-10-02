using System;
using UnityEditor;
using UnityEngine;

namespace SaberFactory.Editor
{
    internal class UnityProjectSetup : EditorWindow
    {
        private void OnGUI()
        {
            GUILayout.Label("Following stuff needs to be set up!");

            var needsFixing = false;

#pragma warning disable 618
            if (!PlayerSettings.virtualRealitySupported)
#pragma warning restore 618
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

            if (!needsFixing) Close();
        }

        [MenuItem("Window/Setup Saber Factory")]
        public static void ShowWindow()
        {
            GetWindow<UnityProjectSetup>(false, "Project Setup");
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
#pragma warning disable 618
            PlayerSettings.virtualRealitySupported = true;
#pragma warning restore 618
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