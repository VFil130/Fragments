using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Timeline.TimelinePlaybackControls;
using UnityEngine.UIElements;

public class DotsAndBoxesController : MonoBehaviour
{
    public bool gameEnded = false;
    public bool isPlayerTurn;
    public LineController[] Lines;
    public BoxController[] BoxesMass;
    public GameObject[] DotsMass;
    public GameObject Colums;
    public GameObject Rows;
    public GameObject Boxes;
    public GameObject Dots;
    [SerializeField] private int GameSize = 5;
    public AIController aiController;

    public LineController LinePrefab;
    public BoxController BoxPrefab;
    public GameObject DotPrefab;

    public Color PlayerColor = Color.blue;
    public Color AIColor = Color.red;

    public TMP_Text playertxt;
    public TMP_Text AItxt;
    public TMP_Text WinText;
    public GameObject WinPanel;

    public int playerScore;
    public int AIScore;

    void Start()
    {
        InitializeUI();
    }

    void Update()
    {
    }

    public void ChangeTurn()
    {
        isPlayerTurn = !isPlayerTurn;
        Debug.Log("Turn changed. Player turn: " + isPlayerTurn);
    }

    void InitializeUI()
    {
        int arraySize = (GameSize * (GameSize - 1)) * 2;
        int arraySizeBox = (GameSize - 1) * (GameSize - 1);
        int arraySizeDot = GameSize * GameSize;

        Lines = new LineController[arraySize];
        BoxesMass = new BoxController[arraySizeBox];
        DotsMass = new GameObject[arraySizeDot];

        for (int i = 0; i < arraySize; i++)
        {
            Lines[i] = Instantiate(LinePrefab);
            Transform parent = (i < arraySize / 2) ? Rows.transform : Colums.transform;
            Lines[i].transform.SetParent(parent, false);
            if (i < arraySize / 2)
            {
                Lines[i].transform.Rotate(0, 0, 90);
            }
            Lines[i].DAB = this;
            Lines[i].name = "Line_" + i;
        }

        for (int i = 0; i < arraySizeBox; i++)
        {
            BoxesMass[i] = Instantiate(BoxPrefab);
            BoxesMass[i].transform.SetParent(Boxes.transform, false);
            BoxesMass[i].DAB = this;
            BoxesMass[i].name = "Box_" + i;
        }

        for (int i = 0; i < arraySizeDot; i++)
        {
            DotsMass[i] = Instantiate(DotPrefab);
            DotsMass[i].transform.SetParent(Dots.transform, false);
            DotsMass[i].name = "Dot_" + i;
        }
        AssignLinesToBoxes();
        Debug.Log("Массив Lines инициализирован с размером: " + arraySize);
    }

    void AssignLinesToBoxes()
    {
        int boxIndex = 0;
        int arraySize = (GameSize * (GameSize - 1)) * 2;
        int halfSize = arraySize / 2; // Количество линий в каждом наборе (вертикальном и горизонтальном)

        for (int row = 0; row < GameSize - 1; row++)
        {
            for (int col = 0; col < GameSize - 1; col++)
            {
                // Индексы линий для текущего бокса (ПРАВИЛЬНЫЕ)
                int topLineIndex = col + row * (GameSize - 1);
                int bottomLineIndex = col + (row + 1) * (GameSize - 1);
                int leftLineIndex = halfSize + col + row * GameSize;
                int rightLineIndex = halfSize + (col + 1) + row * GameSize;

                // Получаем контроллеры линий
                LineController leftLine = Lines[leftLineIndex];
                LineController rightLine = Lines[rightLineIndex];
                LineController topLine = Lines[topLineIndex];
                LineController bottomLine = Lines[bottomLineIndex];

                BoxesMass[boxIndex].upLine = topLine;
                BoxesMass[boxIndex].downLine = bottomLine;
                BoxesMass[boxIndex].leftLine = leftLine;
                BoxesMass[boxIndex].rightLine = rightLine;

                Debug.Log($"Box {boxIndex}: upLine={topLineIndex}, downLine={bottomLineIndex}, leftLine={leftLineIndex}, rightLine={rightLineIndex}"); //Для отладки

                boxIndex++;
            }
        }
    }
    public void CheckAndFillAllBoxes()
    {
        bool boxCaptured = false; // Флаг, показывающий, был ли захвачен хотя бы один бокс
        foreach (BoxController box in BoxesMass)
        {
            if (box.IsBoxCompleted() && !box.isCaptured)
            {
                boxCaptured = true;
                box.isCaptured = true;
                box.CapturedByPlayer = isPlayerTurn;
                box.boxImage.color = isPlayerTurn ? PlayerColor : AIColor;
                Debug.Log("Квадрат захвачен! Игрок: " + (box.CapturedByPlayer ? "Игрок" : "ИИ"));
                if (isPlayerTurn) { playerScore++; playertxt.text = playerScore.ToString(); }
                if (!isPlayerTurn) { AIScore++; AItxt.text = AIScore.ToString(); }
            }
        }

        CheckForWinCondition();

        // Меняем ход только если не было захвачено ни одного квадрата
        if (!boxCaptured)
        {
            ChangeTurn();
        }

        // Запускаем ход ИИ, если нужно
        if (!isPlayerTurn)
        {
            StartCoroutine(MakeAIMoveDelayed()); // Используем корутину для небольшой задержки
        }
    }
    void CheckForWinCondition()
    {
        // Считаем количество захваченных квадратов
        int capturedBoxes = 0;
        foreach (BoxController box in BoxesMass)
        {
            if (box.isCaptured)
            {
                capturedBoxes++;
            }
        }

        // Если все квадраты захвачены
        if (capturedBoxes == BoxesMass.Length && !gameEnded)
        {
            gameEnded = true; // Устанавливаем флаг, чтобы предотвратить повторный запуск
            StartCoroutine(EndGameRoutine());
        }
    }

    IEnumerator EndGameRoutine()
    {
        // Подождите немного, чтобы игрок успел увидеть последний ход
        yield return new WaitForSeconds(1.5f);

        // Определите победителя
        string resultText;
        if (playerScore > AIScore)
        {
            resultText = "Победа!";
        }
        else if (AIScore > playerScore)
        {
            resultText = "Поражение!";
        }
        else
        {
            resultText = "Ничья!";
        }

        // Отобразите панель с результатом
        WinText.text = resultText;
        WinPanel.SetActive(true);

        Debug.Log("Игра окончена! " + resultText + " Счет: Игрок = " + playerScore + ", ИИ = " + AIScore);
        yield return new WaitForSeconds(2);

        Debug.Log("Удаление GameLogic");
        Destroy(gameObject);
    }
    private IEnumerator MakeAIMoveDelayed()
    {
        yield return null; // Ждем один кадр, чтобы дать UI обновиться
        if (aiController != null)
        {
            aiController.TakeTurn();
        }
        else
        {
            Debug.LogError("AIController not found in the scene!");
        }
    }
}