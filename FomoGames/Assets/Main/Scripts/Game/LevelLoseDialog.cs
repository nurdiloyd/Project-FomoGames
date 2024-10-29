using Main.Scripts.General;
using UnityEngine;

namespace Main.Scripts.Game
{
    public class LevelLoseDialog : MonoBehaviour
    {
        public void OnContinueButtonClick()
        {
            ContextController.Instance.GameManager.LoadLevel();
            Destroy(gameObject);
        }
    }
}
