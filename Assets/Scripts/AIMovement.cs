using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class AIMovement : MonoBehaviour
{

    [SerializeField] float moveSpeed = 3.0f;
    [SerializeField] float waitTimeAtWaypoint = 1.0f;
    [SerializeField] List<Transform> walkPatternPositions;
    [SerializeField] Vector3 startPos;

    private PathfindingTileSystem thisTileSystem;
    private List<Vector3> currentPath = new List<Vector3>();
    private int currentWaypointIndex = 0;      // index in currentPath
    private int currentTargetPatternIndex = 0; // index in walkPatternPositions
    private float waitTimer = 0f;

    Animator AIAnimator;
    SpriteRenderer AISpriteRenderer;
    private void OnEnable()
    {
        thisTileSystem = FindAnyObjectByType<PathfindingTileSystem>();
        AIAnimator = GetComponentInChildren<Animator>();
        AISpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (AIAnimator != null)
            AIAnimator.runtimeAnimatorController = AnimationManager.Instance.GetPlayerAnimator();

        PathfindingTileSystem.OnTileSystemReady += HandleTileSystemReady;
    }

    private void HandleTileSystemReady()
    {
        transform.position = startPos;
        if (walkPatternPositions != null && walkPatternPositions.Count > 1)
            SetNewPath(transform.position, walkPatternPositions[0].position);
    }

    void Update()
    {
        if (currentPath == null || currentPath.Count == 0) return;

        // Handle waiting at a waypoint
        if (waitTimer > 0)
        {
            waitTimer -= Time.deltaTime;
            return;
        }

        Vector3 target = currentPath[currentWaypointIndex];
        Vector3 moveDir = (target - transform.position).normalized;
        if (moveDir != Vector3.zero)
        {
            if (AIAnimator != null)
                AIAnimator.SetBool("IsWalking", true);
            if (AISpriteRenderer != null && moveDir.x < 0)
            {
                AISpriteRenderer.flipX = true;
            }
            else
            {
                AISpriteRenderer.flipX = false;
            }

        }
        else
        {
            if (AIAnimator != null)
                AIAnimator.SetBool("IsWalking", false);
            if (AISpriteRenderer != null)
                AISpriteRenderer.flipX = false;
        }

        transform.position += moveDir * moveSpeed * Time.deltaTime;

        // If close enough to current target, move to next
        if (Vector3.Distance(transform.position, target) < 0.1f)
        {
            currentWaypointIndex++;

            if (currentWaypointIndex >= currentPath.Count)
            {
                // Finished this path, wait and set next
                waitTimer = waitTimeAtWaypoint;
                currentTargetPatternIndex = (currentTargetPatternIndex + 1) % walkPatternPositions.Count;

                Vector3 nextTarget = walkPatternPositions[currentTargetPatternIndex].position;
                SetNewPath(transform.position, nextTarget);
            }
        }
    }

    private void SetNewPath(Vector3 start, Vector3 end)
    {
        currentPath = thisTileSystem?.CalculatePathForTwoPoints(start, end);
        currentWaypointIndex = 0;
    }
}

