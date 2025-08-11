using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace GabrielBertasso.Editor
{
    public class ScreenCaptureEditor : EditorWindow
    {
        private const string Directory = "Screenshots/"; // TODO: Make customizable
        private static string s_latestScreenshotPath = "";

        private bool _initDone;

        private GUIStyle _bigText;


        private void InitStyles()
        {
            _initDone = true;
            _bigText = new GUIStyle(GUI.skin.label)
            {
                fontSize = 20,
                fontStyle = FontStyle.Bold
            };
        }

        private void OnGUI()
        {
            if (!_initDone)
            {
                InitStyles();
            }

            GUILayout.Label("Screen Capture", _bigText);
            if (GUILayout.Button("Take a screenshot"))
            {
                TakeScreenshot();
            }

            if (GUILayout.Button("Take a transparent screenshot (only one camera)"))
            {
                TakeTransparentScreenshot();
            }

            GUILayout.Label("Resolution: " + GetResolutionString());

            if (GUILayout.Button("Reveal in Explorer"))
            {
                ShowFolder();
            }

            GUILayout.Label("Directory: " + Directory);
        }

        [MenuItem("Tools/Screenshots/Open Window")]
        public static void ShowWindow()
        {
            GetWindow(typeof(ScreenCaptureEditor));
        }

        [MenuItem("Tools/Screenshots/Reveal in Explorer")]
        private static void ShowFolder()
        {
            if (File.Exists(s_latestScreenshotPath))
            {
                EditorUtility.RevealInFinder(s_latestScreenshotPath);
                return;
            }

            System.IO.Directory.CreateDirectory(Directory);
            EditorUtility.RevealInFinder(Directory);
        }

        [MenuItem("Tools/Screenshots/Take a Screenshot")]
        private static void TakeScreenshot()
        {
            string path = GetPath();
            ScreenCapture.CaptureScreenshot(path);
            s_latestScreenshotPath = path;

            Debug.Log($"Screenshot saved: <b>{path}</b> with resolution <b>{GetResolutionString()}</b>");
        }

        [MenuItem("Tools/Screenshots/Take a transparent Screenshot (only one camera)")]
        private static void TakeTransparentScreenshot()
        {
            Camera camera = FindAnyObjectByType<Camera>(FindObjectsInactive.Exclude);

            if (camera == null)
            {
                Debug.LogError("Camera not found!");
                return;
            }

            Vector2Int resolution = GetResolution();
            int width = resolution.x;
            int height = resolution.y;

            RenderTexture renderTexture = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
            RenderTexture.active = renderTexture;

            camera.targetTexture = renderTexture;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = Color.clear;
            camera.Render();

            Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            texture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            texture.Apply();

            RenderTexture.active = null;
            camera.targetTexture = null;

            if (Application.isEditor)
            {
                DestroyImmediate(renderTexture);
            }
            else
            {
                Destroy(renderTexture);
            }

            string path = GetPath();
            byte[] bytes = texture.EncodeToPNG();
            File.WriteAllBytes(path, bytes);

            Debug.Log($"Screenshot saved: <b>{path}</b> with resolution <b>{GetResolutionString()}</b>.");
        }

        private static string GetPath()
        {
            System.IO.Directory.CreateDirectory(Directory);

            DateTime currentTime = DateTime.Now;
            string filename = currentTime.ToString().Replace('/', '-').Replace(':', '_') + ".png";

            return Directory + filename;
        }

        private static Vector2Int GetResolution()
        {
            Vector2 size = Handles.GetMainGameViewSize();
            return new Vector2Int((int)size.x, (int)size.y);
        }

        private static string GetResolutionString()
        {
            Vector2Int resolution = GetResolution();
            return $"{resolution.x}x{resolution.y}";
        }
    }
}