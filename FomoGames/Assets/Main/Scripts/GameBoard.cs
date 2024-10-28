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
        private Dictionary<int, BlockView> _blocks = new();
        private Vector3 _initialPosition;
        private int _boardTop;
        private int _boardRight;
        private int _boardBottom;
        private int _boardLeft;
        
        public void Init(LevelData levelData)
        {
            var columnCount = levelData.ColCount;
            var rowCount = levelData.RowCount;
            _board = new int[rowCount, columnCount];
            
            _initialPosition = new Vector3((1 - columnCount) / 2f, 0f, -(1 - rowCount) / 2f) * CellWidth;
            _boardTop = 0;
            _boardRight = columnCount - 1;
            _boardBottom = rowCount - 1;
            _boardLeft = 0;
            
            for (var i = 0; i < rowCount; i++)
            {
                for (var j = 0; j < columnCount; j++)
                {
                    _board[i, j] = -1;
                }
            }
            
            PlaceBlocks(levelData.MovableInfo);
            PlaceGates(levelData.ExitInfo);
        }
        
        private void PlaceBlocks(MovableInfo[] movableInfos)
        {
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
                
                var blockView = Instantiate(blockPrefab, position, rotation, transform);
                blockView.Init(this, index, i, j, length, (Direction)direction);
                _blocks.Add(index, blockView);
                
                var meshRenderer = blockView.transform.GetChild(0).GetComponent<MeshRenderer>();
                meshRenderer.material.mainTexture = GetBlockTexture(movableInfo.Length, movableInfo.Colors, movableInfo.Direction[0]);
                
                PlaceBlockToBoard(i, j, index, length, (Direction)direction);
            }
        }
        
        private void PlaceBlockToBoard(int i, int j, int id, int length, Direction direction)
        {
            var iOffset = i + (direction == Direction.Up || direction == Direction.Down ? length : 1);
            var jOffset = j + (direction == Direction.Right || direction == Direction.Left ? length : 1);
            
            for (var k = i; k < iOffset; k++)
            {
                for (var l = j; l < jOffset; l++)
                {
                    _board[k, l] = id;
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
        
        private Texture2D GetBlockTexture(int length, int color, int direction)
        {
            var isParallel = direction == 0 || direction == 2;
            
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
        
        public Vector3 GetTargetPosition(int id, Direction direction)
        {
            var blockView = _blocks[id];
            var pivotI = blockView.PivotI;
            var pivotJ = blockView.PivotJ;
            
            var targetI = pivotI;
            var targetJ = pivotJ;
            
            Debug.Log(direction);
            if (direction == Direction.Down)
            {
                var bottomEdgeIndexes = new List<List<int>>();
                
                var i = pivotI + blockView.RowCount - 1;
                for (var j = pivotJ; j < pivotJ + blockView.ColumnCount; j++)
                {
                    bottomEdgeIndexes.Add(new List<int> {i, j});
                }
                
                var maxI = _boardBottom + 1;
                foreach (var bottomEdgeIndex in bottomEdgeIndexes)
                {
                    for (var k = bottomEdgeIndex[0] + 1; k <= _boardBottom; k++)
                    {
                        var cell = _board[k, bottomEdgeIndex[1]];
                        if (cell != -1 && cell != id)
                        {
                            if (maxI > k)
                            {
                                maxI = k;
                            }
                            
                            break;
                        }
                    }
                }
                
                targetI = maxI - blockView.RowCount;
                targetJ = pivotJ;
            }
            else if (direction == Direction.Up)
            {
                var topEdgeIndexes = new List<List<int>>();
                
                var i = pivotI;
                for (var j = pivotJ; j < pivotJ + blockView.ColumnCount; j++)
                {
                    topEdgeIndexes.Add(new List<int> {i, j});
                }
                
                var minI = _boardTop - 1;
                foreach (var topEdgeIndex in topEdgeIndexes)
                {
                    for (var k = topEdgeIndex[0] - 1; k >= _boardTop; k--)
                    {
                        var cell = _board[k, topEdgeIndex[1]]; 
                        if (cell != -1 && cell != id)
                        {
                            if (minI < k)
                            {
                                minI = k;
                            }
                            
                            break;
                        }
                    }
                }
                
                targetI = minI + 1;
                targetJ = pivotJ;
            }
            else if (direction == Direction.Right)
            {
                var rightEdgeIndexes = new List<List<int>>();
                
                var j = pivotJ + blockView.ColumnCount - 1;
                for (var i = pivotI; i < pivotI + blockView.RowCount; i++)
                {
                    rightEdgeIndexes.Add(new List<int> {i, j});
                }
                
                var maxJ = _boardRight + 1;
                foreach (var rightEdgeIndex in rightEdgeIndexes)
                {
                    for (var l = rightEdgeIndex[1] + 1; l <= _boardRight; l++)
                    {
                        var cell = _board[rightEdgeIndex[0], l]; 
                        if (cell != -1 && cell != id)
                        {
                            if (maxJ > l)
                            {
                                maxJ = l;
                            }
                            
                            break;
                        }
                    }
                }
                
                targetI = pivotI;
                targetJ = maxJ - blockView.ColumnCount;
            }
            else if (direction == Direction.Left)
            {
                var leftEdgeIndexes = new List<List<int>>();
                
                var j = pivotJ;
                for (var i = pivotI; i < pivotI + blockView.RowCount; i++)
                {
                    leftEdgeIndexes.Add(new List<int> {i, j});
                }
                
                var minJ = _boardLeft - 1;
                foreach (var leftEdgeIndex in leftEdgeIndexes)
                {
                    for (var l = leftEdgeIndex[1] - 1; l >= _boardLeft; l--)
                    {
                        var cell = _board[leftEdgeIndex[0], l]; 
                        if (cell != -1 && cell != id)
                        {
                            if (minJ < l)
                            {
                                minJ = l;
                            }
                            
                            break;
                        }
                    }
                }
                
                targetI = pivotI;
                targetJ = minJ + 1;
            }
            
            return GetPosition(targetI, targetJ);
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
