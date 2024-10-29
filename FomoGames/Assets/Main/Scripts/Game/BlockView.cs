using DG.Tweening;
using UnityEngine;

namespace Main.Scripts
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
        public int PivotI { get; private set; }
        public int PivotJ { get; private set; }
        public int RowCount { get; private set; }
        public int ColumnCount { get; private set; }
        public BlockColor BlockColor { get; private set; }
        private BlockDirection _blockDirection;
        
        public void Init(int id, int length, BlockDirection blockDirection, BlockColor color)
        {
            ID = id;
            _blockDirection = blockDirection;
            RowCount = blockDirection.IsHorizontal() ? 1 : length;
            ColumnCount = blockDirection.IsVertical() ? 1 : length;
            BlockColor = color;
            
            var gameManager = ContextController.Instance.GameManager;
            meshRenderer.material.mainTexture = gameManager.BoardAssets.GetBlockTexture(length, BlockColor, _blockDirection.IsHorizontal());
            
            var matColor = gameManager.BoardAssets.GetGateColor(BlockColor);
            foreach (var trailRenderer in trailRenderers)
            {
                trailRenderer.startColor = matColor;
                matColor.a = 0;
                trailRenderer.endColor = matColor;
            }
        }
        
        public void SetPivot(int pivotI, int pivotJ)
        {
            PivotI = pivotI;
            PivotJ = pivotJ;
        }
        
        public bool CanMoveOnAxis(BlockDirection direction)
        {
            return _blockDirection == direction || _blockDirection == direction.GetInverse();
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
