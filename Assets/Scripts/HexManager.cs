using System.Collections.Generic;
using UnityEngine;

public class HexManager
{
    public static List<List<GameObject>> hexGrid = new List<List<GameObject>>();
    public static int numRows;
    public static int numCols;

    public static void InitHexGrid(int rowCount, int colCount, GameObject hexGridPrefab)
    {
        numRows = rowCount;
        numCols = colCount;

        for (int rowIdx = 0; rowIdx < numRows; rowIdx++) {
            List<GameObject> hexRow = new List<GameObject>();
            for (int colIdx = 0; colIdx < (rowIdx % 2 == 0 ? numCols : numCols - 1); colIdx++) {
                GameObject hexGameObject = GameObject.Instantiate(hexGridPrefab, new Vector3(colIdx * Hex.COLUMN_SPACING + rowIdx % 2 * Hex.COLUMN_SPACING / 2, 0, rowIdx * Hex.ROW_SPACING), Quaternion.identity);
                Hex hex = hexGameObject.GetComponent<Hex>();
                hex.rowIdx = rowIdx;
                hex.colIdx = colIdx;

                if (Random.Range(0, 10) < 2) {
                    hex.SetTerrainType(TerrainType.Forest);
                }

                hexRow.Add(hexGameObject);
            }

            hexGrid.Add(hexRow);
        }
    }
    public static List<Hex> GetAdjacentHexes(Hex hex)
    {
        int rowIdx = hex.rowIdx, colIdx = hex.colIdx;
        List<Hex> listOfAdjacentHexes = new List<Hex>();

        if (rowIdx % 2 == 0) {
            if (rowIdx > 0) {
                if (colIdx > 0) {
                    listOfAdjacentHexes.Add(hexGrid[rowIdx - 1][colIdx - 1].GetComponent<Hex>());
                }
                if (colIdx < numCols - 1) {
                    listOfAdjacentHexes.Add(hexGrid[rowIdx - 1][colIdx].GetComponent<Hex>());
                }
            }

            if (colIdx > 0) {
                listOfAdjacentHexes.Add(hexGrid[rowIdx][colIdx - 1].GetComponent<Hex>());
            }
            if (colIdx < numCols - 1) {
                listOfAdjacentHexes.Add(hexGrid[rowIdx][colIdx + 1].GetComponent<Hex>());
            }

            if (rowIdx < numRows - 1) {
                if (colIdx > 0) {
                    listOfAdjacentHexes.Add(hexGrid[rowIdx + 1][colIdx - 1].GetComponent<Hex>());
                }
                if (colIdx < numCols - 1) {
                    listOfAdjacentHexes.Add(hexGrid[rowIdx + 1][colIdx].GetComponent<Hex>());
                }
            }
        } else {
            if (rowIdx > 0) {
                listOfAdjacentHexes.Add(hexGrid[rowIdx - 1][colIdx].GetComponent<Hex>());
                listOfAdjacentHexes.Add(hexGrid[rowIdx - 1][colIdx + 1].GetComponent<Hex>());
            }

            if (colIdx > 0) {
                listOfAdjacentHexes.Add(hexGrid[rowIdx][colIdx - 1].GetComponent<Hex>());
            }
            if (colIdx < numCols - 2) {
                listOfAdjacentHexes.Add(hexGrid[rowIdx][colIdx + 1].GetComponent<Hex>());
            }

            if (rowIdx < numRows - 1) {
                listOfAdjacentHexes.Add(hexGrid[rowIdx + 1][colIdx].GetComponent<Hex>());
                listOfAdjacentHexes.Add(hexGrid[rowIdx + 1][colIdx + 1].GetComponent<Hex>());
            }
        }

        return listOfAdjacentHexes;
    }

    public static Hex GetClosestHexObjectAtPosition(Vector3 pos)
    {
        float rowPos = pos.z / Hex.ROW_SPACING;
        int firstRowIdx = (int)Mathf.Floor(rowPos);
        int secondRowIdx = (int)Mathf.Ceil(rowPos);

        bool isFirstRowEven = firstRowIdx % 2 == 0;

        float colPos = pos.x / Hex.COLUMN_SPACING;
        int firstColIdx = (int)Mathf.Round(colPos - (isFirstRowEven ? 0f : 0.5f));
        int secondColIdx = (int)Mathf.Round(colPos - (isFirstRowEven ? 0.5f : 0f));

        bool isFirstCandidateValid = firstRowIdx >= 0 && firstRowIdx < numRows && firstColIdx >= 0 && firstColIdx < numCols - (isFirstRowEven ? 0 : 1);
        bool isSecondCandidateValid = secondRowIdx >= 0 && secondRowIdx < numRows && secondColIdx >= 0 && secondColIdx < numCols - (isFirstRowEven ? 1 : 0);

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
}