using DG.Tweening;
using Main.Scripts.General;
using Main.Scripts.Utils;
using UnityEngine;

namespace Main.Scripts.Game
{
    public class BlockView : MonoBehaviour
    {
        private static readonly Color HighlightColor = new(0.1f, 0.1f, 0.1f);
        private static readonly Color DefaultColor = new(0.0f, 0.0f, 0.0f);
        private static readonly int EmissionColorProperty = Shader.PropertyToID("_EmissionColor");
        
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private Collider boxCollider;
        [SerializeField] private TrailRenderer[] trailRenderers;
        
        public int ID { get; private set; }
        
        public void Init(Block block)
        {
            ID = block.ID;
            
            var gameManager = ContextController.Instance.GameManager;
            meshRenderer.material.mainTexture = gameManager.BoardAssets.GetBlockTexture(block.Length, block.BlockColor, block.BlockDirection.IsHorizontal());
            
            var matColor = gameManager.BoardAssets.GetGateColor(block.BlockColor);
            foreach (var trailRenderer in trailRenderers)
            {
                trailRenderer.startColor = matColor;
                matColor.a = 0;
                trailRenderer.endColor = matColor;
            }
        }
        
        public void Select()
        {
            var color = meshRenderer.material.GetColor(EmissionColorProperty);
            DOTween.To(() => color, x => color = x, HighlightColor, 0.2f).OnUpdate(() =>
            {
                meshRenderer.material.SetColor(EmissionColorProperty, color);
            }).SetLink(gameObject);
        }
        
        public void Deselect()
        {
            var color = meshRenderer.material.GetColor(EmissionColorProperty);
            DOTween.To(() => color, x => color = x, DefaultColor, 0.2f).OnUpdate(() =>
            {
                meshRenderer.material.SetColor(EmissionColorProperty, color);
            }).SetLink(gameObject);
        }
        
        public void DisableCollider()
        {
            boxCollider.enabled = false;
        }
    }
}
