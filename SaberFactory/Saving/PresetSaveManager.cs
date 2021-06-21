using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SaberFactory.DataStore;
using SaberFactory.Helpers;
using SaberFactory.Models;
using SaberFactory.Models.CustomSaber;
using UnityEngine;
using Zenject;

namespace SaberFactory.Saving
{
    internal class PresetSaveManager
    {
        public event Action OnSaberLoaded;

        private readonly MainAssetStore _mainAssetStore;
        private readonly TextureStore _textureStore;
        private readonly DirectoryInfo _presetDir;

        private PresetSaveManager(MainAssetStore mainAssetStore, TextureStore textureStore, SFDirectories sfDirs)
        {
            _mainAssetStore = mainAssetStore;
            _textureStore = textureStore;
            _presetDir = sfDirs.PresetDir;

            SerMapper.CreateEntry<TrailModel, SerializableTrail>();
            SerMapper.CreateEntry<SaberModel, SerializableSaber>();
        }

        public void SaveSaber(SaberSet saberSet, string fileName)
        {
            var serializableSaberSet = new SerializableSaberSet();
            serializableSaberSet.SaberLeft = GetSerializableSaber(saberSet.LeftSaber);
            serializableSaberSet.SaberRight = GetSerializableSaber(saberSet.RightSaber);
            var file = _presetDir.GetFile(fileName);
            QuickSave.SaveObject(serializableSaberSet, file.FullName);
        }

        private SerializableSaber GetSerializableSaber(SaberModel saberModel)
        {
            if (saberModel == null) return null;

            var serializableSaber = new SerializableSaber();

            SerMapper.Map(saberModel, serializableSaber);

            var pieceList = new List<SerializablePiece>();
            foreach (BasePieceModel pieceModel in saberModel.PieceCollection)
            {
                var piece = new SerializablePiece {Path = pieceModel.StoreAsset.RelativePath};

                var tProp = pieceModel.PropertyBlock.TransformProperty;
                var transform = new SerializableTransform
                {
                    Offset = tProp.Offset,
                    Rotation = tProp.Rotation,
                    Width = tProp.Width
                };

                piece.Transform = transform;

                pieceList.Add(piece);
            }

            serializableSaber.Pieces = pieceList;

            var trailModel = saberModel.GetTrailModel();
            if (trailModel != null)
            {
                var trail = new SerializableTrail();

                SerMapper.Map(trailModel, trail);

                if (trailModel.Material?.Material != null)
                {
                    trail.Material = SerializableMaterial.FromMaterial(trailModel.Material.Material);
                }

                serializableSaber.Trail = trail;
            }

            return serializableSaber;
        }

        public async Task LoadSaber(SaberSet saberSet, string fileName)
        {
            var file = _presetDir.GetFile(fileName);
            if (!file.Exists) return;
            var data = await Readers.BytesToString(await Readers.ReadFileAsync(file.FullName));
            var serializableSaberSet = JsonConvert.DeserializeObject<SerializableSaberSet>(data);
            await LoadSaberModel(saberSet.LeftSaber, serializableSaberSet.SaberLeft);
            await LoadSaberModel(saberSet.RightSaber, serializableSaberSet.SaberRight);

            OnSaberLoaded?.Invoke();
        }

        private async Task LoadSaberModel(SaberModel saberModel, SerializableSaber serializableSaber)
        {
            SerMapper.Map(serializableSaber, saberModel);

            if (_mainAssetStore.IsLoading) await _mainAssetStore.CurrentTask;

            foreach (var piece in serializableSaber.Pieces)
            {
                var comp = await _mainAssetStore[piece.Path];
                if (comp != null)
                {
                    saberModel.PieceCollection.AddPiece(comp.AssetTypeDefinition, comp.GetPiece(saberModel.SaberSlot));

                    var prop = comp.GetLeft()?.PropertyBlock?.TransformProperty;
                    if (piece.Transform != null && prop != null)
                    {
                        prop.Offset = piece.Transform.Offset;
                        prop.Rotation = piece.Transform.Rotation;
                        prop.Width = piece.Transform.Width;
                    }
                }
            }

            var trailModel
                =saberModel.GetCustomSaber(out var customsaber)
                ?customsaber.TrailModel
                :new TrailModel();

            var trail = serializableSaber.Trail;
            if (trail != null)
            {
                SerMapper.Map(trail, trailModel);

                // if trail comes from another saber
                if (!string.IsNullOrEmpty(trail.TrailOrigin))
                {
                    await LoadFromTrailOrigin(trailModel, trail.TrailOrigin);
                }

                // assign trailmodel to custom saber or saber factory saber
                // depending on which trail type is being used
                if (customsaber is null)
                {
                    saberModel.TrailModel = trailModel;
                }

                trail.Material?.ApplyToMaterial(trailModel.Material?.Material, ResolveTexture);

            }
        }

        private async Task<Texture2D> ResolveTexture(string name)
        {
            if (_textureStore.IsLoading) await _textureStore.CurrentLoadingTask;
            return (await _textureStore[name])?.Texture;
        }

        /// <summary>
        /// Load trail from another saber
        /// </summary>
        /// <param name="trailModel">TrailModel to load the other saber's data into</param>
        /// <param name="trailOrigin">Path of the other saber</param>
        /// <returns></returns>
        private async Task LoadFromTrailOrigin(TrailModel trailModel, string trailOrigin)
        {
            var comp = await _mainAssetStore[trailOrigin];
            var originTrailModel = (comp?.GetLeft() as CustomSaberModel)?.GrabTrail(true);
            if (originTrailModel == null) return;
            trailModel.Material.Material = originTrailModel.Material.Material;
        }
    }
}
