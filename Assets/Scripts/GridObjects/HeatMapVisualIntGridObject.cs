using UnityEngine;

public class HeatMapVisualIntGridObject
{
    public const int HEAT_MAP_MAX = 100;
    public const int HEAT_MAP_MIN = 0;
    public TileSetSystem<HeatMapVisualIntGridObject> thisTileSet;
    public HeatMapVisualIntGridObject[,] tilesForThisSet;
    public int value;
    

    public HeatMapVisualIntGridObject(TileSetSystem<HeatMapVisualIntGridObject> tileSet)
    {
        thisTileSet = tileSet;
        tilesForThisSet = thisTileSet.GetTileObjectArray();
    }
    public void AddValueToTile(int x, int y, int value)
    {
        if ((x >= 0 && y >= 0) && (x < thisTileSet.GetTileSetWidth() && y < thisTileSet.GetTileSetHeight()))
        {
            value += thisTileSet.GetTileObject(x, y).value;
            SetTileValueWithHeatMapVisualConstrant(x, y, value);
        }
    }
    public void SetTileValueWithHeatMapVisualConstrant(int x, int y, int value)
    {
            value = Mathf.Clamp(value, HEAT_MAP_MIN, HEAT_MAP_MAX);
            tilesForThisSet[x,y].value = value;
            thisTileSet.TriggerGridObjectChanged(x,y);
    }
    public override string ToString()
    {
        return value.ToString();
    }
    public void AddValueToTile(Vector3 worldPosition, int value, int entireValueRange, int totalRange, TestingTileMap.AddingVauleShapes heatMapShape = TestingTileMap.AddingVauleShapes.StaticValueDistributionDiamond)
    {
        Debug.Log("triggering");
        switch (heatMapShape)
        {
            case TestingTileMap.AddingVauleShapes.StaticValueDistributionDiamond:
                if (thisTileSet.GetTileSetIsHorizontal())
                {
                    thisTileSet.GetTileObjectFromHorizontalGrid(worldPosition, out int originX, out int originY);

                    for (int x = 0; x < totalRange; x++)
                    {
                        for (int y = 0; y < totalRange - x; y++)
                        {
                            AddValueToTile((originX + x), (originY + y), value);
                            if (x != 0)
                                AddValueToTile((originX - x), (originY + y), value);
                            if (y != 0)
                            {
                                AddValueToTile((originX + x), (originY - y), value);

                                if (x != 0)
                                    AddValueToTile((originX - x), (originY - y), value);
                            }
                        }
                    }
                }
                else
                {
                    thisTileSet.GetTileObjectIndexFromVerticalGrid(worldPosition, out int originX, out int originY);

                    for (int x = 0; x < totalRange; x++)
                    {
                        for (int y = 0; y < totalRange - x; y++)
                        {
                            AddValueToTile((originX + x), (originY + y), value);
                            if (x != 0)
                                AddValueToTile((originX - x), (originY + y), value);
                            if (y != 0)
                            {
                                AddValueToTile((originX + x), (originY - y), value);

                                if (x != 0)
                                    AddValueToTile((originX - x), (originY - y), value);
                            }
                        }
                    }
                }
                break;
            case TestingTileMap.AddingVauleShapes.StaticValueDistributionSquare:
                if (thisTileSet.GetTileSetIsHorizontal())
                {
                    thisTileSet.GetTileObjectFromHorizontalGrid(worldPosition, out int originX, out int originY);

                    for (int x = 0; x < totalRange; x++)
                    {
                        for (int y = 0; y < totalRange; y++)
                        {
                            AddValueToTile((originX + x), (originY + y), value);
                            if (x != 0)
                                AddValueToTile((originX - x), (originY + y), value);
                            if (y != 0)
                            {
                                AddValueToTile((originX + x), (originY - y), value);

                                if (x != 0)
                                    AddValueToTile((originX - x), (originY - y), value);
                            }

                        }
                    }
                }
                else
                {
                    thisTileSet.GetTileObjectIndexFromVerticalGrid(worldPosition, out int originX, out int origingY);

                    for (int x = 0; x < totalRange; x++)
                    {
                        for (int y = 0; y < totalRange; y++)
                        {
                            AddValueToTile((originX + x), (origingY + y), value);
                            if (x != 0)
                                AddValueToTile((originX - x), (origingY + y), value);
                            if (y != 0)
                            {
                                AddValueToTile((originX + x), (origingY - y), value);

                                if (x != 0)
                                    AddValueToTile((originX - x), (origingY - y), value);
                            }
                        }
                    }
                }
                break;
            case TestingTileMap.AddingVauleShapes.DecrementValueDistributionDiamond:
                int lowerValueAmountDia = Mathf.RoundToInt((float)value / (float)(totalRange - entireValueRange));

                if (thisTileSet.GetTileSetIsHorizontal())
                {
                    thisTileSet.GetTileObjectFromHorizontalGrid(worldPosition, out int originX, out int originY);

                    for (int x = 0; x < totalRange; x++)
                    {
                        for (int y = 0; y < totalRange - x; y++)
                        {
                            int radiusOfShape = x + y;
                            int addValueAmount = value;
                            if (radiusOfShape >= entireValueRange)
                            {
                                addValueAmount -= lowerValueAmountDia * (radiusOfShape - entireValueRange);
                            }
                            if (addValueAmount >= 0)
                            {
                                AddValueToTile((originX + x), (originY + y), addValueAmount);

                                if (x != 0)
                                    AddValueToTile((originX - x), (originY + y), addValueAmount);

                                if (y != 0)
                                {
                                    AddValueToTile((originX + x), (originY - y), addValueAmount);

                                    if (x != 0)
                                        AddValueToTile((originX - x), (originY - y), addValueAmount);
                                }
                            }
                        }
                    }
                }
                else
                {
                    thisTileSet.GetTileObjectIndexFromVerticalGrid(worldPosition, out int originX, out int originY);

                    for (int x = 0; x < totalRange; x++)
                    {
                        for (int y = 0; y < totalRange - x; y++)
                        {
                            int radiusOfShape = x + y;
                            int addValueAmount = value;
                            if (radiusOfShape >= entireValueRange)
                            {
                                addValueAmount -= lowerValueAmountDia * (radiusOfShape - entireValueRange);
                            }
                            if (addValueAmount >= 0)
                            {
                                AddValueToTile((originX + x), (originY + y), addValueAmount);

                                if (x != 0)
                                    AddValueToTile((originX - x), (originY + y), addValueAmount);

                                if (y != 0)
                                {
                                    AddValueToTile((originX + x), (originY - y), addValueAmount);

                                    if (x != 0)
                                        AddValueToTile((originX - x), (originY - y), addValueAmount);
                                }
                            }
                        }
                    }
                }
                break;
            //Currently DecrementValueDistributionSquare does not function as orignally intended will have to look back at this if we find ourselves wanting this functionallity.
            case TestingTileMap.AddingVauleShapes.DecrementValueDistributionSquare:
                int lowerValueAmountSqur = Mathf.RoundToInt((float)value / (float)(totalRange - entireValueRange));
                if (thisTileSet.GetTileSetIsHorizontal())
                {
                    thisTileSet.GetTileObjectFromHorizontalGrid(worldPosition, out int originX, out int originY);

                    for (int x = 0; x <= totalRange; x++)
                    {
                        for (int y = 0; y <= totalRange; y++)
                        {
                            int radiusOfShape = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y));
                            int addValueAmount = value;
                            if (radiusOfShape >= entireValueRange)
                            {
                                addValueAmount -= lowerValueAmountSqur * (radiusOfShape - entireValueRange);
                            }
                            if (addValueAmount >= 0)
                            {
                                AddValueToTile((originX + x), (originY + y), addValueAmount);

                                if (x != 0)
                                    AddValueToTile((originX - x), (originY + y), addValueAmount);

                                if (y != 0)
                                {
                                    AddValueToTile((originX + x), (originY - y), addValueAmount);

                                    if (x != 0)
                                        AddValueToTile((originX - x), (originY - y), addValueAmount);
                                }
                            }
                        }
                    }
                }
                else
                {
                    thisTileSet.GetTileObjectIndexFromVerticalGrid(worldPosition, out int originX, out int originY);

                    for (int x = 0; x < totalRange; x++)
                    {
                        for (int y = 0; y < totalRange; y++)
                        {
                            int radiusOfShape = x + y;
                            int addValueAmount = value;
                            if (radiusOfShape >= entireValueRange)
                            {
                                addValueAmount -= lowerValueAmountSqur * (radiusOfShape - entireValueRange);
                            }
                            if (addValueAmount >= 0)
                            {
                                AddValueToTile((originX + x), (originY + y), addValueAmount);

                                if (x != 0)
                                    AddValueToTile((originX - x), (originY + y), addValueAmount);

                                if (y != 0)
                                {
                                    AddValueToTile((originX + x), (originY - y), addValueAmount);

                                    if (x != 0)
                                        AddValueToTile((originX - x), (originY - y), addValueAmount);
                                }
                            }
                        }
                    }
                }
                break;
            default:
                break;
        }
    }
}
