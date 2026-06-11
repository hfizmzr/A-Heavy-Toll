using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using AHeavyToll.Horror;
using AHeavyToll.Managers;
using AHeavyToll.Data;
using AHeavyToll.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AHeavyToll.Horror
{
#if UNITY_EDITOR
    [ExecuteInEditMode]
    public class HorrorSetupManager : MonoBehaviour
    {
        [ContextMenu("Run Horror Setup")]
        public void RunSetup()
        {
            Debug.Log("[HorrorSetup] Starting setup...");

            /* ALREADY RAN - COMMENTED OUT TO PREVENT DUPLICATION
            SetupJumpscareManager();
            SetupHorrorEventManager();
            CreateCarDataAssets();
            */
            
            WireEndingVisuals();
            WireUIManager();

            Debug.Log("[HorrorSetup] Setup complete! Check the Inspector for wired references.");
        }

        [ContextMenu("Check UIManager Wiring")]
        public void CheckUIManagerWiring()
        {
            GameObject canvasObj = FindGameObjectByName("Canvas");
            if (canvasObj == null)
            {
                Debug.LogError("[HorrorSetup] Canvas not found in scene!");
                return;
            }

            UIManager ui = canvasObj.GetComponent<UIManager>();
            if (ui == null)
            {
                Debug.LogError("[HorrorSetup] UIManager not found on Canvas!");
                return;
            }

            Debug.Log("[HorrorSetup] === UIManager Wiring Check ===");
            Debug.Log($"  gameOverPanel: {(ui.gameOverPanel != null ? "OK" : "NULL")}");
            Debug.Log($"  hudPanel: {(ui.hudPanel != null ? "OK" : "NULL")}");
            Debug.Log($"  endingText: {(ui.endingText != null ? "OK" : "NULL")}");
            Debug.Log($"  endingDescription: {(ui.endingDescription != null ? "OK" : "NULL")}");
            Debug.Log($"  restartButton: {(ui.restartButton != null ? "OK" : "NULL")}");
            Debug.Log($"  menuButton: {(ui.menuButton != null ? "OK" : "NULL")}");
            Debug.Log($"  nightText: {(ui.nightText != null ? "OK" : "NULL")}");
            Debug.Log($"  incomingCarIndicator: {(ui.incomingCarIndicator != null ? "OK" : "NULL")}");
            Debug.Log("[HorrorSetup] ============================");
        }

        private void SetupJumpscareManager()
        {
            GameObject managersObj = GameObject.Find("Managers");
            if (managersObj == null)
            {
                Debug.LogError("[HorrorSetup] Could not find 'Managers' GameObject!");
                return;
            }

            JumpscareManager jumpscareManager = managersObj.GetComponent<JumpscareManager>();
            if (jumpscareManager == null)
            {
                jumpscareManager = Undo.AddComponent<JumpscareManager>(managersObj);
                Debug.Log("[HorrorSetup] Added JumpscareManager to Managers.");
            }

            AudioSource audioSource = managersObj.GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = Undo.AddComponent<AudioSource>(managersObj);
                audioSource.playOnAwake = false;
                audioSource.spatialBlend = 0f;
                Debug.Log("[HorrorSetup] Added AudioSource to Managers.");
            }
            jumpscareManager.jumpscareAudio = audioSource;

            GameObject jumpscareCanvasObj = FindGameObjectByName("JumpscareCanvas");
            if (jumpscareCanvasObj != null)
            {
                jumpscareManager.jumpscareCanvas = jumpscareCanvasObj;
                Debug.Log("[HorrorSetup] Wired JumpscareCanvas.");
            }
            else
            {
                Debug.LogWarning("[HorrorSetup] Could not find 'JumpscareCanvas' GameObject.");
            }

            GameObject jumpscareImageObj = FindGameObjectByName("JumpscareImage");
            if (jumpscareImageObj != null)
            {
                jumpscareManager.jumpscareImage = jumpscareImageObj.GetComponent<UnityEngine.UI.Image>();
                Debug.Log("[HorrorSetup] Wired JumpscareImage.");
            }
            else
            {
                Debug.LogWarning("[HorrorSetup] Could not find 'JumpscareImage' GameObject.");
            }

            jumpscareManager.badEndingSprite = null;
            jumpscareManager.hiddenEndingSprite = null;
            jumpscareManager.badEndingScream = null;
            jumpscareManager.hiddenEndingAmbience = null;
            Debug.Log("[HorrorSetup] Jumpscare sprite/audio clips left as placeholders (assign manually).");

            Undo.RecordObject(jumpscareManager, "Setup JumpscareManager");
            EditorUtility.SetDirty(jumpscareManager);
        }

        private void SetupHorrorEventManager()
        {
            HorrorEventManager horrorManager = FindObjectOfType<HorrorEventManager>();
            if (horrorManager == null)
            {
                Debug.LogError("[HorrorSetup] HorrorEventManager.Instance is null! Ensure it exists in the scene.");
                return;
            }

            GameObject cameraHolder = GameObject.Find("CameraHolder");
            if (cameraHolder != null)
            {
                horrorManager.mainCamera = cameraHolder.GetComponent<Camera>();
                Debug.Log("[HorrorSetup] Wired mainCamera to CameraHolder.");
            }
            else
            {
                Debug.LogWarning("[HorrorSetup] Could not find 'CameraHolder' GameObject.");
            }

            GameObject directionalLight = GameObject.Find("Directional Light");
            if (directionalLight != null)
            {
                horrorManager.boothLight = directionalLight.GetComponent<Light>();
                Debug.Log("[HorrorSetup] Wired boothLight to Directional Light (temporary).");
            }
            else
            {
                Debug.LogWarning("[HorrorSetup] Could not find 'Directional Light' GameObject.");
            }

            horrorManager.horrorEvents.Clear();
            horrorManager.horrorEvents.Add(CreateHorrorEvent(
                "van_shadow", "Van Shadow",
                "A shadow moves across the booth window after the van passes",
                flickerLights: true, flickerDuration: 3f,
                spawnScratches: true,
                subtitleText: "Something scratched the window...",
                subtitleDuration: 3f,
                screenShake: true, shakeIntensity: 0.3f, shakeDuration: 1f,
                night1: false, night2: true, night3: true, triggerChance: 1f
            ));

            horrorManager.horrorEvents.Add(CreateHorrorEvent(
                "her_presence", "Her Presence",
                "The booth temperature drops. Writing appears on the window.",
                flickerLights: true, flickerDuration: 5f,
                spawnBloodWriting: true,
                subtitleText: "LET ME OUT",
                subtitleDuration: 4f,
                screenShake: true, shakeIntensity: 0.5f, shakeDuration: 2f,
                night1: false, night2: false, night3: true, triggerChance: 1f
            ));

            horrorManager.horrorEvents.Add(CreateHorrorEvent(
                "empty_car", "Empty Car",
                "A car with no driver passes through. The radio plays static.",
                flickerLights: true, flickerDuration: 2f,
                breakGlass: true,
                subtitleText: "There was no one driving...",
                subtitleDuration: 3f,
                screenShake: false,
                night1: false, night2: false, night3: true, triggerChance: 1f
            ));

            horrorManager.horrorEvents.Add(CreateHorrorEvent(
                "random_flicker", "Light Flicker",
                "Random light flicker for atmosphere",
                flickerLights: true, flickerDuration: 1f,
                subtitleText: "",
                screenShake: false,
                night1: false, night2: true, night3: true, triggerChance: 0.1f
            ));

            horrorManager.horrorEvents.Add(CreateHorrorEvent(
                "final_bad_ending", "Final Bad",
                "Ending sequence for bad ending",
                flickerLights: true, flickerDuration: 10f,
                breakGlass: true, spawnScratches: true,
                subtitleText: "They're inside now.",
                subtitleDuration: 5f,
                screenShake: true, shakeIntensity: 1f, shakeDuration: 5f,
                night1: true, night2: true, night3: true, triggerChance: 1f
            ));

            horrorManager.horrorEvents.Add(CreateHorrorEvent(
                "final_hidden_ending", "Final Hidden",
                "Ending sequence for hidden ending",
                flickerLights: true, flickerDuration: 15f,
                spawnBloodWriting: true, breakGlass: true,
                subtitleText: "I'm sorry I couldn't save you then, so let me save you now.",
                subtitleDuration: 6f,
                screenShake: true, shakeIntensity: 1.5f, shakeDuration: 8f,
                night1: true, night2: true, night3: true, triggerChance: 1f
            ));

            Debug.Log("[HorrorSetup] Created 6 HorrorEvent entries.");

            Undo.RecordObject(horrorManager, "Setup HorrorEventManager");
            EditorUtility.SetDirty(horrorManager);
        }

        private HorrorEvent CreateHorrorEvent(
            string eventId, string eventName, string description,
            bool flickerLights = false, float flickerDuration = 2f,
            bool spawnBloodWriting = false, bool spawnScratches = false, bool breakGlass = false,
            string subtitleText = "", float subtitleDuration = 3f,
            bool screenShake = false, float shakeIntensity = 0.5f, float shakeDuration = 0.5f,
            bool night1 = false, bool night2 = true, bool night3 = true,
            float triggerChance = 0.3f)
        {
            return new HorrorEvent
            {
                eventId = eventId,
                eventName = eventName,
                description = description,
                flickerLights = flickerLights,
                flickerDuration = flickerDuration,
                spawnBloodWriting = spawnBloodWriting,
                spawnScratches = spawnScratches,
                breakGlass = breakGlass,
                subtitleText = subtitleText,
                subtitleDuration = subtitleDuration,
                screenShake = screenShake,
                shakeIntensity = shakeIntensity,
                shakeDuration = shakeDuration,
                canTriggerNight1 = night1,
                canTriggerNight2 = night2,
                canTriggerNight3 = night3,
                triggerChance = triggerChance
            };
        }

        private void CreateCarDataAssets()
        {
            string dataPath = "Assets/Scripts/Data";

            CarData car3 = ScriptableObject.CreateInstance<CarData>();
            car3.driverName = "Marcus Webb";
            car3.driverDialogue = "I'm just passing through. Don't make this difficult.";
            car3.destination = "Route 66 - Westbound";
            car3.isMalevolent = true;
            car3.isHer = false;
            car3.documentsText = "Commercial Transport License - Valid until 11/2026";
            car3.documentsAreForged = true;
            car3.licensePlateSuspicious = true;
            car3.cargoIsUnusual = false;
            car3.appearsNight1 = false;
            car3.appearsNight2 = true;
            car3.appearsNight3 = true;
            car3.triggersHorrorEvent = true;
            car3.horrorEventId = "van_shadow";
            SaveCarData(car3, dataPath, "Car3.asset");

            CarData car4 = ScriptableObject.CreateInstance<CarData>();
            car4.driverName = "Unknown Driver";
            car4.driverDialogue = "...";
            car4.destination = "---";
            car4.isMalevolent = true;
            car4.isHer = false;
            car4.documentsText = "No documents provided.";
            car4.documentsAreForged = false;
            car4.licensePlateSuspicious = true;
            car4.cargoIsUnusual = true;
            car4.appearsNight1 = false;
            car4.appearsNight2 = false;
            car4.appearsNight3 = true;
            car4.triggersHorrorEvent = true;
            car4.horrorEventId = "empty_car";
            car4.radioStaticOverride = null;
            SaveCarData(car4, dataPath, "Car4.asset");

            CarData car5 = ScriptableObject.CreateInstance<CarData>();
            car5.driverName = "She";
            car5.driverDialogue = "The road remembers.";
            car5.destination = "Everywhere";
            car5.isMalevolent = false;
            car5.isHer = true;
            car5.documentsText = "A faded photograph. No name. No date.";
            car5.documentsAreForged = false;
            car5.licensePlateSuspicious = false;
            car5.cargoIsUnusual = true;
            car5.appearsNight1 = false;
            car5.appearsNight2 = false;
            car5.appearsNight3 = true;
            car5.triggersHorrorEvent = true;
            car5.horrorEventId = "her_presence";
            car5.radioStaticOverride = null;
            SaveCarData(car5, dataPath, "Car5.asset");

            Debug.Log("[HorrorSetup] Created Car3, Car4, Car5 assets.");
        }

        private void SaveCarData(CarData data, string path, string fileName)
        {
            string fullPath = System.IO.Path.Combine(path, fileName);
            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }
            AssetDatabase.CreateAsset(data, fullPath);
            AssetDatabase.SaveAssets();
        }

        private void WireEndingVisuals()
        {
            EndingManager endingManager = FindObjectOfType<EndingManager>();
            if (endingManager == null)
            {
                Debug.LogWarning("[HorrorSetup] EndingManager not found. Skipping ending visual wiring.");
                return;
            }

            string prefabPath = "Assets/Prefabs/Endings/";

            GameObject goodPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath + "EndingVisual_Good.prefab");
            GameObject badPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath + "EndingVisual_Bad.prefab");
            GameObject hiddenPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath + "EndingVisual_Hidden.prefab");

            if (goodPrefab != null)
            {
                GameObject goodInstance = FindOrCreateSceneInstance("EndingVisual_Good", goodPrefab);
                endingManager.goodEndingVisuals = goodInstance;
                Debug.Log("[HorrorSetup] Wired goodEndingVisuals -> EndingVisual_Good (instantiated in scene)");
            }
            else
            {
                Debug.LogWarning("[HorrorSetup] EndingVisual_Good.prefab not found. Run Tools > Create Ending Visuals first.");
            }

            if (badPrefab != null)
            {
                GameObject badInstance = FindOrCreateSceneInstance("EndingVisual_Bad", badPrefab);
                endingManager.badEndingVisuals = badInstance;
                Debug.Log("[HorrorSetup] Wired badEndingVisuals -> EndingVisual_Bad (instantiated in scene)");
            }
            else
            {
                Debug.LogWarning("[HorrorSetup] EndingVisual_Bad.prefab not found. Run Tools > Create Ending Visuals first.");
            }

            if (hiddenPrefab != null)
            {
                GameObject hiddenInstance = FindOrCreateSceneInstance("EndingVisual_Hidden", hiddenPrefab);
                endingManager.hiddenEndingVisuals = hiddenInstance;
                Debug.Log("[HorrorSetup] Wired hiddenEndingVisuals -> EndingVisual_Hidden (instantiated in scene)");
            }
            else
            {
                Debug.LogWarning("[HorrorSetup] EndingVisual_Hidden.prefab not found. Run Tools > Create Ending Visuals first.");
            }

            Undo.RecordObject(endingManager, "Wire Ending Visuals");
            EditorUtility.SetDirty(endingManager);
        }

        private GameObject FindOrCreateSceneInstance(string name, GameObject prefab)
        {
            GameObject existing = FindGameObjectByName(name);
            if (existing != null)
            {
                Debug.Log("[HorrorSetup] Found existing scene instance: " + name);
                return existing;
            }

            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            instance.name = name;
            instance.SetActive(false);
            Undo.RegisterCreatedObjectUndo(instance, "Create " + name);
            Debug.Log("[HorrorSetup] Instantiated prefab into scene: " + name);
            return instance;
        }

        private void WireUIManager()
        {
            GameObject canvasObj = FindGameObjectByName("Canvas");
            if (canvasObj == null)
            {
                Debug.LogWarning("[HorrorSetup] Canvas not found. Skipping UIManager wiring.");
                return;
            }

            UIManager uiManager = canvasObj.GetComponent<UIManager>();
            if (uiManager == null)
            {
                uiManager = Undo.AddComponent<UIManager>(canvasObj);
                Debug.Log("[HorrorSetup] Added UIManager to Canvas.");
            }

            // Game Over
            GameObject gameOverPanel = FindGameObjectByName("GameOverPanel");
            if (gameOverPanel != null)
            {
                uiManager.gameOverPanel = gameOverPanel;
                Debug.Log("[HorrorSetup] Wired gameOverPanel.");
            }

            GameObject endingTextObj = FindGameObjectByName("EndingText");
            if (endingTextObj != null)
            {
                uiManager.endingText = endingTextObj.GetComponent<TextMeshProUGUI>();
                Debug.Log("[HorrorSetup] Wired endingText.");
            }

            GameObject endingDescObj = FindGameObjectByName("EndingDescription");
            if (endingDescObj != null)
            {
                uiManager.endingDescription = endingDescObj.GetComponent<TextMeshProUGUI>();
                Debug.Log("[HorrorSetup] Wired endingDescription.");
            }

            GameObject restartBtnObj = FindGameObjectByName("RestartButton");
            if (restartBtnObj != null)
            {
                uiManager.restartButton = restartBtnObj.GetComponent<Button>();
                Debug.Log("[HorrorSetup] Wired restartButton.");
            }

            GameObject menuBtnObj = FindGameObjectByName("MenuButton");
            if (menuBtnObj != null)
            {
                uiManager.menuButton = menuBtnObj.GetComponent<Button>();
                Debug.Log("[HorrorSetup] Wired menuButton.");
            }

            // HUD
            GameObject hudPanel = FindGameObjectByName("HUD");
            if (hudPanel != null)
            {
                uiManager.hudPanel = hudPanel;
                Debug.Log("[HorrorSetup] Wired hudPanel.");
            }

            GameObject nightTextObj = FindGameObjectByName("NightText");
            if (nightTextObj != null)
            {
                uiManager.nightText = nightTextObj.GetComponent<TextMeshProUGUI>();
                Debug.Log("[HorrorSetup] Wired nightText.");
            }

            GameObject incomingIndicatorObj = FindGameObjectByName("IncomingIndicator");
            if (incomingIndicatorObj != null)
            {
                uiManager.incomingCarIndicator = incomingIndicatorObj.GetComponent<Image>();
                Debug.Log("[HorrorSetup] Wired incomingCarIndicator.");
            }

            Undo.RecordObject(uiManager, "Wire UIManager");
            EditorUtility.SetDirty(uiManager);
        }

        private GameObject FindGameObjectByName(string name)
        {
            GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
            foreach (GameObject obj in allObjects)
            {
                if (obj.name == name && obj.scene.isLoaded)
                    return obj;
            }
            return null;
        }
    }
#endif
}
