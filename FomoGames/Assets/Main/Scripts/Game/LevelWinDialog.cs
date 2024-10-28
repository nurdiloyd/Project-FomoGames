using UnityEngine;

namespace Main.Scripts.Game
{
    public class LevelWinDialog : MonoBehaviour
    {
        public void OnContinueButtonClick()
        {
            GameController.Instance.GameManager.NextLevel();
            Destroy(gameObject);
        }
    }
}
