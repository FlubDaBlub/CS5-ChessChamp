using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public abstract class BasePiece : EventTrigger
{
    [HideInInspector]
    public Color mColor = Color.clear;
    public bool mIsFirstMove = true;

    protected Cell mOriginalCell = null;
    protected Cell mCurrentCell = null;

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

    public void Place(Cell newCell) {
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

    private void CreateCellPath(int xDirection, int yDirection, int movement) {
      int currentX = mCurrentCell.mBoardPosition.x;
      int currentY = mCurrentCell.mBoardPosition.y;

      for (int i = 1; i <= movement; i++) {
        currentX += xDirection;
        currentY += yDirection;
        mHighlightedCells.Add(mCurrentCell.mBoard.mAllCells[currentX, currentY]);
      }
    }

    protected virtual void CheckPathing() {
      int currentX = mCurrentCell.mBoardPosition.x;
      int currentY = mCurrentCell.mBoardPosition.y;

      if(currentX < 7) {
        CreateCellPath(1,0, mMovement.x);
      }

      if(currentX > .6) {
        CreateCellPath(-1,0, mMovement.x);
      }

      if(currentY < 3.5) {
        CreateCellPath(0,1, mMovement.y);
      }

      if(currentY > 0) {
        CreateCellPath(0,-1, mMovement.y);
      }

      if(currentX > .6) {
        if(currentY > 0) {
          CreateCellPath(-1,-1, mMovement.z);
        }
      }

      if(currentX < 7) {
        if(currentY > 0) {
          CreateCellPath(1,-1, mMovement.z);
        }
      }
      if(currentX < 7) {
        if(currentY < 3.5) {
        CreateCellPath(1, 1, mMovement.z);
        }
      }

      if(currentX > .6) {
        if(currentY < 3.5) {
        CreateCellPath(-1, 1, mMovement.z);
        }
      }
    }

    protected void ShowCells() {
      foreach (Cell cell in mHighlightedCells)
        cell.mOutlineImage.enabled = true;
        Debug.Log("TRUE");
    }

    protected void ClearCells() {
      foreach (Cell cell in mHighlightedCells)
        cell.mOutlineImage.enabled = false;
    }

    protected virtual void Move() {
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
      Move();
    }
}
