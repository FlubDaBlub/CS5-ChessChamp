using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Board mBoard;

    public PieceManager mPieceManager;
    // Start is called before the first frame update
    void Start()
    {
        mBoard.Create();

        mPieceManager.Setup(mBoard);
    }
}
