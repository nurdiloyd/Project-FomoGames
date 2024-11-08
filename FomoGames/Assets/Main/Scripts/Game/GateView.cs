using DG.Tweening;
using Main.Scripts.General;
using UnityEngine;

namespace Main.Scripts.Game
{
    public class GateView : MonoBehaviour
    {
        private const float BottomPosition = -0.67f;
        
        [SerializeField] private Transform gateRoot;
        [SerializeField] private Animator animator;
        [SerializeField] private MeshRenderer[] meshRenderers;
        
        private Sequence _sequence;
        
        public void Init(Gate gate)
        {
            var matColor = ContextController.Instance.GameManager.BoardAssets.GetGateColor(gate.GateColor);
            foreach (var meshRenderer in meshRenderers)
            {
                meshRenderer.material.color = matColor;
            }
            
            animator.PlayInFixedTime("GateBladesAnimation",0, Random.Range(0f, 1f));
        }
        
        public void Open()
        {
            if (_sequence != null && _sequence.IsActive())
            {
                _sequence.Kill();
            }
            
            _sequence = DOTween.Sequence().SetLink(gameObject);
            _sequence.Append(AnimateSpeed(6, 0.1f));
            _sequence.Join(gateRoot.DOLocalMoveY(BottomPosition, 0.2f).SetEase(Ease.OutBack));
            _sequence.AppendInterval(0.2f);
            _sequence.Append(gateRoot.DOLocalMoveY(0, 0.6f).SetEase(Ease.OutBack));
            _sequence.Append(AnimateSpeed(1, 1f));
        }
        
        private Tween AnimateSpeed(float speed, float duration)
        {
            return DOTween.To(() => animator.speed, x => animator.speed = x, speed, duration).SetLink(gameObject);
        }
        
        public void PlayCreationAnimation()
        {
            var pos = gateRoot.localPosition;
            pos.y = BottomPosition;
            gateRoot.localPosition = pos;
            
            var seq = DOTween.Sequence().SetLink(gameObject);
            seq.Append(AnimateSpeed(0.5f, 0.1f));
            seq.Join(gateRoot.DOLocalMoveY(0, 0.6f).SetEase(Ease.OutBack));
            seq.Append(AnimateSpeed(1f, 1f));
        }
    }
}
