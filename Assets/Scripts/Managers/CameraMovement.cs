using System;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private Func<Vector3> GetCameraFollowTargetFunc;

    [SerializeField] float cameraFollowSpeed = 1.0f;
    float cameraZPos = -10.0f;
  
    // Update is called once per frame
    void LateUpdate()
    {
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
            transform.position = newCameraPos;
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
