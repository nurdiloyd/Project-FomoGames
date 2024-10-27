using UnityEngine;

namespace Main.Scripts
{
    public class CameraManager : MonoBehaviour
    {
        public Camera mainCamera;
        public float renderDistance;
        
        public void Bind()
        {
            renderDistance = mainCamera.nearClipPlane;
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
