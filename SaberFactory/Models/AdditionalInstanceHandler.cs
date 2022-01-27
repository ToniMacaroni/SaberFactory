using System;
using SaberFactory.Helpers;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SaberFactory.Models
{
    /// <summary>
    ///     Class for handling gameobject outside of the left and right sabers or parts
    ///     Mostly used for custom sabers
    /// </summary>
    public class AdditionalInstanceHandler
    {
        public bool IsInstantiated => _instance != null;

        private readonly GameObject _prefab;
        private readonly GameObject _fallbackRightSaber;
        private GameObject _customSaberLeftSaber;
        private GameObject _customSaberRightSaber;

        private GameObject _instance;

        public AdditionalInstanceHandler(GameObject prefab, GameObject fallbackRightSaber)
        {
            _prefab = prefab;
            _fallbackRightSaber = fallbackRightSaber;
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
            _instance.TryDestoryImmediate();
        }

        public GameObject GetSaber(ESaberSlot saberSlot)
        {
            if (saberSlot == ESaberSlot.Left && !_customSaberLeftSaber ||
                saberSlot == ESaberSlot.Right && !_customSaberRightSaber)
            {
                var saber = FindInInstance(saberSlot == ESaberSlot.Left ? "LeftSaber" : "RightSaber");

                if (saber)
                {
                    return saber.gameObject;
                }

                return null;
            }

            return saberSlot == ESaberSlot.Left ? _customSaberLeftSaber : _customSaberRightSaber;
        }

        public T GetComponent<T>() where T : Component
        {
            return GetInstance().GetComponent<T>();
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

            if (_customSaberRightSaber == null)
            {
                _customSaberRightSaber = Object.Instantiate(_fallbackRightSaber);
            }

            if (_customSaberLeftSaber != null)
            {
                _customSaberLeftSaber.SetActive(false);
            }

            if (_customSaberRightSaber != null)
            {
                _customSaberRightSaber.SetActive(false);
            }
        }
    }
}