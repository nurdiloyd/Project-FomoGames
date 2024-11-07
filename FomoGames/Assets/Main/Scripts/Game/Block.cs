using Main.Scripts.Utils;

namespace Main.Scripts.Game
{
    public class Block
    {
        public readonly int ID;
        public readonly int RowCount;
        public readonly int ColumnCount;
        public readonly BlockColor BlockColor;
        public readonly BlockDirection BlockDirection;
        public readonly int Length;
        public int PivotI { get; private set; }
        public int PivotJ { get; private set; }
        
        public Block(int id, int length, BlockDirection blockDirection, BlockColor blockColor)
        {
            ID = id;
            BlockDirection = blockDirection;
            RowCount = blockDirection.IsHorizontal() ? 1 : length;
            ColumnCount = blockDirection.IsVertical() ? 1 : length;
            BlockColor = blockColor;
            Length = length;
        }
        
        public void SetPivot(int pivotI, int pivotJ)
        {
            PivotI = pivotI;
            PivotJ = pivotJ;
        }
        
        public bool CanMoveOnAxis(BlockDirection direction)
        {
            return BlockDirection == direction || BlockDirection == direction.GetInverse();
        }
    }
    
    public enum BlockDirection
    {
        Up,
        Right,
        Down,
        Left
    }
    
    public enum BlockColor
    {
        None,
        Red,
        Green,
        Blue,
        Yellow,
        Purple
    }
}
