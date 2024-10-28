using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Math = System.Math;

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
        
        public GameObject gatePrefab;
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
        
        private int[,] _board;
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
            _board = new int[_rowCount, _columnCount];
            
            PlaceBlocks(levelData.MovableInfo);
            PlaceGates(levelData.ExitInfo);
        }
        
        private void PlaceBlocks(MovableInfo[] movableInfos)
        {
            for (var i = 0; i < _rowCount; i++)
            {
                for (var j = 0; j < _columnCount; j++)
                {
                    _board[i, j] = -1;
                }
            }
            
            for (var index = 0; index < movableInfos.Length; index++)
            {
                var movableInfo = movableInfos[index];
                var direction = movableInfo.Direction[0];
                var i = movableInfo.Row;
                var j = movableInfo.Col;
                var length = movableInfo.Length;
                var blockPrefab = GetBlockPrefab(length);
                var position = GetPosition(i, j);
                var rotation = GetRotation((direction + 1) % 2);
                var id = index;
                var texture = GetBlockTexture(length, movableInfo.Colors, (Direction)direction);
                
                var blockView = Instantiate(blockPrefab, position, rotation, transform);
                blockView.Init(this, id, length, (Direction)direction, texture);
                _blocks.Add(blockView.ID, blockView);
                
                PlaceBlockToBoard(blockView.ID, i, j);
            }
        }
        
        private void PlaceBlockToBoard(int id, int pivotI, int pivotJ)
        {
            var blockView = _blocks[id];
            var rowCount = blockView.RowCount;
            var columnCount = blockView.ColumnCount;
            blockView.SetPivot(pivotI, pivotJ);
            
            SetBoardCells(pivotI, pivotJ, rowCount, columnCount, id);
        }
        
        private void RemoveBlockFromBoard(int id)
        {
            var blockView = _blocks[id];
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
                    _board[i, j] = id;
                }
            }
        }
        
        private void PlaceGates(ExitInfo[] exitInfos)
        {
            for (var index = 0; index < exitInfos.Length; index++)
            {
                var exitInfo = exitInfos[index];
                var direction = exitInfo.Direction;
                var iOffset = direction == 0 ? -1 : direction == 2 ? 1 : 0;
                var jOffset = direction == 1 ? 1 : direction == 3 ? -1 : 0;
                var i = exitInfo.Row + iOffset;
                var j = exitInfo.Col + jOffset;
                var position = GetPosition(i, j);
                var rotation = GetRotation(direction);
                
                var gate = Instantiate(gatePrefab, position, rotation, transform);
                var meshRenderers = gate.transform.GetComponentsInChildren<MeshRenderer>();
                foreach (var meshRenderer in meshRenderers)
                {
                    meshRenderer.material.color = GetGateColor(exitInfo.Colors);
                }
            }
        }
        
        private Color GetGateColor(int color)
        {
            return color switch
            {
                0 => colorRed,
                1 => colorGreen,
                2 => colorBlue,
                3 => colorYellow,
                4 => colorPurple,
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
        
        private Texture2D GetBlockTexture(int length, int color, Direction direction)
        {
            var isParallel = direction.IsVertical();
            
            return length switch
            {
                1 => color switch
                {
                    0 => isParallel ? block1TextureRedParallel : block1TextureRedUp,
                    1 => isParallel ? block1TextureGreenParallel : block1TextureGreenUp,
                    2 => isParallel ? block1TextureBlueParallel : block1TextureBlueUp,
                    3 => isParallel ? block1TextureYellowParallel : block1TextureYellowUp,
                    4 => isParallel ? block1TexturePurpleParallel : block1TexturePurpleUp,
                    _ => null
                },
                2 => color switch
                {
                    0 => isParallel ? block2TextureRedParallel : block2TextureRedUp,
                    1 => isParallel ? block2TextureGreenParallel : block2TextureGreenUp,
                    2 => isParallel ? block2TextureBlueParallel : block2TextureBlueUp,
                    3 => isParallel ? block2TextureYellowParallel : block2TextureYellowUp,
                    4 => isParallel ? block2TexturePurpleParallel : block2TexturePurpleUp,
                    _ => null
                },
                _ => null
            };
        }
        
        private Quaternion GetRotation(int direction)
        {
            return Quaternion.Euler(0f, 90f * direction, 0f);
        }
        
        private Vector3 GetPosition(int i, int j)
        {
            return _initialPosition + new Vector3(j, 0f, -i) * CellWidth;
        }
        
        public static Direction DirectionVectorToEnum(Vector2 moveDirection)
        {
            if (moveDirection == Vector2.up)
            {
                return Direction.Up;
            }
            else if (moveDirection == Vector2.right)
            {
                return Direction.Right;
            }
            else if (moveDirection == Vector2.down)
            {
                return Direction.Down;
            }
            else if (moveDirection == Vector2.left)
            {
                return Direction.Left;
            }
            else
            {
                return Direction.Up;
            }
        }
        
        public void MoveBlock(int id, Direction moveDirection)
        {
            var blockView = _blocks[id];
            var pivotI = blockView.PivotI;
            var pivotJ = blockView.PivotJ;
            var (targetI, targetJ) = GetTargetIndex(pivotI, pivotJ, blockView.RowCount, blockView.ColumnCount, moveDirection, out var outsideI, out var outsideJ);
            var difference = Math.Max(Math.Abs(targetI - pivotI), Math.Abs(targetJ - pivotJ));
            var duration = 0.1f * difference;
            var targetPosition = GetPosition(targetI, targetJ);
            var outsidePosition = GetPosition(outsideI, outsideJ);
            
            var seq = DOTween.Sequence();
            seq.Append(blockView.transform.DOMove(targetPosition, duration).SetEase(Ease.OutBack));
            seq.Append(blockView.transform.DOMove(outsidePosition, duration).SetEase(Ease.InBack));
            
            RemoveBlockFromBoard(blockView.ID);
            PlaceBlockToBoard(blockView.ID, targetI, targetJ);
        }
        
        private (int i, int j) GetTargetIndex(int pivotI, int pivotJ, int rowCount, int columnCount, Direction direction, out int outsideI, out int outsideJ)
        {
            var targetI = pivotI;
            var targetJ = pivotJ;
            outsideI = targetI;
            outsideJ = targetJ;
            
            if (direction == Direction.Down)
            {
                var bottomEdgeIndexes = new List<List<int>>();
                
                var i = pivotI + rowCount - 1;
                for (var j = pivotJ; j < pivotJ + columnCount; j++)
                {
                    bottomEdgeIndexes.Add(new List<int> {i, j});
                }
                
                var goOutside = true;
                var maxI = _boardBottom + 1;
                foreach (var bottomEdgeIndex in bottomEdgeIndexes)
                {
                    for (var k = bottomEdgeIndex[0] + 1; k <= _boardBottom; k++)
                    {
                        var cell = _board[k, bottomEdgeIndex[1]];
                        if (cell != -1)
                        {
                            if (maxI > k)
                            {
                                maxI = k;
                            }
                            
                            goOutside = false;
                            
                            break;
                        }
                    }
                }
                
                targetI = maxI - rowCount;
                targetJ = pivotJ;
                
                outsideI = !goOutside ? targetI : _boardBottom + 1;
                outsideJ = targetJ;
            }
            else if (direction == Direction.Up)
            {
                var topEdgeIndexes = new List<List<int>>();
                
                var i = pivotI;
                for (var j = pivotJ; j < pivotJ + columnCount; j++)
                {
                    topEdgeIndexes.Add(new List<int> {i, j});
                }
                
                var goOutside = true;
                var minI = _boardTop - 1;
                foreach (var topEdgeIndex in topEdgeIndexes)
                {
                    for (var k = topEdgeIndex[0] - 1; k >= _boardTop; k--)
                    {
                        var cell = _board[k, topEdgeIndex[1]]; 
                        if (cell != -1)
                        {
                            if (minI < k)
                            {
                                minI = k;
                            }
                            
                            goOutside = false;
                            
                            break;
                        }
                    }
                }
                
                targetI = minI + 1;
                targetJ = pivotJ;
                
                outsideI = !goOutside ? targetI : _boardTop - rowCount;
                outsideJ = targetJ;
            }
            else if (direction == Direction.Right)
            {
                var rightEdgeIndexes = new List<List<int>>();
                
                var j = pivotJ + columnCount - 1;
                for (var i = pivotI; i < pivotI + rowCount; i++)
                {
                    rightEdgeIndexes.Add(new List<int> {i, j});
                }
                
                var goOutside = true;
                var maxJ = _boardRight + 1;
                foreach (var rightEdgeIndex in rightEdgeIndexes)
                {
                    for (var l = rightEdgeIndex[1] + 1; l <= _boardRight; l++)
                    {
                        var cell = _board[rightEdgeIndex[0], l]; 
                        if (cell != -1)
                        {
                            if (maxJ > l)
                            {
                                maxJ = l;
                            }
                            
                            goOutside = false;
                            
                            break;
                        }
                    }
                }
                
                targetI = pivotI;
                targetJ = maxJ - columnCount;
                
                outsideI = targetI;
                outsideJ = !goOutside ? targetJ : _boardRight + 1;
            }
            else if (direction == Direction.Left)
            {
                var leftEdgeIndexes = new List<List<int>>();
                
                var j = pivotJ;
                for (var i = pivotI; i < pivotI + rowCount; i++)
                {
                    leftEdgeIndexes.Add(new List<int> {i, j});
                }
                
                var goOutside = true;
                var minJ = _boardLeft - 1;
                foreach (var leftEdgeIndex in leftEdgeIndexes)
                {
                    for (var l = leftEdgeIndex[1] - 1; l >= _boardLeft; l--)
                    {
                        var cell = _board[leftEdgeIndex[0], l]; 
                        if (cell != -1)
                        {
                            if (minJ < l)
                            {
                                minJ = l;
                            }
                            
                            goOutside = false;
                            
                            break;
                        }
                    }
                }
                
                targetI = pivotI;
                targetJ = minJ + 1;
                
                outsideI = targetI;
                outsideJ = !goOutside ? targetJ : _boardLeft - columnCount;
            }
            
            return (targetI, targetJ);
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
                    if (_board[i, j] != -1)
                    {
                        Gizmos.color = Color.red;
                        Gizmos.DrawCube(GetPosition(i, j) + Vector3.up*1f, Vector3.one * 0.1f);
                    }
                }
            }
        }
    }
    
    public enum Direction
    {
        Up,
        Right,
        Down,
        Left
    }
}
