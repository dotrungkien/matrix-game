using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    void Start()
    {
        GetComponent<SpriteRenderer>().sortingOrder = transform.parent.parent.GetComponent<SpriteRenderer>().sortingOrder + 1;
    }
}
