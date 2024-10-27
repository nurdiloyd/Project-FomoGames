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
        
        public GameObject cellPrefab;
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
        
        private bool[,] _board;
        private Vector3 _initialPosition;
        
        public void Init(LevelData levelData)
        {
            var columnCount = levelData.ColCount;
            var rowCount = levelData.RowCount;
            _board = new bool[rowCount, columnCount];
            
            _initialPosition = new Vector3((1 - columnCount) / 2f, 0f, -(1 - rowCount) / 2f) * CellWidth;
            var pos = _initialPosition;
            
            for (var i = 0; i < rowCount; i++)
            {
                for (var j = 0; j < columnCount; j++)
                {
                    _board[i, j] = false;
                    var cell = Instantiate(cellPrefab, pos, Quaternion.identity, transform);
                    cell.name = $"Cell_{i}_{j}";
                    
                    pos.x += CellWidth;
                }
                
                pos.x = (1 - columnCount) / 2f * CellWidth;
                pos.z -= CellWidth;
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
                var blockPrefab = GetBlockPrefab(movableInfo.Length);
                var position = GetPosition(i, j);
                var rotation = GetRotation((direction + 1) % 2);
                
                var blockView = Instantiate(blockPrefab, position, rotation, transform);
                blockView.Init((Direction)direction);
                
                var meshRenderer = blockView.transform.GetChild(0).GetComponent<MeshRenderer>();
                meshRenderer.material.mainTexture = GetBlockTexture(movableInfo.Length, movableInfo.Colors, movableInfo.Direction[0]);
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
    }
    
    public enum Direction
    {
        Up,
        Right,
        Down,
        Left
    }
}
