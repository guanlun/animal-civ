
using UnityEngine;
using System.Collections.Generic;

public class Buidling : MonoBehaviour {
    public Faction buildingFaction;

    public void SetFaction(Faction faction)
    {
        this.buildingFaction = faction;
    }
}