namespace Main.Scripts
{
    public static class EnumExtensions
    {
        public static Direction GetInverse(this Direction direction)
        {
            return direction switch
            {
                Direction.Up => Direction.Down,
                Direction.Right => Direction.Left,
                Direction.Down => Direction.Up,
                Direction.Left => Direction.Right,
                _ => Direction.Up
            };
        }
        
        public static bool IsHorizontal(this Direction direction)
        {
            return direction == Direction.Left || direction == Direction.Right;
        }
        
        public static bool IsVertical(this Direction direction)
        {
            return direction == Direction.Up || direction == Direction.Down;
        }
        
        public static BlockColor ToBlockColor(this int value)
        {
            return (value + 1) switch
            {
                1 => BlockColor.Red,
                2 => BlockColor.Green,
                3 => BlockColor.Blue,
                4 => BlockColor.Yellow,
                5 => BlockColor.Purple,
                _ => BlockColor.None
            };
        }
    }
}
