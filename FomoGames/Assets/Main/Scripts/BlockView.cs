using DG.Tweening;
using UnityEngine;

namespace Main.Scripts
{
    public class BlockView : MonoBehaviour
    {
        [SerializeField] private MeshRenderer meshRenderer;
        
        private GameBoard _gameBoard;
        private int _id;
        public int PivotI { get; private set; }
        public int PivotJ { get; private set; }
        public int RowCount { get; private set; }
        public int ColumnCount { get; private set; }
        
        private Direction _direction;
        
        public void Init(GameBoard gameBoard, int id, int pivotI, int pivotJ, int length, Direction direction)
        {
            _id = id;
            PivotI = pivotI;
            PivotJ = pivotJ;
            _gameBoard = gameBoard;
            _direction = direction;
            RowCount = direction.IsHorizontal() ? 1 : length;
            ColumnCount = direction.IsVertical() ? 1 : length;
        }
        
        public bool CanMoveOnDirection(Direction direction)
        {
            return _direction == direction || _direction == direction.GetInverse();
        }
        
        public void Move(Direction direction)
        {
            var toPosition = _gameBoard.GetTargetPosition(_id, direction);
            
            transform.DOMove(toPosition, 0.1f).SetEase(Ease.Linear);
        }
        
        public void Select()
        {
            meshRenderer.material.color = Color.red;
        }
        
        public void Deselect()
        {
            meshRenderer.material.color = Color.white;
        }
    }
}
