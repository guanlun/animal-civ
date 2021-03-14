using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGameManager : MonoBehaviour
{
    public GameObject hexGridPrefab;

    // Start is called before the first frame update
    void Start()
    {
        for (int rowIdx = 0; rowIdx < 5; rowIdx++) {
            for (int colIdx = 0; colIdx < 5; colIdx++) {
                GameObject grid = Instantiate(hexGridPrefab, new Vector3(colIdx * 1.732f + rowIdx % 2 * 0.866f, 0, rowIdx * 1.5f), Quaternion.identity);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
