using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text woodResourceDisplayText;

    public void SetWoodValue(int value)
    {
        woodResourceDisplayText.text = value.ToString();
    }
}
