using System;
using SaberFactory.Models;
using SiraUtil.Tools;

namespace SaberFactory.Instances
{
    internal class PieceInstanceManager
    {
        public event Action<BasePieceInstance> InstanceCreated;

        public BasePieceInstance CurrentPiece { get; private set; }

        private readonly SiraLog _logger;
        private readonly BasePieceInstance.Factory _pieceFactory;

        public PieceInstanceManager(SiraLog logger, BasePieceInstance.Factory pieceFactory)
        {
            _logger = logger;
            _pieceFactory = pieceFactory;
        }

        public BasePieceInstance CreatePiece(BasePieceModel model)
        {
            CurrentPiece = _pieceFactory.Create(model);
            InstanceCreated?.Invoke(CurrentPiece);
            return CurrentPiece;
        }
    }
}