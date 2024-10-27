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
    }
}
