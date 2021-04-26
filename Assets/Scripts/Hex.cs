
using UnityEngine;
using System.Collections.Generic;

public enum TerrainType {
    Grassland,
    Forest,
    Hill,
    Desert,
    Water,
    Snow,

    Mountain,
}

public class Hex : MonoBehaviour {
    public Unit unitOnHex;

    public Buidling buildingOnHex;

    public static float COLUMN_SPACING = 1.732f;
    public static float ROW_SPACING = 1.5f;

    public int rowIdx;
    public int colIdx;

    private TerrainType terrainType;

    public bool isMovable = false;

    public Material grasslandMaterial;

    public GameObject movableIndicatorGameObject;

    public GameObject landHexPrefab;
    public GameObject waterHexPrefab;
    public GameObject forestTerrainPrefab;
    public GameObject hillTerrainPrefab;

    public GameObject mountainTerrainPrefab;

    // game objcet for the base terrain (water or land)
    public GameObject hexBaseGameObject;

    // game object on top of the hex (e.g. terrain object, building, etc.)
    public GameObject propGameObject;

    // Sprites for resources
    public GameObject foodSprite;
    public GameObject woodSprite;
    public GameObject ironSprite;

    public GameObject fogOverlay;

    public HashSet<Faction> exploredByFactions = new HashSet<Faction>();

    void Awake()
    {
        this.fogOverlay = this.transform.Find("fog").gameObject;
        this.movableIndicatorGameObject.GetComponent<SpriteRenderer>().material.renderQueue = 4000;
    }

    public void SetTerrainType(TerrainType terrainType) {
        this.terrainType = terrainType;

        this.hexBaseGameObject = Instantiate(
            this.terrainType == TerrainType.Water ? this.waterHexPrefab : this.landHexPrefab,
            this.transform.position,
            Quaternion.identity
        );
        this.hexBaseGameObject.transform.parent = this.transform;

        if (terrainType == TerrainType.Water) {
            this.hexBaseGameObject.transform.Find("hex-triangles").GetComponent<Renderer>().material.SetInt("_RowIndex", this.rowIdx);
        } else {
            this.hexBaseGameObject.GetComponent<Renderer>().material = grasslandMaterial;
        }

        GameObject terrainPrefab = null;
        switch (this.terrainType) {
            case TerrainType.Forest:
                terrainPrefab = forestTerrainPrefab;
                break;
            case TerrainType.Hill:
                terrainPrefab = hillTerrainPrefab;
                break;
            case TerrainType.Mountain:
                terrainPrefab = mountainTerrainPrefab;
                break;
        }

        if (terrainPrefab != null) {
            this.propGameObject = Instantiate(terrainPrefab, this.transform.position, Quaternion.AngleAxis(Random.Range(0, 5) * 60, Vector3.up));
            this.propGameObject.transform.parent = this.transform;

            // Make props inactive so that they aren't visible in fog
            this.propGameObject.SetActive(false);
        }
    }

    public TerrainType GetTerrainType() {
        return this.terrainType;
    }

    public void SetMoveable(bool isMovable)
    {
        this.isMovable = isMovable;
        this.movableIndicatorGameObject.SetActive(isMovable);
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

    public int GetViewingCost()
    {
        switch (this.terrainType) {
            case TerrainType.Forest:
            case TerrainType.Hill:
                return 2;
            default:
                return 1;
        }
    }

    public int GetMovementCost()
    {
        switch (this.terrainType) {
            case TerrainType.Forest:
            case TerrainType.Hill:
                return 2;
            case TerrainType.Water:
                return 100;
            case TerrainType.Mountain:
                return 100;
            default:
                return 1;
        }
    }
}