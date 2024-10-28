using UnityEngine;

namespace Main.Scripts
{
    public class GateView : MonoBehaviour
    {
        [SerializeField] private MeshRenderer[] meshRenderers;
        
        public BlockColor GateColor { get; private set; }
        public BlockDirection GateDirection { get; private set; }
        
        public void Init(BlockDirection direction, BlockColor color)
        {
            GateDirection = direction;
            GateColor = color;
            
            foreach (var meshRenderer in meshRenderers)
            {
                meshRenderer.material.color = GameController.Instance.BoardManager.BoardAssets.GetGateColor(GateColor);
            }
        }
    }
}
