using System;
using System.Collections.Generic;
using UnityEngine;
public abstract class TileObject<T> where T : TileObject<T>
{
    public struct TileMapVisualsUVs
    {
        public System.Enum visualState;
        public Vector2Int uv00Pixels;
        public Vector2Int uv11Pixels;
    }
    public struct UVCoords
    {
        public Vector2 uv00;
        public Vector2 uv11;
    }
    protected TileSetSystem<T> tileSetSystem;
    public Dictionary<System.Enum, UVCoords> uvCoordsDictionary;
    public int x { get; protected set; }
    public int y { get; protected set; }

    public bool HasVisual { get; protected set; }

    /// Enum type describing possible visual states.
    public System.Enum VisualState { get; protected set; }

    /// Total number of visual states for this tile.
    public int VisualStateCount { get; protected set; }

    
    public TileObject(TileSetSystem<T> tileSetSystem, int x, int y)
    {
        SetTileSetSystem(tileSetSystem);
        this.x = x;
        this.y = y;
        HasVisual = false;  // Default
        VisualState = null; // Default
        VisualStateCount = 0;
    }

    public void SetTileSetSystem(TileSetSystem<T> tileSetSystem)
    {
        this.tileSetSystem = tileSetSystem;
    }

    /// Get the current UV offset for this tile based on VisualState.
    public abstract UVCoords GetTileUV();
    public abstract void SetTextureForVisual(Texture visualTexture);
    public override string ToString()
    {
        return $"{x},{y}";
    }
}

