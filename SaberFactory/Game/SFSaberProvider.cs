using System;
using SiraUtil.Interfaces;

namespace SaberFactory.Game
{
    public class SFSaberProvider : IModelProvider
    {
        public Type Type => typeof(SfSaberModelController);
        public int Priority => 300;
    }
}