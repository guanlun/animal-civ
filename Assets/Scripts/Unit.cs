﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public Hex currentHex;

    public Faction unitFaction;

    public int remainingMoves = 1;

    private GameObject bodyGameObject;

    private Unit attackTargetUnit;
    private GameObject attackTargetIndicatorGameObject;

    public Material standByMaterial;
    public Material outOfMoveMaterial;

    public Animator animator;

    private bool isMoving = false;
    private Vector3 movingToPosition;
    private Vector3 movingFromPosition;

    private List<Action> possibleActions = new List<Action>();

    private float moveDuration = 0.15f;
    private float moveStartTime;

    void Awake()
    {
        this.bodyGameObject = this.transform.Find("CatBody").gameObject;
        this.attackTargetIndicatorGameObject = this.transform.Find("AttackTargetIndicator").gameObject;

        this.animator = this.GetComponent<Animator>();
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

    public bool isPlayerUnit()
    {
        return this.unitFaction.isPlayerFaction;
    }

    public void MoveToHex(Hex hex)
    {
        this.transform.rotation = Quaternion.LookRotation(hex.GetCenterPos() - this.gameObject.transform.position, Vector3.up);
        this.currentHex.unitOnHex = null;
        this.currentHex.SetSelected(false);
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
        this.attackTargetUnit = targetUnit;

        Vector3 myPosition = this.transform.position;
        Vector3 targetPosition = targetUnit.transform.position;
        this.transform.rotation = Quaternion.LookRotation(targetPosition - myPosition, Vector3.up);
        targetUnit.transform.rotation = Quaternion.LookRotation(myPosition - targetPosition, Vector3.up);

        this.animator.SetBool("isAttacking", true);
        targetUnit.animator.SetBool("isAttacking", true);
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

    // ---------- animation handlers
    public void MeleeAttackHit()
    {
        // Only the attacker has attackTargetUnit, defender should not handle the logic inside the conditional
        if (this.attackTargetUnit) {
            // TODO: health check
            this.attackTargetUnit.gameObject.SetActive(false);
        }
    }

    public void MeleeAttackAnimationEnd()
    {
        // Only the attacker has attackTargetUnit, defender should not handle the logic inside the conditional
        if (this.attackTargetUnit) {
            // TODO: health check
            this.MoveToHex(this.attackTargetUnit.currentHex);

            Destroy(this.attackTargetUnit.gameObject);
            this.attackTargetUnit = null;
        }

        this.animator.SetBool("isAttacking", false);
    }

    public void ComputePossibleActions()
    {
        this.possibleActions.Clear();
        if (this.remainingMoves > 0) {
            foreach (Hex reachableHex in HexManager.GetHexesByMovementDistance(this.currentHex, 2)) {
                this.possibleActions.Add(new MoveAction(reachableHex));

                Unit unitOnHex = reachableHex.unitOnHex;
                if (unitOnHex && unitOnHex.unitFaction != this.unitFaction) { // enemy unit
                    this.possibleActions.Add(new AttackAction(unitOnHex));
                }
            }
        }
    }

    public void ToggleActionStatesDisplay(bool toggleOn)
    {
        foreach (Action action in this.possibleActions) {
            action.ToggleTargetState(toggleOn);
        }
    }
}
