using DG.Tweening;
using UnityEngine;

namespace Main.Scripts
{
    public class GateView : MonoBehaviour
    {
        [SerializeField] private Transform gateRoot;
        [SerializeField] private MeshRenderer[] meshRenderers;
        
        public BlockColor GateColor { get; private set; }
        public BlockDirection GateDirection { get; private set; }
        
        public void Init(BlockDirection direction, BlockColor color)
        {
            GateDirection = direction;
            GateColor = color;
            
            foreach (var meshRenderer in meshRenderers)
            {
                meshRenderer.material.color = GameController.Instance.GameManager.BoardAssets.GetGateColor(GateColor);
            }
        }
        
        public void Open()
        {
            var seq = DOTween.Sequence();
            seq.Append(gateRoot.DOLocalMoveY(-1, 0.2f).SetEase(Ease.OutBack));
            seq.AppendInterval(0.2f);
            seq.Append(gateRoot.DOLocalMoveY(0, 0.2f).SetEase(Ease.InBack));
        }
    }
}
