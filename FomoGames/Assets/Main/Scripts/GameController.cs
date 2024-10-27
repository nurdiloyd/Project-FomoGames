using UnityEngine;

namespace Main.Scripts
{
    public class GameController : MonoBehaviour
    {
        public static GameController Instance { get; private set; }
        
        public DataManager DataManager;
        public GameInputController GameInputController;
        public BoardManager boardManager;
        public CameraManager cameraManager;
        
        private void Awake()
        {
            Instance = this;
            
            DataManager = new DataManager();
            GameInputController = new GameInputController();
            
            DataManager.Bind();
            boardManager.Bind();
            cameraManager.Bind();
            GameInputController.Bind();
        }
        
        private void Update()
        {
            GameInputController.ManualUpdate();
        }
    }
}
