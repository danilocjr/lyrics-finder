using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollBehaviour : MonoBehaviour
{
    Vector2 startPos;
    public Text lyrics;

    private void Start()
    {
        //startPos = transform.localPosition;
        //lyrics.
    }

    private void Update()
    {
        if (transform.localPosition.y < 0f)
            transform.localPosition = startPos;
    }

   
}
