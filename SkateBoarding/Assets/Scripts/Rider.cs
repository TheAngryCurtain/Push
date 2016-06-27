using UnityEngine;
using System.Collections;

public class Rider : MonoBehaviour
{
    [SerializeField]
    private Board _board;

    [SerializeField]
    private GameObject[] _shoes;

    public void Lean(float h)
    {
        _board.Tilt(h);
    }
}
