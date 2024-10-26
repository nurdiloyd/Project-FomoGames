using UnityEngine;

namespace Main.Scripts
{
    public class LevelEndDialog : MonoBehaviour
    {
        private SceneChangeManager _sceneChangeManager;
        
        public void Init()
        {
            _sceneChangeManager = GameController.Instance.sceneChangeManager;
        }
        
        public void OnContinueButtonClick()
        {
            _sceneChangeManager.LoadHomeScene();
        }
    }
}
