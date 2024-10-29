using UnityEngine;

namespace Main.Scripts.General
{
    public class CameraManager : IContextUnit
    {
        private Camera MainCamera { get; set; }
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
