using DG.Tweening;
using UnityEngine;

namespace Main.Scripts
{
    public class GateView : MonoBehaviour
    {
        [SerializeField] private Transform gateRoot;
        [SerializeField] private Animator animator;
        [SerializeField] private MeshRenderer[] meshRenderers;
        
        public BlockColor GateColor { get; private set; }
        public BlockDirection GateDirection { get; private set; }
        
        public void Init(BlockDirection direction, BlockColor color)
        {
            GateDirection = direction;
            GateColor = color;
            
            var matColor = GameController.Instance.GameManager.BoardAssets.GetGateColor(GateColor);
            foreach (var meshRenderer in meshRenderers)
            {
                meshRenderer.material.color = matColor;
            }
            
            animator.PlayInFixedTime("GateBladesAnimation",0, Random.Range(0f, 1f));
        }
        
        public void Open()
        {
            var seq = DOTween.Sequence();
            seq.Append(gateRoot.DOLocalMoveY(-0.8f, 0.2f).SetEase(Ease.OutBack));
            seq.AppendInterval(0.1f);
            seq.Append(gateRoot.DOLocalMoveY(0, 0.6f).SetEase(Ease.OutBack));
        }
    }
}
