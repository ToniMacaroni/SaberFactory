using SaberFactory.Helpers;
using UnityEngine;

namespace SaberFactory.Models
{
    internal class AdditionalInstanceHandler
    {
        public bool IsInstantiated => _instance != null;

        private GameObject _instance;
        private GameObject _customSaberLeftSaber;
        private GameObject _customSaberRightSaber;

        private readonly GameObject _prefab;

        public AdditionalInstanceHandler(GameObject prefab)
        {
            _prefab = prefab;
        }

        public GameObject GetInstance()
        {
            if (!_instance)
            {
                Instantiate();
            }

            return _instance;
        }

        public void Destroy()
        {
            _instance.TryDestroy();
        }

        public GameObject GetSaber(ESaberSlot saberSlot)
        {
            if ((saberSlot == ESaberSlot.Left && !_customSaberLeftSaber) ||
                (saberSlot == ESaberSlot.Right && !_customSaberRightSaber))
            {
                var saber = FindInInstance(saberSlot == ESaberSlot.Left ? "LeftSaber" : "RightSaber");

                if (saber) return saber.gameObject;
                return null;
            }

            return saberSlot == ESaberSlot.Left ? _customSaberLeftSaber : _customSaberRightSaber;
        }

        public Transform FindInInstance(string name)
        {
            return GetInstance().transform.Find(name);
        }

        private void Instantiate()
        {
            _instance = Object.Instantiate(_prefab);
            _instance.name = "Additional Instances";
            _customSaberLeftSaber = GetSaber(ESaberSlot.Left);
            _customSaberRightSaber = GetSaber(ESaberSlot.Right);

            _customSaberLeftSaber.SetActive(false);
            _customSaberRightSaber.SetActive(false);
        }
    }
}