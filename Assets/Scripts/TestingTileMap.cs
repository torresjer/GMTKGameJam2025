using UnityEngine;

public class TestingTileMap : MonoBehaviour
{
    public enum AddingVauleShapes
    {
        StaticValueDistributionDiamond,
        StaticValueDistributionSquare,
        DecrementValueDistributionDiamond,
        DecrementValueDistributionSquare

    }


    [SerializeField] Vector3 tileMapOrigin = Vector3.zero;
    [SerializeField] bool isHorizontalGrid = true;
    [SerializeField] int width;
    [SerializeField] int height;
    [SerializeField] float tileSize;

    [SerializeField] HeatMapTileVisual heatMapTileVisual;
    [SerializeField] bool hasHeatMapVisual = true;
    [SerializeField] AddingVauleShapes thisAddingValueShape = AddingVauleShapes.DecrementValueDistributionDiamond;

    [SerializeField] bool boolTestValue = true;
    [SerializeField] int intTestValue = 5;
    [SerializeField] int entireValueRange = 1;
    [SerializeField] int testRange = 5;

    [SerializeField] bool showDebugVisual = true;
    TileSetSystem<int> thisIntTileMap;
    TileSetSystem<bool> thisBoolTileMap;
    TileSetSystem<HeatMapVisualIntGridObject> thisHeatMapVisualIntGridObject;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //thisIntTileMap = new TileSetSystem<int>(this.transform, "IntTile", width, height, tileSize, isHorizontalGrid, tileMapOrigin, hasHeatMapVisual, showDebugVisual, () => default(int));
        //thisBoolTileMap = new TileSetSystem<bool>(this.transform, "BoolTile", width, height, tileSize, isHorizontalGrid, tileMapOrigin, hasHeatMapVisual, showDebugVisual, () => default(bool));
        thisHeatMapVisualIntGridObject = new TileSetSystem<HeatMapVisualIntGridObject>(this.transform, "HeatMapTile", width, height, tileSize, isHorizontalGrid, tileMapOrigin, hasHeatMapVisual, showDebugVisual, (TileSetSystem<HeatMapVisualIntGridObject> tileSet) => new HeatMapVisualIntGridObject(tileSet));
        if (hasHeatMapVisual)
        {
            if (heatMapTileVisual != null)
                heatMapTileVisual.SetCurrentTileSet(thisHeatMapVisualIntGridObject); 
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (InputManager.Instance.GetisLeftClickPressed())
        {
           thisHeatMapVisualIntGridObject.GetTileObject(InputManager.Instance.GetMouseWorldPosition()).AddValueToTile(
               InputManager.Instance.GetMouseWorldPosition(), intTestValue, entireValueRange, testRange, thisAddingValueShape);
        }

    }
}
