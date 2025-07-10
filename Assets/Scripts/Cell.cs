using UnityEngine;
using UnityEngine.UI;

public class Cell
{
    public CellState State { get; set; }
    public Button Button { get; set; }
    public Image Image { get; set; }

    public Cell(Button button, Image image)
    {
        Button = button;
        Image = image;
        State = CellState.Empty;
    }
}

public enum CellState
{
    Empty,
    Dandelion,
    Seed
}