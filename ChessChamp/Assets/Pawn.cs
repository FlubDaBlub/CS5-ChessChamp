using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class Pawn : BasePiece
{
  private bool hasMove = false;

  public override void Setup(Color newTeamColor, Color32 newSpriteColor, PieceManager newPieceManager) {
    base.Setup(newTeamColor, newSpriteColor, newPieceManager);
    hasMove = false;
    mMovement = mColor == Color.white ? new Vector3Int(0, 1, 1) : new Vector3Int(0, -1, -1);
    GetComponent<Image>().sprite = Resources.Load<Sprite>("T_Pawn");
  }

  protected override void Move() {
    base.Move();
    hasMove = true;
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

  public override void EnPassant(Cell mTargetCell, int currentX, int currentY) {

    int targetX = mTargetCell.mBoardPosition.x;
    int targetY = mTargetCell.mBoardPosition.y;

    Sprite sprite = mCurrentCell.mCurrentPiece.GetComponent<Image>().sprite;

    if(Math.Abs(targetY - currentY) == 2) {
      if(mColor == Color.white) {
        for(int i = 0; i < 8; i++) {
          Cell findPawn = mCurrentCell.mBoard.mAllCells[i, 3];
          if(findPawn.mCurrentPiece != null) {
            Sprite isPawnSprite = findPawn.mCurrentPiece.GetComponent<Image>().sprite;
            if(isPawnSprite == sprite && findPawn.mCurrentPiece.mColor == Color.black) {
              if(targetX + 1 == findPawn.mBoardPosition.x) {
                bRight = true;
              }
              if(targetX - 1 == findPawn.mBoardPosition.x) {
                bLeft = true;
              }
            }
          }
        }
      }
      if(mColor == Color.black) {
        for(int i = 0; i < 8; i++) {
          Cell findPawn = mCurrentCell.mBoard.mAllCells[i, 4];
          if(findPawn.mCurrentPiece != null) {
            Sprite isPawnSprite = findPawn.mCurrentPiece.GetComponent<Image>().sprite;
            if(isPawnSprite == sprite && findPawn.mCurrentPiece.mColor == Color.white) {
              if(targetX + 1 == findPawn.mBoardPosition.x) {
                wRight = true;
              }
              if(targetX - 1 == findPawn.mBoardPosition.x) {
                wLeft = true;
              }
            }
          }
        }
      }
    }
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
      if (!hasMove) {
        MatchesState(currentX, currentY + (mMovement.y * 2), CellState.Free);
      }
    }
  }
}
