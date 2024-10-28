using TMPro;
using UnityEngine;

namespace Main.Scripts.Game
{
    public class GameUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private TextMeshProUGUI moveCountText;
        
        public void SetLevelText(int level)
        {
            levelText.text = $"Level {level}";
        }
        
        public void SetMoveCountText(int moveCount)
        {
            moveCountText.text = $"Move {moveCount}";
        }
    }
}
