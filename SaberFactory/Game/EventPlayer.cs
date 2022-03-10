using System;
using System.Collections;
using System.Collections.Generic;
using BeatSaberMarkupLanguage.FloatingScreen;
using IPA.Utilities;
using ModestTree;
using SaberFactory.Configuration;
using SaberFactory.Helpers;
using SaberFactory.Models;
using SiraUtil.Logging;
using UnityEngine;
using Zenject;

namespace SaberFactory.Game
{
    internal class EventPlayer : IDisposable
    { 
        [Inject] private readonly BeatmapObjectManager _beatmapObjectManager = null;

        [Inject] private readonly GameEnergyCounter _energyCounter = null;

        [InjectOptional] private readonly ObstacleSaberSparkleEffectManager _obstacleSaberSparkleEffectManager = null;

        [Inject] private readonly PluginConfig _pluginConfig = null;

        [Inject] private readonly IScoreController _scoreController = null;

        [Inject] private readonly IComboController _comboController = null;
        
        [Inject] private readonly RelativeScoreAndImmediateRankCounter _scoreCounter = null;

        [Inject] private readonly IReadonlyBeatmapData _beatmapData = null;

        [InjectOptional] private readonly GameCoreSceneSetupData _gameCoreSceneSetupData = null;

        [Inject] private readonly SiraLog _logger = null;

        public bool IsActive;

        private float? _lastNoteTime;
        private List<PartEvents> _partEventsList;
        private SaberType _saberType;

        private float _prevScore;

        public void Dispose()
        {
            _beatmapObjectManager.noteWasCutEvent -= OnNoteCut;
            _beatmapObjectManager.noteWasMissedEvent -= OnNoteMiss;

            if (_obstacleSaberSparkleEffectManager)
            {
                _obstacleSaberSparkleEffectManager.sparkleEffectDidStartEvent -= SaberStartCollide;
                _obstacleSaberSparkleEffectManager.sparkleEffectDidEndEvent -= SaberEndCollide;
            }

            _energyCounter.gameEnergyDidReach0Event -= InvokeOnLevelFail;

            _scoreController.multiplierDidChangeEvent -= MultiplayerDidChange;

            _scoreCounter.relativeScoreOrImmediateRankDidChangeEvent -= ScoreChanged;

            _comboController.comboDidChangeEvent -= OnComboDidChangeEvent;
        }

        public void SetPartEventList(List<PartEvents> partEventsList, SaberType saberType)
        {
            _partEventsList = partEventsList;
            _saberType = saberType;
            
            if (!_pluginConfig.EnableEvents)
            {
                return;
            }

            if (_gameCoreSceneSetupData == null)
            {
                return;
            }

            IsActive = true;

            _lastNoteTime = _beatmapData.CastChecked<BeatmapData>()?.GetLastNoteTime();

            if (!_lastNoteTime.HasValue)
            {
                _logger.Warn("Couldn't get last note time. Certain level end events won't work");
            }

            // OnSlice LevelEnded Combobreak
            _beatmapObjectManager.noteWasCutEvent += OnNoteCut;
            _beatmapObjectManager.noteWasMissedEvent += OnNoteMiss;

            // Sabers clashing
            if (_obstacleSaberSparkleEffectManager)
            {
                _obstacleSaberSparkleEffectManager.sparkleEffectDidStartEvent += SaberStartCollide;
                _obstacleSaberSparkleEffectManager.sparkleEffectDidEndEvent += SaberEndCollide;
            }

            // OnLevelFail
            _energyCounter.gameEnergyDidReach0Event += InvokeOnLevelFail;

            // MultiplierUp
            _scoreController.multiplierDidChangeEvent += MultiplayerDidChange;

            // Accuracy changed
            _scoreCounter.relativeScoreOrImmediateRankDidChangeEvent += ScoreChanged;
            
            // Combo changed
            _comboController.comboDidChangeEvent += OnComboDidChangeEvent;

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

        private void ScoreChanged()
        {
            var score = _scoreCounter.relativeScore;
            if (Math.Abs(_prevScore - score) >= 0.001f)
            {
                InvokeAccuracyChanged(score);
                _prevScore = score;
            }
        }
        
        private void OnComboDidChangeEvent(int combo)
        {
            InvokeComboChanged(combo);
        }
        
        private void OnNoteCut(NoteController noteController, in NoteCutInfo noteCutInfo)
        {
            if (!_lastNoteTime.HasValue)
            {
                return;
            }
            
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

            if (Mathf.Approximately(noteController.noteData.time, _lastNoteTime.Value))
            {
                _lastNoteTime = 0;
                InvokeLevelEnded();
            }
        }

        private void OnNoteMiss(NoteController noteController)
        {
            if (!_lastNoteTime.HasValue)
            {
                return;
            }
            
            if (noteController.noteData.colorType != ColorType.None)
            {
                InvokeCombobreak();
            }

            if (Mathf.Approximately(noteController.noteData.time, _lastNoteTime.Value))
            {
                _lastNoteTime = 0;
                InvokeLevelEnded();
            }
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

        #endregion
    }
}