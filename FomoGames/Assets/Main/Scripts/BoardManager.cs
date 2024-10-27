using UnityEngine;

namespace Main.Scripts
{
    public class BoardManager : MonoBehaviour
    {
        public GameBoard gameBoard;
        
        public void Bind()
        {
            var levelData = GameController.Instance.DataManager.GetCurrentLevelData();
            gameBoard.Init(levelData);
        }
    }
}
