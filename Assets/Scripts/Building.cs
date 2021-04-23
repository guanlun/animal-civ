
using UnityEngine;
using System.Collections.Generic;

public abstract class Buidling : MonoBehaviour
{
    public Faction buildingFaction;

    public GameObject selectedIndicator;

    public bool isSelected;

    // A list of UI buttons for the units this building can produce
    public List<GameObject> unitProductionButtons = new List<GameObject>();

    protected Hex currentHex;

    void Awake()
    {
        GameObject selectedIndicatorPrefab = Resources.Load<GameObject>("Prefabs/building-selection-indicator");
        // TODO: change the 0.05f + up to rendering queue
        this.selectedIndicator = Instantiate(selectedIndicatorPrefab, this.transform.position + 0.05f * Vector3.up, selectedIndicatorPrefab.transform.rotation);
        this.selectedIndicator.layer = 9; // Map UI layer
        this.selectedIndicator.transform.parent = this.transform;

        // Building selected indicator starts unselected
        this.selectedIndicator.SetActive(false);
    }

    public abstract void ComputeResources(FactionResources resources);

    public void SetHex(Hex hex)
    {
        this.currentHex = hex;
        hex.buildingOnHex = this;
    }

    public Hex GetHex()
    {
        return this.currentHex;
    }

    public void SetFaction(Faction faction)
    {
        this.buildingFaction = faction;
    }

    public void SetSelected(bool selected)
    {
        this.isSelected = selected;
        this.selectedIndicator.SetActive(selected);
    }
}