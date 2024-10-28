using UnityEngine;
using UnityEngine.SceneManagement;

namespace Main.Scripts
{
    public class SceneChangeManager : MonoBehaviour
    {
        private const int HomeSceneIndex = 0;
        private const int GameSceneIndex = 1;

        public void Bind()
        {
        }
        
        public void LoadHomeScene()
        {
            SceneManager.LoadScene(HomeSceneIndex);
        }
        
        public void LoadGameScene()
        {
            SceneManager.LoadScene(GameSceneIndex);
        }
    }
}
