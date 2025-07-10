using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BoxController : MonoBehaviour
{
    public LineController upLine;
    public LineController downLine;
    public LineController leftLine;
    public LineController rightLine;
    public Color playerColor;
    public Color AIColor;
    public DotsAndBoxesController DAB;
    public Image boxImage;

    public bool isCaptured = false;
    public bool CapturedByPlayer { get; set; }

    void Start()
    {
        DAB = FindFirstObjectByType<DotsAndBoxesController>();
        boxImage = GetComponent<Image>();
        if (boxImage == null)
        {
            Debug.LogError("BoxController: Image component not found on this GameObject!");
        }
        //boxImage.color = Color.clear;
    }

    public bool IsBoxCompleted()
    {
        return upLine != null && upLine.isActive && downLine != null && downLine.isActive && leftLine != null && leftLine.isActive && rightLine != null && rightLine.isActive;
    }
}