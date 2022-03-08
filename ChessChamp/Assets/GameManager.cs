using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Board mBoard;
    // Start is called before the first frame update
    void Start()
    {
        mBoard.Create();
    }
}
