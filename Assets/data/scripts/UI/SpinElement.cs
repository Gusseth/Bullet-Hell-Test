using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinElement : MonoBehaviour
{
    public float rotateRate;
    public bool rotateCounterClockwise;

    // Start is called before the first frame update
    void Start()
    {
        if (rotateCounterClockwise)
        {
            rotateRate *= -1;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.Rotate(0, 0, rotateRate);
    }
}
