using System.Collections.Generic;
using UnityEngine;

public class PathfindingTileSystem : MonoBehaviour
{
    [SerializeField] Vector3 tileMapOrigin = Vector3.zero;
    [SerializeField] int tileMapSortingOrder = 0;
    [SerializeField] int tileMapRenderLayerMask = 0;
    [SerializeField] bool isHorizontalGrid = true;
    [SerializeField] int width;
    [SerializeField] int height;
    [SerializeField] float tileSize;

    [SerializeField] HeatMapTileVisual heatMapTileVisual;
    [SerializeField] bool hasHeatMapVisual = true;
    [SerializeField] TestingTileMap.AddingVauleShapes thisAddingValueShape = TestingTileMap.AddingVauleShapes.DecrementValueDistributionDiamond;
    [SerializeField] bool showDebugVisual = true;

    PathFinding thisPathFindingTileSystem;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        thisPathFindingTileSystem = new PathFinding(this.transform, "PathFindingNode", width, height, tileSize, isHorizontalGrid, tileMapOrigin, hasHeatMapVisual, showDebugVisual, tileMapSortingOrder, tileMapRenderLayerMask);
    }

    // Update is called once per frame
    void Update()
    {
        if (InputManager.Instance.GetisLeftClickPressed())
        {
            Vector3 mouseWorldPosition = InputManager.Instance.GetMouseWorldPosition();
            thisPathFindingTileSystem.GetPathFindingTileSet().GetTileObjectFromVerticalGrid(mouseWorldPosition, out int x, out int y);
            List<PathNodeTileObject> path = thisPathFindingTileSystem.FindPath(0, 0, x, y);
            if (path != null)
            {
                for (int i = 0; i < path.Count - 1; i++)
                {
                    Vector3 start = tileMapOrigin + new Vector3(path[i].x, path[i].y) * tileSize + Vector3.one * tileSize * 0.5f;
                    Vector3 end = tileMapOrigin + new Vector3(path[i + 1].x, path[i + 1].y) * tileSize + Vector3.one * tileSize * 0.5f;
                    Debug.DrawLine(start, end, Color.black, 5f, false);
                }

            }
        }
        if (InputManager.Instance.GetisRightClickPressed())
        {
            Vector3 mousWorldPositon = InputManager.Instance.GetMouseWorldPosition();
            thisPathFindingTileSystem.GetPathFindingTileSet().GetTileObjectFromVerticalGrid(mousWorldPositon, out int x, out int y);
            thisPathFindingTileSystem.GetNode(x, y).SetIsWalkable(!thisPathFindingTileSystem.GetNode(x, y).isWalkable);
        }
    }
}
