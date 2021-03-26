using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGameManager : MonoBehaviour
{
    public GameObject hexGridPrefab;
    public GameObject unitPrefab;

    public GameObject lumberMillPrefab;
    public int numRows;
    public int numCols;

    private List<Unit> units = new List<Unit>();

    private Faction playerFaction;
    private List<Faction> aiFactions = new List<Faction>();

    private Hex selectedHex = null;
    private bool isUnitSelected = false;

    void Awake()
    {
        HexManager.InitHexGrid(this.numRows, this.numCols, this.hexGridPrefab);
    }

    // Start is called before the first frame update
    void Start()
    {
        this.playerFaction = new Faction(true);

        GameObject unitGameObject1 = Instantiate(unitPrefab);
        Unit unit1 = unitGameObject1.GetComponent<Unit>();
        this.playerFaction.AddUnit(unit1);
        unit1.SetCurrentHex(HexManager.hexGrid[1][1].GetComponent<Hex>());

        GameObject unitGameObject2 = Instantiate(unitPrefab);
        Unit unit2 = unitGameObject2.GetComponent<Unit>();
        this.playerFaction.AddUnit(unit2);
        unit2.SetCurrentHex(HexManager.hexGrid[2][1].GetComponent<Hex>());

        Faction aiFaction1 = new Faction();
        GameObject aiGameObject1 = Instantiate(unitPrefab);
        Unit aiUnit1 = aiGameObject1.GetComponent<Unit>();
        aiFaction1.AddUnit(aiUnit1);
        aiUnit1.SetCurrentHex(HexManager.hexGrid[2][2].GetComponent<Hex>());

        this.aiFactions.Add(aiFaction1);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(0)) {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit)) {
                GameObject hitObject = hit.collider.gameObject;
                if (hitObject.name == "HitPlane") {
                    Hex clickedHex = HexManager.GetClosestHexObjectAtPosition(hit.point);

                    if (clickedHex) {
                        if (clickedHex.unitOnHex) {
                            this.ClearAllHexStates();
                            clickedHex.SetSelected(true); // TODO: move selected unit state to unit
                            this.selectedHex = clickedHex;
                            this.isUnitSelected = true;

                            if (clickedHex.unitOnHex.unitFaction.isPlayerFaction) {
                                if (clickedHex.unitOnHex.remainingMoves > 0) {
                                    foreach (Hex adjacentHex in HexManager.GetAdjacentHexes(clickedHex)) {
                                        adjacentHex.SetAdjacent(true);
                                    }
                                }
                            }
                        } else if (this.isUnitSelected) {
                            if (clickedHex && !clickedHex.unitOnHex) {
                                if (clickedHex.isAdjacent) {
                                    this.selectedHex.unitOnHex.MoveToHex(clickedHex);
                                }
                                this.ClearAllHexStates();
                                this.isUnitSelected = false;
                            }
                        }
                    } else {
                        // not clicked on any hexes
                        this.ClearAllHexStates();
                        this.isUnitSelected = false;
                    }
                }
            }
        }
    }

    private void ClearAllHexStates()
    {
        foreach (List<GameObject> hexRow in HexManager.hexGrid) {
            foreach(GameObject hexObject in hexRow) {
                Hex hex = hexObject.GetComponent<Hex>();
                hex.SetSelected(false);
                hex.SetAdjacent(false);
            }
        }
    }

    public void EndTurn()
    {
        foreach (Faction aiFaction in this.aiFactions) {
            aiFaction.StartTurn();
        }

        foreach (Unit unit in this.playerFaction.units) {
            unit.ResetRemainingMoves();
        }
        this.ClearAllHexStates();
    }

    public void Build()
    {
        GameObject lumberMillGameObject = Instantiate(this.lumberMillPrefab, this.selectedHex.transform.position, Quaternion.identity);
    }
}
