using UnityEngine;

namespace Main.Scripts
{
    public class CameraManager : MonoBehaviour
    {
        public Camera mainCamera;
        public float RenderDistance { get; private set; }
        
        public void Bind()
        {
            RenderDistance = mainCamera.nearClipPlane;
        }
        
        public Vector3 ScreenToWorldPoint(Vector3 screenPoint)
        {
            return mainCamera.ScreenToWorldPoint(screenPoint);
        }
        
        public Ray ScreenPointToRay(Vector3 position)
        {
            return mainCamera.ScreenPointToRay(position);
        }
    }
}
