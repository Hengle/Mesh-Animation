namespace CodeWriter.MeshAnimation
{
    using System;
    using System.Collections.Generic;
    using Sirenix.OdinInspector;
    using UnityEngine;

    [CreateAssetMenu(menuName = "Mesh Animation")]
    public class MeshAnimationAsset : ScriptableObject
    {
        [InfoBox("@GetValidationMessage()", InfoMessageType.Error, visibleIfMemberName: nameof(IsInvalid))]
        [Required]
        [SerializeField]
        internal GameObject skin = default;

        [Required]
        [SerializeField]
        internal Shader shader = default;

        [Required]
        [SerializeField]
        [ListDrawerSettings(Expanded = true, ShowPaging = false, AlwaysAddDefaultValue = true)]
        internal AnimationClip[] animationClips = new AnimationClip[0];

        [DisableIf("@true")]
        [SerializeField]
        internal Texture2D bakedTexture = default;

        [DisableIf("@true")]
        [SerializeField]
        internal Material bakedMaterial = default;

        [TableList(AlwaysExpanded = true, ShowPaging = false)]
        [DisableIf("@true")]
        [SerializeField]
        internal List<AnimationData> animationData = new List<AnimationData>();

        [Serializable]
        internal class AnimationData
        {
            public AnimationClip clip;
            public float startFrame;
            public float lengthFrames;
        }

        public bool IsInvalid => GetValidationMessage() != null;

        public string GetValidationMessage()
        {
            if (animationClips.Length == 0) return "No animation clips";

            foreach (var clip in animationClips)
            {
                if (clip == null) return "Animation clip is null";
                if (clip.legacy) return "Legacy Animation clips not supported";
            }

            if (shader == null) return "shader is null";
            if (skin == null) return "skin is null";
            
            var skinnedMeshRenderer = skin.GetComponentInChildren<SkinnedMeshRenderer>();
            if (skinnedMeshRenderer == null) return "skin.GetComponentInChildren<SkinnedMeshRenderer>() == null";
                
            var skinAnimator = skin.GetComponent<Animator>();
            if (skinAnimator == null) return "skin.GetComponent<Animator>() == null";
            if (skinAnimator.runtimeAnimatorController == null)
                return "skin.GetComponent<Animator>().runtimeAnimatorController == null";

            return null;
        }

#if UNITY_EDITOR
        [DisableIf(nameof(IsInvalid))]
        [PropertySpace(10)]
        [Button(ButtonSizes.Large, Name = "Bake")]
        private void Bake()
        {
            MeshAnimationBaker.Bake(this);
        }

        [PropertySpace(5)]
        [Button(ButtonSizes.Small, Name = "Clear baked data")]
        private void Clear()
        {
            MeshAnimationBaker.Clear(this);
        }
#endif
    }
}