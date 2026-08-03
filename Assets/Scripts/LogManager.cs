using TMPro;
using UnityEngine;

public class LogManager : MonoBehaviour
{
    public static LogManager Instance;

    [SerializeField]
    private TextMeshProUGUI logText;

    private void Awake()
    {
        Instance = this;
    }

    public void AddLog(string message)
    {
        logText.text += message + "\n";
    }
}