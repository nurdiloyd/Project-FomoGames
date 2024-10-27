using UnityEngine;

namespace Main.Scripts
{
    public class GameController : MonoBehaviour
    {
        public static GameController Instance { get; private set; }
        
        public DataManager DataManager;
        public BoardManager boardManager;
        
        private void Awake()
        {
            Instance = this;
            
            DataManager = new DataManager();
            
            DataManager.Bind();
            boardManager.Bind();
        }
    }
}
