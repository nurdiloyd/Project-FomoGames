using DG.Tweening;
using UnityEngine;

namespace Main.Scripts
{
    public class BlockView : MonoBehaviour
    {
        [SerializeField] private MeshRenderer meshRenderer;
        
        private Direction _direction;
        
        public void Init(Direction direction)
        {
            _direction = direction;
        }
        
        public bool CanMoveOnDirection(Direction direction)
        {
            return _direction == direction || _direction == direction.GetInverse();
        }
        
        public void Move(Direction direction)
        {
            var toPosition = transform.position;// + new Vector3(direction.x, 0, direction.y);
            transform.DOMove(toPosition, 0.5f);
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
