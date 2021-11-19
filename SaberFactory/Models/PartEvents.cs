using CustomSaber;
using UnityEngine;
using UnityEngine.Events;

namespace SaberFactory.Models
{
    internal class PartEvents
    {
        public UnityEvent MultiplierUp;

        public UnityEvent<float> OnAccuracyChanged;
        public UnityEvent OnBlueLightOn;
        public UnityEvent OnComboBreak;

        public UnityEvent<int> OnComboChanged;
        public UnityEvent OnLevelEnded;
        public UnityEvent OnLevelFail;
        public UnityEvent OnLevelStart;
        public UnityEvent OnRedLightOn;
        public UnityEvent OnSlice;
        public UnityEvent SaberStartColliding;
        public UnityEvent SaberStopColliding;

        public static PartEvents FromCustomSaber(GameObject saberObject)
        {
            var eventManager = saberObject.GetComponent<EventManager>();

            if (!eventManager)
            {
                return null;
            }

            var partEvents = new PartEvents
            {
                OnSlice = eventManager.OnSlice,
                OnComboBreak = eventManager.OnComboBreak,
                MultiplierUp = eventManager.MultiplierUp,
                SaberStartColliding = eventManager.SaberStartColliding,
                SaberStopColliding = eventManager.SaberStopColliding,
                OnLevelStart = eventManager.OnLevelStart,
                OnLevelFail = eventManager.OnLevelFail,
                OnLevelEnded = eventManager.OnLevelEnded,
                OnBlueLightOn = eventManager.OnBlueLightOn,
                OnRedLightOn = eventManager.OnRedLightOn,
                OnComboChanged = eventManager.OnComboChanged,
                OnAccuracyChanged = eventManager.OnAccuracyChanged
            };

            return partEvents;
        }
    }
}