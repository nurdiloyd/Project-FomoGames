using UnityEngine;

namespace Main.Scripts
{
    public static class EnumExtensions
    {
        public static BlockDirection GetInverse(this BlockDirection blockDirection)
        {
            return blockDirection switch
            {
                BlockDirection.Up => BlockDirection.Down,
                BlockDirection.Right => BlockDirection.Left,
                BlockDirection.Down => BlockDirection.Up,
                BlockDirection.Left => BlockDirection.Right,
                _ => BlockDirection.Up
            };
        }
        
        public static bool IsHorizontal(this BlockDirection blockDirection)
        {
            return blockDirection == BlockDirection.Left || blockDirection == BlockDirection.Right;
        }
        
        public static bool IsVertical(this BlockDirection blockDirection)
        {
            return blockDirection == BlockDirection.Up || blockDirection == BlockDirection.Down;
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
        
        public static BlockDirection ToBlockDirection(this int value)
        {
            return value switch
            {
                0 => BlockDirection.Up,
                1 => BlockDirection.Right,
                2 => BlockDirection.Down,
                3 => BlockDirection.Left,
                _ => BlockDirection.Up
            };
        }
        
        public static BlockDirection ToBlockDirection(this Vector2 moveDirection)
        {
            if (moveDirection == Vector2.up)
            {
                return BlockDirection.Up;
            }
            else if (moveDirection == Vector2.right)
            {
                return BlockDirection.Right;
            }
            else if (moveDirection == Vector2.down)
            {
                return BlockDirection.Down;
            }
            else if (moveDirection == Vector2.left)
            {
                return BlockDirection.Left;
            }
            else
            {
                return BlockDirection.Up;
            }
        }
    }
}
