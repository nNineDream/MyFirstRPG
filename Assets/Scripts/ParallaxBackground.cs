using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    private Camera mainCamera;
    private float lastCameraPosition;
    private float cameraHalfWidth;
    [SerializeField] private ParallaxLayer[] backgroundLayers; 
    private void Awake()
    {
        mainCamera = Camera.main;
        cameraHalfWidth = mainCamera.orthographicSize * mainCamera.aspect;
        //orthographic 正交模式 orthographicSize正交尺寸（摄像机总高度的一半）
        //aspect 宽高比
        CalculateImageLength();
    }

    private void FixedUpdate()
    {
        float currentCameraPositionX = mainCamera.transform.position.x;
        float distanceToMove = currentCameraPositionX - lastCameraPosition;
        lastCameraPosition = currentCameraPositionX;

        float cameraLeftEdge = currentCameraPositionX - cameraHalfWidth;
        float cameraRightEdge = currentCameraPositionX + cameraHalfWidth;

        foreach(ParallaxLayer layer in backgroundLayers)
        {
            layer.Move(distanceToMove);
            layer.LoopBackground(cameraLeftEdge, cameraRightEdge);
        }
    }

    private void CalculateImageLength() { 
        foreach(ParallaxLayer layer in backgroundLayers)
            layer.CalculateImageWidth();
    }

}
