using DG.Tweening;
using Main.Scripts.General;
using UnityEngine;

namespace Main.Scripts.Game
{
    public class GateView : MonoBehaviour
    {
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
            _sequence.Append(SpeedUp());
            _sequence.Join(gateRoot.DOLocalMoveY(-0.67f, 0.2f).SetEase(Ease.OutBack));
            _sequence.AppendInterval(0.2f);
            _sequence.Append(gateRoot.DOLocalMoveY(0, 0.6f).SetEase(Ease.OutBack));
            _sequence.Append(SpeedDown());
        }
        
        private Tween SpeedUp()
        {
            return DOTween.To(() => animator.speed, x => animator.speed = x, 6, 0.1f).SetLink(gameObject);
        }
        
        private Tween SpeedDown()
        {
            return DOTween.To(() => animator.speed, x => animator.speed = x, 1, 1f).SetLink(gameObject);
        }
    }
}
