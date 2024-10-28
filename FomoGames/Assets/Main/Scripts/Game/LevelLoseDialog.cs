using UnityEngine;

namespace Main.Scripts.Game
{
    public class LevelLoseDialog : MonoBehaviour
    {
        public void OnContinueButtonClick()
        {
            GameController.Instance.GameManager.TryAgain();
            Destroy(gameObject);
        }
    }
}
