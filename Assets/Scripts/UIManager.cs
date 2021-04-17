using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text woodResourceDisplayText;

    public GameObject gameManager;
    public MainGameManager mainGameManager;

    public GameObject buildingMenu;

    void Awake()
    {
        this.mainGameManager = this.gameManager.GetComponent<MainGameManager>();
    }

    public void SetWoodValue(int value)
    {
        woodResourceDisplayText.text = value.ToString();
    }

    public void Build(GameObject buildingPrefab)
    {
        this.mainGameManager.Build(buildingPrefab);
        this.buildingMenu.SetActive(false);
    }

    public void ToggleBuildMenuVisibility()
    {
        this.buildingMenu.SetActive(!buildingMenu.activeInHierarchy);
    }
}
