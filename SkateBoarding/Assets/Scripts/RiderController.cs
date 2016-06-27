using UnityEngine;
using System.Collections;

public class RiderController : MonoBehaviour
{
    [SerializeField]
    private Rider _rider;

    void Start()
    {
        if (_rider == null)
        {
            Debug.LogError("No Rider assigned in inspector!");
        }
    }

    void Update()
    {
        GetAxisInput();
        GetButtonInput();
    }

    private void GetAxisInput()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        _rider.Lean(h);
    }

    private void GetButtonInput()
    {
        if (Input.GetButtonDown("Push"))
        {

        }
    }
}
