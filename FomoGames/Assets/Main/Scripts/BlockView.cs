using UnityEngine;

namespace Main.Scripts
{
    public class BlockView : MonoBehaviour
    {
        [SerializeField] private MeshRenderer meshRenderer;
        
        public void Init()
        {
        }
        
        public bool CanMoveOnDirection(Vector2 direction)
        {
            return true;
        }
        
        public void Move(Vector2 direction)
        {
            
        }
        
        public void Select()
        {
            meshRenderer.material.color = Color.red;
        }
        
        public void Deselect()
        {
            meshRenderer.material.color = Color.white;
        }
    }
}
