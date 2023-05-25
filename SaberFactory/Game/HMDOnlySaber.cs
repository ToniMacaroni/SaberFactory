using IPA.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Zenject;

namespace SaberFactory.Game
{
    internal class HMDOnlySaber : MonoBehaviour
    {

        PlayerTransforms _playerTransforms;
        public static GameObject _defaultSaberPrefab = null;

        [Inject]
        private void Init(PlayerTransforms playerTransforms)
        {
            //I do this this way because I get a null reference after the first decorate occurs, I assume it just gets replaced?

            _playerTransforms = playerTransforms;

            if (_defaultSaberPrefab != null) return;
            _defaultSaberPrefab = GameObject.Instantiate(playerTransforms.GetComponentInChildren<SaberModelContainer>().GetField<SaberModelController, SaberModelContainer>("_saberModelControllerPrefab")).gameObject;
            _defaultSaberPrefab.SetActive(false);
            _defaultSaberPrefab.name = "SaberVisualGOPrefab";
            _defaultSaberPrefab.transform.position = Vector3.zero;
            DontDestroyOnLoad(_defaultSaberPrefab);
        }

    }
}
