using UnityEngine;

namespace HighlightPlus {

    [CreateAssetMenu(menuName = "Highlight Plus Profile", fileName = "Highlight Plus Profile", order = 100)]
    public class HighlightProfile : ScriptableObject {

        public TargetOptions effectGroup = TargetOptions.Children;
        public LayerMask effectGroupLayer = -1;
		public string effectNameFilter;
        public bool combineMeshes;
        [Range(0, 1)]
        public float alphaCutOff = 0;
        public bool cullBackFaces = true;
        public bool depthClip;
        public NormalsOption normalsOption;

        public float fadeInDuration;
        public float fadeOutDuration;

        public bool constantWidth = true;

        [Range(0, 1)]
        public float overlay = 0.5f;
		[ColorUsage(true, true)] public Color overlayColor = Color.yellow;
        public float overlayAnimationSpeed = 1f;
        [Range(0, 1)]
        public float overlayMinIntensity = 0.5f;
        [Range(0, 1)]
        public float overlayBlending = 1.0f;

        [Range(0, 1)]
        public float outline = 1f;
		[ColorUsage(true, true)] public Color outlineColor = Color.black;
        public float outlineWidth = 0.45f;
        public QualityLevel outlineQuality = QualityLevel.High;
        [Range(1, 8)]
        public int outlineDownsampling = 2;
        public bool outlineOptimalBlit = true;
        public Visibility outlineVisibility = Visibility.Normal;
        public bool outlineIndependent;

        [Range(0, 5)]
        public float glow = 1f;
        public float glowWidth = 0.4f;
        public QualityLevel glowQuality = QualityLevel.High;
        [Range(1, 8)]
        public int glowDownsampling = 2;
		[ColorUsage(true, true)] public Color glowHQColor = new Color (0.64f, 1f, 0f, 1f);
        public bool glowDithering = true;
        public bool glowOptimalBlit = true;
        public float glowMagicNumber1 = 0.75f;
        public float glowMagicNumber2 = 0.5f;
        public float glowAnimationSpeed = 1f;
        public Visibility glowVisibility = Visibility.Normal;
        public bool glowBlendPasses = true;
        public GlowPassData[] glowPasses;

        [Range(0, 5f)]
        public float innerGlow = 0f;
        [Range(0, 2)]
        public float innerGlowWidth = 1f;
		[ColorUsage(true, true)] public Color innerGlowColor = Color.white;
        public Visibility innerGlowVisibility = Visibility.Normal;

        public bool targetFX;
        public Texture2D targetFXTexture;
		[ColorUsage(true, true)] public Color targetFXColor = Color.white;
        public float targetFXRotationSpeed = 50f;
        public float targetFXInitialScale = 4f;
        public float targetFXEndScale = 1.5f;
        [Tooltip("Makes target scale relative to object renderer bounds")]
        public bool targetFXScaleToRenderBounds;
        public float targetFXTransitionDuration = 0.5f;
        public float targetFXStayDuration = 1.5f;
        public Visibility targetFXVisibility = Visibility.AlwaysOnTop;

        public SeeThroughMode seeThrough;
        public LayerMask seeThroughOccluderMask = -1;
        public bool seeThroughOccluderMaskAccurate;
        [Range(0.01f, 0.9f)] public float seeThroughOccluderThreshold = 0.4f;
        public float seeThroughOccluderCheckInterval = 1f;
        [Range(0, 5f)] public float seeThroughIntensity = 0.8f;
        [Range(0, 1)] public float seeThroughTintAlpha = 0.5f;
        public Color seeThroughTintColor = Color.red;
        [Range(0, 1)] public float seeThroughNoise = 1f;
        [Range(0, 1)] public float seeThroughBorder;
        public Color seeThroughBorderColor = Color.black;
        public float seeThroughBorderWidth = 0.45f;

        [Range(0, 1)] public float hitFxInitialIntensity = 1f;
        public HitFxMode hitFxMode = HitFxMode.Overlay;
        public float hitFxFadeOutDuration = 0.25f;
        [ColorUsage(true, true)] public Color hitFxColor = Color.white;

        public void Load(HighlightEffect effect) {
            effect.effectGroup = effectGroup;
            effect.effectGroupLayer = effectGroupLayer;
			effect.effectNameFilter = effectNameFilter;
            effect.combineMeshes = combineMeshes;
            effect.alphaCutOff = alphaCutOff;
            effect.cullBackFaces = cullBackFaces;
            effect.depthClip = depthClip;
            effect.normalsOption = normalsOption;
            effect.fadeInDuration = fadeInDuration;
            effect.fadeOutDuration = fadeOutDuration;
            effect.constantWidth = constantWidth;
            effect.overlay = overlay;
            effect.overlayColor = overlayColor;
            effect.overlayAnimationSpeed = overlayAnimationSpeed;
            effect.overlayMinIntensity = overlayMinIntensity;
            effect.overlayBlending = overlayBlending;
            effect.outline = outline;
            effect.outlineColor = outlineColor;
            effect.outlineWidth = outlineWidth;
            effect.outlineQuality = outlineQuality;
            effect.outlineOptimalBlit = outlineOptimalBlit;
            effect.outlineDownsampling = outlineDownsampling;
            effect.outlineVisibility = outlineVisibility;
            effect.outlineIndependent = outlineIndependent;
            effect.glow = glow;
            effect.glowWidth = glowWidth;
            effect.glowQuality = glowQuality;
            effect.glowOptimalBlit = glowOptimalBlit;
            effect.glowDownsampling = glowDownsampling;
            effect.glowHQColor = glowHQColor;
            effect.glowDithering = glowDithering;
            effect.glowMagicNumber1 = glowMagicNumber1;
            effect.glowMagicNumber2 = glowMagicNumber2;
            effect.glowAnimationSpeed = glowAnimationSpeed;
            effect.glowVisibility = glowVisibility;
            effect.glowBlendPasses = glowBlendPasses;
            effect.glowPasses = GetGlowPassesCopy(glowPasses);
            effect.innerGlow = innerGlow;
            effect.innerGlowWidth = innerGlowWidth;
            effect.innerGlowColor = innerGlowColor;
            effect.innerGlowVisibility = innerGlowVisibility;
            effect.targetFX = targetFX;
            effect.targetFXColor = targetFXColor;
            effect.targetFXInitialScale = targetFXInitialScale;
            effect.targetFXEndScale = targetFXEndScale;
            effect.targetFXScaleToRenderBounds = targetFXScaleToRenderBounds;
            effect.targetFXRotationSpeed = targetFXRotationSpeed;
            effect.targetFXStayDuration = targetFXStayDuration;
            effect.targetFXTexture = targetFXTexture;
            effect.targetFXTransitionDuration = targetFXTransitionDuration;
            effect.targetFXVisibility = targetFXVisibility;
            effect.seeThrough = seeThrough;
            effect.seeThroughOccluderMask = seeThroughOccluderMask;
            effect.seeThroughOccluderMaskAccurate = seeThroughOccluderMaskAccurate;
            effect.seeThroughOccluderThreshold = seeThroughOccluderThreshold;
            effect.seeThroughOccluderCheckInterval = seeThroughOccluderCheckInterval;
            effect.seeThroughIntensity = seeThroughIntensity;
            effect.seeThroughTintAlpha = seeThroughTintAlpha;
            effect.seeThroughTintColor = seeThroughTintColor;
            effect.seeThroughNoise = seeThroughNoise;
            effect.seeThroughBorder = seeThroughBorder;
            effect.seeThroughBorderColor = seeThroughBorderColor;
            effect.seeThroughBorderWidth = seeThroughBorderWidth;
            effect.hitFxInitialIntensity = hitFxInitialIntensity;
            effect.hitFxMode = hitFxMode;
            effect.hitFxFadeOutDuration = hitFxFadeOutDuration;
            effect.hitFxColor = hitFxColor;
            effect.UpdateMaterialProperties();
        }

        public void Save(HighlightEffect effect) {
            effectGroup = effect.effectGroup;
            effectGroupLayer = effect.effectGroupLayer;
			effectNameFilter = effect.effectNameFilter;
            combineMeshes = effect.combineMeshes;
            alphaCutOff = effect.alphaCutOff;
            cullBackFaces = effect.cullBackFaces;
            depthClip = effect.depthClip;
            normalsOption = effect.normalsOption;
            fadeInDuration = effect.fadeInDuration;
            fadeOutDuration = effect.fadeOutDuration;
            constantWidth = effect.constantWidth;
            overlay = effect.overlay;
            overlayColor = effect.overlayColor;
            overlayAnimationSpeed = effect.overlayAnimationSpeed;
            overlayMinIntensity = effect.overlayMinIntensity;
            overlayBlending = effect.overlayBlending;
            outline = effect.outline;
            outlineColor = effect.outlineColor;
            outlineWidth = effect.outlineWidth;
            outlineQuality = effect.outlineQuality;
            outlineDownsampling = effect.outlineDownsampling;
            outlineVisibility = effect.outlineVisibility;
            outlineIndependent = effect.outlineIndependent;
            outlineOptimalBlit = effect.outlineOptimalBlit;
            glow = effect.glow;
            glowWidth = effect.glowWidth;
            glowQuality = effect.glowQuality;
            glowOptimalBlit = effect.glowOptimalBlit;
            glowDownsampling = effect.glowDownsampling;
            glowHQColor = effect.glowHQColor;
            glowDithering = effect.glowDithering;
            glowMagicNumber1 = effect.glowMagicNumber1;
            glowMagicNumber2 = effect.glowMagicNumber2;
            glowAnimationSpeed = effect.glowAnimationSpeed;
            glowVisibility = effect.glowVisibility;
            glowBlendPasses = effect.glowBlendPasses;
            glowPasses = GetGlowPassesCopy(effect.glowPasses);
            innerGlow = effect.innerGlow;
            innerGlowWidth = effect.innerGlowWidth;
            innerGlowColor = effect.innerGlowColor;
            innerGlowVisibility = effect.innerGlowVisibility;
            targetFX = effect.targetFX;
            targetFXColor = effect.targetFXColor;
            targetFXInitialScale = effect.targetFXInitialScale;
            targetFXEndScale = effect.targetFXEndScale;
            targetFXScaleToRenderBounds = effect.targetFXScaleToRenderBounds;
            targetFXRotationSpeed = effect.targetFXRotationSpeed;
            targetFXStayDuration = effect.targetFXStayDuration;
            targetFXTexture = effect.targetFXTexture;
            targetFXTransitionDuration = effect.targetFXTransitionDuration;
            targetFXVisibility = effect.targetFXVisibility;
            seeThrough = effect.seeThrough;
            seeThroughOccluderMask = effect.seeThroughOccluderMask;
            seeThroughOccluderMaskAccurate = effect.seeThroughOccluderMaskAccurate;
            seeThroughOccluderThreshold = effect.seeThroughOccluderThreshold;
            seeThroughOccluderCheckInterval = effect.seeThroughOccluderCheckInterval;
            seeThroughIntensity = effect.seeThroughIntensity;
            seeThroughTintAlpha = effect.seeThroughTintAlpha;
            seeThroughTintColor = effect.seeThroughTintColor;
            seeThroughNoise = effect.seeThroughNoise;
            seeThroughBorder = effect.seeThroughBorder;
            seeThroughBorderColor = effect.seeThroughBorderColor;
            seeThroughBorderWidth = effect.seeThroughBorderWidth;
            hitFxInitialIntensity = effect.hitFxInitialIntensity;
            hitFxMode = effect.hitFxMode;
            hitFxFadeOutDuration = effect.hitFxFadeOutDuration;
            hitFxColor = effect.hitFxColor;
        }

        GlowPassData[] GetGlowPassesCopy(GlowPassData[] glowPasses) {
            if (glowPasses == null) {
                return new GlowPassData[0];
            }
            GlowPassData[] pd = new GlowPassData[glowPasses.Length];
            for (int k = 0; k < glowPasses.Length; k++) {
                pd[k].alpha = glowPasses[k].alpha;
                pd[k].color = glowPasses[k].color;
                pd[k].offset = glowPasses[k].offset;
            }
            return pd;
        }

        public void OnValidate() {
            if (glowPasses == null || glowPasses.Length == 0) {
                glowPasses = new GlowPassData[4];
                glowPasses[0] = new GlowPassData() { offset = 4, alpha = 0.1f, color = new Color(0.64f, 1f, 0f, 1f) };
                glowPasses[1] = new GlowPassData() { offset = 3, alpha = 0.2f, color = new Color(0.64f, 1f, 0f, 1f) };
                glowPasses[2] = new GlowPassData() { offset = 2, alpha = 0.3f, color = new Color(0.64f, 1f, 0f, 1f) };
                glowPasses[3] = new GlowPassData() { offset = 1, alpha = 0.4f, color = new Color(0.64f, 1f, 0f, 1f) };
            }
        }

    }
}

