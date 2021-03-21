using UnityEngine;

public enum TerrainType {
    Grassland,
    Forest,
    Desert,
    Water,
    Snow,
}

public class Hex : MonoBehaviour {
    public Unit unitOnHex;

    public static float COLUMN_SPACING = 1.732f * 1.02f;
    public static float ROW_SPACING = 1.5f * 1.02f;

    public int rowIdx;
    public int colIdx;

    private TerrainType terrainType;

    public bool isAdjacent = false;

    public Material defaultStateMaterial;
    public Material selectedStateMaterial;
    public Material adjacentStateMaterial;

    public GameObject forestTerrainPrefab;

    public GameObject fogOverlay;

    void Awake()
    {
        this.gameObject.GetComponent<Renderer>().material = defaultStateMaterial;
        this.fogOverlay = this.transform.Find("fog").gameObject;
    }

    public void SetTerrainType(TerrainType terrainType) {
        this.terrainType = terrainType;

        if (this.terrainType == TerrainType.Forest) {
            Instantiate(forestTerrainPrefab, this.transform.position, Quaternion.identity);
        }
    }

    public void SetSelected(bool selected)
    {
        this.gameObject.GetComponent<Renderer>().material = selected ? selectedStateMaterial : defaultStateMaterial;
    }

    public void SetAdjacent(bool isAdjacent)
    {
        this.isAdjacent = isAdjacent;
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

    public void SetExplored()
    {
        this.fogOverlay.SetActive(false);
    }
}