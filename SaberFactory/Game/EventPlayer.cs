using System;
using System.Collections;
using System.Collections.Generic;
using IPA.Utilities;
using ModestTree;
using SaberFactory.Configuration;
using SaberFactory.Models;
using UnityEngine;
using Zenject;

namespace SaberFactory.Game
{
    internal class EventPlayer : IDisposable
    {
        private static readonly FieldAccessor<ScoreController, int>.Accessor _scoreControllerNotes =
            FieldAccessor<ScoreController, int>.GetAccessor("_cutOrMissedNotes");

        [Inject] private readonly BeatmapObjectManager _beatmapObjectManager = null;

        [Inject] private readonly GameEnergyCounter _energyCounter = null;

        [InjectOptional] private ObstacleSaberSparkleEffectManager _obstacleSaberSparkleEffectManager;

        [Inject] private readonly PluginConfig _pluginConfig = null;

        [Inject] private readonly ScoreController _scoreController = null;

        [Inject] private readonly MonoKernel _monoKernel = null;

        private bool _didInit;

        [Inject(Id = "LastNoteId")] private float _lastNoteTime;
        private List<PartEvents> _partEventsList;
        private SaberType _saberType;

        public void Dispose()
        {
            if (_didInit)
            {
                _beatmapObjectManager.noteWasCutEvent -= OnNoteCut;
                _beatmapObjectManager.noteWasMissedEvent -= OnNoteMiss;

                _obstacleSaberSparkleEffectManager.sparkleEffectDidStartEvent -= SaberStartCollide;
                _obstacleSaberSparkleEffectManager.sparkleEffectDidEndEvent -= SaberEndCollide;

                _energyCounter.gameEnergyDidReach0Event -= InvokeOnLevelFail;

                _scoreController.multiplierDidChangeEvent -= MultiplayerDidChange;

                _scoreController.comboDidChangeEvent -= InvokeComboChanged;
            }
        }

        public void SetPartEventList(List<PartEvents> partEventsList, SaberType saberType)
        {
            _partEventsList = partEventsList;
            _saberType = saberType;

            if (!_pluginConfig.EnableEvents)
            {
                return;
            }

            _didInit = true;

            // OnSlice LevelEnded Combobreak
            _beatmapObjectManager.noteWasCutEvent += OnNoteCut;
            _beatmapObjectManager.noteWasMissedEvent += OnNoteMiss;

            // Sabers clashing
            if (_obstacleSaberSparkleEffectManager == null)
            {
                _obstacleSaberSparkleEffectManager = _monoKernel.GetComponentInChildren<ObstacleSaberSparkleEffectManager>();
            }
            _obstacleSaberSparkleEffectManager.sparkleEffectDidStartEvent += SaberStartCollide;
            _obstacleSaberSparkleEffectManager.sparkleEffectDidEndEvent += SaberEndCollide;

            // OnLevelFail
            _energyCounter.gameEnergyDidReach0Event += InvokeOnLevelFail;

            // MultiplierUp
            _scoreController.multiplierDidChangeEvent += MultiplayerDidChange;

            // Combo changed
            _scoreController.comboDidChangeEvent += InvokeComboChanged;

            InvokeOnLevelStart();
        }

        public void InvokeLevelEnded()
        {
            foreach (var partEvents in _partEventsList)
            {
                partEvents.OnLevelEnded?.Invoke();
            }
        }

        public void InvokeCombobreak()
        {
            foreach (var partEvents in _partEventsList)
            {
                partEvents.OnComboBreak?.Invoke();
            }
        }

        public void InvokeOnLevelFail()
        {
            foreach (var partEvents in _partEventsList)
            {
                partEvents.OnLevelFail?.Invoke();
            }
        }

        public void InvokeMultiplierUp()
        {
            foreach (var partEvents in _partEventsList)
            {
                partEvents.MultiplierUp?.Invoke();
            }
        }

        public void InvokeOnSlice()
        {
            foreach (var partEvents in _partEventsList)
            {
                partEvents.OnSlice?.Invoke();
            }
        }

        public void InvokeSaberStartColliding()
        {
            foreach (var partEvents in _partEventsList)
            {
                partEvents.SaberStartColliding?.Invoke();
            }
        }

        public void InvokeSaberStopColliding()
        {
            foreach (var partEvents in _partEventsList)
            {
                partEvents.SaberStopColliding?.Invoke();
            }
        }

        public void InvokeOnLevelStart()
        {
            foreach (var partEvents in _partEventsList)
            {
                partEvents.OnLevelStart?.Invoke();
            }
        }

        public void InvokeComboChanged(int combo)
        {
            foreach (var partEvents in _partEventsList)
            {
                partEvents.OnComboChanged?.Invoke(combo);
            }
        }

        public void InvokeAccuracyChanged(float accuracy)
        {
            foreach (var partEvents in _partEventsList)
            {
                partEvents.OnAccuracyChanged?.Invoke(accuracy);
            }
        }

        #region Events

        private void OnNoteCut(NoteController noteController, in NoteCutInfo noteCutInfo)
        {
            if (!noteCutInfo.allIsOK)
            {
                InvokeCombobreak();
            }
            else
            {
                if (_saberType == noteCutInfo.saberType)
                {
                    InvokeOnSlice();
                }
            }

            FireAccuracyEvents();

            if (Mathf.Approximately(noteController.noteData.time, _lastNoteTime))
            {
                _lastNoteTime = 0;
                InvokeLevelEnded();
            }
        }

        private void OnNoteMiss(NoteController noteController)
        {
            if (noteController.noteData.colorType != ColorType.None)
            {
                InvokeCombobreak();
            }

            if (Mathf.Approximately(noteController.noteData.time, _lastNoteTime))
            {
                _lastNoteTime = 0;
                InvokeLevelEnded();
            }

            FireAccuracyEvents();
        }

        private void SaberEndCollide(SaberType saberType)
        {
            if (saberType == _saberType)
            {
                InvokeSaberStopColliding();
            }
        }

        private void SaberStartCollide(SaberType saberType)
        {
            if (saberType == _saberType)
            {
                InvokeSaberStartColliding();
            }
        }

        private void MultiplayerDidChange(int multiplier, float progress)
        {
            if (multiplier > 1 && progress < 0.1f)
            {
                InvokeMultiplierUp();
            }
        }

        private IEnumerator CalculateAccuracyAndFireEventsCoroutine()
        {
            yield return null;

            var scoreController = _scoreController;

            var rawScore = scoreController.prevFrameRawScore;
            var maxScore = ScoreModel.MaxRawScoreForNumberOfNotes(_scoreControllerNotes(ref scoreController));
            var accuracy = rawScore / (float)maxScore;

            InvokeAccuracyChanged(accuracy);
        }

        private void FireAccuracyEvents()
        {
            SharedCoroutineStarter.instance.StartCoroutine(CalculateAccuracyAndFireEventsCoroutine());
        }

        #endregion
    }
}