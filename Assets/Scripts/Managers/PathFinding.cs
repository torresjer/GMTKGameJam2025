using System;
using System.Collections.Generic;
using UnityEngine;

public class PathFinding 
{
    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;

    TileSetSystem<PathNodeTileObject> pathFindingTileSystem;
    List<PathNodeTileObject> openList;
    HashSet<PathNodeTileObject> closeList;

    public PathFinding(Transform parentGridObject, string tileGameObjectName, int width, int height, float tileSize, bool horizontalGrid, Vector3 originPosition, bool hasVisual, bool hasBlankVisual, bool showDebugVisual, int sortingOrder, int renderLayerMask)
    {
        pathFindingTileSystem = new TileSetSystem<PathNodeTileObject>(parentGridObject, tileGameObjectName, width, height, tileSize, horizontalGrid, originPosition, hasVisual, showDebugVisual,(TileSetSystem<PathNodeTileObject> g, int x, int y) => new PathNodeTileObject(g, x, y, hasBlankVisual), sortingOrder, renderLayerMask);
    }

    public List<PathNodeTileObject> FindPath(int startX, int startY, int endX, int endY)
    {
        PathNodeTileObject startNode = pathFindingTileSystem.GetTileObject(startX, startY);
        PathNodeTileObject endNode = pathFindingTileSystem.GetTileObject(endX, endY);
        openList = new List<PathNodeTileObject> { startNode };
        closeList = new HashSet<PathNodeTileObject>();

        for (int x = 0; x < pathFindingTileSystem.GetTileSetWidth(); x++)
        {
            for (int y = 0; y < pathFindingTileSystem.GetTileSetHeight(); y++)
            {
                PathNodeTileObject currentPathNode = pathFindingTileSystem.GetTileObject(x, y);
                currentPathNode.gCost = int.MaxValue;
                currentPathNode.CalculateFCost();
                currentPathNode.prevNode = null;
            } 
        }
        startNode.gCost = 0;
        startNode.hCost = CalculateDistanceCost(startNode, endNode);
        startNode.CalculateFCost();

        while (openList.Count > 0) 
        {
            PathNodeTileObject currentNode = GetLowestFCostNode(openList);
            if(currentNode == endNode)
            {
                //reached final node
                return CalculatePath(endNode);
            }

            openList.Remove(currentNode);
            closeList.Add(currentNode);

            foreach (PathNodeTileObject neighborNode in GetNeighborList(currentNode)) 
            {
                if(closeList.Contains(neighborNode)) continue;
                if (!neighborNode.isWalkable)
                {
                    closeList.Add(neighborNode);
                    continue;
                }
                   
                int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNode, neighborNode);

                if (tentativeGCost < neighborNode.gCost)
                {
                    neighborNode.prevNode = currentNode;
                    neighborNode.gCost = tentativeGCost;
                    neighborNode.hCost = CalculateDistanceCost(neighborNode, endNode);
                    neighborNode.CalculateFCost();

                    if (!openList.Contains(neighborNode))
                    {
                        openList.Add(neighborNode);
                    }
                }
            }
        }

        //Out of nodes in open list (no path)
        return null;
    }
    private List<PathNodeTileObject> GetNeighborList(PathNodeTileObject currentNode)
    {
        List<PathNodeTileObject> neighborList = new List<PathNodeTileObject>();

        if(currentNode.x - 1 >= 0)
        {
            //left neighbor
            neighborList.Add(GetNode(currentNode.x - 1, currentNode.y));
            //Left Down
            if (currentNode.y - 1 >= 0) neighborList.Add(GetNode(currentNode.x - 1, currentNode.y - 1));
            //left Up
            if (currentNode.y + 1 < pathFindingTileSystem.GetTileSetHeight()) neighborList.Add(GetNode(currentNode.x - 1, currentNode.y + 1));
        }
        if (currentNode.x + 1 < pathFindingTileSystem.GetTileSetWidth())
        {
            //Right neighbor
            neighborList.Add(GetNode(currentNode.x + 1, currentNode.y));
            //Right Down
            if (currentNode.y - 1 >= 0) neighborList.Add(GetNode(currentNode.x + 1, currentNode.y - 1));
            //Right Up
            if (currentNode.y + 1 < pathFindingTileSystem.GetTileSetHeight()) neighborList.Add(GetNode(currentNode.x + 1, currentNode.y + 1));
        }
        //Down
        if (currentNode.y - 1 >= 0) neighborList.Add(GetNode(currentNode.x, currentNode.y - 1));
        //Up
        if (currentNode.y + 1 < pathFindingTileSystem.GetTileSetHeight()) neighborList.Add(GetNode(currentNode.x, currentNode.y + 1));

        return neighborList;
    }
    public PathNodeTileObject GetNode(int x, int y)
    {
        return pathFindingTileSystem.GetTileObject(x, y);
    }
    public TileSetSystem<PathNodeTileObject> GetPathFindingTileSet()
    {
        return pathFindingTileSystem;
    }
    private List<PathNodeTileObject> CalculatePath(PathNodeTileObject endNode)
    {
        List<PathNodeTileObject> path = new List<PathNodeTileObject>();
        path.Add(endNode);
        PathNodeTileObject currentNode = endNode;
        while (currentNode.prevNode != null)
        {
            path.Add(currentNode.prevNode);
            currentNode = currentNode.prevNode;
        }
        path.Reverse();
        return path;
    }
    private int CalculateDistanceCost(PathNodeTileObject a,  PathNodeTileObject b)
    {
        if (a != null && b != null)
        {
            int xDistance = Mathf.Abs(a.x - b.x);
            int yDistance = Mathf.Abs(a.y - b.y);
            int remaining = Mathf.Abs(xDistance - yDistance);

            return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining; 
        }
        return 0;
    }
    private PathNodeTileObject GetLowestFCostNode(List<PathNodeTileObject> pathNodeList)
    {
        PathNodeTileObject lowestFCostNode = pathNodeList[0];
        for (int i = 1; i < pathNodeList.Count; i++) 
        {
            if (pathNodeList[i].fCost < lowestFCostNode.fCost) 
            { 
                lowestFCostNode = pathNodeList[i];
            }
        }
        return lowestFCostNode;
    }

}
