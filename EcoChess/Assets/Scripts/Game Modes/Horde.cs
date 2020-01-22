using UnityEngine;
using System.Collections.Generic;

namespace ChessGameModes {
    /// <summary>
    /// Board layout: 
    ///     r n b q k b n r
    ///     p p p p p p p p
    ///     . . . . . . . .
    ///     . p p . . p p .
    ///     p p p p p p p p
    ///     p p p p p p p p
    ///     p p p p p p p p
    ///     p p p p p p p p
    /// </summary>
    public class Horde : Chess {
        public Horde() : base() { }

        public override string ToString() {
            return "Horde";
        }

        public override VariantHelpDetails GetVariantHelpDetails() {
            return new VariantHelpDetails(
                this.ToString(),
                "Invented by Lord Dunsany (1942)",
                this.ToString() + " is a variant that pits 36 white pawns against standard team black.",
                "Checkmate team black OR capture all of team white.",
                "- White pawns on the 7th & 8th ranks may double-move. It does not need to be the initial move.\n" +
                "- Note: The white pawns on the 8th rank may choose to move 1 square, then 2.",
                "https://en.wikipedia.org/wiki/Dunsany%27s_chess"
            );
        }

        public override bool CheckWinState() {
            if (GetCurrentTeamTurn() == Team.WHITE) {
                if (GetPieces(Team.WHITE).TrueForAll((x) => (x.IsAlive == false))) {
                    UIManager.Instance.LogCustom("Team Black wins by elimination!");
                    return true;
                }
            } else {
                return base.CheckWinState();
            }

            if (CapturelessMovesLimit()) {
                return true;
            }

            return false;
        }

        public override void PopulateBoard() {
            AddPieceToBoard(new Pawn(Team.WHITE, new BoardCoord(1, 4), initialMoveLimit: 1));
            AddPieceToBoard(new Pawn(Team.WHITE, new BoardCoord(2, 4), initialMoveLimit: 1));
            AddPieceToBoard(new Pawn(Team.WHITE, new BoardCoord(5, 4), initialMoveLimit: 1));
            AddPieceToBoard(new Pawn(Team.WHITE, new BoardCoord(6, 4), initialMoveLimit: 1));
            AddPieceToBoard(new Queen(Team.BLACK, new BoardCoord(3, BLACK_BACKROW)));

            currentRoyalPiece = (King)AddPieceToBoard(new King(Team.BLACK, new BoardCoord(4, BLACK_BACKROW)));
            AddPieceToBoard(new Rook(Team.BLACK, new BoardCoord(0, BLACK_BACKROW)));
            AddPieceToBoard(new Rook(Team.BLACK, new BoardCoord(7, BLACK_BACKROW)));

            for (int x = 0; x < BOARD_WIDTH; x++) {
                AddPieceToBoard(new Pawn(Team.WHITE, new BoardCoord(x, 0)));
                AddPieceToBoard(new Pawn(Team.WHITE, new BoardCoord(x, 1)));
                AddPieceToBoard(new Pawn(Team.WHITE, new BoardCoord(x, 2), initialMoveLimit: 1));
                AddPieceToBoard(new Pawn(Team.WHITE, new BoardCoord(x, 3), initialMoveLimit: 1));
                AddPieceToBoard(new Pawn(Team.BLACK, new BoardCoord(x, BLACK_PAWNROW)));

                if (x == 1 || x == BOARD_WIDTH - 2) {
                    AddPieceToBoard(new Knight(Team.BLACK, new BoardCoord(x, BLACK_BACKROW)));
                } else if (x == 2 || x == BOARD_WIDTH - 3) {
                    AddPieceToBoard(new Bishop(Team.BLACK, new BoardCoord(x, BLACK_BACKROW)));
                }
            }
        }

        public override List<BoardCoord> CalculateAvailableMoves(ChessPiece mover) {
            if (GetCurrentTeamTurn() == Team.WHITE) {
                BoardCoord[] templateMoves = mover.CalculateTemplateMoves().ToArray();
                List<BoardCoord> availableMoves = new List<BoardCoord>(templateMoves.Length);

                availableMoves.AddRange(templateMoves);

                // Special rule for when a white pawn from the 8th rank moves to the 7th -- they are still able to double-move.
                if (mover is Pawn && (mover.GetBoardPosition().y == 1) && mover.MoveCount == 1) {
                    if (availableMoves.Contains(mover.GetRelativeBoardCoord(0, 1))) {
                        availableMoves.Add(TryGetSpecificMove(mover, mover.GetRelativeBoardCoord(0, 2)));
                    }
                }

                // Otherwise, just add the rest of the pawn's template moves as there is no royalty to check for.
                return availableMoves;
            }

            return base.CalculateAvailableMoves(mover);
        }
    }
}