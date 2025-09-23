using System.Collections.Generic;
using UnityEngine;

public class TileSetSystemVisual<T> where T : TileObject<T>
{
    private TileSetSystem<T> thisTileSetForVisual;
    private Mesh currentobjectMesh;
    private MeshRenderer currentobjectRenderer;
    private bool visual;

    // mesh data
    private Vector3[] vertices;
    private Vector2[] uvs;
    private int[] triangles;

    // tiles that need updating this frame
    private List<Vector2Int> dirtyTiles = new List<Vector2Int>();

    public TileSetSystemVisual(GameObject gameobject, TileSetSystem<T> tileSetForHeatMap)
    {
        currentobjectMesh = new Mesh();
        gameobject.GetComponent<MeshFilter>().mesh = currentobjectMesh;
        currentobjectRenderer = gameobject.GetComponent<MeshRenderer>();

        SetCurrentTileSet(tileSetForHeatMap);
        visual = false;
    }

    private void SetCurrentTileSet(TileSetSystem<T> tileSetForHeatMap)
    {
        this.thisTileSetForVisual = tileSetForHeatMap;

        // setup texture for each tile
        SetCurrentTextureForTileSetObjects();

        // allocate mesh arrays once
        ReformEnt.Utilities.MeshUtils.CreateEmptyMeshArrays(
            thisTileSetForVisual.GetTileSetWidth() * thisTileSetForVisual.GetTileSetHeight(),
            out vertices, out uvs, out triangles);

        // initial build of the whole grid
        BuildFullMesh();

        // subscribe ONCE
        thisTileSetForVisual.OnTileValueChanged += ThisTileSet_OnTileVisualChanged;
    }

    private void SetCurrentTextureForTileSetObjects()
    {
        Texture mainTex = currentobjectRenderer.material.mainTexture;
        for (int x = 0; x < thisTileSetForVisual.GetTileSetWidth(); x++)
        {
            for (int y = 0; y < thisTileSetForVisual.GetTileSetHeight(); y++)
            {
                T tileObject = (T)thisTileSetForVisual.GetTileObject(x, y);
                if (tileObject != null)
                    tileObject.SetTextureForVisual(mainTex);
            }
        }
    }

    // full grid build (first time only)
    private void BuildFullMesh()
    {
        for (int x = 0; x < thisTileSetForVisual.GetTileSetWidth(); x++)
        {
            for (int y = 0; y < thisTileSetForVisual.GetTileSetHeight(); y++)
            {
                UpdateSingleTile(x, y);
            }
        }
        ApplyMesh();
    }

    // event fires when a single tile changes
    private void ThisTileSet_OnTileVisualChanged(object sender, TileSetSystem<T>.OnTileValueChangeEventArgs e)
    {
        dirtyTiles.Add(new Vector2Int(e.x, e.y));
        visual = true;
    }

    // called from LateUpdate in PathfindingTileSystem
    public void UpdateVisual()
    {
        if (dirtyTiles.Count == 0) return;

        foreach (var pos in dirtyTiles)
        {
            UpdateSingleTile(pos.x, pos.y);
        }
        dirtyTiles.Clear();

        ApplyMesh();
    }

    // update one tile’s quad
    private void UpdateSingleTile(int x, int y)
    {
        int tileIndex = x * thisTileSetForVisual.GetTileSetHeight() + y;
        T tileObject = (T)thisTileSetForVisual.GetTileObject(x, y);
        TileObject<T>.UVCoords tileUV = tileObject.GetTileUV();

        if (thisTileSetForVisual.GetTileSetIsHorizontal())
        {
            Vector3 baseSize = new Vector3(1, 0, 1) * thisTileSetForVisual.GetTileSize();
            if ((tileUV.uv00 == Vector2.zero) && (tileUV.uv11 == Vector2.zero))
                baseSize = Vector3.zero;
            Vector3 offset = baseSize * .5f;

            ReformEnt.Utilities.MeshUtils.AddToMeshArrays(
                vertices, uvs, triangles, tileIndex,
                thisTileSetForVisual.GetWorldPositionForHorizontalTile(x, y) + offset,
                0.0f, baseSize,
                tileUV.uv00, tileUV.uv11,
                ReformEnt.Utilities.MeshUtils.MeshOrientation.Horizontal);
        }
        else
        {
            Vector3 baseSize = new Vector3(1, 1) * thisTileSetForVisual.GetTileSize();
            if ((tileUV.uv00 == Vector2.zero) && (tileUV.uv11 == Vector2.zero))
                baseSize = Vector3.zero;
            Vector3 offset = baseSize * .5f;

            ReformEnt.Utilities.MeshUtils.AddToMeshArrays(
                vertices, uvs, triangles, tileIndex,
                thisTileSetForVisual.GetWorldPositionForVerticalTile(x, y) + offset,
                0.0f, baseSize,
                tileUV.uv00, tileUV.uv11,
                ReformEnt.Utilities.MeshUtils.MeshOrientation.Vertical);
        }
    }

    // push updated arrays to mesh
    private void ApplyMesh()
    {
        currentobjectMesh.Clear();
        currentobjectMesh.vertices = vertices;
        currentobjectMesh.uv = uvs;
        currentobjectMesh.triangles = triangles;
        currentobjectMesh.RecalculateBounds();
    }

    public bool GetTileSetSystemVisual() { return visual; }
    public void SetTileSetSystemVisual(bool currentVisualState) { visual = currentVisualState; }
}
