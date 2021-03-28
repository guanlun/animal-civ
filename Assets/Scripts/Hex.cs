
using UnityEngine;
using System.Collections.Generic;

public enum TerrainType {
    Grassland,
    Forest,
    Hill,
    Desert,
    Water,
    Snow,
}

public class Hex : MonoBehaviour {
    public Unit unitOnHex;

    public Buidling buildingOnHex;

    public static float COLUMN_SPACING = 1.732f * 1.02f;
    public static float ROW_SPACING = 1.5f * 1.02f;

    public int rowIdx;
    public int colIdx;

    private TerrainType terrainType;

    public bool isMovable = false;

    public Material defaultStateMaterial;
    public Material selectedStateMaterial;
    public Material adjacentStateMaterial;

    public GameObject forestTerrainPrefab;

    // game object on top of the hex (e.g. terrain object, building, etc.)
    public GameObject propGameObject;

    public GameObject fogOverlay;

    public HashSet<Faction> exploredByFactions = new HashSet<Faction>();

    void Awake()
    {
        this.gameObject.GetComponent<Renderer>().material = defaultStateMaterial;
        this.fogOverlay = this.transform.Find("fog").gameObject;
    }

    public void SetTerrainType(TerrainType terrainType) {
        this.terrainType = terrainType;

        if (this.terrainType == TerrainType.Forest) {
            this.propGameObject = Instantiate(forestTerrainPrefab, this.transform.position, Quaternion.identity);
            this.propGameObject.SetActive(false);
        }
    }

    public void SetSelected(bool selected)
    {
        this.gameObject.GetComponent<Renderer>().material = selected ? selectedStateMaterial : defaultStateMaterial;
    }

    public void SetAdjacent(bool isAdjacent)
    {
        this.isMovable = isAdjacent;
        this.gameObject.GetComponent<Renderer>().material = isAdjacent ? adjacentStateMaterial : defaultStateMaterial;
    }

    public Vector3 GetCenterPos()
    {
        bool isOnEvenRow = this.rowIdx % 2 == 0;

        return new Vector3(
            colIdx * COLUMN_SPACING + (isOnEvenRow ? 0f : COLUMN_SPACING / 2),
            0,
            this.rowIdx * ROW_SPACING
        );
    }

    public void SetExplored(Faction exploringFaction)
    {
        this.exploredByFactions.Add(exploringFaction);
        if (exploringFaction.isPlayerFaction) {
            this.fogOverlay.SetActive(false);

            if (this.propGameObject) {
                // Not every hex has prop
                this.propGameObject.SetActive(true);
            }

            if (this.unitOnHex) {
                this.unitOnHex.gameObject.SetActive(true);
            }
        }
    }

    public bool IsExploredByPlayer()
    {
        foreach (Faction faction in this.exploredByFactions) {
            if (faction.isPlayerFaction) {
                return true;
            }
        }

        return false;
    }

    public int GetMovementCost()
    {
        switch (this.terrainType) {
            case TerrainType.Forest:
            case TerrainType.Hill:
                return 2;
            default:
                return 1;
        }
    }
}