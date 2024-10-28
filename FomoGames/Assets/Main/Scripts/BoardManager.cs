using System;
using DG.Tweening;
using UnityEngine;

namespace Main.Scripts
{
    public class BoardManager : IContextUnit
    {
        private GameBoard _gameBoard;
        public BoardAssets BoardAssets { get; private set; }
        
        public void Bind()
        {
            BoardAssets = Resources.Load<BoardAssets>("BoardAssets");
            var levelData = GameController.Instance.DataManager.GetCurrentLevelData();
            _gameBoard = new GameBoard();
            _gameBoard.Init(levelData);
        }
        
        public void MoveBlock(int id, BlockDirection moveDirection)
        {
            var blockView = _gameBoard.GetBlock(id);
            var pivotI = blockView.PivotI;
            var pivotJ = blockView.PivotJ;
            var (targetI, targetJ) = _gameBoard.GetTargetIndex(pivotI, pivotJ, blockView.RowCount, blockView.ColumnCount,
                moveDirection, blockView.BlockColor, out var outsideI, out var outsideJ, out var goOutside);
            var difference = Math.Max(Math.Abs(targetI - pivotI), Math.Abs(targetJ - pivotJ));
            var duration = 0.1f * difference;
            var targetPosition = _gameBoard.GetCellPosition(targetI, targetJ);
            var outsidePosition = _gameBoard.GetCellPosition(outsideI, outsideJ);
            
            var seq = DOTween.Sequence();
            seq.Append(blockView.transform.DOMove(targetPosition, duration).SetEase(Ease.OutBack));
            seq.Append(blockView.transform.DOMove(outsidePosition, duration).SetEase(Ease.InBack));
            if (goOutside)
            {
                seq.AppendCallback(() => _gameBoard.DestroyBlock(id));
            }
            
            _gameBoard.RemoveBlock(blockView.ID);
            if (!goOutside)
            {
                _gameBoard.PlaceBlock(blockView.ID, targetI, targetJ);
            }
        }
    }
}
