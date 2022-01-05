#if !UNITY
using IPA.Utilities;
#endif
using System;
using UnityEngine;

namespace SaberFactory.ProjectComponents
{
    public class SFSaberSound : MonoBehaviour
    {
        public Transform SaberTop;

        public AudioSource AudioSource;

        public AnimationCurve PitchBySpeedCurve;

        public AnimationCurve GainBySpeedCurve;

        public float SpeedMultiplier = 0.05f;

        public float UpSmooth = 4f;

        public float DownSmooth = 4f;

        [Tooltip("No sound is produced if saber point moves more than this distance in one frame.")]
        public float NoSoundTopThresholdSqr = 1f;

#if !UNITY
        public SaberSound ToSaberSound()
        {
            var saberSound = gameObject.AddComponent<SaberSound>();
            SaberTopAccessor(ref saberSound) = SaberTop;
            AudioSourceAccessor(ref saberSound) = AudioSource;
            PitchBySpeedCurveAccessor(ref saberSound) = PitchBySpeedCurve;
            GainBySpeedCurveAccessor(ref saberSound) = GainBySpeedCurve;
            SpeedMultiplierAccessor(ref saberSound) = SpeedMultiplier;
            UpSmoothAccessor(ref saberSound) = UpSmooth;
            DownSmoothAccessor(ref saberSound) = DownSmooth;
            NoSoundTopThresholdSqrAccessor(ref saberSound) = NoSoundTopThresholdSqr;
            DestroyImmediate(this);
            return saberSound;
        }

        private void Awake()
        {
            ToSaberSound();
        }

        private static readonly FieldAccessor<SaberSound, Transform>.Accessor SaberTopAccessor = FieldAccessor<SaberSound, Transform>.GetAccessor("_saberTop");
        private static readonly FieldAccessor<SaberSound, AudioSource>.Accessor AudioSourceAccessor = FieldAccessor<SaberSound, AudioSource>.GetAccessor("_audioSource");
        private static readonly FieldAccessor<SaberSound, AnimationCurve>.Accessor PitchBySpeedCurveAccessor = FieldAccessor<SaberSound, AnimationCurve>.GetAccessor("_pitchBySpeedCurve");
        private static readonly FieldAccessor<SaberSound, AnimationCurve>.Accessor GainBySpeedCurveAccessor = FieldAccessor<SaberSound, AnimationCurve>.GetAccessor("_gainBySpeedCurve");
        private static readonly FieldAccessor<SaberSound, float>.Accessor SpeedMultiplierAccessor = FieldAccessor<SaberSound, float>.GetAccessor("_speedMultiplier");
        private static readonly FieldAccessor<SaberSound, float>.Accessor UpSmoothAccessor = FieldAccessor<SaberSound, float>.GetAccessor("_upSmooth");
        private static readonly FieldAccessor<SaberSound, float>.Accessor DownSmoothAccessor = FieldAccessor<SaberSound, float>.GetAccessor("_downSmooth");
        private static readonly FieldAccessor<SaberSound, float>.Accessor NoSoundTopThresholdSqrAccessor = FieldAccessor<SaberSound, float>.GetAccessor("_noSoundTopThresholdSqr");
#endif
    }
}