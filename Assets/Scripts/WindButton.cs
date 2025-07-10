using UnityEngine;
using UnityEngine.UI;

public class WindButton : MonoBehaviour
{
    public string Direction; // Направление ветра (N, S, W, E, NW, NE, SW, SE)
    private DandelionGame game;

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnWindButtonClicked);
        game = FindObjectOfType<DandelionGame>();
    }

    void OnWindButtonClicked()
    {
        Blow();
    }
    void Blow()
    {
        game.WindBlows(Direction);
    }
}