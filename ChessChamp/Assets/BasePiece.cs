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

    public bool bRight = false;
    public bool plsHelp = true;

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
      Place(mOriginalCell);
    }

    public virtual void Kill() {
      mCurrentCell.mCurrentPiece = null;
      gameObject.SetActive(false);
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
      base.OnBeginDrag(eventData);
      CheckPathing();
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
      EnPassant(mCurrentCell, curX, curY);

      if (bRight == true) {
        Debug.Log("attempted");
        mOtherCell = mAllCells[4,3];
        mCurrentCell.mBoardPosition.x += 1;
        mHighlightedCells.Add(mOtherCell.mBoard.mAllCells[mCurrentCell.mBoardPosition.x - 1, mCurrentCell.mBoardPosition.y - 1]);
        bRight = false;
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
