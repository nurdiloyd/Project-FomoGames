using UnityEngine;

namespace Main.Scripts
{
    public class GameController : MonoBehaviour
    {
        public static GameController Instance { get; private set; }
        
        public GUIController guiController;
        public BoardManager boardManager;
        public SceneChangeManager sceneChangeManager;
        
        private void Awake()
        {
            Instance = this;
            
            guiController.Bind();
            boardManager.Bind();
            sceneChangeManager.Bind();
        }
    }
}
