using System;
using DG.Tweening;
using Main.Scripts.General;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Main.Scripts.Game
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
            var dataManager = ContextController.Instance.DataManager;
            var levelData = dataManager.GetCurrentLevelData();
            _gameBoard.Clear();
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
            var willExit = _gameBoard.GetTargetIndex(id, moveDirection, 
                out var targetI, out var targetJ, 
                out var outsideI, out var outsideJ, 
                out var gateView);
            
            var isMoved = !(targetI == pivotI && targetJ == pivotJ);
            if (isMoved)
            {
                DecreaseMoveCount();
            }
            
            var targetPosition = _gameBoard.GetCellPosition(targetI, targetJ);
            var difference = Math.Max(Math.Abs(targetI - pivotI), Math.Abs(targetJ - pivotJ));
            var duration = 0.14f * difference;
            
            var seq = DOTween.Sequence().SetLink(blockView.gameObject);
            if (willExit)
            {
                _gameBoard.ExitBlock(blockView.ID);
                
                var outsidePosition = _gameBoard.GetCellPosition(outsideI, outsideJ);
                var difference2 = Math.Max(Math.Abs(outsideI - targetI), Math.Abs(outsideJ - targetJ));
                var duration2 = 0.1f * difference2;
                
                seq.AppendCallback(gateView.Open);
                seq.Append(blockView.transform.DOMove((targetPosition + outsidePosition) / 2f, duration).SetEase(Ease.OutExpo));
                seq.Append(blockView.transform.DOMove(outsidePosition, duration2).SetEase(Ease.Linear));
                seq.AppendInterval(0.1f);
                seq.AppendCallback(() => Object.Destroy(blockView.gameObject));
            }
            else
            {
                _gameBoard.ReplaceBlock(blockView.ID, targetI, targetJ);
                
                seq.Append(blockView.transform.DOMove(targetPosition, duration).SetEase(Ease.OutExpo));
            }
            
            var isWin = !_gameBoard.IsThereAnyBlock;
            var isLose = !HasMove;
            if (isWin)
            {
                ContextController.Instance.DataManager.IncreaseCurrentLevelIndex();
                seq.AppendCallback(_gameUI.ShowLevelWinDialog);
            }
            else if (isLose)
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
    }
}
