using System;
using DG.Tweening;
using Main.Scripts.Game;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Main.Scripts
{
    public class GameManager : IContextUnit
    {
        public bool HasMove => _moveCount > 0;
        public BoardAssets BoardAssets { get; private set; }
        private GameUI _gameUI;
        private GameBoard _gameBoard;
        private int _moveCount;
        
        public void Bind()
        {
            BoardAssets = Resources.Load<BoardAssets>("BoardAssets");
            var levelData = GameController.Instance.DataManager.GetCurrentLevelData();
            _gameBoard = new GameBoard();
            _gameBoard.Init(levelData);
            _moveCount = 3;//levelData.MoveLimit;
        }
        
        public void SetGameUI(GameUI gameUI)
        {
            _gameUI = gameUI;
            _gameUI.SetLevelText(GameController.Instance.DataManager.CurrentLevelIndex + 1);
            _gameUI.SetMoveCountText(_moveCount);
        }
        
        public void MoveBlock(int id, BlockDirection moveDirection)
        {
            var blockView = _gameBoard.GetBlock(id);
            var pivotI = blockView.PivotI;
            var pivotJ = blockView.PivotJ;
            var (targetI, targetJ) = _gameBoard.GetTargetIndex(pivotI, pivotJ, blockView.RowCount, blockView.ColumnCount,
                moveDirection, blockView.BlockColor, out var outsideI, out var outsideJ, out var goOutside);
            
            var isMoved = !(targetI == pivotI && targetJ == pivotJ);
            
            _gameBoard.RemoveBlock(blockView.ID);
            if (goOutside)
            {
                _gameBoard.ExitBlock(blockView.ID);
            }
            else
            {
                _gameBoard.PlaceBlock(blockView.ID, targetI, targetJ);
            }
            
            if (isMoved)
            {
                DecreaseMoveCount();
            }
            
            var difference = Math.Max(Math.Abs(targetI - pivotI), Math.Abs(targetJ - pivotJ));
            var duration = 0.1f * difference;
            var targetPosition = _gameBoard.GetCellPosition(targetI, targetJ);
            var outsidePosition = _gameBoard.GetCellPosition(outsideI, outsideJ);
            
            var seq = DOTween.Sequence();
            seq.Append(blockView.transform.DOMove(targetPosition, duration).SetEase(Ease.OutBack));
            seq.Append(blockView.transform.DOMove(outsidePosition, duration).SetEase(Ease.InBack));
            if (goOutside)
            {
                seq.AppendCallback(() => Object.Destroy(blockView.gameObject));
            }
            seq.AppendCallback(CheckEndOfGame);
        }
        
        private void CheckEndOfGame()
        {
            if (!_gameBoard.IsThereAnyBlock)
            {
                _gameUI.ShowLevelWinDialog();
            }
            else if (!HasMove)
            {
                _gameUI.ShowLevelLoseDialog();
            }
        }
        
        private void DecreaseMoveCount()
        {
            _moveCount -= 1;
            _gameUI.SetMoveCountText(_moveCount);
        }
        
        public void TryAgain()
        {
        }
        
        public void NextLevel()
        {
        }
    }
}
