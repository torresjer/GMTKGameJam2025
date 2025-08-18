using NUnit.Framework.Constraints;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class PathNodeTileObject : TileObject<PathNodeTileObject>
{
    const int NUM_OF_PATHNODE_SPRITES = 3; 
    public enum PathNodeSprite
    {
        None,
        Blank,
        UnWalkable
    }

    public PathNodeSprite nodeSprite;
    //Walking cost from start node
    public int gCost;
    //heuristic cost to reach end node
    public int hCost;
    //G+H
    public int fCost;

    public bool isWalkable;
    public PathNodeTileObject prevNode;

    public PathNodeTileObject.TileMapVisualsUVs[] PathNodeSpriteUVs = new PathNodeTileObject.TileMapVisualsUVs[NUM_OF_PATHNODE_SPRITES] 
    {    (new PathNodeTileObject.TileMapVisualsUVs
            { visualState = PathNodeTileObject.PathNodeSprite.None, uv00Pixels = new Vector2Int { x = 0, y = 0 }, uv11Pixels = new Vector2Int { x = 0, y = 0 }}),
        (new PathNodeTileObject.TileMapVisualsUVs
            { visualState = PathNodeTileObject.PathNodeSprite.Blank, uv00Pixels = new Vector2Int { x = 0, y = 0 }, uv11Pixels = new Vector2Int { x = 1, y = 1 }}),
         (new PathNodeTileObject.TileMapVisualsUVs
            { visualState = PathNodeTileObject.PathNodeSprite.UnWalkable, uv00Pixels = new Vector2Int { x = 63, y = 0 }, uv11Pixels = new Vector2Int { x = 64, y = 1 }})};

    public PathNodeTileObject(TileSetSystem<PathNodeTileObject> pathFindingTileSet, int x, int y, bool hasBlankVisual) 
        : base(pathFindingTileSet, x, y)
    {
        isWalkable = true;
        HasVisual = true;
        
        VisualStateCount = Enum.GetValues(typeof(PathNodeSprite)).Length;
        uvCoordsDictionary = new Dictionary<System.Enum, TileObject<PathNodeTileObject>.UVCoords>();
        if (!hasBlankVisual)
        {
            VisualState = PathNodeSprite.None;
        }
        else
        {
            VisualState = PathNodeSprite.Blank;
        }

    }
    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }
    public void SetIsWalkable(bool isWalkable, bool hasBlankVisual)
    {
        this.isWalkable = isWalkable;
        SetTilemapSprite(isWalkable, hasBlankVisual);
        Debug.Log("X: " + x + " Y: " + y);
        tileSetSystem.TriggerGridObjectChanged(x, y);
    }
    public override string ToString()
    {
        return x + "," + y;
    }
    private void SetTilemapSprite(bool isWalkable, bool hasBlankVisual)
    {
        if (isWalkable && !hasBlankVisual)
        {
            this.nodeSprite = PathNodeSprite.None;
            VisualState = PathNodeSprite.None;
        }
        if (isWalkable && hasBlankVisual)
        {
            this.nodeSprite = PathNodeSprite.Blank;
            VisualState = PathNodeSprite.Blank;
        }
        if (!isWalkable)
        {
            this.nodeSprite = PathNodeSprite.UnWalkable;
            VisualState = PathNodeSprite.UnWalkable;
        }
    }
    public override void SetTextureForVisual(Texture visualTexture)
    {
        float textureWidth = visualTexture.width;
        float textureHeight = visualTexture.height;
        SetUVCoords(textureWidth, textureHeight);
    }
    private void SetUVCoords(float visualTextureWidth, float visualTextureHeight)
    {
        foreach (TileMapVisualsUVs pathNodeVisualUV in PathNodeSpriteUVs)
        {
            uvCoordsDictionary[pathNodeVisualUV.visualState] = new UVCoords
            {
                uv00 = new Vector2(pathNodeVisualUV.uv00Pixels.x / visualTextureWidth, pathNodeVisualUV.uv00Pixels.y / visualTextureHeight),
                uv11 = new Vector2(pathNodeVisualUV.uv11Pixels.x / visualTextureWidth, pathNodeVisualUV.uv11Pixels.y / visualTextureHeight),
            };
        }
    }
    public override PathNodeTileObject.UVCoords GetTileUV()
    {
        if(!uvCoordsDictionary.ContainsKey(this.VisualState))
            return new UVCoords { uv00 = Vector2.zero, uv11 = Vector2.zero };

        return uvCoordsDictionary[this.VisualState];
    }

    public SaveObject SavePathNode()
    {
        return new SaveObject
        {
            pathNodeSprite = (PathNodeSprite)VisualState,
            x = x,
            y = y,

        };

    }

    public void LoadPathNode(SaveObject saveObject)
    {
        VisualState = saveObject.pathNodeSprite;
    }

    [System.Serializable]
    public class SaveObject
    {
        public PathNodeTileObject.PathNodeSprite pathNodeSprite;
        public int x;
        public int y;
    }

}
