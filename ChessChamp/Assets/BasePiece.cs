using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public abstract class BasePiece : EventTrigger
{
    [HideInInspector]
    public Color mColor = Color.clear;
    public bool hasMoved = false;
    public bool isKing = false;
    private bool anyChecks = false;

    protected Cell mOriginalCell = null;
    protected Cell mCurrentCell = null;

    public int whiteKingX = 4;
    public int whiteKingY = 0;
    public int blackKingX = 4;
    public int blackKingY = 7;

    public int turnTracker = 0;

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
      if (mCurrentCell.mBoardPosition.x == whiteKingX && mCurrentCell.mBoardPosition.y == whiteKingY) {
        whiteKingX = whiteKingX + mTargetCell.mBoardPosition.x - mCurrentCell.mBoardPosition.x;
        whiteKingY = whiteKingY + mTargetCell.mBoardPosition.y - mCurrentCell.mBoardPosition.y;
      }
      if (mCurrentCell.mBoardPosition.x == blackKingX && mCurrentCell.mBoardPosition.y == blackKingY) {
        blackKingX = blackKingX + mTargetCell.mBoardPosition.x - mCurrentCell.mBoardPosition.x;
        blackKingY = blackKingY + mTargetCell.mBoardPosition.y - mCurrentCell.mBoardPosition.y;
      }
      mTargetCell.RemovePiece();
      mCurrentCell.mCurrentPiece = null;
      mCurrentCell = mTargetCell;
      mCurrentCell.mCurrentPiece = this;
      transform.position = mCurrentCell.transform.position;
      mTargetCell = null;
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

      bool anyChecks = findChecks();
      if(anyChecks == true) {

      }
      else {
        Move();
      }
      ClearCells();
      mPieceManager.SwitchSides(mColor);
      isKing = false;
      anyChecks = false;
      turnTracker = turnTracker + 1;
    }

    public bool findChecks() {
      if (turnTracker % 2 == 0) {
        // white is moving
        foreach(BasePiece wPiece in mPieceManager.mWhitePieces) {
// it does 16 of these
          kingFinder(wPiece, false);
          if (isKing == true) {
            anyChecks = true;
          }
        }
      }
      else {
        // black is moving
        foreach(BasePiece bPiece in mPieceManager.mBlackPieces) {
// it does 16 of these
          kingFinder(bPiece, true);
          if (isKing == true) {
            anyChecks = true;
          }
        }
      }
      return anyChecks;
    }
}
