using UnityEngine;
using System.Collections;

public class Board : MonoBehaviour
{
    [SerializeField]
    private GameObject[] _trucks;

    [SerializeField]
    private GameObject _fullDeck;

    [SerializeField]
    private GameObject[] _deckPieces;

    [SerializeField]
    private GameObject[] _wheelSets;

    public float turnAngle;

    private Quaternion localRotation;
    private float currentTilt = 0f;

    public void Tilt(float h)
    {
        //localRotation = _fullDeck.transform.localRotation;
        //localRotation.z = Mathf.Clamp(currentTilt + (1 * h), -turnAngle, turnAngle);
        //Debug.Log(currentTilt);

        //_fullDeck.transform.rotation = localRotation;
    }
}
