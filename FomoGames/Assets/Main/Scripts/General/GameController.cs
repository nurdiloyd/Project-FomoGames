using System.Collections.Generic;
using UnityEngine;

namespace Main.Scripts
{
    public class GameController : MonoBehaviour
    {
        public static GameController Instance { get; private set; }
        
        [SerializeField] private Camera mainCamera;
        
        public DataManager DataManager;
        public GameInputController GameInputController;
        public BoardManager BoardManager;
        public CameraManager CameraManager;
        private List<IContextUnit> _contexts;
        
        private void Awake()
        {
            Instance = this;
            
            DataManager = new DataManager();
            GameInputController = new GameInputController();
            BoardManager = new BoardManager();
            CameraManager = new CameraManager();
            
            DataManager.Bind();
            BoardManager.Bind();
            CameraManager.Bind();
            GameInputController.Bind();
        }
        
        private void Start()
        {
            CameraManager.SetCamera(mainCamera);
        }
        
        private void Update()
        {
            GameInputController.ManualUpdate();
        }
    }
}
