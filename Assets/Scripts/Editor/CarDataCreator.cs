#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using AHeavyToll.Data;
using AHeavyToll.Managers;
using AHeavyToll.Gameplay;
using AHeavyToll.Horror;
using AHeavyToll.UI;
using AHeavyToll.Data;
using AHeavyToll.VFX;

namespace AHeavyToll.Editor
{
    public class CarDataCreator : EditorWindow
    {
        private string carName = "New Car";
        private string driverName = "Unknown Driver";
        private string dialogue = "...";
        private string destination = "Unknown";
        private bool isMalevolent = false;
        private bool isHer = false;
        private bool forgedDocs = false;
        private bool suspiciousPlate = false;
        private bool night1 = true;
        private bool night2 = false;
        private bool night3 = false;

        [MenuItem("A Heavy Toll/Create Car Data")]
        public static void ShowWindow()
        {
            GetWindow<CarDataCreator>("Car Data Creator");
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Car Data Creator", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            carName = EditorGUILayout.TextField("Asset Name", carName);
            driverName = EditorGUILayout.TextField("Driver Name", driverName);
            dialogue = EditorGUILayout.TextField("Dialogue", dialogue);
            destination = EditorGUILayout.TextField("Destination", destination);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Threat Properties", EditorStyles.boldLabel);
            isMalevolent = EditorGUILayout.Toggle("Is Malevolent", isMalevolent);
            isHer = EditorGUILayout.Toggle("Is Her (Hidden Ending)", isHer);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Document Properties", EditorStyles.boldLabel);
            forgedDocs = EditorGUILayout.Toggle("Forged Documents", forgedDocs);
            suspiciousPlate = EditorGUILayout.Toggle("Suspicious Plate", suspiciousPlate);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Night Availability", EditorStyles.boldLabel);
            night1 = EditorGUILayout.Toggle("Night 1", night1);
            night2 = EditorGUILayout.Toggle("Night 2", night2);
            night3 = EditorGUILayout.Toggle("Night 3", night3);

            EditorGUILayout.Space();
            if (GUILayout.Button("Create Car Data Asset"))
            {
                CreateCarData();
            }
        }

        private void CreateCarData()
        {
            CarData asset = ScriptableObject.CreateInstance<CarData>();
            asset.driverName = driverName;
            asset.driverDialogue = dialogue;
            asset.destination = destination;
            asset.isMalevolent = isMalevolent;
            asset.isHer = isHer;
            asset.documentsAreForged = forgedDocs;
            asset.licensePlateSuspicious = suspiciousPlate;
            asset.appearsNight1 = night1;
            asset.appearsNight2 = night2;
            asset.appearsNight3 = night3;

            string path = $"Assets/Data/Cars/{carName}.asset";
            System.IO.Directory.CreateDirectory("Assets/Data/Cars");

            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;

            Debug.Log($"Created CarData: {path}");
        }
    }
}
#endif
