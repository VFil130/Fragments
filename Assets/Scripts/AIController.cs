using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    public DotsAndBoxesController DAB;
    [Range(0.1f, 2.0f)]
    public float aiMoveDelay = 0.5f;

    void Start()
    {
    }

    // Вызывается из DotsAndBoxesController, когда наступает ход ИИ
    public void TakeTurn()
    {
        if (!DAB.isPlayerTurn && !DAB.gameEnded)
        {
            StartCoroutine(MakeAIMoveDelayed());
        }
    }

    private IEnumerator MakeAIMoveDelayed()
    {
        yield return new WaitForSeconds(aiMoveDelay); // Задержка перед ходом ИИ
        MakeAIMove();
    }

    public void MakeAIMove()
    {
        if (DAB == null)
        {
            Debug.LogError("AIController: DotsAndBoxesController not found!");
            return;
        }

        // 1. Поиск линии для завершения бокса
        LineController lineToCompleteBox = FindLineToCompleteBox();
        if (lineToCompleteBox != null)
        {
            Debug.Log("AI: Completing a box!");
            lineToCompleteBox.Active();
            return;
        }

        // 2. Поиск случайной линии в боксе, где не хватает только одной линии
        LineController randomLine = FindRandomLineInBoxWithOneMissing();
        if (randomLine != null)
        {
            Debug.Log("AI: Taking a safe move!");
            randomLine.Active();
            return;
        }

        // 3. Если все боксы имеют две или больше закрашенных линии, выбираем случайную линию на поле
        LineController desperateMove = FindRandomLineAnywhere();
        if (desperateMove != null)
        {
            Debug.Log("AI: Making a desperate move!");
            desperateMove.Active();
            return;
        }

        Debug.Log("AI: No moves available!");
    }

    // Находит линию, которая завершит бокс (если такая есть)
    LineController FindLineToCompleteBox()
    {
        foreach (BoxController box in DAB.BoxesMass)
        {
            if (!box.isCaptured)
            {
                int activeLines = 0;
                if (box.upLine.isActive) activeLines++;
                if (box.downLine.isActive) activeLines++;
                if (box.leftLine.isActive) activeLines++;
                if (box.rightLine.isActive) activeLines++;

                if (activeLines == 3)
                {
                    if (!box.upLine.isActive) return box.upLine;
                    if (!box.downLine.isActive) return box.downLine;
                    if (!box.leftLine.isActive) return box.leftLine;
                    if (!box.rightLine.isActive) return box.rightLine;
                }
            }
        }
        return null;
    }

    // Находит случайную незакрашенную линию в случайном боксе, у которого меньше двух линий закрашено
    LineController FindRandomLineInBoxWithOneMissing()
    {
        List<BoxController> eligibleBoxes = new List<BoxController>();

        foreach (BoxController box in DAB.BoxesMass)
        {
            if (!box.isCaptured)
            {
                int activeLines = 0;
                if (box.upLine.isActive) activeLines++;
                if (box.downLine.isActive) activeLines++;
                if (box.leftLine.isActive) activeLines++;
                if (box.rightLine.isActive) activeLines++;

                if (activeLines < 2)
                {
                    eligibleBoxes.Add(box);
                }
            }
        }

        if (eligibleBoxes.Count > 0)
        {
            BoxController selectedBox = eligibleBoxes[Random.Range(0, eligibleBoxes.Count)];
            List<LineController> inactiveLines = new List<LineController>();

            if (!selectedBox.upLine.isActive) inactiveLines.Add(selectedBox.upLine);
            if (!selectedBox.downLine.isActive) inactiveLines.Add(selectedBox.downLine);
            if (!selectedBox.leftLine.isActive) inactiveLines.Add(selectedBox.leftLine);
            if (!selectedBox.rightLine.isActive) inactiveLines.Add(selectedBox.rightLine);

            if (inactiveLines.Count > 0)
            {
                return inactiveLines[Random.Range(0, inactiveLines.Count)];
            }
        }
        return null;
    }

    // Находит случайную незакрашенную линию на всем поле
    LineController FindRandomLineAnywhere()
    {
        List<LineController> inactiveLines = new List<LineController>();

        foreach (LineController line in DAB.Lines)
        {
            if (!line.isActive)
            {
                inactiveLines.Add(line);
            }
        }

        if (inactiveLines.Count > 0)
        {
            return inactiveLines[Random.Range(0, inactiveLines.Count)];
        }
        return null;
    }
}
