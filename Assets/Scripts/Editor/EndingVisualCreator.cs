using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using AHeavyToll.UI;
using System.IO;

namespace AHeavyToll.Editor
{
    public class EndingVisualCreator : EditorWindow
    {
        [MenuItem("Tools/Create Ending Visuals")]
        public static void CreateEndingVisuals()
        {
            string prefabPath = "Assets/Prefabs/Endings";

            if (!Directory.Exists(prefabPath))
                Directory.CreateDirectory(prefabPath);

            CreateGoodEndingPrefab(prefabPath);
            CreateBadEndingPrefab(prefabPath);
            CreateHiddenEndingPrefab(prefabPath);
            CreateFiredEndingPrefab(prefabPath);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("[EndingVisualCreator] Created 4 ending visual prefabs in " + prefabPath);
        }

        private static void CreateGoodEndingPrefab(string path)
        {
            GameObject root = new GameObject("EndingVisual_Good");
            root.AddComponent<RectTransform>().sizeDelta = Vector2.zero;
            root.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            root.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            root.AddComponent<GraphicRaycaster>();

            // Background - warm golden gradient feel
            GameObject bg = new GameObject("Background");
            bg.transform.SetParent(root.transform, false);
            RectTransform bgRect = bg.AddComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.sizeDelta = Vector2.zero;
            Image bgImage = bg.AddComponent<Image>();
            bgImage.color = new Color(0.95f, 0.85f, 0.5f, 1f);

            // Vignette - soft dark edges
            GameObject vignette = new GameObject("Vignette");
            vignette.transform.SetParent(root.transform, false);
            RectTransform vigRect = vignette.AddComponent<RectTransform>();
            vigRect.anchorMin = Vector2.zero;
            vigRect.anchorMax = Vector2.one;
            vigRect.sizeDelta = Vector2.zero;
            Image vigImage = vignette.AddComponent<Image>();
            vigImage.color = new Color(0.3f, 0.2f, 0.1f, 0.6f);

            // Glow overlay - gentle warm pulse
            GameObject glow = new GameObject("GlowOverlay");
            glow.transform.SetParent(root.transform, false);
            RectTransform glowRect = glow.AddComponent<RectTransform>();
            glowRect.anchorMin = Vector2.zero;
            glowRect.anchorMax = Vector2.one;
            glowRect.sizeDelta = Vector2.zero;
            Image glowImage = glow.AddComponent<Image>();
            glowImage.color = new Color(1f, 0.9f, 0.6f, 0.3f);

            // Title text
            GameObject titleObj = new GameObject("TitleText");
            titleObj.transform.SetParent(root.transform, false);
            RectTransform titleRect = titleObj.AddComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0.5f, 0.6f);
            titleRect.anchorMax = new Vector2(0.5f, 0.6f);
            titleRect.pivot = new Vector2(0.5f, 0.5f);
            titleRect.sizeDelta = new Vector2(600, 100);
            TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
            titleText.text = "DAWN";
            titleText.fontSize = 72;
            titleText.alignment = TextAlignmentOptions.Center;
            titleText.color = new Color(1f, 0.95f, 0.7f);
            titleText.font = GetDefaultFont();
            titleText.alpha = 0f;

            // Subtitle text
            GameObject subtitleObj = new GameObject("SubtitleText");
            subtitleObj.transform.SetParent(root.transform, false);
            RectTransform subtitleRect = subtitleObj.AddComponent<RectTransform>();
            subtitleRect.anchorMin = new Vector2(0.5f, 0.45f);
            subtitleRect.anchorMax = new Vector2(0.5f, 0.45f);
            subtitleRect.pivot = new Vector2(0.5f, 0.5f);
            subtitleRect.sizeDelta = new Vector2(800, 60);
            TextMeshProUGUI subtitleText = subtitleObj.AddComponent<TextMeshProUGUI>();
            subtitleText.text = "The sun rises. You survived.";
            subtitleText.fontSize = 28;
            subtitleText.alignment = TextAlignmentOptions.Center;
            subtitleText.color = new Color(0.9f, 0.85f, 0.7f);
            subtitleText.font = GetDefaultFont();
            subtitleText.alpha = 0f;

            // Controller
            EndingVisualController controller = root.AddComponent<EndingVisualController>();
            controller.background = bgImage;
            controller.vignette = vigImage;
            controller.glowOverlay = glowImage;
            controller.titleText = titleText;
            controller.subtitleText = subtitleText;
            controller.fadeInDuration = 2.5f;
            controller.holdDuration = 6f;
            controller.fadeOutDuration = 2f;
            controller.enableGlowPulse = true;
            controller.pulseSpeed = 0.8f;
            controller.pulseMinAlpha = 0.15f;
            controller.pulseMaxAlpha = 0.4f;
            controller.enableColorShift = true;
            controller.targetColor = new Color(1f, 0.95f, 0.8f);
            controller.colorShiftDuration = 3f;

            // Create prefab
            string fullPath = Path.Combine(path, "EndingVisual_Good.prefab");
            if (File.Exists(fullPath)) File.Delete(fullPath);
            PrefabUtility.SaveAsPrefabAsset(root, fullPath);
            DestroyImmediate(root);

            Debug.Log("[EndingVisualCreator] Created EndingVisual_Good.prefab");
        }

        private static void CreateBadEndingPrefab(string path)
        {
            GameObject root = new GameObject("EndingVisual_Bad");
            root.AddComponent<RectTransform>().sizeDelta = Vector2.zero;
            root.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            root.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            root.AddComponent<GraphicRaycaster>();

            // Background - dark red
            GameObject bg = new GameObject("Background");
            bg.transform.SetParent(root.transform, false);
            RectTransform bgRect = bg.AddComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.sizeDelta = Vector2.zero;
            Image bgImage = bg.AddComponent<Image>();
            bgImage.color = new Color(0.4f, 0f, 0f, 1f);

            // Vignette - heavy dark edges
            GameObject vignette = new GameObject("Vignette");
            vignette.transform.SetParent(root.transform, false);
            RectTransform vigRect = vignette.AddComponent<RectTransform>();
            vigRect.anchorMin = Vector2.zero;
            vigRect.anchorMax = Vector2.one;
            vigRect.sizeDelta = Vector2.zero;
            Image vigImage = vignette.AddComponent<Image>();
            vigImage.color = new Color(0f, 0f, 0f, 0.8f);

            // Glow overlay - pulsing red
            GameObject glow = new GameObject("GlowOverlay");
            glow.transform.SetParent(root.transform, false);
            RectTransform glowRect = glow.AddComponent<RectTransform>();
            glowRect.anchorMin = Vector2.zero;
            glowRect.anchorMax = Vector2.one;
            glowRect.sizeDelta = Vector2.zero;
            Image glowImage = glow.AddComponent<Image>();
            glowImage.color = new Color(0.8f, 0f, 0f, 0.2f);

            // Title text
            GameObject titleObj = new GameObject("TitleText");
            titleObj.transform.SetParent(root.transform, false);
            RectTransform titleRect = titleObj.AddComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0.5f, 0.6f);
            titleRect.anchorMax = new Vector2(0.5f, 0.6f);
            titleRect.pivot = new Vector2(0.5f, 0.5f);
            titleRect.sizeDelta = new Vector2(600, 100);
            TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
            titleText.text = "BREACHED";
            titleText.fontSize = 72;
            titleText.alignment = TextAlignmentOptions.Center;
            titleText.color = new Color(1f, 0.2f, 0.2f);
            titleText.font = GetDefaultFont();
            titleText.alpha = 0f;

            // Subtitle text
            GameObject subtitleObj = new GameObject("SubtitleText");
            subtitleObj.transform.SetParent(root.transform, false);
            RectTransform subtitleRect = subtitleObj.AddComponent<RectTransform>();
            subtitleRect.anchorMin = new Vector2(0.5f, 0.45f);
            subtitleRect.anchorMax = new Vector2(0.5f, 0.45f);
            subtitleRect.pivot = new Vector2(0.5f, 0.5f);
            subtitleRect.sizeDelta = new Vector2(800, 60);
            TextMeshProUGUI subtitleText = subtitleObj.AddComponent<TextMeshProUGUI>();
            subtitleText.text = "You shouldn't have let them through.";
            subtitleText.fontSize = 28;
            subtitleText.alignment = TextAlignmentOptions.Center;
            subtitleText.color = new Color(0.8f, 0.3f, 0.3f);
            subtitleText.font = GetDefaultFont();
            subtitleText.alpha = 0f;

            // Controller
            EndingVisualController controller = root.AddComponent<EndingVisualController>();
            controller.background = bgImage;
            controller.vignette = vigImage;
            controller.glowOverlay = glowImage;
            controller.titleText = titleText;
            controller.subtitleText = subtitleText;
            controller.fadeInDuration = 0.5f;
            controller.holdDuration = 5f;
            controller.fadeOutDuration = 2f;
            controller.enableGlowPulse = true;
            controller.pulseSpeed = 2.5f;
            controller.pulseMinAlpha = 0.1f;
            controller.pulseMaxAlpha = 0.6f;
            controller.enableColorShift = false;

            // Create prefab
            string fullPath = Path.Combine(path, "EndingVisual_Bad.prefab");
            if (File.Exists(fullPath)) File.Delete(fullPath);
            PrefabUtility.SaveAsPrefabAsset(root, fullPath);
            DestroyImmediate(root);

            Debug.Log("[EndingVisualCreator] Created EndingVisual_Bad.prefab");
        }

        private static void CreateHiddenEndingPrefab(string path)
        {
            GameObject root = new GameObject("EndingVisual_Hidden");
            root.AddComponent<RectTransform>().sizeDelta = Vector2.zero;
            root.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            root.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            root.AddComponent<GraphicRaycaster>();

            // Background - deep crimson
            GameObject bg = new GameObject("Background");
            bg.transform.SetParent(root.transform, false);
            RectTransform bgRect = bg.AddComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.sizeDelta = Vector2.zero;
            Image bgImage = bg.AddComponent<Image>();
            bgImage.color = new Color(0.3f, 0f, 0.05f, 1f);

            // Vignette - heavy dark with red tint
            GameObject vignette = new GameObject("Vignette");
            vignette.transform.SetParent(root.transform, false);
            RectTransform vigRect = vignette.AddComponent<RectTransform>();
            vigRect.anchorMin = Vector2.zero;
            vigRect.anchorMax = Vector2.one;
            vigRect.sizeDelta = Vector2.zero;
            Image vigImage = vignette.AddComponent<Image>();
            vigImage.color = new Color(0.2f, 0f, 0f, 0.9f);

            // Glow overlay - slow crimson drift
            GameObject glow = new GameObject("GlowOverlay");
            glow.transform.SetParent(root.transform, false);
            RectTransform glowRect = glow.AddComponent<RectTransform>();
            glowRect.anchorMin = Vector2.zero;
            glowRect.anchorMax = Vector2.one;
            glowRect.sizeDelta = Vector2.zero;
            Image glowImage = glow.AddComponent<Image>();
            glowImage.color = new Color(0.6f, 0f, 0.1f, 0.15f);

            // Title text
            GameObject titleObj = new GameObject("TitleText");
            titleObj.transform.SetParent(root.transform, false);
            RectTransform titleRect = titleObj.AddComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0.5f, 0.6f);
            titleRect.anchorMax = new Vector2(0.5f, 0.6f);
            titleRect.pivot = new Vector2(0.5f, 0.5f);
            titleRect.sizeDelta = new Vector2(600, 100);
            TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
            titleText.text = "SHE WALKS";
            titleText.fontSize = 72;
            titleText.alignment = TextAlignmentOptions.Center;
            titleText.color = new Color(0.9f, 0.1f, 0.2f);
            titleText.font = GetDefaultFont();
            titleText.alpha = 0f;

            // Subtitle text
            GameObject subtitleObj = new GameObject("SubtitleText");
            subtitleObj.transform.SetParent(root.transform, false);
            RectTransform subtitleRect = subtitleObj.AddComponent<RectTransform>();
            subtitleRect.anchorMin = new Vector2(0.5f, 0.45f);
            subtitleRect.anchorMax = new Vector2(0.5f, 0.45f);
            subtitleRect.pivot = new Vector2(0.5f, 0.5f);
            subtitleRect.sizeDelta = new Vector2(800, 60);
            TextMeshProUGUI subtitleText = subtitleObj.AddComponent<TextMeshProUGUI>();
            subtitleText.text = "The road is Hers now. The toll is paid.";
            subtitleText.fontSize = 28;
            subtitleText.alignment = TextAlignmentOptions.Center;
            subtitleText.color = new Color(0.7f, 0.15f, 0.2f);
            subtitleText.font = GetDefaultFont();
            subtitleText.alpha = 0f;

            // Controller
            EndingVisualController controller = root.AddComponent<EndingVisualController>();
            controller.background = bgImage;
            controller.vignette = vigImage;
            controller.glowOverlay = glowImage;
            controller.titleText = titleText;
            controller.subtitleText = subtitleText;
            controller.fadeInDuration = 3.5f;
            controller.holdDuration = 8f;
            controller.fadeOutDuration = 3f;
            controller.enableGlowPulse = true;
            controller.pulseSpeed = 0.5f;
            controller.pulseMinAlpha = 0.05f;
            controller.pulseMaxAlpha = 0.35f;
            controller.enableColorShift = true;
            controller.targetColor = new Color(0.5f, 0f, 0.08f);
            controller.colorShiftDuration = 5f;

            // Create prefab
            string fullPath = Path.Combine(path, "EndingVisual_Hidden.prefab");
            if (File.Exists(fullPath)) File.Delete(fullPath);
            PrefabUtility.SaveAsPrefabAsset(root, fullPath);
            DestroyImmediate(root);

            Debug.Log("[EndingVisualCreator] Created EndingVisual_Hidden.prefab");
        }

        private static void CreateFiredEndingPrefab(string path)
        {
            GameObject root = new GameObject("EndingVisual_Fired");
            root.AddComponent<RectTransform>().sizeDelta = Vector2.zero;
            root.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            root.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            root.AddComponent<GraphicRaycaster>();

            GameObject bg = new GameObject("Background");
            bg.transform.SetParent(root.transform, false);
            RectTransform bgRect = bg.AddComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.sizeDelta = Vector2.zero;
            Image bgImage = bg.AddComponent<Image>();
            bgImage.color = new Color(0.2f, 0.2f, 0.25f, 1f);

            GameObject vignette = new GameObject("Vignette");
            vignette.transform.SetParent(root.transform, false);
            RectTransform vigRect = vignette.AddComponent<RectTransform>();
            vigRect.anchorMin = Vector2.zero;
            vigRect.anchorMax = Vector2.one;
            vigRect.sizeDelta = Vector2.zero;
            Image vigImage = vignette.AddComponent<Image>();
            vigImage.color = new Color(0f, 0f, 0f, 0.85f);

            GameObject glow = new GameObject("GlowOverlay");
            glow.transform.SetParent(root.transform, false);
            RectTransform glowRect = glow.AddComponent<RectTransform>();
            glowRect.anchorMin = Vector2.zero;
            glowRect.anchorMax = Vector2.one;
            glowRect.sizeDelta = Vector2.zero;
            Image glowImage = glow.AddComponent<Image>();
            glowImage.color = new Color(0.4f, 0.4f, 0.5f, 0.1f);

            GameObject titleObj = new GameObject("TitleText");
            titleObj.transform.SetParent(root.transform, false);
            RectTransform titleRect = titleObj.AddComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0.5f, 0.6f);
            titleRect.anchorMax = new Vector2(0.5f, 0.6f);
            titleRect.pivot = new Vector2(0.5f, 0.5f);
            titleRect.sizeDelta = new Vector2(600, 100);
            TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
            titleText.text = "TERMINATED";
            titleText.fontSize = 72;
            titleText.alignment = TextAlignmentOptions.Center;
            titleText.color = new Color(0.7f, 0.7f, 0.8f);
            titleText.font = GetDefaultFont();
            titleText.alpha = 0f;

            GameObject subtitleObj = new GameObject("SubtitleText");
            subtitleObj.transform.SetParent(root.transform, false);
            RectTransform subtitleRect = subtitleObj.AddComponent<RectTransform>();
            subtitleRect.anchorMin = new Vector2(0.5f, 0.45f);
            subtitleRect.anchorMax = new Vector2(0.5f, 0.45f);
            subtitleRect.pivot = new Vector2(0.5f, 0.5f);
            subtitleRect.sizeDelta = new Vector2(800, 60);
            TextMeshProUGUI subtitleText = subtitleObj.AddComponent<TextMeshProUGUI>();
            subtitleText.text = "You denied every car. They have questions.";
            subtitleText.fontSize = 28;
            subtitleText.alignment = TextAlignmentOptions.Center;
            subtitleText.color = new Color(0.6f, 0.6f, 0.7f);
            subtitleText.font = GetDefaultFont();
            subtitleText.alpha = 0f;

            EndingVisualController controller = root.AddComponent<EndingVisualController>();
            controller.background = bgImage;
            controller.vignette = vigImage;
            controller.glowOverlay = glowImage;
            controller.titleText = titleText;
            controller.subtitleText = subtitleText;
            controller.fadeInDuration = 2f;
            controller.holdDuration = 6f;
            controller.fadeOutDuration = 2f;
            controller.enableGlowPulse = false;
            controller.enableColorShift = false;

            string fullPath = Path.Combine(path, "EndingVisual_Fired.prefab");
            if (File.Exists(fullPath)) File.Delete(fullPath);
            PrefabUtility.SaveAsPrefabAsset(root, fullPath);
            DestroyImmediate(root);

            Debug.Log("[EndingVisualCreator] Created EndingVisual_Fired.prefab");
        }

        private static TMP_FontAsset GetDefaultFont()
        {
            string[] guids = AssetDatabase.FindAssets("t:TMP_FontAsset");
            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                return AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(path);
            }
            return null;
        }
    }
}
