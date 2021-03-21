﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGameManager : MonoBehaviour
{
    public GameObject hexGridPrefab;
    public GameObject unitPrefab;
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
        this.aiFactions.Add(new Faction());

        GameObject unitGameObject1 = Instantiate(unitPrefab);
        Unit unit1 = unitGameObject1.GetComponent<Unit>();
        unit1.SetCurrentHex(HexManager.hexGrid[1][1].GetComponent<Hex>());

        GameObject unitGameObject2 = Instantiate(unitPrefab);
        Unit unit2 = unitGameObject2.GetComponent<Unit>();
        unit2.SetCurrentHex(HexManager.hexGrid[2][1].GetComponent<Hex>());

        this.playerFaction.AddUnit(unit1);
        this.playerFaction.AddUnit(unit2);

        GameObject aiGameObject1 = Instantiate(unitPrefab);
        Unit aiUnit1 = aiGameObject1.GetComponent<Unit>();
        aiUnit1.SetCurrentHex(HexManager.hexGrid[5][5].GetComponent<Hex>());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(0)) {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit)) {
                GameObject hitObject = hit.collider.gameObject;
                if (hitObject.name == "HitPlane") {
                    Hex clickedHex = this.GetClosestHexObjectAtPosition(hit.point);

                    if (clickedHex) {
                        if (clickedHex.unitOnHex) {
                            this.ClearAllHexStates();
                            clickedHex.SetSelected(true); // TODO: move selected unit state to unit
                            this.selectedHex = clickedHex;
                            this.isUnitSelected = true;

                            if (clickedHex.unitOnHex.remainingMoves > 0) {
                                foreach (Hex adjacentHex in HexManager.GetAdjacentHexes(clickedHex)) {
                                    adjacentHex.SetAdjacent(true);
                                }
                            }
                        } else if (this.isUnitSelected) {
                            if (clickedHex && clickedHex.isAdjacent && !clickedHex.unitOnHex) {
                                this.ClearAllHexStates();
                                this.selectedHex.unitOnHex.MoveToHex(clickedHex);

                                this.isUnitSelected = false;
                            }
                        }
                    }
                }
            }
        }
    }

    public void EndTurn() {
        foreach (Faction aiFaction in this.aiFactions) {
            aiFaction.StartTurn();
        }

        foreach (Unit unit in this.playerFaction.units) {
            unit.ResetRemainingMoves();
        }
        this.ClearAllHexStates();
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

    private Hex GetClosestHexObjectAtPosition(Vector3 pos)
    {
        float rowPos = pos.z / Hex.ROW_SPACING;
        int firstRowIdx = (int)Mathf.Floor(rowPos);
        int secondRowIdx = (int)Mathf.Ceil(rowPos);

        bool isFirstRowEven = firstRowIdx % 2 == 0;

        float colPos = pos.x / Hex.COLUMN_SPACING;
        int firstColIdx = (int)Mathf.Round(colPos - (isFirstRowEven ? 0f : 0.5f));
        int secondColIdx = (int)Mathf.Round(colPos - (isFirstRowEven ? 0.5f : 0f));

        bool isFirstCandidateValid = firstRowIdx >= 0 && firstRowIdx < this.numRows && firstColIdx >= 0 && firstColIdx < this.numCols - (isFirstRowEven ? 0 : 1);
        bool isSecondCandidateValid = secondRowIdx >= 0 && secondRowIdx < this.numRows && secondColIdx >= 0 && secondColIdx < this.numCols - (isFirstRowEven ? 1 : 0);

        Hex firstCandidate = null, secondCandidate = null;
        if (!isFirstCandidateValid && !isSecondCandidateValid) {
            return null;
        }

        if (isFirstCandidateValid) {
            firstCandidate = HexManager.hexGrid[firstRowIdx][firstColIdx].GetComponent<Hex>();
        }

        if (isSecondCandidateValid) {
            secondCandidate = HexManager.hexGrid[secondRowIdx][secondColIdx].GetComponent<Hex>();
        }

        if (!isFirstCandidateValid) {
            return secondCandidate;
        } else if (!isSecondCandidateValid) {
            return firstCandidate;
        }

        if (Vector3.Distance(firstCandidate.GetCenterPos(), pos) < Vector3.Distance(secondCandidate.GetCenterPos(), pos)) {
            return firstCandidate;
        } else {
            return secondCandidate;
        }
    }

    // private List<Hex> GetAdjacentHexes(Hex hex) {
    //     List<Hex> listOfAdjacentHexes = new List<Hex>();

    //     foreach (HexIndex hexIndex in HexUtils.GetAdjacentHexes(hex.rowIdx, hex.colIdx)) {
    //         listOfAdjacentHexes.Add(HexManager.hexGrid[hexIndex.row][hexIndex.col].GetComponent<Hex>());
    //     }

    //     return listOfAdjacentHexes;
    // }
}
