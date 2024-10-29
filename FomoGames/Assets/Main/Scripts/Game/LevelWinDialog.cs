using Main.Scripts.General;
using UnityEngine;

namespace Main.Scripts.Game
{
    public class LevelWinDialog : MonoBehaviour
    {
        public void OnContinueButtonClick()
        {
            ContextController.Instance.GameManager.LoadLevel();
            Destroy(gameObject);
        }
    }
}
