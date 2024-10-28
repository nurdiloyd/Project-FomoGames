using UnityEngine;

namespace Main.Scripts
{
    public class LevelManager : MonoBehaviour 
    {
        private bool _gameEnd;
    
        public void Bind() 
        {
        }
        
        public void GameEnd(bool isWin)
        {
            if (!_gameEnd)
            {
                _gameEnd = true;
                
                if (isWin)
                {
                    WinLevel();
                }
                else
                {
                    LoseLevel();
                }
            }
        }

        private void WinLevel()
        {
        }
        
        private void LoseLevel()
        {
        }
    }
}
