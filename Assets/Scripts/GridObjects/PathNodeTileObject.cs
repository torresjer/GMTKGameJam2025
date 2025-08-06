using UnityEngine;

public class PathNodeTileObject 
{
    private TileSetSystem<PathNodeTileObject> pathFindingTileSet;
    public int x;
    public int y;

    //Walking cost from start node
    public int gCost;
    //heuristic cost to reach end node
    public int hCost;
    //G+H
    public int fCost;

    public bool isWalkable;
    public PathNodeTileObject prevNode;

    public PathNodeTileObject(TileSetSystem<PathNodeTileObject> pathFindingTileSet, int x, int y)
    {
        this.pathFindingTileSet = pathFindingTileSet;
        this.x = x;
        this.y = y;
        isWalkable = true;
    }
    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }
    public void SetIsWalkable(bool isWalkable)
    {
        this.isWalkable = isWalkable;
        pathFindingTileSet.TriggerGridObjectChanged(x, y);
    }
    public override string ToString()
    {
        return x + "," + y;
    }
}
