using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Knight : BasePiece
{
  public override void Setup(Color newTeamColor, Color32 newSpriteColor, PieceManager newPieceManager) {
    base.Setup(newTeamColor, newSpriteColor, newPieceManager);

    GetComponent<Image>().sprite = Resources.Load<Sprite>("T_Knight");
  }

  protected override void CheckPathing() {
    CreateCellPath(1);
    CreateCellPath(-1);
  }

  private void MatchesState(int targetX, int targetY) {
    CellState cellState = CellState.None;
    cellState = mCurrentCell.mBoard.ValidateCell(targetX, targetY, this);

    int currentX = mCurrentCell.mBoardPosition.x;
    int currentY = mCurrentCell.mBoardPosition.y;

    if (cellState != CellState.Friendly && cellState != CellState.OutOfBounds)
      mHighlightedCells.Add(mCurrentCell.mBoard.mAllCells[targetX, targetY]);
  }

  private void CreateCellPath(int moveUp) {
    int currentX = mCurrentCell.mBoardPosition.x;
    int currentY = mCurrentCell.mBoardPosition.y;

    MatchesState(currentX - 1, currentY + (2 * moveUp));
    MatchesState(currentX + 1, currentY + (2 * moveUp));
    MatchesState(currentX - 2, currentY + (1 * moveUp));
    MatchesState(currentX + 2, currentY + (1 * moveUp));
  }
}
