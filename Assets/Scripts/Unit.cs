using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public Hex currentHex;

    public int remainingMoves = 1;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void MoveToHex(Hex hex) {
        this.transform.rotation = Quaternion.LookRotation(hex.GetCenterPos() - this.gameObject.transform.position, Vector3.up);
        this.currentHex.unitOnHex = null;
        this.SetCurrentHex(hex);
    }

    public void SetCurrentHex(Hex hex)
    {
        this.gameObject.transform.position = hex.GetCenterPos();
        hex.unitOnHex = this;
        this.currentHex = hex;
    }

    public void ResetRemainingMoves() {
        this.remainingMoves = 1;
    }
}
