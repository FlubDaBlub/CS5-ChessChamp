using System;
using System.Collections.Generic;
using UnityEngine;



public class PieceManager : MonoBehaviour
{

    [HideInInspector]
    public bool mIsKingAlive = true;
    public bool rBlack = false;
    public bool lBlack = false;
    public bool rWhite = false;
    public bool lWhite = false;
    public int x = 10;
    public int y = 10;

    public GameObject mPiecePrefab;

    public List<BasePiece> mWhitePieces = null;
    public List<BasePiece> mBlackPieces = null;
    private List<BasePiece> mPromotedPieces = new List<BasePiece>();

    public string[] mPieceOrder = new string[16]
    {
      "P", "P", "P", "P", "P", "P", "P", "P",
      "R", "KN", "B", "Q", "K", "B", "KN", "R"
    };

    public Dictionary<string, Type> mPieceLibrary = new Dictionary<string, Type>() {
      {"P", typeof(Pawn)},
      {"R", typeof(Rook)},
      {"KN", typeof(Knight)},
      {"B", typeof(Bishop)},
      {"K", typeof(King)},
      {"Q", typeof(Queen)}
    };

    public void Setup(Board board) {

      mWhitePieces = CreatePieces(Color.white, new Color32(80, 144, 159, 255), board);
      mBlackPieces = CreatePieces(Color.black, new Color32(10, 35, 64, 255), board);

      PlacePieces(1, 0, mWhitePieces, board);
      PlacePieces(6, 7, mBlackPieces, board);

      SwitchSides(Color.black);
    }

    private List<BasePiece> CreatePieces(Color teamColor, Color32 spriteColor, Board board)
    {
        List<BasePiece> newPieces = new List<BasePiece>();

        for (int i = 0; i < mPieceOrder.Length; i++)
        {
            // Get the type
            string key = mPieceOrder[i];
            Type pieceType = mPieceLibrary[key];

            // Create
            BasePiece newPiece = CreatePiece(pieceType);
            newPieces.Add(newPiece);

            // Setup
            newPiece.Setup(teamColor, spriteColor, this);
        }

        return newPieces;
    }

    private BasePiece CreatePiece(Type pieceType)
    {
        // Create new object
        GameObject newPieceObject = Instantiate(mPiecePrefab);
        newPieceObject.transform.SetParent(transform);

        // Set scale and position
        newPieceObject.transform.localScale = new Vector3(1, 1, 1);
        newPieceObject.transform.localRotation = Quaternion.identity;

        // Store new piece
        BasePiece newPiece = (BasePiece)newPieceObject.AddComponent(pieceType);

        return newPiece;
    }

    private void PlacePieces(int pawnRow, int royaltyRow, List<BasePiece> pieces, Board board) {
      for (int i = 0; i < 8; i++) {
        pieces[i].Place(board.mAllCells[i, pawnRow]);
        pieces[i + 8].Place(board.mAllCells[i, royaltyRow]);
      }
    }

    private void SetInteractive(List<BasePiece> allPieces, bool value) {
      foreach(BasePiece piece in allPieces) {
        piece.enabled = value;
      }
    }

    private void blackAIMove() {
      BasePiece pieceToMove = null;
        while (!pieceToMove)
        {
            // Get piece
            int i = UnityEngine.Random.Range(0, mBlackPieces.Count);
            BasePiece newPiece = mBlackPieces[i];
            // Does this piece have any moves?
            if (!newPiece.HasMove())
                continue;
            // Is piece active?
            if (newPiece.gameObject.activeInHierarchy)
                pieceToMove = newPiece;
        }
        pieceToMove.aiMove();
    }

    public void SwitchSides(Color color) {
      //Resets an ended game
      if(!mIsKingAlive) {
        ResetPieces();
        mIsKingAlive = true;
        color = Color.black;
      }

      bool isBlackTurn = color == Color.white ? true : false;

      SetInteractive(mWhitePieces, !isBlackTurn);
      SetInteractive(mBlackPieces, false);
      // normally isBlackTurn instead of false

      foreach(BasePiece piece in mPromotedPieces) {
        bool isBlack = piece.mColor != Color.white ? true : false;
        bool isOnTeam = isBlack == true ? isBlackTurn : !isBlackTurn;
        piece.enabled = isOnTeam;
      }
      if(isBlackTurn) {
        blackAIMove();
      }
    }

    public void ResetPieces() {

          foreach (BasePiece piece in mWhitePieces) {
              piece.Kill();
          }

          foreach (BasePiece piece in mBlackPieces) {
              piece.Kill();
          }

          foreach (BasePiece piece in mPromotedPieces) {
              piece.Kill();
              Destroy(piece.gameObject);
          }

          mPromotedPieces.Clear();

          foreach (BasePiece piece in mWhitePieces) {
              piece.Reset();
          }

          foreach (BasePiece piece in mBlackPieces) {
              piece.Reset();
          }
      }

      public void PromotePiece(Pawn pawn, Cell cell, Color teamColor, Color spriteColor) {
        pawn.Kill();
        BasePiece promotedPiece = CreatePiece(typeof(Queen));
        promotedPiece.Setup(teamColor, spriteColor, this);
        promotedPiece.Place(cell);
        mPromotedPieces.Add(promotedPiece);
      }

//en passant stuff
      public void currentX(int curX) {
        x = curX;
      }
      public void currentY(int curY) {
        y = curY;
      }
      public int getX() {
        return x;
      }
      public int getY() {
        return y;
      }

      public void bRightTrue() {
        rBlack = true;
      }
      public void bRightFalse() {
        rBlack = false;
      }
      public bool getbRight() {
        return rBlack;
      }

      public void bLeftTrue() {
        lBlack = true;
      }
      public void bLeftFalse() {
        lBlack = false;
      }
      public bool getbLeft() {
        return lBlack;
      }

      public void wRightTrue() {
        rWhite = true;
      }
      public void wRightFalse() {
        rWhite = false;
      }
      public bool getwRight() {
        return rWhite;
      }

      public void wLeftTrue() {
        lWhite = true;
      }
      public void wLeftFalse() {
        lWhite = false;
      }
      public bool getwLeft() {
        return lWhite;
      }
}
