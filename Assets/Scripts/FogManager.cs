using UnityEngine;

public class FogManager : MonoBehaviour
{
    [Header("霧設定")]
    [SerializeField]
    private Color fogColor = new Color(0.8f, 0.9f, 1f);

    [SerializeField]
    private float fogStart = 15f;

    [SerializeField]
    private float fogEnd = 60f;

    void Start()
    {
        // 霧on
        RenderSettings.fog = true;

        // 霧色
        RenderSettings.fogColor = fogColor;

        // 霧タイプ
        RenderSettings.fogMode = FogMode.Linear;

        // 開始距離
        RenderSettings.fogStartDistance = fogStart;

        // 終了距離
        RenderSettings.fogEndDistance = fogEnd;
    }
}
