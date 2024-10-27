using UnityEngine;

namespace Main.Scripts
{
    public class GameBoard : MonoBehaviour
    {
        private const float Margin = 1;
        
        public GameObject cellPrefab;
        
        private int Height => _board.GetLength(0);
        private int Width => _board.GetLength(1);
        
        private bool[,] _board;
        
        public void Init(LevelData levelData)
        {
            _board = new bool[levelData.ColCount, levelData.RowCount];
            
            var pos = new Vector3((1 - Width) / 2f, 0f, (1 - Height) / 2f) * Margin;
            
            for (var i = 0; i < Height; i++)
            {
                for (var j = 0; j < Width; j++)
                {
                    Instantiate(cellPrefab, pos, Quaternion.identity, transform);
                    _board[i, j] = false;
                    
                    pos.x += Margin;
                }
                
                pos.x = (1 - Width) / 2f * Margin;
                pos.z += Margin;
            }
        }
    }
}
