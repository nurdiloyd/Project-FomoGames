using Main.Scripts.General;
using UnityEngine;

namespace Main.Scripts.Game
{
    public class LevelWinDialog : MonoBehaviour
    {
        public void OnContinueButtonClick()
        {
            ContextController.Instance.GameManager.NextLevel();
            Destroy(gameObject);
        }
    }
}
