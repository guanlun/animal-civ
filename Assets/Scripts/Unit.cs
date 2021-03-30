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

    private bool isMoving = false;
    private Vector3 movingToPosition;
    private Vector3 movingFromPosition;

    private float moveDuration = 0.15f;
    private float moveStartTime;

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
        if (this.isMoving) {
            float moveInterpolation = (Time.time - this.moveStartTime) / this.moveDuration;
            if (moveInterpolation >= 1) {
                this.transform.position = this.movingToPosition;

                this.isMoving = false;
            } else {
                this.transform.position = Vector3.Lerp(this.movingFromPosition, this.movingToPosition, moveInterpolation);
            }
        }
    }

    public void MoveToHex(Hex hex)
    {
        this.transform.rotation = Quaternion.LookRotation(hex.GetCenterPos() - this.gameObject.transform.position, Vector3.up);
        this.currentHex.unitOnHex = null;
        this.SetCurrentHex(hex, true);

        // Start the move animation
        this.AnimateMoveTo(hex.transform.position);

        this.remainingMoves--;

        if (this.remainingMoves == 0) {
            this.bodyGameObject.GetComponent<SkinnedMeshRenderer>().material = outOfMoveMaterial;
        }
    }

    public void AnimateMoveTo(Vector3 targetPosition)
    {
        this.isMoving = true;
        this.movingToPosition = targetPosition;
        this.movingFromPosition = this.transform.position;
        this.moveStartTime = Time.time;
    }
    public void AttackTarget(Unit targetUnit)
    {
        Vector3 myPosition = this.transform.position;
        Vector3 targetPosition = targetUnit.transform.position;
        this.transform.rotation = Quaternion.LookRotation(targetPosition - myPosition, Vector3.up);
        targetUnit.transform.rotation = Quaternion.LookRotation(myPosition - targetPosition, Vector3.up);

        this.AnimateMoveTo(myPosition * 0.8f + targetPosition * 0.2f);
        targetUnit.AnimateMoveTo(myPosition * 0.2f + targetPosition * 0.8f);
        // TODO
        // Destroy(targetUnit.gameObject);
    }

    public void SetFaction(Faction faction)
    {
        this.unitFaction = faction;
    }

    public void SetCurrentHex(Hex hex, bool animate = false)
    {
        if (!animate) {
            this.transform.position = hex.GetCenterPos();
        }

        // When setting current hex during unit initialization, only show units that are located on explored hexes
        // (units in my faction and very close-by enemy units)
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
