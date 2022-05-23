using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class King : BasePiece
{
  private Rook mLeftRook = null;
  private Rook mRightRook = null;

  public override void Setup(Color newTeamColor, Color32 newSpriteColor, PieceManager newPieceManager) {
    base.Setup(newTeamColor, newSpriteColor, newPieceManager);
    mMovement = new Vector3Int(1, 1, 1);
    GetComponent<Image>().sprite = Resources.Load<Sprite>("T_King");
  }

  public override void Kill() {
    base.Kill();
    mPieceManager.mIsKingAlive = false;
  }

  public override void OnBeginDrag(PointerEventData eventData) {
    int currentX = getX();
    int currentY = getY();
    mLeftRook = GetRook(-1, 4, currentX, currentY);
    Debug.Log("L: " + mLeftRook);
    mRightRook = GetRook(1, 3, currentX, currentY);
    Debug.Log("R: " + mRightRook);
    base.OnBeginDrag(eventData);
  }

  protected override void Move() {

    bool c = false;

    if(CanCastle(mLeftRook) == true || CanCastle(mRightRook)== true) {
      mMovement = new Vector3Int(2, 1, 1);
      c = true;
    }

    else {
      mMovement = new Vector3Int(1, 1, 1);
    }

    base.Move();

    if(c == true) {
      int newX = getX();
      if(newX < 5) {
        mLeftRook.Castle();
      }
      else if(newX > 5) {
        mRightRook.Castle();
      }
    }
  }

  private bool CanCastle(Rook rook) {
    if(rook == null) {
      return false;
    }

    if(rook.mCastleTrigger == mCurrentCell) {
      return false;
    }

    if(rook.mColor != mColor || rook.hasMoved) {
      return false;
    }

    return true;
  }

  private int getX() {
    int currentX = mCurrentCell.mBoardPosition.x;
    return currentX;
  }

  private int getY() {
    int currentY = mCurrentCell.mBoardPosition.y;
    return currentY;
  }

  private Rook GetRook(int direction, int count, int currentX, int currentY) {

    if(hasMoved == true) {
      return null;
    }

    for(int i = 1; i < count; i++) {
      int offsetX = currentX + (i * direction);
      CellState cellState = mCurrentCell.mBoard.ValidateCell(offsetX, currentY, this);
      if(cellState != CellState.Free) {
        return null;
      }
    }

    Cell rookCell = mCurrentCell.mBoard.mAllCells[currentX + (count * direction), currentY];
    Rook rook = null;

    if(rookCell.mCurrentPiece is Rook) {
      rook = (Rook)rookCell.mCurrentPiece;
    }

    if(rook != null) {
      mHighlightedCells.Add(rook.mCastleTrigger);
    }

    return rook;
  }
}
