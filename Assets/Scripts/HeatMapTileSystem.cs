using UnityEngine;

public class HeatMapTileVisual : MonoBehaviour 
{
    
    private TileSetSystem<HeatMapVisualIntGridObject> thisTileSetForHeatMap;
    private Mesh currentobjectMesh;
    private bool heatmapUpdated;
   
   

    //HeatMap Mesh Data
    Vector3[] vertices;
    Vector2[] uvs;
    int[] triangles;

    Vector3 tileOriginPosition = Vector3.zero;
    int tileIndex;

    private void Awake()
    {
        currentobjectMesh = new Mesh();
        GetComponent<MeshFilter>().mesh = currentobjectMesh;
        heatmapUpdated = false;
    }
    public void SetCurrentTileSet(TileSetSystem<HeatMapVisualIntGridObject> tileSetForHeatMap) 
    { 
        this.thisTileSetForHeatMap = tileSetForHeatMap;
        UpdateHeatMapVisual();

    }

    private void ThisTileSetForHeatMap_OnTileValueChanged(object sender, TileSetSystem<HeatMapVisualIntGridObject>.OnTileValueChangeEventArgs e)
    {
       heatmapUpdated = true;
    }
    private void LateUpdate()
    {
        if (heatmapUpdated) 
        { 
            heatmapUpdated = false;
            UpdateHeatMapVisual();
          
        }
    }


    private void UpdateHeatMapVisual()
    {
        ReformEnt.Utilities.MeshUtils.CreateEmptyMeshArrays(thisTileSetForHeatMap.GetTileSetWidth() * thisTileSetForHeatMap.GetTileSetHeight(), out Vector3[] vertices, out Vector2[] uvs, out int[] triangles);
        for (int x = 0; x < thisTileSetForHeatMap.GetTileSetWidth(); x++)
        {
            for (int y = 0; y < thisTileSetForHeatMap.GetTileSetHeight(); y++)
            {
                tileIndex = x * thisTileSetForHeatMap.GetTileSetHeight() + y;

                int tilevalue = thisTileSetForHeatMap.GetTileObjectArray()[x,y].value;
                float tileValueNorm = (float)tilevalue / HeatMapVisualIntGridObject.HEAT_MAP_MAX;
                Vector2 tileValueUV = new Vector2((float)tileValueNorm, 0.0f);

                if (thisTileSetForHeatMap.GetTileSetIsHorizontal())
                {
                    Vector3 baseSize = new Vector3(1,0,1) * thisTileSetForHeatMap.GetTileSize();
                    Vector3 heatMapOffsetToGrid = baseSize * .5f;
                    ReformEnt.Utilities.MeshUtils.AddToMeshArrays(vertices, uvs, triangles, tileIndex, thisTileSetForHeatMap.GetWorldPositionForHorizontalTile(x, y) + heatMapOffsetToGrid, 0.0f, baseSize, tileValueUV, tileValueUV, ReformEnt.Utilities.MeshUtils.MeshOrientation.Horizontal);
                }
                else {
                    Vector3 baseSize = new Vector3(1, 1) * thisTileSetForHeatMap.GetTileSize();
                    Vector3 heatMapOffsetToGrid = baseSize * .5f;
                    ReformEnt.Utilities.MeshUtils.AddToMeshArrays(vertices, uvs, triangles, tileIndex, thisTileSetForHeatMap.GetWorldPositionForVerticalTile(x, y) + heatMapOffsetToGrid, 0.0f, baseSize, tileValueUV, tileValueUV, ReformEnt.Utilities.MeshUtils.MeshOrientation.Vertical);
                }
                thisTileSetForHeatMap.OnTileValueChanged += ThisTileSetForHeatMap_OnTileValueChanged;
            }
        }

        currentobjectMesh.vertices = vertices;
        currentobjectMesh.uv = uvs;
        currentobjectMesh.triangles = triangles;
    }
}
