using UnityEngine;

namespace Main.Scripts
{
    public class GateView : MonoBehaviour
    {
        [SerializeField] private MeshRenderer[] meshRenderers;
        
        private GameBoard _gameBoard;
        public int PivotI { get; private set; }
        public int PivotJ { get; private set; }
        public BlockColor GateColor { get; private set; }
        private BlockDirection _gateDirection;
        
        public void Init(GameBoard gameBoard, BlockDirection direction, BlockColor color)
        {
            _gameBoard = gameBoard;
            _gateDirection = direction;
            GateColor = color;
            
            foreach (var meshRenderer in meshRenderers)
            {
                meshRenderer.material.color = _gameBoard.GetGateColor(GateColor);
            }
        }
        
        public void SetPivot(int pivotI, int pivotJ)
        {
            PivotI = pivotI;
            PivotJ = pivotJ;
        }
    }
}
