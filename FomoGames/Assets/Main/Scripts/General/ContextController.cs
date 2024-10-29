using Main.Scripts.Game;
using UnityEngine;

namespace Main.Scripts.General
{
    public class ContextController : MonoBehaviour
    {
        public static ContextController Instance { get; private set; }
        
        [SerializeField] private Camera mainCamera;
        [SerializeField] private GameUI gameUI;
        
        public DataManager DataManager;
        public GameInputController GameInputController;
        public GameManager GameManager;
        public CameraManager CameraManager;
        
        private void Awake()
        {
            Instance = this;
            
            DataManager = new DataManager();
            GameInputController = new GameInputController();
            GameManager = new GameManager();
            CameraManager = new CameraManager();
            
            DataManager.Bind();
            GameManager.Bind();
            CameraManager.Bind();
            GameInputController.Bind();
        }
        
        private void Start()
        {
            CameraManager.SetCamera(mainCamera);
            GameManager.SetGameUI(gameUI);
            
            StartGame();
        }

        private void StartGame()
        {
            GameManager.LoadLevel();
        }
        
        private void Update()
        {
            GameInputController.ManualUpdate();
        }
    }
}
