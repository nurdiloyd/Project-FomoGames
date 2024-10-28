using System.Collections.Generic;
using Main.Scripts.Game;
using UnityEngine;

namespace Main.Scripts
{
    public class GameController : MonoBehaviour
    {
        public static GameController Instance { get; private set; }
        
        [SerializeField] private Camera mainCamera;
        [SerializeField] private GameUI gameUI;
        
        public DataManager DataManager;
        public GameInputController GameInputController;
        public GameManager GameManager;
        public CameraManager CameraManager;
        private List<IContextUnit> _contexts;
        
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
        }
        
        private void Update()
        {
            GameInputController.ManualUpdate();
        }
    }
}
