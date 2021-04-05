using System.Collections.Generic;
using UnityEngine;

public class HexManager
{
    public static List<List<GameObject>> hexGrid = new List<List<GameObject>>();
    public static int numRows;
    public static int numCols;

    public static void InitHexGrid(int rowCount, int colCount, GameObject hexGridParent, GameObject hexGridPrefab)
    {
        numRows = rowCount;
        numCols = colCount;

        for (int rowIdx = 0; rowIdx < numRows; rowIdx++) {
            List<GameObject> hexRow = new List<GameObject>();
            for (int colIdx = 0; colIdx < (rowIdx % 2 == 0 ? numCols : numCols - 1); colIdx++) {
                GameObject hexGameObject = GameObject.Instantiate(hexGridPrefab, new Vector3(colIdx * Hex.COLUMN_SPACING + rowIdx % 2 * Hex.COLUMN_SPACING / 2, 0, rowIdx * Hex.ROW_SPACING), Quaternion.identity);
                hexGameObject.transform.parent = hexGridParent.transform;
                Hex hex = hexGameObject.GetComponent<Hex>();
                hex.rowIdx = rowIdx;
                hex.colIdx = colIdx;

                int randomNumber = Random.Range(0, 10);
                TerrainType terrainType;
                if (randomNumber < 2) {
                    terrainType = TerrainType.Grassland;
                } else if (randomNumber < 4) {
                    terrainType = TerrainType.Forest;
                } else if (randomNumber < 6) {
                    terrainType = TerrainType.Hill;
                } else if (randomNumber < 4) {
                    terrainType = TerrainType.Desert;
                } else {
                    terrainType = TerrainType.Water;
                }
                hex.SetTerrainType(terrainType);

                hexRow.Add(hexGameObject);
            }

            hexGrid.Add(hexRow);
        }
    }

    public static HashSet<Hex> GetHexesByMovementDistance(Hex hex, int distance)
    {
        HashSet<Hex> results = new HashSet<Hex>();
        GetHexesByMovementDistanceRecur(hex, distance, results);

        // remove the original hex
        results.Remove(hex);
        return results;
    }

    private static void GetHexesByMovementDistanceRecur(Hex hex, int distance, HashSet<Hex> existingHexes)
    {
        if (distance <= 0) {
            return;
        }

        foreach (Hex adjacentHex in GetAdjacentHexes(hex)) {
            existingHexes.Add(adjacentHex);
            GetHexesByMovementDistanceRecur(adjacentHex, distance - adjacentHex.GetMovementCost(), existingHexes);
        }
    }

    public static HashSet<Hex> GetAdjacentHexes(Hex hex)
    {
        int rowIdx = hex.rowIdx, colIdx = hex.colIdx;
        HashSet<Hex> adjacentHexes = new HashSet<Hex>();

        if (rowIdx % 2 == 0) {
            if (rowIdx > 0) {
                if (colIdx > 0) {
                    adjacentHexes.Add(hexGrid[rowIdx - 1][colIdx - 1].GetComponent<Hex>());
                }
                if (colIdx < numCols - 1) {
                    adjacentHexes.Add(hexGrid[rowIdx - 1][colIdx].GetComponent<Hex>());
                }
            }

            if (colIdx > 0) {
                adjacentHexes.Add(hexGrid[rowIdx][colIdx - 1].GetComponent<Hex>());
            }
            if (colIdx < numCols - 1) {
                adjacentHexes.Add(hexGrid[rowIdx][colIdx + 1].GetComponent<Hex>());
            }

            if (rowIdx < numRows - 1) {
                if (colIdx > 0) {
                    adjacentHexes.Add(hexGrid[rowIdx + 1][colIdx - 1].GetComponent<Hex>());
                }
                if (colIdx < numCols - 1) {
                    adjacentHexes.Add(hexGrid[rowIdx + 1][colIdx].GetComponent<Hex>());
                }
            }
        } else {
            if (rowIdx > 0) {
                adjacentHexes.Add(hexGrid[rowIdx - 1][colIdx].GetComponent<Hex>());
                adjacentHexes.Add(hexGrid[rowIdx - 1][colIdx + 1].GetComponent<Hex>());
            }

            if (colIdx > 0) {
                adjacentHexes.Add(hexGrid[rowIdx][colIdx - 1].GetComponent<Hex>());
            }
            if (colIdx < numCols - 2) {
                adjacentHexes.Add(hexGrid[rowIdx][colIdx + 1].GetComponent<Hex>());
            }

            if (rowIdx < numRows - 1) {
                adjacentHexes.Add(hexGrid[rowIdx + 1][colIdx].GetComponent<Hex>());
                adjacentHexes.Add(hexGrid[rowIdx + 1][colIdx + 1].GetComponent<Hex>());
            }
        }

        return adjacentHexes;
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