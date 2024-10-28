using UnityEngine;

namespace Main.Scripts
{
    public class GameInputController : IContextBehaviour
    {
        private Vector3 _startPosition;
        private Vector3 _endPosition;
        private GameManager _gameManager;
        private BlockView _blockView;
        private bool _blockSelected;
        private const float SwipeLengthThreshold = 0.25f;
        private const float SwipeAngleThreshold = 30f;
        private CameraManager _cameraManager;
        
        public void Bind()
        {
            _cameraManager = GameController.Instance.CameraManager;
            _gameManager = GameController.Instance.GameManager;
        }
        
        public void ManualUpdate()
        {
            if (!_gameManager.HasMove)
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
                    _blockSelected = true;
                    _blockView = hit.transform.GetComponent<BlockView>();
                    _blockView.Select();
                }
            }
            
            if (Input.GetMouseButton(0) && _blockSelected)
            {
                _endPosition = Input.mousePosition;
                
                var toward = _endPosition - _startPosition;
                var rad = (int)Mathf.Round(Vector2.SignedAngle(Vector2.right, toward) / 90f);
                var direction = new Vector2((1 - Mathf.Abs(rad)) % 2, rad % 2);
                var moveDirection = direction.ToBlockDirection();
                
                if (_blockView.CanMoveOnAxis(moveDirection))
                {
                    var worldPointA = _startPosition;
                    worldPointA.z = _cameraManager.RenderDistance + 0.1f;
                    worldPointA = _cameraManager.ScreenToWorldPoint(worldPointA);
                
                    var worldPointB = _endPosition;
                    worldPointB.z = _cameraManager.RenderDistance + 0.1f;
                    worldPointB = _cameraManager.ScreenToWorldPoint(worldPointB);
                    
                    var worldDistance = (worldPointA - worldPointB).magnitude;
                    var angle = Vector2.Angle(toward, direction);
                    
                    var canMove = worldDistance > GameBoard.CellWidth * SwipeLengthThreshold && angle < SwipeAngleThreshold;
                    if (canMove)
                    {
                        _blockSelected = false;
                        _gameManager.MoveBlock(_blockView.ID, moveDirection);
                        _blockView.Deselect();
                        _blockView = null;
                    }
                }
            }
            
            if (Input.GetMouseButtonUp(0) && _blockSelected)
            {
                _blockSelected = false;
                _blockView.Deselect();
                _blockView = null;
            }
        }
    }
}
