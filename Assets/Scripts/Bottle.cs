using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Rigidbody))]
public class Bottle : MonoBehaviour
{

    [Header("回収時")]
    public int element;



    private BoxCollider bc;
    private Rigidbody rb;

    void Start()
    {
        bc = GetComponent<BoxCollider>();
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    void Update()
    {

    }
}
