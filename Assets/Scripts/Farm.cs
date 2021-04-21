
using UnityEngine;
using System.Collections.Generic;

public class Farm : Buidling
{
    public override void ComputeResources(FactionResources resources)
    {
        foreach (Hex adjacentHex in HexManager.GetAdjacentHexes(this.currentHex)) {
            if (adjacentHex.GetTerrainType() == TerrainType.Grassland) {
                resources.food += 1;
            }
        }
    }
}