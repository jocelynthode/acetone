using UnityEngine;
using System.Collections;

public class Letter : MonoBehaviour {

    public Canvas letter;

    void Start () {
        letter.enabled = false;
    }

    public void CloseLetter() {
        letter.enabled = false;
    }
}
