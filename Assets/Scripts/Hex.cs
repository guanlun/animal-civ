using UnityEngine;

public class Hex : MonoBehaviour {
    public static float COLUMN_SPACING = 1.732f * 1.02f;
    public static float ROW_SPACING = 1.5f * 1.02f;

    public int rowIdx;
    public int colIdx;

    public Material defaultStateMaterial;
    public Material selectedStateMaterial;

    void Start()
    {
        this.gameObject.GetComponent<Renderer>().material = defaultStateMaterial;
    }

    public void SetSelected(bool selected)
    {
        this.gameObject.GetComponent<Renderer>().material = selected ? selectedStateMaterial : defaultStateMaterial;
    }

    public Vector3 GetCenterPos()
    {
        bool isOnEvenRow = this.rowIdx % 2 == 0;

        return new Vector3(
            colIdx * COLUMN_SPACING + (isOnEvenRow ? 0f : COLUMN_SPACING / 2),
            0,
            this.rowIdx * ROW_SPACING
        );
    }
}