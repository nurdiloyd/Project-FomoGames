using System.Collections.Generic;
using Main.Scripts.General;
using Main.Scripts.Utils;

namespace Main.Scripts.Game
{
    public class Board
    {
        public const float CellWidth = 1;
        private const int NoBlock = -1;
        
        public bool IsThereAnyBlock => Blocks.Count > 0;
        
        public readonly int RowCount;
        public readonly int ColumnCount;
        public readonly Dictionary<int, Block> Blocks = new();
        public readonly Dictionary<int, Gate> Gates = new();
        
        private readonly Cell[,] _board;
        private readonly int _boardTop;
        private readonly int _boardRight;
        private readonly int _boardBottom;
        private readonly int _boardLeft;
        
        public Board(LevelData levelData)
        {
            RowCount = levelData.RowCount;
            ColumnCount = levelData.ColCount;
            
            _boardTop = 0;
            _boardRight = ColumnCount - 1;
            _boardBottom = RowCount - 1;
            _boardLeft = 0;
            
            _board = new Cell[RowCount, ColumnCount];
            for (var i = 0; i < RowCount; i++)
            {
                for (var j = 0; j < ColumnCount; j++)
                {
                    _board[i, j] = new Cell(NoBlock);
                }
            }
            
            CreateGates(levelData.ExitInfo);
            CreateBlocks(levelData.MovableInfo);
        }
        
        public Board(Board refBoard)
        {
            RowCount = refBoard.RowCount;
            ColumnCount = refBoard.ColumnCount;
            
            _boardTop = refBoard._boardTop;
            _boardRight = refBoard._boardRight;
            _boardBottom = refBoard._boardBottom;
            _boardLeft = refBoard._boardLeft;
            
            _board = new Cell[RowCount, ColumnCount];
            for (var i = 0; i < RowCount; i++)
            {
                for (var j = 0; j < ColumnCount; j++)
                {
                    var refCell = refBoard._board[i, j];
                    var cell = new Cell(refCell.BlockID);
                    if (refCell.Gates != null)
                    {
                        foreach (var refCellGate in refCell.Gates)
                        {
                            var gate = new Gate(refCellGate);
                            Gates.Add(gate.ID, gate);
                            
                            cell.Gates ??= new List<Gate>();
                            cell.Gates.Add(gate);
                        }
                    }
                    
                    _board[i, j] = cell;
                }
            }
            
            foreach (var refBlock in refBoard.Blocks.Values)
            {
                var block = new Block(refBlock);
                Blocks.Add(block.ID, block);
            }
        }
        
        private void CreateBlocks(MovableInfo[] movableInfos)
        {
            for (var index = 0; index < movableInfos.Length; index++)
            {
                var movableInfo = movableInfos[index];
                var direction = movableInfo.Direction[0].ToBlockDirection();
                var color = movableInfo.Colors.ToBlockColor();
                var i = movableInfo.Row;
                var j = movableInfo.Col;
                var length = movableInfo.Length;
                
                var block = new Block(index, length, direction, color);
                Blocks.Add(block.ID, block);
                
                PlaceBlock(block.ID, i, j);
            }
        }
        
        private void CreateGates(ExitInfo[] exitInfos)
        {
            for (var index = 0; index < exitInfos.Length; index++)
            {
                var exitInfo = exitInfos[index];
                var direction = exitInfo.Direction;
                var color = exitInfo.Colors.ToBlockColor();
                var i = exitInfo.Row;
                var j = exitInfo.Col;
                var iOffset = direction == 0 ? -1 : direction == 2 ? 1 : 0;
                var jOffset = direction == 1 ? 1 : direction == 3 ? -1 : 0;
                var pivotI = i + iOffset;
                var pivotJ = j + jOffset;
                
                var gate = new Gate(index, direction.ToBlockDirection(), color, pivotI, pivotJ);
                Gates.Add(gate.ID, gate);
                
                var cell = _board[i, j];
                cell.Gates ??= new List<Gate>();
                cell.Gates.Add(gate);
            }
        }
        
        public void ReplaceBlock(int id, int targetI, int targetJ)
        {
            RemoveBlock(id);
            PlaceBlock(id, targetI, targetJ);
        }
        
        public void ExitBlock(int id)
        {
            RemoveBlock(id);
            Blocks.Remove(id);
        }
        
        private void PlaceBlock(int id, int pivotI, int pivotJ)
        {
            var block = GetBlock(id);
            var rowCount = block.RowCount;
            var columnCount = block.ColumnCount;
            block.SetPivot(pivotI, pivotJ);
            
            SetBoardCells(pivotI, pivotJ, rowCount, columnCount, block.ID);
        }
        
        private void RemoveBlock(int id)
        {
            var block = GetBlock(id);
            var rowCount = block.RowCount;
            var columnCount = block.ColumnCount;
            var pivotI = block.PivotI;
            var pivotJ = block.PivotJ;
            block.SetPivot(-1, -1);
            
            SetBoardCells(pivotI, pivotJ, rowCount, columnCount, NoBlock);
        }
        
        private void SetBoardCells(int pivotI, int pivotJ, int rowCount, int columnCount, int id)
        {
            var iOffset = pivotI + rowCount;
            var jOffset = pivotJ + columnCount;
            
            for (var i = pivotI; i < iOffset; i++)
            {
                for (var j = pivotJ; j < jOffset; j++)
                {
                    _board[i, j].BlockID = id;
                }
            }
        }
        
        public bool GetTargetIndex(int id, BlockDirection moveDirection,
            out int targetI, out int targetJ,
            out float outsideI, out float outsideJ, 
            out Gate gate)
        {
            var block = GetBlock(id);
            var pivotI = block.PivotI;
            var pivotJ = block.PivotJ;
            var rowCount = block.RowCount;
            var columnCount = block.ColumnCount;
            var blockColor = block.BlockColor;
            
            targetI = pivotI;
            targetJ = pivotJ;
            outsideI = -1;
            outsideJ = -1;
            gate = null;
            var willExit = false;
            var outsideOffset = 0.5f;
            
            if (moveDirection == BlockDirection.Down)
            {
                var j = pivotJ;
                var noBlock = true;
                var maxI = _boardBottom + 1;
                for (var i = pivotI + rowCount; i <= _boardBottom; i++)
                {
                    var cell = _board[i, j];
                    if (cell.BlockID != NoBlock)
                    {
                        if (maxI > i)
                        {
                            maxI = i;
                        }
                        
                        noBlock = false;
                        
                        break;
                    }
                }
                
                targetI = maxI - rowCount;
                targetJ = pivotJ;
                
                if (noBlock)
                {
                    var gates = _board[_boardBottom, targetJ].Gates;
                    if (CanExit(gates, out gate))
                    {
                        outsideI = _boardBottom + 1 + outsideOffset;
                        outsideJ = targetJ;
                        willExit = true;
                    }
                }
            }
            else if (moveDirection == BlockDirection.Up)
            {
                var j = pivotJ;
                var noBlock = true;
                var minI = _boardTop - 1;
                for (var i = pivotI - 1; i >= _boardTop; i--)
                {
                    var cell = _board[i, j]; 
                    if (cell.BlockID != NoBlock)
                    {
                        if (minI < i)
                        {
                            minI = i;
                        }
                        
                        noBlock = false;
                        
                        break;
                    }
                }
                
                targetI = minI + 1;
                targetJ = pivotJ;
                
                if (noBlock)
                {
                    var gates = _board[_boardTop, targetJ].Gates;
                    if (CanExit(gates, out gate))
                    {
                        outsideI = _boardTop - rowCount - outsideOffset;
                        outsideJ = targetJ;
                        willExit = true;
                    }
                }
            }
            else if (moveDirection == BlockDirection.Right)
            {
                var i = pivotI;
                var noBlock = true;
                var maxJ = _boardRight + 1;
                for (var j = pivotJ + columnCount; j <= _boardRight; j++)
                {
                    var cell = _board[i, j]; 
                    if (cell.BlockID != NoBlock)
                    {
                        if (maxJ > j)
                        {
                            maxJ = j;
                        }
                        
                        noBlock = false;
                        
                        break;
                    }
                }
                
                targetI = pivotI;
                targetJ = maxJ - columnCount;
                
                if (noBlock)
                {
                    var gates = _board[targetI, _boardRight].Gates;
                    if (CanExit(gates, out gate))
                    {
                        outsideI = targetI;
                        outsideJ = _boardRight + 1 + outsideOffset;
                        willExit = true;
                    }
                }
            }
            else if (moveDirection == BlockDirection.Left)
            {
                var i = pivotI;
                var noBlock = true;
                var minJ = _boardLeft - 1;
                for (var j = pivotJ - 1; j >= _boardLeft; j--)
                {
                    var cell = _board[i, j]; 
                    if (cell.BlockID != NoBlock)
                    {
                        if (minJ < j)
                        {
                            minJ = j;
                        }
                        
                        noBlock = false;
                        
                        break;
                    }
                }
                
                targetI = pivotI;
                targetJ = minJ + 1;
                
                if (noBlock)
                {
                    var gates = _board[targetI, _boardLeft].Gates;
                    if (CanExit(gates, out gate))
                    {
                        outsideI = targetI;
                        outsideJ = _boardLeft - columnCount - outsideOffset;
                        willExit = true;
                    }
                }
            }
            
            return willExit;
            
            bool CanExit(List<Gate> gates, out Gate gate)
            {
                gate = null;
                if (gates != null)
                {
                    for (var m = 0; m < gates.Count; m++)
                    {
                        var gateTmp = gates[m];
                        if (gateTmp.GateColor == blockColor && gateTmp.GateDirection == moveDirection)
                        {
                            gate = gateTmp;
                            return true;
                        }
                    }
                }
                
                return false;
            }
        }
        
        public bool TryMoveBlock(int id, BlockDirection moveDirection)
        {
            var block = GetBlock(id);
            var willExit = GetTargetIndex(block.ID, moveDirection, out var targetI, out var targetJ, out _, out _, out _);
            var isMoved = !(targetI == block.PivotI && targetJ == block.PivotJ);
            
            if (willExit)
            {
                ExitBlock(block.ID);
            }
            else
            {
                ReplaceBlock(block.ID, targetI, targetJ);
            }
            
            return isMoved;
        }
        
        public Block GetBlock(int id)
        {
            return Blocks[id];
        }
    }
    
    public class Cell
    {
        public int BlockID;
        public List<Gate> Gates;
        
        public Cell(int id)
        {
            BlockID = id;
        }
    }
}
