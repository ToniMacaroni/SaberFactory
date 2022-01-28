#if !UNITY
using SaberFactory.Helpers;
#endif
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

        [Range(0, 1)]
        public float Volume = 1;

#if !UNITY
        public float ConfigVolume = 1;

        private Vector3 _prevPos;
        private float _speed;

        public virtual void Start()
        {
            _prevPos = SaberTop.position;

            var saberMb = SaberHelpers.GetSaberMonoBehaviour(gameObject);
            if (saberMb)
            {
                saberMb.RegisterComponent(this);
            }
        }

        public virtual void Update()
        {
            var position = SaberTop.position;
            if ((_prevPos - position).sqrMagnitude > NoSoundTopThresholdSqr) _prevPos = position;

            float targetSpeed;
            if (Time.deltaTime == 0f)
            {
                targetSpeed = 0f;
            }
            else
            {
                targetSpeed = SpeedMultiplier * Vector3.Distance(position, _prevPos) / Time.deltaTime;
            }

            if (targetSpeed < _speed)
            {
                _speed = Mathf.Clamp01(Mathf.Lerp(_speed, targetSpeed, Time.deltaTime * DownSmooth));
            }
            else
            {
                _speed = Mathf.Clamp01(Mathf.Lerp(_speed, targetSpeed, Time.deltaTime * UpSmooth));
            }

            AudioSource.pitch = PitchBySpeedCurve.Evaluate(_speed);
            AudioSource.volume = GainBySpeedCurve.Evaluate(_speed) * Volume * ConfigVolume;

            _prevPos = position;
        }
#endif
    }
}