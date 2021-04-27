
using UnityEngine;
using System.Collections.Generic;

public class LoggingCamp : Buidling {
    public override void ComputeResources(FactionResources resources)
    {
        foreach (Hex adjacentHex in HexManager.GetAdjacentHexes(this.currentHex)) {
            if (adjacentHex.GetTerrainType() == TerrainType.Forest) {
                resources.wood += 1;
            }
        }
    }
}