using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public Hex currentHex;

    public Faction unitFaction;

    public int remainingMoves = 1;

    private bool isMoving = false;

    private GameObject bodyGameObject;

    private Unit attackTargetUnit;

    private GameObject selectionIndicatorGameObject;
    private GameObject attackTargetIndicatorGameObject;

    public Material standByMaterial;
    public Material outOfMoveMaterial;

    public Animator animator;

    private List<Action> possibleActions = new List<Action>();

    void Awake()
    {
        this.bodyGameObject = this.transform.Find("CatBody").gameObject;
        this.selectionIndicatorGameObject = this.transform.Find("SelectionIndicator").gameObject;
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
    }

    public void SetSelected(bool selected)
    {
        this.selectionIndicatorGameObject.SetActive(selected);
    }

    public bool isPlayerUnit()
    {
        return this.unitFaction.isPlayerFaction;
    }

    public void MoveToHex(Hex hex)
    {
        NavNode currNavNode = HexManager.GetNavNodeByHex(hex);
        List<Hex> path = new List<Hex>();
        path.Add(hex);
        while (currNavNode.fromHex != this.currentHex) {
            path.Insert(0, currNavNode.fromHex);
            currNavNode = HexManager.GetNavNodeByHex(currNavNode.fromHex);
        }
        path.Insert(0, this.currentHex);

        StartCoroutine(this.AnimateMoveAlongPath(path));

        this.currentHex.unitOnHex = null;
        this.SetCurrentHex(hex, true);

        this.remainingMoves--;

        if (this.remainingMoves == 0) {
            // this.bodyGameObject.GetComponent<SkinnedMeshRenderer>().material = outOfMoveMaterial;
        }
    }

    private IEnumerator AnimateMoveAlongPath(List<Hex> path)
    {
        for (int i = 0; i < path.Count - 1; i++) {
            Vector3 fromPosition = path[i].GetCenterPos();
            Vector3 toPosition = path[i + 1].GetCenterPos();

            // Start the move animation
            yield return StartCoroutine(this.AnimateMoveTo(fromPosition, toPosition));
        }
    }

    private IEnumerator AnimateMoveTo(Vector3 fromPosition, Vector3 toPosition)
    {
        this.isMoving = true;
        this.transform.rotation = Quaternion.LookRotation(toPosition - fromPosition, Vector3.up);

        float movedRatio = 0f;
        Vector3 moveVector = toPosition - fromPosition;
        while (movedRatio < 1f) {
            float delta = Time.deltaTime * 5;
            movedRatio += delta;
            this.transform.position += moveVector * delta;
            yield return null;
        }

        this.isMoving = false;
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

        foreach (Hex adjacentHex in HexManager.GetHexesByVisibleDistance(hex)) {
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
            foreach (KeyValuePair<Hex, NavNode> reachableHexEntry in HexManager.GetHexesByMovementDistance(this.currentHex, 2)) {
                Hex reachableHex = reachableHexEntry.Key;

                this.possibleActions.Add(new MoveAction(this, reachableHex, reachableHexEntry.Value));

                Unit unitOnHex = reachableHex.unitOnHex;
                if (unitOnHex && unitOnHex.unitFaction != this.unitFaction) { // enemy unit
                    this.possibleActions.Add(new AttackAction(this, unitOnHex));
                }
            }
        }
    }

    public void TakeBestAction()
    {
        float maxScore = 0f;
        Action argMaxAction = null;
        foreach (Action action in this.possibleActions) {
            float actionScore = action.GetResultScore();
            if (actionScore > maxScore) {
                maxScore = actionScore;
                argMaxAction = action;
            }
        }

        argMaxAction.Execute();
    }

    public void ToggleActionStatesDisplay(bool toggleOn)
    {
        foreach (Action action in this.possibleActions) {
            action.ToggleTargetState(toggleOn);
        }
    }
}
