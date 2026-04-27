using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using AHeavyToll.Managers;
using AHeavyToll.Gameplay;
using AHeavyToll.Horror;
using AHeavyToll.UI;
using AHeavyToll.Data;
using AHeavyToll.VFX;

namespace AHeavyToll.VFX
{
    public class PSXPostProcess : MonoBehaviour
    {
        [Header("VHS Effect")]
        [SerializeField] private Volume postProcessVolume;
        [SerializeField] private bool enableVHS = true;

        [Header("PSX Settings")]
        [SerializeField] private float targetFrameRate = 15f;
        [SerializeField] private bool affineTextureWarping = true;
        [SerializeField] private float vertexJitterAmount = 0.05f;

        [Header("Render Scale")]
        [SerializeField] private float renderScale = 0.5f;

        [Header("Dithering")]
        [SerializeField] private bool enableDithering = true;
        [SerializeField] private Texture2D ditherPattern;

        private float nextJitterTime;

        private void Start()
        {
            var pipeline = GraphicsSettings.defaultRenderPipeline as UniversalRenderPipelineAsset;
            if (pipeline != null && renderScale > 0)
            {
                pipeline.renderScale = renderScale;
            }

            if (postProcessVolume != null)
            {
                postProcessVolume.enabled = enableVHS;
            }
        }

        private void Update()
        {
            if (affineTextureWarping && Time.time >= nextJitterTime)
            {
                nextJitterTime = Time.time + (1f / targetFrameRate);
            }
        }

        public void SetVHSEnabled(bool enabled)
        {
            enableVHS = enabled;
            if (postProcessVolume != null)
                postProcessVolume.enabled = enabled;
        }

        public void SetRenderScale(float scale)
        {
            renderScale = scale;
            var pipeline = GraphicsSettings.defaultRenderPipeline as UniversalRenderPipelineAsset;
            if (pipeline != null)
            {
                pipeline.renderScale = scale;
            }
        }
    }
}
