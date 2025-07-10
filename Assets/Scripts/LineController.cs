using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LineController : MonoBehaviour
{
    public bool isActive = false;
    public Button lineButton;
    public Color playerColor;
    public Color AIColor;
    public DotsAndBoxesController DAB;

    private void Start()
    {
        DAB = FindFirstObjectByType<DotsAndBoxesController>();
        lineButton = GetComponent<Button>();
        if (lineButton == null)
        {
            Debug.LogError("LineController: Button component not found on this GameObject!");
        }
        lineButton.onClick.AddListener(OnLineClicked);
    }
    private void Update()
    {
        if (DAB != null)
        {
            // Устанавливаем interactable в false, если не ход игрока, линия уже активна или игра закончена.
            lineButton.interactable = DAB.isPlayerTurn && !isActive && !DAB.gameEnded;
        }
    }
    public void Active()
    {
        if (!isActive)
        {
            isActive = true;
            Color colorToApply = DAB.isPlayerTurn ? DAB.PlayerColor : DAB.AIColor;
            ChangeAllColors(colorToApply);
            if (DAB != null)
            {
                if (DAB.isPlayerTurn == true)
                    Debug.Log("Игрок сходил");
                else
                    Debug.Log("ИИ сходил");

                DAB.CheckAndFillAllBoxes();
            }
            else
                Debug.Log("Нет ссылки на DAB");
        }
    }

    public void ChangeAllColors(Color color)
    {
        ColorBlock colors = lineButton.colors;

        colors.normalColor = color;
        colors.highlightedColor = color;
        colors.pressedColor = color;
        colors.selectedColor = color;
        colors.disabledColor = color;

        lineButton.colors = colors;
    }
    private void OnLineClicked()
    {
        if (DAB != null && DAB.isPlayerTurn && !isActive && !DAB.gameEnded)
        {
            Active();
        }
        else
        {
            Debug.Log("Сейчас не ваш ход, линия уже активна, или игра закончена.");
        }
    }
}