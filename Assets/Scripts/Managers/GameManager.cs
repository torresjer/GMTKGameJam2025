using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] Vector2 xMinMaxBoundsForLevel;
    [SerializeField] Vector2 yMinMaxBoundsForLevel;
    [SerializeField] Vector2 zMinMaxBoundsForLevel;

    private CameraMovement cameraMovement;
    private Vector3 playerPos = Vector3.zero;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cameraMovement = Camera.main.GetComponent<CameraMovement>();
        cameraMovement.SetIntialTargetForCameraFollow(() => playerPos);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetPlayerPos(Vector3 currentPlayerPos) { playerPos = currentPlayerPos; } 
    public Vector2 GetXMinMaxBoundsForLevel() { return xMinMaxBoundsForLevel; }
    public Vector2 GetYMinMaxBoundsForLevel() { return yMinMaxBoundsForLevel; }
    public Vector2 GetZMinMaxBoundsForLevel() { return zMinMaxBoundsForLevel; }
    public Vector3 GetPlayerPos() { return playerPos; }
    public Vector3 ClampPosToLvlBounds(Vector3 nextPos)
    {
        nextPos.x = Mathf.Clamp(nextPos.x, xMinMaxBoundsForLevel.x, xMinMaxBoundsForLevel.y);
        nextPos.y = Mathf.Clamp(nextPos.y, yMinMaxBoundsForLevel.x, yMinMaxBoundsForLevel.y);
        nextPos.z = Mathf.Clamp(nextPos.z, zMinMaxBoundsForLevel.x, zMinMaxBoundsForLevel.y);

        return new Vector3(nextPos.x, nextPos.y, nextPos.z);
    }
   public Vector3 ClampCameraPosToLvlBounds(Vector3 nextPos, float xOffset, float yOffset)
    {
        nextPos.x = Mathf.Clamp(nextPos.x, xMinMaxBoundsForLevel.x + xOffset, xMinMaxBoundsForLevel.y - xOffset);
        nextPos.y = Mathf.Clamp(nextPos.y, yMinMaxBoundsForLevel.x + yOffset, yMinMaxBoundsForLevel.y - yOffset);
        nextPos.z = Mathf.Clamp(nextPos.z, zMinMaxBoundsForLevel.x, zMinMaxBoundsForLevel.y);

        return new Vector3(nextPos.x, nextPos.y, nextPos.z);
    }
}
