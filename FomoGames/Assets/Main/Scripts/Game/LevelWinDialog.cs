using Main.Scripts.General;

namespace Main.Scripts.Game
{
    public class LevelWinDialog : UIDialog
    {
        public void OnContinueButtonClick()
        {
            ContextController.Instance.GameManager.LoadLevel();
            Destroy(gameObject);
        }
    }
}
