using UnityEngine;
using System.Collections.Generic;

public class TerrainGenerator
{
    public static float[,] GenerateHeightMap(int boardSize)
    {
        float[,] heightMap = new float[boardSize,boardSize];

        heightMap[0,0] = Random.Range(0f, 1f);
        heightMap[0,boardSize - 1] = Random.Range(0f, 1f);
        heightMap[boardSize - 1,0] = Random.Range(0f, 1f);
        heightMap[boardSize - 1, boardSize - 1] = Random.Range(0f, 1f);

        int squareSize = boardSize;
        while (squareSize >= 3) {
            int halfSizeFloor = (squareSize - 1) / 2;
            int halfSizeCeil = halfSizeFloor + 1;

            for (int startRow = 0; startRow < boardSize - 1; startRow += squareSize - 1) {
                for (int startCol = 0; startCol < boardSize - 1; startCol += squareSize - 1) {
                    heightMap[startRow + halfSizeFloor, startCol + halfSizeFloor] = Mathf.Clamp(
                        (
                            heightMap[startRow, startCol] +
                            heightMap[startRow + squareSize - 1, startCol] +
                            heightMap[startRow, startCol + squareSize - 1] +
                            heightMap[startRow + squareSize - 1, startCol + squareSize - 1]
                        ) / 4 + Random.Range(-0.5f, 0.5f),
                        0,
                        1
                    );

                    ComputeSquareStep(heightMap, startRow, startCol + halfSizeFloor, halfSizeFloor, boardSize);
                    ComputeSquareStep(heightMap, startRow + halfSizeFloor, startCol, halfSizeFloor, boardSize);
                    ComputeSquareStep(heightMap, startRow + halfSizeFloor, startCol + squareSize - 1, halfSizeFloor, boardSize);
                    ComputeSquareStep(heightMap, startRow + squareSize - 1, startCol + halfSizeFloor, halfSizeFloor, boardSize);
                }
            }

            squareSize = halfSizeCeil;
        }

        return heightMap;
    }

    private static void ComputeSquareStep(float[,] heightMap, int row, int col, int halfStep, int totalSize)
    {
        float sum = 0f;
        int top = row - halfStep;
        int sampleCount = 0;
        if (top >= 0) {
            sum += heightMap[top,col];
            sampleCount++;
        }

        int left = col - halfStep;
        if (left >= 0) {
            sum += heightMap[row,left];
            sampleCount++;
        }

        int bottom = row + halfStep;
        if (bottom < totalSize) {
            sum += heightMap[bottom,col];
            sampleCount++;
        }

        int right = col + halfStep;
        if (right < totalSize) {
            sum += heightMap[row,right];
            sampleCount++;
        }

        heightMap[row,col] = Mathf.Clamp(sum / sampleCount + Random.Range(-0.5f, 0.5f), 0, 1);
    }
}