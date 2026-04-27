using UnityEngine;
using AHeavyToll.Managers;
using AHeavyToll.Gameplay;
using AHeavyToll.Horror;
using AHeavyToll.UI;
using AHeavyToll.Data;
using AHeavyToll.VFX;

namespace AHeavyToll.Gameplay
{
    public class BoothController : MonoBehaviour
    {
        [Header("Booth Components")]
        [SerializeField] private Light interiorLight;
        [SerializeField] private MeshRenderer windowGlass;
        [SerializeField] private Transform documentScanner;
        [SerializeField] private Transform radioUnit;

        [Header("State")]
        [SerializeField] private bool lightsOn = true;
        [SerializeField] private bool windowIntact = true;
        [SerializeField] private bool radioFunctional = true;

        [Header("Damage Visuals")]
        [SerializeField] private Material crackedGlassMaterial;
        [SerializeField] private GameObject bloodStainPrefab;
        [SerializeField] private GameObject scratchMarkPrefab;

        private Material originalGlassMaterial;

        private void Start()
        {
            if (windowGlass != null)
                originalGlassMaterial = windowGlass.material;
        }

        public void BreakWindow()
        {
            if (!windowIntact) return;
            windowIntact = false;

            if (windowGlass != null && crackedGlassMaterial != null)
            {
                windowGlass.material = crackedGlassMaterial;
            }

            SubtitleManager.Instance?.ShowSubtitle("The window... something hit it.", 3f);
        }

        public void AddBloodWriting(string text)
        {
            // Spawn blood writing decal
        }

        public void AddScratches()
        {
            if (scratchMarkPrefab != null)
            {
                Instantiate(scratchMarkPrefab, transform);
            }
        }

        public void SetLights(bool on)
        {
            lightsOn = on;
            if (interiorLight != null)
                interiorLight.enabled = on;
        }
    }
}
