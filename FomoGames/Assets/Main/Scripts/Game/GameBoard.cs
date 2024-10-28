using System.Collections.Generic;
using UnityEngine;

namespace Main.Scripts
{
    public class GameBoard
    {
        public const float CellWidth = 1;
        private const int NoBlock = -1;
        
        private BoardManager _boardManager;
        private Cell[,] _board;
        private readonly Dictionary<int, BlockView> _blocks = new();
        private Vector3 _initialPosition;
        private int _boardTop;
        private int _boardRight;
        private int _boardBottom;
        private int _boardLeft;
        private int _rowCount;
        private int _columnCount;
        private Transform _boardParent;
        
        public void Init(LevelData levelData)
        {
            _boardManager = GameController.Instance.BoardManager;
            _boardParent = new GameObject("Board").transform;
            
            _rowCount = levelData.RowCount;
            _columnCount = levelData.ColCount;
            
            _boardTop = 0;
            _boardRight = _columnCount - 1;
            _boardBottom = _rowCount - 1;
            _boardLeft = 0;
            
            _initialPosition = new Vector3((1 - _columnCount) / 2f, 0f, -(1 - _rowCount) / 2f) * CellWidth;
            _board = new Cell[_rowCount, _columnCount];
            for (var i = 0; i < _rowCount; i++)
            {
                for (var j = 0; j < _columnCount; j++)
                {
                    _board[i, j] = new Cell(NoBlock);
                }
            }
            
            SpawnBlocks(levelData.MovableInfo);
            SpawnGates(levelData.ExitInfo);
        }
        
        private void SpawnBlocks(MovableInfo[] movableInfos)
        {
            for (var index = 0; index < movableInfos.Length; index++)
            {
                var movableInfo = movableInfos[index];
                var direction = movableInfo.Direction[0];
                var i = movableInfo.Row;
                var j = movableInfo.Col;
                var length = movableInfo.Length;
                var color = movableInfo.Colors.ToBlockColor();
                var position = GetCellPosition(i, j);
                var rotation = Quaternion.Euler(0f, 90f * ((direction + 1) % 2), 0f);
                var blockPrefab = _boardManager.BoardAssets.GetBlockPrefab(length);
                
                var blockView = Object.Instantiate(blockPrefab, position, rotation, _boardParent);
                blockView.Init(index, length, direction.ToBlockDirection(), color);
                _blocks.Add(blockView.ID, blockView);
                
                PlaceBlock(blockView.ID, i, j);
            }
        }
        
        public void PlaceBlock(int id, int pivotI, int pivotJ)
        {
            var blockView = GetBlock(id);
            var rowCount = blockView.RowCount;
            var columnCount = blockView.ColumnCount;
            blockView.SetPivot(pivotI, pivotJ);
            
            SetBoardCells(pivotI, pivotJ, rowCount, columnCount, id);
        }
        
        public void RemoveBlock(int id)
        {
            var blockView = GetBlock(id);
            var rowCount = blockView.RowCount;
            var columnCount = blockView.ColumnCount;
            var pivotI = blockView.PivotI;
            var pivotJ = blockView.PivotJ;
            blockView.SetPivot(-1, -1);
            
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
        
        private void SpawnGates(ExitInfo[] exitInfos)
        {
            for (var index = 0; index < exitInfos.Length; index++)
            {
                var exitInfo = exitInfos[index];
                var direction = exitInfo.Direction;
                var iOffset = direction == 0 ? -1 : direction == 2 ? 1 : 0;
                var jOffset = direction == 1 ? 1 : direction == 3 ? -1 : 0;
                var i = exitInfo.Row;
                var j = exitInfo.Col;
                var position = GetCellPosition(i + iOffset, j + jOffset);
                var rotation = Quaternion.Euler(0f, 90f * direction, 0f);
                var gateColor = exitInfo.Colors.ToBlockColor();
                var gatePrefab = _boardManager.BoardAssets.GetGatePrefab();
                
                var gateView = Object.Instantiate(gatePrefab, position, rotation, _boardParent);
                gateView.Init(direction.ToBlockDirection(), gateColor);
                
                PlaceGateToBoard(i, j, gateView);
            }
        }
        
        private void PlaceGateToBoard(int pivotI, int pivotJ, GateView gateView)
        {
            var cell = _board[pivotI, pivotJ];
            cell.Gates ??= new List<GateView>();
            cell.Gates.Add(gateView);
        }
        
        public BlockView GetBlock(int id)
        {
            return _blocks[id];
        }
        
        public Vector3 GetCellPosition(int i, int j)
        {
            return _initialPosition + new Vector3(j, 0f, -i) * CellWidth;
        }
        
        public (int i, int j) GetTargetIndex(int pivotI, int pivotJ, int rowCount, int columnCount, BlockDirection blockDirection, BlockColor blockColor, out int outsideI, out int outsideJ, out bool goOutside)
        {
            var targetI = pivotI;
            var targetJ = pivotJ;
            outsideI = targetI;
            outsideJ = targetJ;
            goOutside = false;
            
            if (blockDirection == BlockDirection.Down)
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
                
                outsideI = targetI;
                outsideJ = targetJ;
                
                if (noBlock)
                {
                    var gates = _board[_boardBottom, targetJ].Gates;
                    if (CanExit(gates))
                    {
                        outsideI = _boardBottom + 1;
                        outsideJ = targetJ;
                        goOutside = true;
                    }
                }
            }
            else if (blockDirection == BlockDirection.Up)
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
                
                outsideI = targetI;
                outsideJ = targetJ;
                
                if (noBlock)
                {
                    var gates = _board[_boardTop, targetJ].Gates;
                    if (CanExit(gates))
                    {
                        outsideI = _boardTop - rowCount;
                        outsideJ = targetJ;
                        goOutside = true;
                    }
                }
            }
            else if (blockDirection == BlockDirection.Right)
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
                
                outsideI = targetI;
                outsideJ = targetJ;
                
                if (noBlock)
                {
                    var gates = _board[targetI, _boardRight].Gates;
                    if (CanExit(gates))
                    {
                        outsideI = targetI;
                        outsideJ = _boardRight + 1;
                        goOutside = true;
                    }
                }
            }
            else if (blockDirection == BlockDirection.Left)
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
                
                outsideI = targetI;
                outsideJ = targetJ;
                
                if (noBlock)
                {
                    var gates = _board[targetI, _boardLeft].Gates;
                    if (CanExit(gates))
                    {
                        outsideI = targetI;
                        outsideJ = _boardLeft - columnCount;
                        goOutside = true;
                    }
                }
            }
            
            return (targetI, targetJ);
            
            bool CanExit(List<GateView> gates)
            {
                if (gates != null)
                {
                    for (var m = 0; m < gates.Count; m++)
                    {
                        var gate = gates[m];
                        if (gate.GateColor == blockColor && gate.GateDirection == blockDirection)
                        {
                            return true;
                        }
                    }
                }
                
                return false;
            }
        }
        
        public void DestroyBlock(int id)
        {
            var blockView = GetBlock(id);
            _blocks.Remove(id);
            Object.Destroy(blockView.gameObject);
        }
    }
    
    public class Cell
    {
        public int BlockID;
        public List<GateView> Gates;
        
        public Cell(int id)
        {
            BlockID = id;
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
