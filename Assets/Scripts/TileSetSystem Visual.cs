using UnityEngine;

public class TileSetSystemVisual<T> where T : TileObject<T>
{
    
    private TileSetSystem<T> thisTileSetForVisual;
    private Mesh currentobjectMesh;
    private MeshRenderer currentobjectRenderer;
    private bool visual;
   
    int tileIndex;

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
        //Sets the current texture for the TileSetObjects
        SetCurrentTextureForTileSetObjects();
        UpdateVisual();
       
    }
    private void ThisTileSet_OnTileVisualChanged(object sender, TileSetSystem<T>.OnTileValueChangeEventArgs e)
    {
       visual = true;
        Debug.Log("Event triggered");
    }
    private void SetCurrentTextureForTileSetObjects()
    {
        for (int x = 0; x < thisTileSetForVisual.GetTileSetWidth(); x++)
        {
            for (int y = 0; y < thisTileSetForVisual.GetTileSetHeight(); y++)
            {
                T tileObject = (T)thisTileSetForVisual.GetTileObject(x, y);
                if (tileObject != null)
                    tileObject.SetTextureForVisual(currentobjectRenderer.material.mainTexture);
            }
        }
    }
    public void UpdateVisual()
    {
        if (thisTileSetForVisual.GetHasVisual())
        {
            
            ReformEnt.Utilities.MeshUtils.CreateEmptyMeshArrays(thisTileSetForVisual.GetTileSetWidth() * thisTileSetForVisual.GetTileSetHeight(), out Vector3[] vertices, out Vector2[] uvs, out int[] triangles);
            for (int x = 0; x < thisTileSetForVisual.GetTileSetWidth(); x++)
            {
                for (int y = 0; y < thisTileSetForVisual.GetTileSetHeight(); y++)
                {
                    tileIndex = x * thisTileSetForVisual.GetTileSetHeight() + y;

                    T tileObject = (T)thisTileSetForVisual.GetTileObject(x, y);

                    TileObject<T>.UVCoords tileUV = tileObject.GetTileUV();


                    if (thisTileSetForVisual.GetTileSetIsHorizontal())
                    {
                        Vector3 baseSize = new Vector3(1, 0, 1) * thisTileSetForVisual.GetTileSize();
                        if ((tileUV.uv00 == Vector2.zero) && (tileUV.uv11 == Vector2.zero))
                            baseSize = Vector3.zero;
                        Vector3 visualOffsetToGrid = baseSize * .5f;
                        ReformEnt.Utilities.MeshUtils.AddToMeshArrays(vertices, uvs, triangles, tileIndex, thisTileSetForVisual.GetWorldPositionForHorizontalTile(x, y) + visualOffsetToGrid, 0.0f, baseSize, tileUV.uv00, tileUV.uv11, ReformEnt.Utilities.MeshUtils.MeshOrientation.Horizontal);
                    }
                    else
                    {
                        Vector3 baseSize = new Vector3(1, 1) * thisTileSetForVisual.GetTileSize();
                        if ((tileUV.uv00 == Vector2.zero) && (tileUV.uv11 == Vector2.zero))
                            baseSize = Vector3.zero;
                        Vector3 visualOffsetToGrid = baseSize * .5f;
                        ReformEnt.Utilities.MeshUtils.AddToMeshArrays(vertices, uvs, triangles, tileIndex, thisTileSetForVisual.GetWorldPositionForVerticalTile(x, y) + visualOffsetToGrid, 0.0f, baseSize, tileUV.uv00, tileUV.uv11, ReformEnt.Utilities.MeshUtils.MeshOrientation.Vertical);
                    }
                    thisTileSetForVisual.OnTileValueChanged += ThisTileSet_OnTileVisualChanged;
                }
            }

            currentobjectMesh.vertices = vertices;
            currentobjectMesh.uv = uvs;
            currentobjectMesh.triangles = triangles;
        }
    }
    public bool GetTileSetSystemVisual() { return visual; }
    public void SetTileSetSystemVisual(bool currentVisualState) { visual = currentVisualState; }
}
