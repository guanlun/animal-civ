using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGameManager : MonoBehaviour
{
    public GameObject UICanvas;
    private UIManager uiManager;

    public GameObject hexGridParent;

    public GameObject hexContainerPrefab;
    public GameObject landHexGridPrefab;
    public GameObject waterHexGridPrefab;
    public GameObject unitPrefab;

    public GameObject lumberMillPrefab;
    public int numRows;
    public int numCols;

    private Faction playerFaction;
    private List<Faction> aiFactions = new List<Faction>();

    private Unit selectedUnit = null;
    private bool isUnitSelected = false;

    void Awake()
    {
        float[,] heightMap = TerrainGenerator.GenerateHeightMap(33);
        HexManager.InitHexGrid(this.numRows, this.numCols, heightMap, this.hexGridParent, this.hexContainerPrefab);

        this.uiManager = UICanvas.GetComponent<UIManager>();
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
                        if (this.isUnitSelected) {
                            if (clickedHex) {
                                Unit unitOnHex = clickedHex.unitOnHex;
                                if (unitOnHex) {
                                    if (unitOnHex.unitFaction == this.selectedUnit.unitFaction) { // enemy unit
                                        // TODO: friendly unit action
                                    } else {
                                        this.selectedUnit.AttackTarget(unitOnHex);
                                    }
                                } else {
                                    if (clickedHex.isMovable) {
                                        this.selectedUnit.MoveToHex(clickedHex);
                                    }
                                }

                                this.ClearActiveStates();
                                this.isUnitSelected = false;
                            }
                        } else if (clickedHex.unitOnHex) {
                            this.ClearActiveStates();
                            clickedHex.SetSelected(true); // TODO: move selected unit state to unit
                            this.selectedUnit = clickedHex.unitOnHex;
                            this.isUnitSelected = true;

                            if (selectedUnit.unitFaction.isPlayerFaction) {
                                selectedUnit.ComputePossibleActions();
                                if (selectedUnit.isPlayerUnit()) {
                                    selectedUnit.ToggleActionStatesDisplay(true);
                                }
                            }
                        }
                    } else {
                        // not clicked on any hexes
                        this.ClearActiveStates();
                        this.isUnitSelected = false;
                    }
                }
            }
        }
    }

    private void ClearActiveStates()
    {
        if (this.selectedUnit && this.selectedUnit.isPlayerUnit()) {
            this.selectedUnit.currentHex.SetSelected(false);
            this.selectedUnit.ToggleActionStatesDisplay(false);
        }
    }

    public void EndTurn()
    {
        foreach (Unit unit in this.playerFaction.units) {
            unit.ResetRemainingMoves();
        }
        this.ClearActiveStates();

        foreach (Buidling buidling in this.playerFaction.buildings) {
            // TODO: if resource building, add resources
            this.uiManager.SetWoodValue(10);
        }

        foreach (Faction aiFaction in this.aiFactions) {
            aiFaction.StartTurn();
        }
    }

    public void Build()
    {
        Hex currentHex = this.selectedUnit.currentHex;

        if (currentHex.buildingOnHex) {
            // TODO: make sure it's my own building, what do we do if it's enemy building?
            // TODO: show UI asking if player wants to replace building
            Destroy(currentHex.buildingOnHex.gameObject);

            // TODO: remove from player faction
        }

        GameObject lumberMillGameObject = Instantiate(this.lumberMillPrefab, currentHex.transform.position, Quaternion.identity);
        currentHex.buildingOnHex = lumberMillGameObject.GetComponent<LumberMill>();

        this.playerFaction.AddBuilding(currentHex.buildingOnHex);
    }
}
