
using UnityEngine;
using System.Collections;

public class Farm : Buidling
{
    public override void ComputeResources(FactionResources resources)
    {
        foreach (Hex adjacentHex in HexManager.GetAdjacentHexes(this.currentHex)) {
            if (adjacentHex.GetTerrainType() == TerrainType.Grassland) {
                StartCoroutine(this.AnimateSpriteMove(adjacentHex.foodSprite, this.currentHex.transform.position));
                resources.food += 1;
            }
        }
    }

    private IEnumerator AnimateSpriteMove(GameObject sprite, Vector3 destPosition)
    {
        sprite.SetActive(true);
        float movedRatio = 0f;
        Vector3 moveVector = destPosition - sprite.transform.position;

        while (movedRatio < 1f) {
            float delta = Time.deltaTime * 5;
            movedRatio += delta;
            sprite.transform.position += moveVector * delta;
            yield return null;
        }

        sprite.SetActive(false);
    }
}