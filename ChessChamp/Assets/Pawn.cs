using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pawn : BasePiece
{
  public override void Setup(Color newTeamColor, Color32 newSpriteColor, PieceManager newPieceManager) {
    base.Setup(newTeamColor, newSpriteColor, newPieceManager);

    mMovement = mColor == Color.white ? new Vector3Int(0, 1, 1) : new Vector3Int(0, -1, -1);
    GetComponent<Image>().sprite = Resources.Load<Sprite>("T_Pawn");
  }

  protected override void Move() {
    base.Move();
    PromotionCheck();
  }

  private bool MatchesState(int targetX, int targetY, CellState targetState) {
    CellState cellState = CellState.None;
    cellState = mCurrentCell.mBoard.ValidateCell(targetX, targetY, this);

    if (cellState == targetState) {
      mHighlightedCells.Add(mCurrentCell.mBoard.mAllCells[targetX, targetY]);
      return true;
    }
    return false;
  }

  private void PromotionCheck() {
    int currentX = mCurrentCell.mBoardPosition.x;
    int currentY = mCurrentCell.mBoardPosition.y;
    CellState cellState = mCurrentCell.mBoard.ValidateCell(currentX, currentY + mMovement.y, this);

    if(cellState == CellState.OutOfBounds) {
      Color spriteColor = GetComponent<Image>().color;
      mPieceManager.PromotePiece(this, mCurrentCell, mColor, spriteColor);
    }
  }

  protected override void CheckPathing() {
    int currentX = mCurrentCell.mBoardPosition.x;
    int currentY = mCurrentCell.mBoardPosition.y;

    MatchesState(currentX - mMovement.z, currentY + mMovement.z, CellState.Enemy);

    MatchesState(currentX + mMovement.z, currentY + mMovement.z, CellState.Enemy);

    if (MatchesState(currentX, currentY + mMovement.y, CellState.Free)) {
      if (!hasMoved) {
        MatchesState(currentX, currentY + (mMovement.y * 2), CellState.Free);
      }
    }
  }
}
