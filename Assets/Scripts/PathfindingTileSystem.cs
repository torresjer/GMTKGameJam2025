using System;
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

    [SerializeField] bool hasVisual = true;
    [SerializeField] bool hasBlankVisual = true;
    [SerializeField] bool showDebugVisual = true;



    
    TileSetSystemVisual<PathNodeTileObject> tileSetVisual = null;
    PathFinding thisPathFindingTileSystem;

    public static event Action OnTileSystemReady;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        thisPathFindingTileSystem = new PathFinding(this.transform, "PathFindingNode", width, height, tileSize, isHorizontalGrid, tileMapOrigin, hasVisual, hasBlankVisual, showDebugVisual, tileMapSortingOrder, tileMapRenderLayerMask);
        tileSetVisual = new TileSetSystemVisual<PathNodeTileObject>(this.gameObject, thisPathFindingTileSystem.GetPathFindingTileSet());
        OnTileSystemReady?.Invoke();
    }

    private void LateUpdate()
    {
        if (tileSetVisual.GetTileSetSystemVisual())
        {
            tileSetVisual.SetTileSetSystemVisual(false);
            tileSetVisual.UpdateVisual();
        }
        
    }
    // Update is called once per frame
   /* void Update()
    {
        if (InputManager.Instance.GetisLeftClickPressed())
        {
           CalculatePathFromPosToMouseClick();
        }
        if (InputManager.Instance.GetisRightClickPressed())
        {
            SetPathNodeToUnwalkable();
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            Save();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            Load();
        }

    }
   */
    private void SetPathNodeToUnwalkable() 
    {
        Vector3 mousWorldPositon = InputManager.Instance.GetMouseWorldPosition();
        thisPathFindingTileSystem.GetPathFindingTileSet().GetTileObjectIndexFromVerticalGrid(mousWorldPositon, out int x, out int y);
        thisPathFindingTileSystem.GetNode(x, y).SetIsWalkable(!thisPathFindingTileSystem.GetNode(x, y).isWalkable, hasBlankVisual);
    }
    public List<Vector3> CalculatePathForTwoPoints(Vector3 startPos, Vector3 endPos)
    {
        List<PathNodeTileObject> path;
        if (!isHorizontalGrid)
        {
            thisPathFindingTileSystem.GetPathFindingTileSet().GetTileObjectIndexFromVerticalGrid(startPos, out int x1, out int y1);
            thisPathFindingTileSystem.GetPathFindingTileSet().GetTileObjectIndexFromVerticalGrid(endPos, out int x2, out int y2);
            path = thisPathFindingTileSystem.FindPath(x1, y1, x2, y2);
        }
        else
        {
            thisPathFindingTileSystem.GetPathFindingTileSet().GetTileObjectFromHorizontalGrid(startPos, out int x1, out int y1);
            thisPathFindingTileSystem.GetPathFindingTileSet().GetTileObjectFromHorizontalGrid(endPos, out int x2, out int y2);
            path = thisPathFindingTileSystem.FindPath(x1, y1, x2, y2);
        }

        List<Vector3> wayPoints = new List<Vector3>();

        if (path != null)
        {
            foreach(PathNodeTileObject pathNode in path)
            {
                Vector3 worldPos = tileMapOrigin + new Vector3(pathNode.x, pathNode.y) * tileSize + Vector3.one * tileSize * 0.5f;
                worldPos.z = 0;
                wayPoints.Add(worldPos);
            }

        }

        return wayPoints;
    }
    public void CalculatePathFromPosToMouseClick(Vector3 startPos = default(Vector3))
    {
        Vector3 mouseWorldPosition = InputManager.Instance.GetMouseWorldPosition();
        List<PathNodeTileObject> path;
        if (!isHorizontalGrid && (thisPathFindingTileSystem != null))
        {

            thisPathFindingTileSystem.GetPathFindingTileSet().GetTileObjectIndexFromVerticalGrid(startPos, out int x1, out int y1);
            thisPathFindingTileSystem.GetPathFindingTileSet().GetTileObjectIndexFromVerticalGrid(mouseWorldPosition, out int x2, out int y2);
            path = thisPathFindingTileSystem?.FindPath(x1, y1, x2, y2);
        }
        else
        {
            thisPathFindingTileSystem.GetPathFindingTileSet().GetTileObjectFromHorizontalGrid(startPos, out int x1, out int y1);
            thisPathFindingTileSystem.GetPathFindingTileSet().GetTileObjectFromHorizontalGrid(mouseWorldPosition, out int x2, out int y2);
            path = thisPathFindingTileSystem?.FindPath(x1, y1, x2, y2);
        }
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
    public void Save()
    {
        try
        {
            List<PathNodeTileObject.SaveObject> PathNodeSaveObjectList = new List<PathNodeTileObject.SaveObject>();
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    PathNodeTileObject pathNodeTile = thisPathFindingTileSystem.GetPathFindingTileSet().GetTileObject(x, y);
                    if (pathNodeTile != null)
                    {
                        PathNodeSaveObjectList.Add(pathNodeTile.SavePathNode());
                    }
                }
            }
            PathFindingTileSystemSaveObject currentSaveTileSystem = new PathFindingTileSystemSaveObject { PathFindingSaveObjectArray = PathNodeSaveObjectList.ToArray() };
            SaveLoadSystem.SaveObject("PathFindingLayouts", "TestLayout", currentSaveTileSystem, true);
            Debug.Log($"[Save] Successfully saved {PathNodeSaveObjectList.Count} nodes.");
        }
        catch(System.Exception ex)
        {
            Debug.LogError($"[Save] Failed to save PathfindingTileSystem: {ex.Message}\n{ex.StackTrace}");
        }

    }
    public void Load()
    {
        PathFindingTileSystemSaveObject saveObject = SaveLoadSystem.LoadObject<PathFindingTileSystemSaveObject>("PathFindingLayouts","TestLayout");
        foreach (PathNodeTileObject.SaveObject pathNodeSaveObject in saveObject.PathFindingSaveObjectArray)
        {
            PathNodeTileObject pathNode = thisPathFindingTileSystem.GetPathFindingTileSet().GetTileObject(pathNodeSaveObject.x, pathNodeSaveObject.y);
            pathNode.LoadPathNode(pathNodeSaveObject);
        }
        tileSetVisual.SetTileSetSystemVisual(true);
        tileSetVisual.UpdateVisual();
    }
       
    public class PathFindingTileSystemSaveObject
    {
        public PathNodeTileObject.SaveObject[] PathFindingSaveObjectArray;
    }
}
