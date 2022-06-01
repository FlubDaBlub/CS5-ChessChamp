using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public abstract class BasePiece : EventTrigger
{
    [HideInInspector]
    public Color mColor = Color.clear;
    public bool isKing = false;
    public bool lastMove = false;
    public bool hasMoved = false;

    public bool bRight = false;
    public bool bLeft = false;
    public bool wLeft = false;
    public bool wRight = false;

    protected Cell mOriginalCell = null;
    protected Cell mCurrentCell = null;
    protected Cell mOtherCell = null;

    public int whiteKingX = 4;
    public int whiteKingY = 0;
    public int blackKingX = 4;
    public int blackKingY = 7;

    public int turnTracker = 0;
    public int countdown = -1;

    protected RectTransform mRectTransform = null;
    protected PieceManager mPieceManager;
    protected Board newBoard;

    protected Cell mTargetCell = null;

    protected Vector3Int mMovement = Vector3Int.one;
    protected List<Cell> mHighlightedCells = new List<Cell>();

    public virtual void Setup(Color newTeamColor, Color32 newSpriteColor, PieceManager newPieceManager) {
      mPieceManager = newPieceManager;

      mColor = newTeamColor;
      GetComponent<Image>().color = newSpriteColor;
      mRectTransform = GetComponent<RectTransform>();
    }

    public virtual void Place(Cell newCell) {
      mCurrentCell = newCell;
      mOriginalCell = newCell;
      mCurrentCell.mCurrentPiece = this;

      transform.position = newCell.transform.position;
      gameObject.SetActive(true);
    }

    public void Reset() {
      Kill();
      hasMoved = false;
      Place(mOriginalCell);
    }

    public virtual void Kill() {
      mCurrentCell.mCurrentPiece = null;
      gameObject.SetActive(false);
      hasMoved = false;
    }

    public bool HasMove() {
       CheckPathing();
       if (mHighlightedCells.Count == 0)
           return false;
       return true;
   }

    private void CreateCellPath(int xDirection, int yDirection, int movement) {
      int currentX = mCurrentCell.mBoardPosition.x;
      int currentY = mCurrentCell.mBoardPosition.y;

      for (int i = 1; i <= movement; i++) {
        currentX += xDirection;
        currentY += yDirection;

        CellState cellState = CellState.None;
        cellState = mCurrentCell.mBoard.ValidateCell(currentX, currentY, this);

        if(cellState == CellState.Enemy) {
          mHighlightedCells.Add(mCurrentCell.mBoard.mAllCells[currentX, currentY]);
          break;
        }

        if(cellState != CellState.Free) {
          break;
        }

        mHighlightedCells.Add(mCurrentCell.mBoard.mAllCells[currentX, currentY]);
      }
    }

    protected virtual void CheckPathing() {
      int currentX = mCurrentCell.mBoardPosition.x;
      int currentY = mCurrentCell.mBoardPosition.y;

        CreateCellPath(1,0, mMovement.x);

        CreateCellPath(-1,0, mMovement.x);

        CreateCellPath(0,1, mMovement.y);

        CreateCellPath(0,-1, mMovement.y);

        CreateCellPath(-1,-1, mMovement.z);

        CreateCellPath(1,-1, mMovement.z);

        CreateCellPath(1, 1, mMovement.z);

        CreateCellPath(-1, 1, mMovement.z);
    }

    protected virtual void kingFinder(BasePiece piece, bool black) {

      CreateCellPath(1,0, mMovement.x);

      CreateCellPath(-1,0, mMovement.x);

      CreateCellPath(0,1, mMovement.y);

      CreateCellPath(0,-1, mMovement.y);

      CreateCellPath(-1,-1, mMovement.z);

      CreateCellPath(1,-1, mMovement.z);

      CreateCellPath(1, 1, mMovement.z);

      CreateCellPath(-1, 1, mMovement.z);

      foreach (Cell cell in mHighlightedCells) {
        int cellX = cell.mBoardPosition.x;
        int cellY = cell.mBoardPosition.y;
//        Debug.Log(black);
        if (black == true) {
          if (cellX == whiteKingX && cellY == whiteKingY) {
            isKing = true;
          }
        }
        else {
          if (cellX == blackKingX && cellY == blackKingY) {
            isKing = true;
          }
        }
      }
    }

    protected void ShowCells() {
      foreach (Cell cell in mHighlightedCells)
        cell.mOutlineImage.enabled = true;
    }

    protected void ClearCells() {
      foreach (Cell cell in mHighlightedCells)
        cell.mOutlineImage.enabled = false;
      mHighlightedCells.Clear();
    }

    protected virtual void Move() {
//      if (mCurrentCell.mBoardPosition.x == whiteKingX && mCurrentCell.mBoardPosition.y == whiteKingY) {
//        whiteKingX = whiteKingX + mTargetCell.mBoardPosition.x - mCurrentCell.mBoardPosition.x;
//        whiteKingY = whiteKingY + mTargetCell.mBoardPosition.y - mCurrentCell.mBoardPosition.y;
//      }
//      if (mCurrentCell.mBoardPosition.x == blackKingX && mCurrentCell.mBoardPosition.y == blackKingY) {
//        blackKingX = blackKingX + mTargetCell.mBoardPosition.x - mCurrentCell.mBoardPosition.x;
//        blackKingY = blackKingY + mTargetCell.mBoardPosition.y - mCurrentCell.mBoardPosition.y;
//      }
      mTargetCell.RemovePiece();
      mCurrentCell.mCurrentPiece = null;
      mCurrentCell = mTargetCell;
      mCurrentCell.mCurrentPiece = this;
      transform.position = mCurrentCell.transform.position;
      mTargetCell = null;
      hasMoved = true;
    }

    public virtual void EnPassant(Cell cell, int curX, int curY) {

    }

    public void addBR(int num) {

    }

    public override void OnBeginDrag(PointerEventData eventData) {
      int currentX = mCurrentCell.mBoardPosition.x;
      int currentY = mCurrentCell.mBoardPosition.y;
      base.OnBeginDrag(eventData);
      CheckPathing();

      //en passant stuff
      int pawnX = mPieceManager.getX();
      int pawnY = mPieceManager.getY();
      if(mPieceManager.getbRight() == true && currentY == pawnY && currentX - 1 == pawnX) {
        mHighlightedCells.Add(mCurrentCell.mBoard.mAllCells[currentX - 1, currentY - 1]);
      }
      if(mPieceManager.getbLeft() == true && currentY == pawnY && currentX + 1 == pawnX) {
        mHighlightedCells.Add(mCurrentCell.mBoard.mAllCells[currentX + 1, currentY - 1]);
      }
      if(mPieceManager.getwRight() == true && currentY == pawnY && currentX - 1 == pawnX) {
        mHighlightedCells.Add(mCurrentCell.mBoard.mAllCells[currentX - 1, currentY + 1]);
      }
      if(mPieceManager.getwLeft() == true && currentY == pawnY && currentX + 1 == pawnX) {
        mHighlightedCells.Add(mCurrentCell.mBoard.mAllCells[currentX + 1, currentY + 1]);
      }
      ShowCells();
    }

    public override void OnDrag(PointerEventData eventData) {
      base.OnDrag(eventData);
      transform.position += (Vector3)eventData.delta;
      foreach (Cell cell in mHighlightedCells) {
        if (RectTransformUtility.RectangleContainsScreenPoint(cell.mRectTransform, Input.mousePosition)) {
          mTargetCell = cell;
          break;
        }
        mTargetCell = null;
      }
    }

    public override void OnEndDrag(PointerEventData eventData) {
      base.OnEndDrag(eventData);
      ClearCells();
      if (!mTargetCell) {
        transform.position = mCurrentCell.gameObject.transform.position;
        return;
      }

      int curX = mCurrentCell.mBoardPosition.x;
      int curY = mCurrentCell.mBoardPosition.y;
      Move();
      // the next line is old code
      mPieceManager.SwitchSides(mColor);

      //en passant stuff
      int currentX = mCurrentCell.mBoardPosition.x;
      int currentY = mCurrentCell.mBoardPosition.y;

      if(mPieceManager.getbRight() == true && currentY == curY - 1 && currentX == curX - 1) {
        mOtherCell = mCurrentCell.mBoard.mAllCells[currentX, currentY + 1];
        mOtherCell.RemovePiece();
      }
      if(mPieceManager.getbLeft() == true && currentY == curY - 1 && currentX == curX + 1) {
        mOtherCell = mCurrentCell.mBoard.mAllCells[currentX, currentY + 1];
        mOtherCell.RemovePiece();
      }
      if(mPieceManager.getwRight() == true && currentY == curY + 1 && currentX == curX - 1) {
        mOtherCell = mCurrentCell.mBoard.mAllCells[currentX, currentY - 1];
        mOtherCell.RemovePiece();
      }
      if(mPieceManager.getwLeft() == true && currentY == curY + 1 && currentX == curX + 1) {
        mOtherCell = mCurrentCell.mBoard.mAllCells[currentX, currentY - 1];
        mOtherCell.RemovePiece();
      }

      mPieceManager.bRightFalse();
      mPieceManager.bLeftFalse();
      mPieceManager.wRightFalse();
      mPieceManager.wLeftFalse();

      EnPassant(mCurrentCell, curX, curY);
      mPieceManager.currentX(currentX);
      mPieceManager.currentY(currentY);

      if (bRight == true) {
        mPieceManager.bRightTrue();
        bRight = false;
      }
      if (bLeft == true) {
        mPieceManager.bLeftTrue();
        bLeft = false;
      }
      if (wRight == true) {
        mPieceManager.wRightTrue();
        wRight = false;
      }
      if (wLeft == true) {
        mPieceManager.wLeftTrue();
        wLeft = false;
      }
    }


    public bool findChecks() {
      bool checksFound = false;
//      if (turnTracker % 2 == 0) {
        // looking for white's checks on the black king
        for (int i = 0; i < mPieceManager.mWhitePieces.Count; i++) {
// it does 16 of these
          kingFinder(mPieceManager.mWhitePieces[i], false);
          if (isKing == true) {
            Debug.Log("anychecks1");
            checksFound = true;
          }
//        }
      }
//      if (turnTracker % 2 == 1) {
        // looking for black's checks on the white king
        for (int i = 0; i < mPieceManager.mBlackPieces.Count; i++) {
// it does 16 of these
          kingFinder(mPieceManager.mBlackPieces[i], true);
          if (isKing == true) {
            Debug.Log("anychecks2");
            checksFound = true;
          }
//        }
      }
      return checksFound;
    }
}
