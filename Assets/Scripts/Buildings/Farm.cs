
using UnityEngine;
using System.Collections;

public class Farm : Buidling
{
    public override void ComputeResources(FactionResources resources)
    {
        foreach (Hex adjacentHex in HexManager.GetAdjacentHexes(this.currentHex)) {
            if (adjacentHex.GetTerrainType() == TerrainType.Grassland) {
                StartCoroutine(this.AnimateResourceSpriteMove(adjacentHex.foodSprite, this.currentHex.transform.position));
                resources.food += 1;
            }
        }
    }
}