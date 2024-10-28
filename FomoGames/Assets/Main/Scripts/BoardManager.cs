using System;
using DG.Tweening;
using UnityEngine;

namespace Main.Scripts
{
    public class BoardManager : MonoBehaviour
    {
        public GameBoard gameBoard;
        
        public void Bind()
        {
            var levelData = GameController.Instance.DataManager.GetCurrentLevelData();
            gameBoard.Init(levelData);
        }
        
        public void MoveBlock(int id, BlockDirection moveDirection)
        {
            var blockView = gameBoard.GetBlock(id);
            var pivotI = blockView.PivotI;
            var pivotJ = blockView.PivotJ;
            var (targetI, targetJ) = gameBoard.GetTargetIndex(pivotI, pivotJ, blockView.RowCount, blockView.ColumnCount,
                moveDirection, blockView.BlockColor, out var outsideI, out var outsideJ, out var goOutside);
            var difference = Math.Max(Math.Abs(targetI - pivotI), Math.Abs(targetJ - pivotJ));
            var duration = 0.1f * difference;
            var targetPosition = gameBoard.GetCellPosition(targetI, targetJ);
            var outsidePosition = gameBoard.GetCellPosition(outsideI, outsideJ);
            
            var seq = DOTween.Sequence();
            seq.Append(blockView.transform.DOMove(targetPosition, duration).SetEase(Ease.OutBack));
            seq.Append(blockView.transform.DOMove(outsidePosition, duration).SetEase(Ease.InBack));
            
            gameBoard.RemoveBlock(blockView.ID);
            if (!goOutside)
            {
                gameBoard.PlaceBlock(blockView.ID, targetI, targetJ);
            }
        }
    }
}
