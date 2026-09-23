using UnityEngine;

public class HeavyStoneWeight : MonoBehaviour
{
    [Header("重さ")]
    [Tooltip("この重い石が持つ重さ")]
    [SerializeField] private float 重さ = 3f;

    public float Weight
    {
        get
        {
            return 重さ;
        }
    }
}