using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CameraMovement : MonoBehaviour
{

    [SerializeField] float cameraFollowSpeed = 1.0f;
    [SerializeField] float offsetPaddingFromcenterX;
    [SerializeField] float offsetPaddingFromcenterY;

    private Func<Vector3> GetCameraFollowTargetFunc;
    private PixelPerfectCamera thisPixelPerfectCamera;

    float offsetPositionFromCenterX;
    float offsetPositionFromCenterY;
    float cameraZPos = -10.0f;
   

    private void OnEnable()
    {
        thisPixelPerfectCamera = GetComponent<PixelPerfectCamera>();
       
        
    }
    // Update is called once per frame
    void LateUpdate()
    {
        offsetPositionFromCenterX = (thisPixelPerfectCamera.refResolutionX / thisPixelPerfectCamera.assetsPPU) + offsetPaddingFromcenterX;
        offsetPositionFromCenterY = (thisPixelPerfectCamera.refResolutionY / thisPixelPerfectCamera.assetsPPU) + offsetPaddingFromcenterY;
        Vector3 cameraFollowTarget = GetCameraFollowTargetFunc();
        cameraFollowTarget.z = cameraZPos;

        Vector3 cameraFollowDir = (cameraFollowTarget - transform.position).normalized;
        float distanceToFollowTarget = Vector3.Distance(cameraFollowTarget, transform.position);

        if (distanceToFollowTarget > 0)
        {
            Vector3 newCameraPos = transform.position + (cameraFollowDir * distanceToFollowTarget * cameraFollowSpeed * Time.deltaTime);
            //calculates for the camera overshooting its position
            float distanceLeft = Vector3.Distance(newCameraPos, cameraFollowTarget);
            if(distanceLeft > distanceToFollowTarget)
            {
                newCameraPos = cameraFollowTarget;
            }
            
            transform.position = GameManager.Instance.ClampCameraPosToLvlBounds(newCameraPos, offsetPositionFromCenterX, offsetPositionFromCenterY);
        }
    }

    public void SetIntialTargetForCameraFollow(Func<Vector3> GetNewCameraFollowTargetFunc)
    {
        this.GetCameraFollowTargetFunc = GetNewCameraFollowTargetFunc;
    }

    public void SetNewTargetForCameraFollow(Func<Vector3> GetNewCameraFollowTargetFunc)
    {
        this.GetCameraFollowTargetFunc = GetNewCameraFollowTargetFunc;
    }
}
