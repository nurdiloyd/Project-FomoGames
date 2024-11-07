using System.Collections.Generic;
using Main.Scripts.Utils;

namespace Main.Scripts.Game
{
    public abstract class BoardUtil
    {
        public static int CalculateMinMoveCount(Board board)
        {
            var levelQueue = new Queue<Board>();
            levelQueue.Enqueue(board);
            
            var visitedBoards = new HashSet<string>();
            var boardID = GetBoardID(board);
            visitedBoards.Add(boardID);
            
            var isFindMinimum = false;
            var level = 0;
            while (true)
            {
                var nextLevel = new Queue<Board>();
                while (levelQueue.Count > 0)
                {
                    var currentBoard = levelQueue.Dequeue();
                    if (!currentBoard.IsThereAnyBlock)
                    {
                        isFindMinimum = true;
                        break;
                    }
                    
                    var nextStateBoards = GetNextStateBoards(currentBoard);
                    foreach (var nextStateBoard in nextStateBoards)
                    {
                        var nextStateBoardID = GetBoardID(nextStateBoard);
                        if (visitedBoards.Contains(nextStateBoardID))
                        {
                            continue;
                        }
                        
                        visitedBoards.Add(nextStateBoardID);
                        nextLevel.Enqueue(nextStateBoard);
                    }
                }
                
                if (nextLevel.Count <= 0 || isFindMinimum)
                {
                    break;
                }
                
                levelQueue = nextLevel;
                level += 1;
            }
            
            return isFindMinimum ? level : -1;
        }
        
        private static List<Board> GetNextStateBoards(Board board)
        {
            var nextStateBoards = new List<Board>();
            
            var blocks = board.Blocks.Values;
            foreach (var block in blocks)
            {
                var moveDirections = GetMoveDirections(block);
                foreach (var moveDirection in moveDirections)
                {
                    var nextStateBoard = new Board(board);
                    var isMoved = nextStateBoard.TryMoveBlock(block.ID, moveDirection);
                    if (isMoved)
                    {
                        nextStateBoards.Add(nextStateBoard);
                    }
                }
            }
            
            return nextStateBoards;
        }
        
        private static BlockDirection[] GetMoveDirections(Block block)
        {
            var moveDirections = new BlockDirection[2];
            
            moveDirections[0] = block.BlockDirection;
            moveDirections[1] = block.BlockDirection.GetInverse();
            
            return moveDirections;
        }
        
        private static string GetBoardID(Board board)
        {
            var id = "";
            
            var sortedBlocks = new List<Block>(board.Blocks.Values);
            sortedBlocks.Sort((a, b) => a.ID.CompareTo(b.ID));
            foreach (var block in sortedBlocks)
            {
                id += $"{block.ID}{block.PivotI}{block.PivotJ}|";
            }
            
            return id;
        }
    }
}
