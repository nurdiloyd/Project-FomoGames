using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Main.Scripts.General
{
    public class UIDialog : MonoBehaviour
    {
        [SerializeField] private Image background;
        
        public void Show()
        {
            var color = background.color;
            color.a = 0;
            background.color = color;
            color.a = 0.98f;
            
            var seq = DOTween.Sequence().SetLink(gameObject);
            seq.Append(background.DOColor(color, 0.2f));
            seq.Join(transform.DOScale(Vector3.one * 1.08f, 0.18f).SetEase(Ease.OutBack));
            seq.Append(transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutExpo));
        }
    }
}
