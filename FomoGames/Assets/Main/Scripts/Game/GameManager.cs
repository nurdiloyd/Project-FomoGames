using System;
using DG.Tweening;
using Main.Scripts.Game;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Main.Scripts
{
    public class GameManager : IContextUnit
    {
        public const int InfinityMove = 1000;
        public bool HasMove => _moveCount > 0;
        public BoardAssets BoardAssets { get; private set; }
        private GameUI _gameUI;
        private GameBoard _gameBoard;
        private int _moveCount;
        
        public void Bind()
        {
            BoardAssets = Resources.Load<BoardAssets>("BoardAssets");
            _gameBoard = new GameBoard();
        }
        
        public void SetGameUI(GameUI gameUI)
        {
            _gameUI = gameUI;
        }
        
        public void LoadLevel()
        {
            var dataManager = GameController.Instance.DataManager;
            var levelData = dataManager.GetCurrentLevelData();
            _gameBoard.Init(levelData);
            _moveCount = levelData.MoveLimit == 0 ? InfinityMove : levelData.MoveLimit;
            
            _gameUI.SetLevelText(dataManager.User.levelIndex + 1);
            _gameUI.SetMoveCountText(_moveCount);
        }
        
        public void MoveBlock(int id, BlockDirection moveDirection)
        {
            var blockView = _gameBoard.GetBlock(id);
            var pivotI = blockView.PivotI;
            var pivotJ = blockView.PivotJ;
            var (targetI, targetJ) = _gameBoard.GetTargetIndex(id, moveDirection,
                out var outsideI, out var outsideJ, out var goOutside, out var gateView);
            
            _gameBoard.RemoveBlock(blockView.ID);
            if (goOutside)
            {
                _gameBoard.ExitBlock(blockView.ID);
            }
            else
            {
                _gameBoard.PlaceBlock(blockView.ID, targetI, targetJ);
            }
            
            var isMoved = !(targetI == pivotI && targetJ == pivotJ);
            if (isMoved)
            {
                DecreaseMoveCount();
            }
            
            var targetPosition = _gameBoard.GetCellPosition(targetI, targetJ);
            var outsidePosition = _gameBoard.GetCellPosition(outsideI, outsideJ);
            
            var moveSpeed = 0.14f;
            var seq = DOTween.Sequence();
            if (goOutside)
            {
                var difference = Math.Max(Math.Abs(outsideI - pivotI), Math.Abs(outsideJ - pivotJ));
                var duration = moveSpeed * difference;
                seq.Append(blockView.transform.DOMove(outsidePosition, duration).SetEase(Ease.OutExpo));
                seq.InsertCallback(duration * 0.1f, gateView.Open);
                seq.AppendInterval(0.1f);
                seq.AppendCallback(() => Object.Destroy(blockView.gameObject));
            }
            else
            {
                var difference = Math.Max(Math.Abs(targetI - pivotI), Math.Abs(targetJ - pivotJ));
                var duration = moveSpeed * difference;
                seq.Append(blockView.transform.DOMove(targetPosition, duration).SetEase(Ease.OutExpo));
            }
            
            if (!_gameBoard.IsThereAnyBlock)
            {
                GameController.Instance.DataManager.IncreaseCurrentLevelIndex();
                seq.AppendCallback(_gameUI.ShowLevelWinDialog);
            }
            else if (!HasMove)
            {
                seq.AppendCallback(_gameUI.ShowLevelLoseDialog);
            }
        }
        
        private void DecreaseMoveCount()
        {
            if (_moveCount == InfinityMove)
            {
                return;
            }
            
            _moveCount -= 1;
            _gameUI.SetMoveCountText(_moveCount);
        }
        
        public void TryAgain()
        {
            _gameBoard.Clear();
            LoadLevel();
        }
        
        public void NextLevel()
        {
            _gameBoard.Clear();
            LoadLevel();
        }
    }
}
