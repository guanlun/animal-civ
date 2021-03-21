using System.Collections.Generic;
public class HexIndex
{
    public int row;
    public int col;

    public HexIndex(int row, int col)
    {
        this.row = row;
        this.col = col;
    }
}

public class HexUtils
{
    public static int numRows;
    public static int numCols;
    public static List<HexIndex> GetAdjacentHexes(int rowIdx, int colIdx)
    {
        List<HexIndex> listOfAdjacentHexes = new List<HexIndex>();

        if (rowIdx % 2 == 0) {
            if (rowIdx > 0) {
                if (colIdx > 0) {
                    listOfAdjacentHexes.Add(new HexIndex(rowIdx - 1, colIdx - 1));
                }
                if (colIdx < numCols - 1) {
                    listOfAdjacentHexes.Add(new HexIndex(rowIdx - 1, colIdx));
                }
            }

            if (colIdx > 0) {
                listOfAdjacentHexes.Add(new HexIndex(rowIdx, colIdx - 1));
            }
            if (colIdx < numCols - 1) {
                listOfAdjacentHexes.Add(new HexIndex(rowIdx, colIdx + 1));
            }

            if (rowIdx < numRows - 1) {
                if (colIdx > 0) {
                    listOfAdjacentHexes.Add(new HexIndex(rowIdx + 1, colIdx - 1));
                }
                if (colIdx < numCols - 1) {
                    listOfAdjacentHexes.Add(new HexIndex(rowIdx + 1, colIdx));
                }
            }
        } else {
            if (rowIdx > 0) {
                listOfAdjacentHexes.Add(new HexIndex(rowIdx - 1, colIdx));
                listOfAdjacentHexes.Add(new HexIndex(rowIdx - 1, colIdx + 1));
            }

            if (colIdx > 0) {
                listOfAdjacentHexes.Add(new HexIndex(rowIdx, colIdx - 1));
            }
            if (colIdx < numCols - 2) {
                listOfAdjacentHexes.Add(new HexIndex(rowIdx, colIdx + 1));
            }

            if (rowIdx < numRows - 1) {
                listOfAdjacentHexes.Add(new HexIndex(rowIdx + 1, colIdx));
                listOfAdjacentHexes.Add(new HexIndex(rowIdx + 1, colIdx + 1));
            }
        }

        return listOfAdjacentHexes;
    }
}