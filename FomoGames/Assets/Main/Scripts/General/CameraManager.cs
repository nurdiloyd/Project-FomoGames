using UnityEngine;

namespace Main.Scripts
{
    public class CameraManager : IContextUnit
    {
        public Camera MainCamera { get; private set; }
        public float RenderDistance { get; private set; }
        
        public void Bind()
        {
        }
        
        public void SetCamera(Camera camera)
        {
            MainCamera = camera;
            RenderDistance = MainCamera.nearClipPlane;
        }
        
        public Vector3 ScreenToWorldPoint(Vector3 screenPoint)
        {
            return MainCamera.ScreenToWorldPoint(screenPoint);
        }
        
        public Ray ScreenPointToRay(Vector3 position)
        {
            return MainCamera.ScreenPointToRay(position);
        }
    }
}
