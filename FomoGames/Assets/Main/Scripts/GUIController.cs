using UnityEngine;

namespace Main.Scripts
{
    public class GUIController : MonoBehaviour
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] public LevelEndDialog levelEndDialogPrefab;
        
        public void Bind()
        {
        }
        
        public void OpenLevelEndDialog()
        {
            var levelEndDialog = Instantiate(levelEndDialogPrefab, canvas.transform);
            levelEndDialog.Init();
        }
    }
}
