using ReformEnt;
using System;
using UnityEngine;

public class TileSetSystem<T>
{

    public event EventHandler<OnTileValueChangeEventArgs> OnTileValueChanged;

    public class OnTileValueChangeEventArgs : EventArgs
    {
        public int x;
        public int y;
    }

    const int FIRST_DIMENTION = 0;
    const int SECOND_DIMENTION = 1;

    int width;
    int height;

    Vector3 tileOriginPosition = Vector3.zero;
    Vector2 currentTileIndex = Vector2.zero;
    T[,] tiles;
 
    float tileSize;
    bool isHorizontalTileSet;
    bool hasHeatMapVisual;

    TextMesh[,] debugArray;

    public TileSetSystem(Transform parentGridObject, string tileGameObjectName, int width, int height, float tileSize, bool horizontalGrid, Vector3 originPosition, bool hasHeatMapVisual, bool showDebugVisual, Func<TileSetSystem<T>, T> IntializeGridObject)
    {
        this.width = width;
        this.height = height;
        this.tileSize = tileSize;
        this.isHorizontalTileSet = horizontalGrid;
        this.tileOriginPosition = originPosition;
        this.hasHeatMapVisual = hasHeatMapVisual;

        tiles = new T[width, height];
        debugArray = new TextMesh[width, height];

        //intialize custom object
        for (int x = 0; x < tiles.GetLength(FIRST_DIMENTION); x++)
        {
            for (int y = 0; y < tiles.GetLength(SECOND_DIMENTION); y++)
            {
                tiles[x, y] = IntializeGridObject(this);
            }
        }

        if (showDebugVisual)
        {
            for (int x = 0; x < tiles.GetLength(FIRST_DIMENTION); x++)
            {
                for (int y = 0; y < tiles.GetLength(SECOND_DIMENTION); y++)
                {
                    if (isHorizontalTileSet)
                    {
                        debugArray[x, y] = Utilities.UI.CreateWorldText(tileGameObjectName, tiles[x, y]?.ToString(), parentGridObject, GetWorldPositionForHorizontalTile(x, y) + new Vector3(tileSize, tileSize) * .5f, 8, Color.white, TextAnchor.MiddleCenter, TextAlignment.Center);
                        Debug.DrawLine(GetWorldPositionForHorizontalTile(x, y), GetWorldPositionForHorizontalTile(x, y + 1), Color.white, 100f);
                        Debug.DrawLine(GetWorldPositionForHorizontalTile(x, y), GetWorldPositionForHorizontalTile(x + 1, y), Color.white, 100f);
                    }
                    else
                    {
                        debugArray[x, y] = Utilities.UI.CreateWorldText(tileGameObjectName, tiles[x, y]?.ToString(), parentGridObject, GetWorldPositionForVerticalTile(x, y) + new Vector3(tileSize, tileSize) * .5f, 8, Color.white, TextAnchor.MiddleCenter, TextAlignment.Center);
                        Debug.DrawLine(GetWorldPositionForVerticalTile(x, y), GetWorldPositionForVerticalTile(x, y + 1), Color.white, 100f);
                        Debug.DrawLine(GetWorldPositionForVerticalTile(x, y), GetWorldPositionForVerticalTile(x + 1, y), Color.white, 100f);
                    }

                }
            }

            if (isHorizontalTileSet)
            {
                Debug.DrawLine(GetWorldPositionForHorizontalTile(0, height), GetWorldPositionForHorizontalTile(width, height), Color.white, 100f);
                Debug.DrawLine(GetWorldPositionForHorizontalTile(width, 0), GetWorldPositionForHorizontalTile(width, height), Color.white, 100f);
            }
            else
            {
                Debug.DrawLine(GetWorldPositionForVerticalTile(0, height), GetWorldPositionForVerticalTile(width, height), Color.white, 100f);
                Debug.DrawLine(GetWorldPositionForVerticalTile(width, 0), GetWorldPositionForVerticalTile(width, height), Color.white, 100f);
            }


            OnTileValueChanged += HandleTileSetChange;
        }
    }
    void HandleTileSetChange(object sender, OnTileValueChangeEventArgs eventArgs)
    {

        debugArray[eventArgs.x, eventArgs.y].text = tiles[eventArgs.x, eventArgs.y]?.ToString();
    }
    public Vector3 GetWorldPositionForVerticalTile(int x, int y)
    {
        return new Vector3(x, y) * tileSize + tileOriginPosition;
    }
    public Vector3 GetWorldPositionForHorizontalTile(int x, int y)
    {
        return new Vector3(x, 0, y) * tileSize + tileOriginPosition;
    }
    public void GetTileObjectFromVerticalGrid(Vector3 worldPosition, out int x, out int y)
    {
        x = Mathf.FloorToInt((worldPosition - tileOriginPosition).x / tileSize);
        y = Mathf.FloorToInt((worldPosition - tileOriginPosition).y / tileSize);
    }
    public void GetTileObjectFromHorizontalGrid(Vector3 worldPosition, out int x, out int y)
    {
        x = Mathf.FloorToInt((worldPosition - tileOriginPosition).x / tileSize);
        y = Mathf.FloorToInt((worldPosition - tileOriginPosition).z / tileSize);
    }
    public T GetTileObject(int x, int y)
    {

        if ((x >= 0 && y >= 0) && (x < width && y < height))
        {
            return tiles[x, y];
        }

        //tile not found
        return default(T);
    }
    public T GetTileObject(Vector3 worldPosition)
    {
        int x, y;
        if (isHorizontalTileSet)
        {
            GetTileObjectFromHorizontalGrid(worldPosition, out x, out y);
            currentTileIndex.x = x;
            currentTileIndex.y = y;
            return GetTileObject(x, y);
        }
        else
        {
            GetTileObjectFromVerticalGrid(worldPosition, out x, out y);
            currentTileIndex.x = x;
            currentTileIndex.y = y;
            return GetTileObject(x, y);
        }
    }
    public T[,] GetTileObjectArray() { return tiles; }
    public Vector2 GetCurrentTileIndex() { return currentTileIndex; }
    public float GetTileSize() { return tileSize; }
    public int GetTileSetWidth() { return width; }
    public int GetTileSetHeight() { return height; }
    public bool GetTileSetIsHorizontal() { return isHorizontalTileSet; }
    public void SetTileObject(int x, int y, T value)
    {
        if (!hasHeatMapVisual)
        {
            if ((x >= 0 && y >= 0) && (x < width && y < height))
            {
                tiles[x, y] = value;
                OnTileValueChanged?.Invoke(this, new OnTileValueChangeEventArgs() { x = x, y = y });
            }
        }

    }
    public void SetTileObject(Vector3 worldPosition, T value)
    {
        int x, y;
        if (isHorizontalTileSet)
        {
            GetTileObjectFromHorizontalGrid(worldPosition, out x, out y);
            SetTileObject(x, y, value);
        }
        else
        {
            GetTileObjectFromVerticalGrid(worldPosition, out x, out y);
            SetTileObject(x, y, value);
        }
    }
    public void TriggerGridObjectChanged(int x, int y)
    {
        OnTileValueChanged?.Invoke(this, new OnTileValueChangeEventArgs() { x = x, y = y });
    }


}
