using Main.Scripts.General;
using Main.Scripts.Utils;
using UnityEngine;

namespace Main.Scripts.Game
{
    public class GameInputController : IContextBehaviour
    {
        private Vector3 _startPosition;
        private Vector3 _endPosition;
        private GameManager _gameManager;
        private Block _block;
        private bool _blockSelected;
        private const float SwipeLengthThreshold = 0.1f;
        private const float SwipeAngleThreshold = 45f;
        private CameraManager _cameraManager;
        
        public void Bind()
        {
            _cameraManager = ContextController.Instance.CameraManager;
            _gameManager = ContextController.Instance.GameManager;
        }
        
        public void ManualUpdate()
        {
            if (!_gameManager.CanPlay)
            {
                return;
            }
            
            if (Input.GetMouseButtonDown(0))
            {
                _startPosition = Input.mousePosition;
                
                var ray = _cameraManager.ScreenPointToRay(_startPosition);
                var isHit = Physics.Raycast(ray, out var hit);
                if (isHit && hit.transform.CompareTag(Constants.BlockTag))
                {
                    var blockView = hit.transform.GetComponent<BlockView>();
                    _gameManager.SelectBlock(blockView.ID, out _block);
                    _blockSelected = _block != null;
                }
            }
            
            if (Input.GetMouseButton(0) && _blockSelected)
            {
                _endPosition = Input.mousePosition;
                
                var toward = _endPosition - _startPosition;
                var rad = (int)Mathf.Round(Vector2.SignedAngle(Vector2.right, toward) / 90f);
                var direction = new Vector2((1 - Mathf.Abs(rad)) % 2, rad % 2);
                var moveDirection = direction.ToBlockDirection();
                
                if (_block.CanMoveOnAxis(moveDirection))
                {
                    var worldPointA = _startPosition;
                    worldPointA.z = _cameraManager.RenderDistance;
                    worldPointA = _cameraManager.ScreenToWorldPoint(worldPointA);
                
                    var worldPointB = _endPosition;
                    worldPointB.z = _cameraManager.RenderDistance;
                    worldPointB = _cameraManager.ScreenToWorldPoint(worldPointB);
                    
                    var worldDistance = (worldPointA - worldPointB).magnitude;
                    var angle = Vector2.Angle(toward, direction);
                    
                    var canMove = worldDistance > Board.CellWidth * SwipeLengthThreshold && angle < SwipeAngleThreshold;
                    if (canMove)
                    {
                        _blockSelected = false;
                        _gameManager.DeselectBlock(_block.ID);
                        _gameManager.MoveBlock(_block.ID, moveDirection);
                    }
                }
            }
            
            if (Input.GetMouseButtonUp(0) && _blockSelected)
            {
                _blockSelected = false;
                _gameManager.DeselectBlock(_block.ID);
            }
        }
    }
}
