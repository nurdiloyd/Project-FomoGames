using TMPro;
using UnityEngine;

namespace Main.Scripts.Game
{
    public class GameUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private TextMeshProUGUI moveCountText;
        [SerializeField] private GameObject infinitySign;
        
        public void SetLevelText(int level)
        {
            levelText.text = $"Level {level}";
        }
        
        public void SetMoveCountText(int moveCount)
        {
            if (GameManager.InfinityMove == moveCount)
            {
                infinitySign.SetActive(true);
                moveCountText.text = "Move";
            }
            else
            {
                infinitySign.SetActive(false);
                moveCountText.text = $"Move {moveCount}";
            }
        }
        
        public void ShowLevelWinDialog()
        {
            var dialogPrefab = Resources.Load<LevelWinDialog>("LevelWinDialogPrefab");
            var dialog = Instantiate(dialogPrefab, transform);
            dialog.Show();
        }
        
        public void ShowLevelLoseDialog()
        {
            var dialogPrefab = Resources.Load<LevelLoseDialog>("LevelLoseDialogPrefab");
            var dialog = Instantiate(dialogPrefab, transform);
            dialog.Show();
        }
    }
}
