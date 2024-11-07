using DG.Tweening;
using Main.Scripts.General;
using UnityEngine;

namespace Main.Scripts.Game
{
    public class GameManager : IContextUnit
    {
        public const int InfinityMove = 1000;
        public bool HasMove => _moveCount > 0;
        public BoardAssets BoardAssets { get; private set; }
        private GameUI _gameUI;
        private GameBoardController _gameBoardController;
        private int _moveCount;
        
        public void Bind()
        {
            BoardAssets = Resources.Load<BoardAssets>("BoardAssets");
            _gameBoardController = new GameBoardController();
        }
        
        public void SetGameUI(GameUI gameUI)
        {
            _gameUI = gameUI;
        }
        
        public void LoadLevel()
        {
            _gameBoardController.Clear();
            var dataManager = ContextController.Instance.DataManager;
            var levelData = dataManager.GetCurrentLevelData();
            _gameBoardController.Init(levelData);
            _moveCount = levelData.MoveLimit == 0 ? InfinityMove : levelData.MoveLimit;
            
            _gameUI.SetLevelText(dataManager.User.levelIndex + 1);
            _gameUI.SetMoveCountText(_moveCount);
        }
        
        public void SelectBlock(int id, out Block block)
        {
            _gameBoardController.SelectBlock(id, out block);
        }
        
        public void DeselectBlock(int id)
        {
            _gameBoardController.DeselectBlock(id);
        }
        
        public void MoveBlock(int id, BlockDirection moveDirection)
        {
            var isMoved = _gameBoardController.TryMoveBlock(id, moveDirection, out var seq);
            if (isMoved)
            {
                DecreaseMoveCount();
                
                var isWin = !_gameBoardController.IsThereAnyBlock;
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
