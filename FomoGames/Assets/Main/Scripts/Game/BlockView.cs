using UnityEngine;

namespace Main.Scripts
{
    public class BlockView : MonoBehaviour
    {
        [SerializeField] private MeshRenderer meshRenderer;
        
        public int ID { get; private set; }
        public int PivotI { get; private set; }
        public int PivotJ { get; private set; }
        public int RowCount { get; private set; }
        public int ColumnCount { get; private set; }
        public BlockColor BlockColor { get; private set; }
        private BlockDirection _blockDirection;
        
        public void Init(int id, int length, BlockDirection blockDirection, BlockColor color)
        {
            ID = id;
            _blockDirection = blockDirection;
            RowCount = blockDirection.IsHorizontal() ? 1 : length;
            ColumnCount = blockDirection.IsVertical() ? 1 : length;
            BlockColor = color;
            var gameManager = GameController.Instance.GameManager;
            meshRenderer.material.mainTexture = gameManager.BoardAssets.GetBlockTexture(length, BlockColor, _blockDirection.IsVertical());
        }
        
        public void SetPivot(int pivotI, int pivotJ)
        {
            PivotI = pivotI;
            PivotJ = pivotJ;
        }
        
        public bool CanMoveOnAxis(BlockDirection direction)
        {
            return _blockDirection == direction || _blockDirection == direction.GetInverse();
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
