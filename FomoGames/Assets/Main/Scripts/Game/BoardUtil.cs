using System.Collections.Generic;
using Main.Scripts.Utils;
using UnityEngine;

namespace Main.Scripts.Game
{
    public abstract class BoardUtil
    {
        public static int CalculateMinMoveCount(Board board)
        {
            var aa = 0;
            
            var queue = new Queue<Board>();
            queue.Enqueue(board);
            
            var visitedBoards = new HashSet<string>(); 
            var boardID = GetBoardID(board);
            visitedBoards.Add(boardID);
            
            while (queue.Count > 0 && aa < 10000)
            {
                var currentBoard = queue.Dequeue();
                if (!currentBoard.IsThereAnyBlock)
                {
                    return currentBoard.MoveCount;
                }
                
                aa += 1;
                
                var nextStateBoards = GetNextStateBoards(currentBoard);
                foreach (var nextStateBoard in nextStateBoards)
                {
                    var nextStateBoardID = GetBoardID(nextStateBoard);
                    if (visitedBoards.Contains(nextStateBoardID))// && nextStateBoard.MoveCount >= moveCount)
                    {
                        continue;
                    }
                    
                    visitedBoards.Add(nextStateBoardID);
                    queue.Enqueue(nextStateBoard);
                }
            }
            
            Debug.Log($"var {aa}");
            
            return -1;
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
