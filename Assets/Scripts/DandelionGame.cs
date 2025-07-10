using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DandelionGame : MonoBehaviour
{
    [SerializeField] private int fieldSize = 5;
    [SerializeField] private GameObject cellPrefab;
    [SerializeField] private Transform gridParent;
    [SerializeField] private Transform visualGridParent;
    [SerializeField] private Sprite emptySprite;
    [SerializeField] private Sprite dandelionSprite;
    [SerializeField] private Sprite seedSprite;
    [SerializeField] private Sprite visualCellSprite;
    [SerializeField] private List<WindButton> windButtons;
    [SerializeField] private TMP_Text gameStatusText;

    private Cell[][] field;
    private Image[][] visualField;
    private bool isPlayerDandelion = true;
    private bool isDandelionTurn = true;
    private List<string> usedWindDirections = new List<string>();
    private bool gameEnded = false;

    void Start()
    {
        InitializeField();
        InitializeVisualField();
        AssignRoles();

        if ((!isPlayerDandelion && isDandelionTurn) || (isPlayerDandelion && !isDandelionTurn))
        {
            StartCoroutine(HandleAITurn());
        }
    }

    private void InitializeField()
    {
        field = new Cell[fieldSize][];
        for (int i = 0; i < fieldSize; i++)
        {
            field[i] = new Cell[fieldSize];
            for (int j = 0; j < fieldSize; j++)
            {
                GameObject cellObject = Instantiate(cellPrefab, gridParent);
                Button button = cellObject.GetComponent<Button>();
                Image cellImage = cellObject.GetComponent<Image>();
                field[i][j] = new Cell(button, cellImage);
                int row = i;
                int col = j;
                button.onClick.AddListener(() => OnCellClicked(row, col));
                cellObject.name = $"Cell_{i}_{j}";
                UpdateCellVisual(i, j);
                button.interactable = true;
            }
        }
    }

    private void InitializeVisualField()
    {
        visualField = new Image[fieldSize][];
        for (int i = 0; i < fieldSize; i++)
        {
            visualField[i] = new Image[fieldSize];
            for (int j = 0; j < fieldSize; j++)
            {
                GameObject cellObject = new GameObject($"VisualCell_{i}_{j}", typeof(Image));
                cellObject.transform.SetParent(visualGridParent, false);

                Image cellImage = cellObject.GetComponent<Image>();
                cellImage.sprite = visualCellSprite;
                visualField[i][j] = cellImage;
            }
        }
    }

    private void AssignRoles()
    {
        isPlayerDandelion = Random.Range(0, 2) == 0;
        isDandelionTurn = true;

        Debug.Log("Player is Dandelion: " + isPlayerDandelion);
    }


    private void OnCellClicked(int row, int col)
    {
        if (gameEnded) return;

        if (isDandelionTurn && isPlayerDandelion)
        {
            if (field[row][col].State == CellState.Empty)
            {
                field[row][col].State = CellState.Dandelion;
                UpdateCellVisual(row, col);
                isDandelionTurn = false;

                StartCoroutine(HandleAITurn());
            }
        }
    }

    private void UpdateCellVisual(int row, int col)
    {
        Cell cell = field[row][col];
        Sprite sprite = null;

        switch (cell.State)
        {
            case CellState.Empty:
                sprite = emptySprite;
                break;
            case CellState.Dandelion:
                sprite = dandelionSprite;
                break;
            case CellState.Seed:
                sprite = seedSprite;
                break;
        }

        cell.Image.sprite = sprite;
    }

    public void WindBlows(string direction)
    {
        if (gameEnded) return;

        if (!isDandelionTurn && !isPlayerDandelion)
        {
            if (usedWindDirections.Contains(direction)) return;

            usedWindDirections.Add(direction);
            SpreadSeeds(direction);
            isDandelionTurn = true;
            CheckGameStatus();

            StartCoroutine(HandleAITurn());
        }
    }

    private void SpreadSeeds(string direction)
    {
        for (int startRow = 0; startRow < fieldSize; startRow++)
        {
            for (int startCol = 0; startCol < fieldSize; startCol++)
            {
                if (field[startRow][startCol].State == CellState.Dandelion)
                {
                    int currentRow = startRow;
                    int currentCol = startCol;

                    switch (direction)
                    {
                        case "N":
                            while (currentRow > 0)
                            {
                                currentRow--;
                                if (field[currentRow][currentCol].State == CellState.Empty)
                                {
                                    field[currentRow][currentCol].State = CellState.Seed;
                                    UpdateCellVisual(currentRow, currentCol);
                                }
                            }
                            break;
                        case "S":
                            while (currentRow < fieldSize - 1)
                            {
                                currentRow++;
                                if (field[currentRow][currentCol].State == CellState.Empty)
                                {
                                    field[currentRow][currentCol].State = CellState.Seed;
                                    UpdateCellVisual(currentRow, currentCol);
                                }
                            }
                            break;
                        case "W":
                            while (currentCol > 0)
                            {
                                currentCol--;
                                if (field[currentRow][currentCol].State == CellState.Empty)
                                {
                                    field[currentRow][currentCol].State = CellState.Seed;
                                    UpdateCellVisual(currentRow, currentCol);
                                }
                            }
                            break;
                        case "E":
                            while (currentCol < fieldSize - 1)
                            {
                                currentCol++;
                                if (field[currentRow][currentCol].State == CellState.Empty)
                                {
                                    field[currentRow][currentCol].State = CellState.Seed;
                                    UpdateCellVisual(currentRow, currentCol);
                                }
                            }
                            break;
                        case "NW":
                            while (currentRow > 0 && currentCol > 0)
                            {
                                currentRow--;
                                currentCol--;
                                if (field[currentRow][currentCol].State == CellState.Empty)
                                {
                                    field[currentRow][currentCol].State = CellState.Seed;
                                    UpdateCellVisual(currentRow, currentCol);
                                }
                            }
                            break;
                        case "NE":
                            while (currentRow > 0 && currentCol < fieldSize - 1)
                            {
                                currentRow--;
                                currentCol++;
                                if (field[currentRow][currentCol].State == CellState.Empty)
                                {
                                    field[currentRow][currentCol].State = CellState.Seed;
                                    UpdateCellVisual(currentRow, currentCol);
                                }
                            }
                            break;
                        case "SW":
                            while (currentRow < fieldSize - 1 && currentCol > 0)
                            {
                                currentRow++;
                                currentCol--;
                                if (field[currentRow][currentCol].State == CellState.Empty)
                                {
                                    field[currentRow][currentCol].State = CellState.Seed;
                                    UpdateCellVisual(currentRow, currentCol);
                                }
                            }
                            break;
                        case "SE":
                            while (currentRow < fieldSize - 1 && currentCol < fieldSize - 1)
                            {
                                currentRow++;
                                currentCol++;
                                if (field[currentRow][currentCol].State == CellState.Empty)
                                {
                                    field[currentRow][currentCol].State = CellState.Seed;
                                    UpdateCellVisual(currentRow, currentCol);
                                }
                            }
                            break;
                    }
                }
            }
        }
    }

    private void CheckGameStatus()
    {
        bool allCellsFilled = true;
        foreach (var row in field)
        {
            foreach (var cell in row)
            {
                if (cell.State == CellState.Empty)
                {
                    allCellsFilled = false;
                    break;
                }
            }
            if (!allCellsFilled) break;
        }

        if (allCellsFilled)
        {
            Debug.Log("Одуванчик победил!");
            gameEnded = true;
            gameStatusText.text = "Одуванчик победил!";
        }
        else if (usedWindDirections.Count >= 7)
        {
            Debug.Log("Ветер победил!");
            gameEnded = true;
            gameStatusText.text = "Ветер победил!";
        }

        if (gameEnded)
        {
            DisableAllButtons();
        }
    }

    private void DisableAllButtons()
    {
        foreach (var row in field)
        {
            foreach (var cell in row)
            {
                cell.Button.interactable = false;
            }
        }

        foreach (WindButton windButton in windButtons)
        {
            windButton.GetComponent<Button>().interactable = false;
        }
    }

    private IEnumerator HandleAITurn()
    {
        yield return new WaitForSeconds(1f);

        if (gameEnded) yield break;

        if (isDandelionTurn && !isPlayerDandelion)
        {
            yield return StartCoroutine(AITurnDandelion());
            isDandelionTurn = false;
        }
        else if (!isDandelionTurn && isPlayerDandelion)
        {
            yield return StartCoroutine(AITurnWind());
            isDandelionTurn = true;
        }
    }

    private IEnumerator AITurnDandelion()
    {
        List<Cell> emptyCells = new List<Cell>();
        foreach (var row in field)
        {
            foreach (var cell in row)
            {
                if (cell.State == CellState.Empty)
                {
                    emptyCells.Add(cell);
                }
            }
        }

        if (emptyCells.Count > 0)
        {
            int randomIndex = Random.Range(0, emptyCells.Count);
            Cell selectedCell = emptyCells[randomIndex];

            int row = -1, col = -1;
            for (int i = 0; i < fieldSize; i++)
            {
                for (int j = 0; j < fieldSize; j++)
                {
                    if (field[i][j] == selectedCell)
                    {
                        row = i;
                        col = j;
                        break;
                    }
                }
                if (row != -1) break;
            }

            selectedCell.State = CellState.Dandelion;
            UpdateCellVisual(row, col);

        }
        else
        {
            CheckGameStatus();
        }
        yield return null;
    }

    private IEnumerator AITurnWind()
    {
        if (gameEnded) yield break;

        string bestDirection = FindBestWindDirection();

        if (bestDirection != null)
        {
            if (!usedWindDirections.Contains(bestDirection))
            {
                usedWindDirections.Add(bestDirection);
                SpreadSeeds(bestDirection);
                CheckGameStatus();
            }
            else
            {
                Debug.LogWarning("AI tried to use wind direction that was already used!");
                string alternativeDirection = FindAlternativeWindDirection();
                if (alternativeDirection != null)
                {
                    if (!usedWindDirections.Contains(alternativeDirection))
                    {
                        usedWindDirections.Add(alternativeDirection);
                        SpreadSeeds(alternativeDirection);
                        CheckGameStatus();
                    }
                }
                else
                {
                    CheckGameStatus();
                    yield break;
                }
            }

        }
        else
        {
            CheckGameStatus();
        }
        yield return null;
    }

    private string FindBestWindDirection()
    {
        List<string> possibleDirections = new List<string> { "N", "S", "W", "E", "NW", "NE", "SW", "SE" };
        possibleDirections.RemoveAll(direction => usedWindDirections.Contains(direction));

        if (possibleDirections.Count == 0) return null;

        string bestDirection = null;
        int minSeedsSpread = int.MaxValue;

        foreach (string direction in possibleDirections)
        {
            int seedsSpread = CalculateSeedsSpread(direction);

            if (seedsSpread < minSeedsSpread)
            {
                minSeedsSpread = seedsSpread;
                bestDirection = direction;
            }
        }

        return bestDirection;
    }

    private string FindAlternativeWindDirection()
    {
        List<string> possibleDirections = new List<string> { "N", "S", "W", "E", "NW", "NE", "SW", "SE" };
        possibleDirections.RemoveAll(direction => usedWindDirections.Contains(direction));

        if (possibleDirections.Count == 0) return null;

        return possibleDirections[Random.Range(0, possibleDirections.Count)];
    }

    private int CalculateSeedsSpread(string direction)
    {
        int seedsSpread = 0;

        for (int startRow = 0; startRow < fieldSize; startRow++)
        {
            for (int startCol = 0; startCol < fieldSize; startCol++)
            {
                if (field[startRow][startCol].State == CellState.Dandelion)
                {
                    int currentRow = startRow;
                    int currentCol = startCol;

                    switch (direction)
                    {
                        case "N":
                            while (currentRow > 0)
                            {
                                currentRow--;
                                if (field[currentRow][currentCol].State == CellState.Empty)
                                {
                                    seedsSpread++;
                                }
                                else
                                {
                                    break;
                                }
                            }
                            break;
                        case "S":
                            while (currentRow < fieldSize - 1)
                            {
                                currentRow++;
                                if (field[currentRow][currentCol].State == CellState.Empty)
                                {
                                    seedsSpread++;
                                }
                                else
                                {
                                    break;
                                }
                            }
                            break;
                        case "W":
                            while (currentCol > 0)
                            {
                                currentCol--;
                                if (field[currentRow][currentCol].State == CellState.Empty)
                                {
                                    seedsSpread++;
                                }
                                else
                                {
                                    break;
                                }
                            }
                            break;
                        case "E":
                            while (currentCol < fieldSize - 1)
                            {
                                currentCol++;
                                if (field[currentRow][currentCol].State == CellState.Empty)
                                {
                                    seedsSpread++;
                                }
                                else
                                {
                                    break;
                                }
                            }
                            break;
                        case "NW":
                            while (currentRow > 0 && currentCol > 0)
                            {
                                currentRow--;
                                currentCol--;
                                if (field[currentRow][currentCol].State == CellState.Empty)
                                {
                                    seedsSpread++;
                                }
                                else
                                {
                                    break;
                                }
                            }
                            break;
                        case "NE":
                            while (currentRow > 0 && currentCol < fieldSize - 1)
                            {
                                currentRow--;
                                currentCol++;
                                if (field[currentRow][currentCol].State == CellState.Empty)
                                {
                                    seedsSpread++;
                                }
                                else
                                {
                                    break;
                                }
                            }
                            break;
                        case "SW":
                            while (currentRow < fieldSize - 1 && currentCol > 0)
                            {
                                currentRow++;
                                currentCol--;
                                if (field[currentRow][currentCol].State == CellState.Empty)
                                {
                                    seedsSpread++;
                                }
                                else
                                {
                                    break;
                                }
                            }
                            break;
                        case "SE":
                            while (currentRow < fieldSize - 1 && currentCol < fieldSize - 1)
                            {
                                currentRow++;
                                currentCol++;
                                if (field[currentRow][currentCol].State == CellState.Empty)
                                {
                                    seedsSpread++;
                                }
                                else
                                {
                                    break;
                                }
                            }
                            break;
                    }
                }
            }
        }
        return seedsSpread;
    }

    private string DetermineWinner()
    {
        if (gameEnded)
        {
            if (usedWindDirections.Count >= 7)
            {
                return "Ветер победил!";
            }
            else
            {
                return "Одуванчик победил!";
            }
        }
        return "Game is not over yet!";
    }
}