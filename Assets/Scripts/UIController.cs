using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] Text health;

    public void UpdateHealth(int value)
    {
        health.text = "Health: " + value;
    }
}
