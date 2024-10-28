using System.Collections.Generic;
using UnityEngine;

namespace Main.Scripts
{
    public class GameBoard : MonoBehaviour
    {
        [SerializeField] private Color colorBlue; 
        [SerializeField] private Color colorGreen; 
        [SerializeField] private Color colorPurple; 
        [SerializeField] private Color colorRed;
        [SerializeField] private Color colorYellow;
        
        public const float CellWidth = 1;
        private const int NoBlock = -1;
        
        public GateView gatePrefab;
        public BlockView block1Prefab;
        public BlockView block2Prefab;
        public Texture2D block1TextureBlueParallel;
        public Texture2D block1TextureBlueUp;
        public Texture2D block1TextureGreenParallel;
        public Texture2D block1TextureGreenUp;
        public Texture2D block1TexturePurpleParallel;
        public Texture2D block1TexturePurpleUp;
        public Texture2D block1TextureRedParallel;
        public Texture2D block1TextureRedUp;
        public Texture2D block1TextureYellowParallel;
        public Texture2D block1TextureYellowUp;
        public Texture2D block2TextureBlueParallel;
        public Texture2D block2TextureBlueUp;
        public Texture2D block2TextureGreenParallel;
        public Texture2D block2TextureGreenUp;
        public Texture2D block2TexturePurpleParallel;
        public Texture2D block2TexturePurpleUp;
        public Texture2D block2TextureRedParallel;
        public Texture2D block2TextureRedUp;
        public Texture2D block2TextureYellowParallel;
        public Texture2D block2TextureYellowUp;
        
        private Cell[,] _board;
        private readonly Dictionary<int, BlockView> _blocks = new();
        private Vector3 _initialPosition;
        private int _boardTop;
        private int _boardRight;
        private int _boardBottom;
        private int _boardLeft;
        private int _rowCount;
        private int _columnCount;
        
        public void Init(LevelData levelData)
        {
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
                var blockPrefab = GetBlockPrefab(length);
                var position = GetCellPosition(i, j);
                var rotation = Quaternion.Euler(0f, 90f * ((direction + 1) % 2), 0f);
                
                var blockView = Instantiate(blockPrefab, position, rotation, transform);
                blockView.Init(this, index, length, direction.ToBlockDirection(), color);
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
            
            SetBoardCells(pivotI, pivotJ, rowCount, columnCount, -1);
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
                
                var gateView = Instantiate(gatePrefab, position, rotation, transform);
                gateView.Init(this, direction.ToBlockDirection(), gateColor);
                
                PlaceGateToBoard(i, j, direction.ToBlockDirection(), gateColor);
            }
        }
        
        private void PlaceGateToBoard(int pivotI, int pivotJ, BlockDirection gateDirection, BlockColor gateColor)
        {
            var cell = _board[pivotI, pivotJ];
            cell.Gates ??= new List<Gate>();
            cell.Gates.Add(new Gate
            {
                Color = gateColor,
                Direction = gateDirection
            });
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
            
            bool CanExit(List<Gate> gates)
            {
                if (gates != null)
                {
                    for (var m = 0; m < gates.Count; m++)
                    {
                        var gate = gates[m];
                        if (gate.Color == blockColor && gate.Direction == blockDirection)
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
            Destroy(blockView.gameObject);
        }
        
        public Texture GetBlockTexture(int length, BlockColor color, bool isParallel)
        {
            return length switch
            {
                1 => color switch
                {
                    BlockColor.Red => isParallel ? block1TextureRedParallel : block1TextureRedUp,
                    BlockColor.Green => isParallel ? block1TextureGreenParallel : block1TextureGreenUp,
                    BlockColor.Blue => isParallel ? block1TextureBlueParallel : block1TextureBlueUp,
                    BlockColor.Yellow => isParallel ? block1TextureYellowParallel : block1TextureYellowUp,
                    BlockColor.Purple => isParallel ? block1TexturePurpleParallel : block1TexturePurpleUp,
                    _ => null
                },
                2 => color switch
                {
                    BlockColor.Red => isParallel ? block2TextureRedParallel : block2TextureRedUp,
                    BlockColor.Green => isParallel ? block2TextureGreenParallel : block2TextureGreenUp,
                    BlockColor.Blue => isParallel ? block2TextureBlueParallel : block2TextureBlueUp,
                    BlockColor.Yellow => isParallel ? block2TextureYellowParallel : block2TextureYellowUp,
                    BlockColor.Purple => isParallel ? block2TexturePurpleParallel : block2TexturePurpleUp,
                    _ => null
                },
                _ => null
            };
        }
        
        public Color GetGateColor(BlockColor color)
        {
            return color switch
            {
                BlockColor.Red => colorRed,
                BlockColor.Green => colorGreen,
                BlockColor.Blue => colorBlue,
                BlockColor.Yellow => colorYellow,
                BlockColor.Purple => colorPurple,
                _ => Color.white
            };
        }
        
        private BlockView GetBlockPrefab(int length)
        {
            return length switch
            {
                1 => block1Prefab,
                2 => block2Prefab,
                _ => block1Prefab,
            };
        }
        
        private void OnDrawGizmos()
        {
            if (_board == null)
            {
                return;
            }
            
            for (var i = 0; i < _board.GetLength(0); i++)
            {
                for (var j = 0; j < _board.GetLength(1); j++)
                {
                    if (_board[i, j].BlockID != NoBlock)
                    {
                        Gizmos.color = Color.red;
                        Gizmos.DrawCube(GetCellPosition(i, j) + Vector3.up*1f, Vector3.one * 0.1f);
                    }
                }
            }
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
    
    public struct Gate
    {
        public BlockColor Color;
        public BlockDirection Direction;
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
