using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public Hex currentHex;

    public Faction unitFaction;

    public int remainingMoves = 1;

    private GameObject bodyGameObject;
    private GameObject attackTargetIndicatorGameObject;

    public Material standByMaterial;
    public Material outOfMoveMaterial;

    void Awake()
    {
        this.bodyGameObject = this.transform.Find("CatBody").gameObject;
        this.attackTargetIndicatorGameObject = this.transform.Find("AttackTargetIndicator").gameObject;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void MoveToHex(Hex hex)
    {
        this.transform.rotation = Quaternion.LookRotation(hex.GetCenterPos() - this.gameObject.transform.position, Vector3.up);
        this.currentHex.unitOnHex = null;
        this.SetCurrentHex(hex);
        this.remainingMoves--;

        if (this.remainingMoves == 0) {
            this.bodyGameObject.GetComponent<SkinnedMeshRenderer>().material = outOfMoveMaterial;
        }
    }
    public void AttackTarget(Unit targetUnit)
    {
        // TODO
        Destroy(targetUnit.gameObject);
    }

    public void SetFaction(Faction faction)
    {
        this.unitFaction = faction;
    }

    public void SetCurrentHex(Hex hex)
    {
        this.gameObject.transform.position = hex.GetCenterPos();

        this.gameObject.SetActive(hex.IsExploredByPlayer());

        hex.unitOnHex = this;
        this.currentHex = hex;

        hex.SetExplored(this.unitFaction);

        foreach (Hex adjacentHex in HexManager.GetAdjacentHexes(hex)) {
            adjacentHex.SetExplored(this.unitFaction);
        }
    }

    public void ResetRemainingMoves()
    {
        this.remainingMoves = 1;
        this.bodyGameObject.GetComponent<SkinnedMeshRenderer>().material = standByMaterial;
    }

    public void SetAttackTargetIndicatorActive(bool active)
    {
        this.attackTargetIndicatorGameObject.SetActive(active);
    }
}
