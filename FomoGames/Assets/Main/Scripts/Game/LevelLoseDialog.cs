using Main.Scripts.General;

namespace Main.Scripts.Game
{
    public class LevelLoseDialog : UIDialog
    {
        public void OnContinueButtonClick()
        {
            ContextController.Instance.GameManager.LoadLevel();
            Destroy(gameObject);
        }
    }
}
