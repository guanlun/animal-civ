using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text woodResourceDisplayText;

    public GameObject gameManager;
    public MainGameManager mainGameManager;

    // Top-level UI containers for building/producing
    public GameObject buildingMenuUI;
    public GameObject productionMenuUI;

    public GameObject buildingSelectionUI;

    void Awake()
    {
        this.mainGameManager = this.gameManager.GetComponent<MainGameManager>();
        this.SetBuildingMenuUIActive(false);
        this.SetProductionMenuUIActive(false);
    }

    public void SetResourcesValue(FactionResources resources)
    {
        woodResourceDisplayText.text = resources.wood.ToString();
    }

    public void SetBuildingMenuUIActive(bool active)
    {
        this.buildingMenuUI.SetActive(active);
    }

    public void SetProductionMenuUIActive(bool active)
    {
        this.productionMenuUI.SetActive(active);
    }

    public void Build(GameObject buildingPrefab)
    {
        this.mainGameManager.Build(buildingPrefab);
        this.buildingSelectionUI.SetActive(false);
    }

    public void Produce(GameObject unitPrefab)
    {
        this.mainGameManager.CreateUnit(unitPrefab);
    }

    public void ToggleBuildMenuVisibility()
    {
        this.buildingSelectionUI.SetActive(!buildingSelectionUI.activeInHierarchy);
    }
}
