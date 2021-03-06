using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainGameManager : MonoBehaviour
{
    public GameObject UICanvas;
    private UIManager uiManager;

    public GameObject mapCameraGameObject;
    public GameObject mapUICameraGameObject;
    public GameObject objectCameraGameObject;

    private Camera mapCamera;
    private Camera mapUICamera;
    private Camera objectCamera;

    public GameObject hexGridParent;

    public GameObject hexContainerPrefab;
    public GameObject landHexGridPrefab;
    public GameObject waterHexGridPrefab;
    public GameObject unitPrefab;

    public GameObject lumberMillPrefab;
    public GameObject farmPrefab;

    public int numRows;
    public int numCols;

    private Faction playerFaction;
    private List<Faction> factions = new List<Faction>();

    private Unit selectedUnit = null;
    private bool isUnitSelected = false;

    private Buidling selectedBuilding = null;

    void Awake()
    {
        this.mapCamera = this.mapCameraGameObject.GetComponent<Camera>();
        this.mapUICamera = this.mapUICameraGameObject.GetComponent<Camera>();
        this.objectCamera = this.objectCameraGameObject.GetComponent<Camera>();
        float[,] heightMap = TerrainGenerator.GenerateHeightMap(33);
        HexManager.InitHexGrid(this.numRows, this.numCols, heightMap, this.hexGridParent, this.hexContainerPrefab);

        this.uiManager = UICanvas.GetComponent<UIManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        this.playerFaction = new Faction(true);
        this.factions.Add(this.playerFaction);

        Faction aiFaction1 = new Faction();
        this.factions.Add(aiFaction1);

        Faction aiFaction2 = new Faction();
        this.factions.Add(aiFaction2);

        this.SetFactionStartingLocations();
    }

    // Update is called once per frame
    void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject()) {
            // Mouse on UI elements
            return;
        }

        // Temporary camera move logic for development
        if (Input.GetKey(KeyCode.A)) {
            this.mapCamera.transform.Translate(new Vector3(-0.03f, 0, 0));
            this.mapUICamera.transform.Translate(new Vector3(-0.03f, 0, 0));
            this.objectCamera.transform.Translate(new Vector3(-0.03f, 0, 0));
        } else if (Input.GetKey(KeyCode.D)) {
            this.mapCamera.transform.Translate(new Vector3(0.03f, 0, 0));
            this.mapUICamera.transform.Translate(new Vector3(0.03f, 0, 0));
            this.objectCamera.transform.Translate(new Vector3(0.03f, 0, 0));
        }

        if (Input.GetKey(KeyCode.S)) {
            this.mapCamera.transform.Translate(new Vector3(0, -0.03f, 0));
            this.mapUICamera.transform.Translate(new Vector3(0, -0.03f, 0));
            this.objectCamera.transform.Translate(new Vector3(0, -0.03f, 0));
        } else if (Input.GetKey(KeyCode.W)) {
            this.mapCamera.transform.Translate(new Vector3(0, 0.03f, 0));
            this.mapUICamera.transform.Translate(new Vector3(0, 0.03f, 0));
            this.objectCamera.transform.Translate(new Vector3(0, 0.03f, 0));
        }

        if (Input.GetMouseButtonUp(0)) {
            RaycastHit hit;
            if (Physics.Raycast(this.mapCamera.ScreenPointToRay(Input.mousePosition), out hit)) {
                GameObject hitObject = hit.collider.gameObject;
                if (hitObject.name == "HitPlane") {
                    Hex clickedHex = HexManager.GetClosestHexObjectAtPosition(hit.point);

                    if (clickedHex) {
                        if (this.isUnitSelected) {
                            Unit unitOnHex = clickedHex.unitOnHex;
                            if (unitOnHex) {
                                if (unitOnHex == this.selectedUnit) {
                                    // Clicking on the hex of the currently selected unit - Check if their are any building on the hex to select
                                    if (clickedHex.buildingOnHex) {
                                        this.SetSelectedBuilding(clickedHex.buildingOnHex);
                                    }

                                } else if (unitOnHex.unitFaction == this.selectedUnit.unitFaction) {
                                    // TODO: friendly unit action
                                } else {
                                    // Enemy unit
                                    this.selectedUnit.AttackTarget(unitOnHex);
                                    this.ClearActiveStates();
                                }
                            } else {
                                if (clickedHex.isMovable) {
                                    this.selectedUnit.MoveToHex(clickedHex);
                                }

                                this.ClearActiveStates();
                            }
                        } else if (clickedHex.unitOnHex) {
                            this.SetSelectedUnit(clickedHex.unitOnHex);

                            if (selectedUnit.unitFaction.isPlayerFaction) {
                                selectedUnit.ComputePossibleActions();
                                if (selectedUnit.isPlayerUnit()) {
                                    selectedUnit.ToggleActionStatesDisplay(true);
                                }
                            }
                        } else if (clickedHex.buildingOnHex) {
                            this.SetSelectedBuilding(clickedHex.buildingOnHex);
                        } else {
                            // Clicking on a hex that doesn't have any units or buildings on it
                            this.ClearActiveStates();
                        }
                    } else {
                        // not clicked on any hexes
                        this.ClearActiveStates();
                    }
                }
            }
        }
    }

    private void SetFactionStartingLocations()
    {
        int minDistanceApart = (int)((this.numRows + this.numCols) / this.factions.Count / 1.5f);
        List<Hex> availableLandHexes = HexManager.GetAllHexesInList().FindAll(hex => hex.GetTerrainType() != TerrainType.Water);

        foreach (Faction faction in this.factions) {
            if (availableLandHexes.Count == 0) {
                // TODO: re-generate map?
                break;
            }

            Hex factionStartingHex = availableLandHexes[Random.Range(0, availableLandHexes.Count)];
            availableLandHexes = availableLandHexes.FindAll(hex => (factionStartingHex.rowIdx - hex.rowIdx + factionStartingHex.colIdx - hex.colIdx) >= minDistanceApart);

            GameObject unitGameObject = Instantiate(this.unitPrefab);
            Unit startingUnit = unitGameObject.GetComponent<Unit>();
            faction.AddUnit(startingUnit);
            startingUnit.SetCurrentHex(factionStartingHex);

            if (faction.isPlayerFaction) {
                this.mapCamera.transform.position = unitGameObject.transform.position + new Vector3(0, 10, -10);
                this.mapUICamera.transform.position = unitGameObject.transform.position + new Vector3(0, 10, -10);
                this.objectCamera.transform.position = unitGameObject.transform.position + new Vector3(0, 10, -10);
            }
        }
    }

    private void SetSelectedUnit(Unit unit)
    {
        this.ClearActiveStates();
        unit.SetSelected(true);
        this.selectedUnit = unit;
        this.isUnitSelected = true;
        this.uiManager.SetBuildingMenuUIActive(true);
    }

    private void SetSelectedBuilding(Buidling building)
    {
        this.ClearActiveStates();
        building.SetSelected(true);
        this.selectedBuilding = building;

        if (!building.GetHex().unitOnHex) {
            // Only show production UI when no other unit is on the hex
            this.uiManager.SetProductionMenuUIActive(true);
        }
    }

    private void ClearActiveStates()
    {
        if (this.selectedUnit && this.selectedUnit.isPlayerUnit()) {
            this.selectedUnit.SetSelected(false);
            this.selectedUnit.ToggleActionStatesDisplay(false);

            this.selectedUnit = null;
            this.isUnitSelected = false;

            this.uiManager.SetBuildingMenuUIActive(false);
        }

        if (this.selectedBuilding) {
            this.selectedBuilding.SetSelected(false);
            this.selectedBuilding = null;

            this.uiManager.SetProductionMenuUIActive(false);
        }
    }

    public void EndTurn()
    {
        this.ClearActiveStates();

        this.playerFaction.EndTurn();
        this.uiManager.UpdateResourcesValue();

        foreach (Faction faction in this.factions) {
            if (faction != this.playerFaction) {
                faction.StartTurn();
                faction.EndTurn();
            }
        }
    }

    public void CreateUnit(GameObject unitPrefab)
    {
        Hex currentHex = this.selectedBuilding.GetHex();

        GameObject unitGameObject = Instantiate(this.unitPrefab, currentHex.transform.position, Quaternion.identity);
        Unit unit = unitGameObject.GetComponent<Unit>();
        this.playerFaction.AddUnit(unit);
        unit.SetCurrentHex(currentHex);
    }

    public void Build(GameObject buildingPrefab)
    {
        Hex currentHex = this.selectedUnit.currentHex;

        if (currentHex.buildingOnHex) {
            // TODO: make sure it's my own building, what do we do if it's enemy building?
            // TODO: show UI asking if player wants to replace building
            Destroy(currentHex.buildingOnHex.gameObject);

            // TODO: remove from player faction
        }

        GameObject buildingGameObject = Instantiate(buildingPrefab, currentHex.transform.position, Quaternion.AngleAxis(180, Vector3.up));
        Buidling building = buildingGameObject.GetComponent<Buidling>();
        building.SetHex(currentHex);
        this.playerFaction.AddBuilding(currentHex.buildingOnHex);
    }

    public Faction GetPlayerFaction()
    {
        return this.playerFaction;
    }
}
