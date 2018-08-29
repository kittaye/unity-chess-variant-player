using UnityEngine;
using System.Collections.Generic;

namespace ChessGameModes {
    /// <summary>
    /// Horde.cs is a chess variant that pits 36 white pawns against standard team black.
    /// 
    /// Winstate: Checkmate team black OR Eliminate team white.
    /// Piece types: Orthodox.
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
            AddPieceToBoard(new Pawn(Team.WHITE, new BoardCoord(1, 4), false));
            AddPieceToBoard(new Pawn(Team.WHITE, new BoardCoord(2, 4), false));
            AddPieceToBoard(new Pawn(Team.WHITE, new BoardCoord(5, 4), false));
            AddPieceToBoard(new Pawn(Team.WHITE, new BoardCoord(6, 4), false));
            AddPieceToBoard(new Queen(Team.BLACK, new BoardCoord(3, BLACK_BACKROW)));

            currentRoyalPiece = (King)AddPieceToBoard(new King(Team.BLACK, new BoardCoord(4, BLACK_BACKROW)));
            aSideBlackRook = (Rook)AddPieceToBoard(new Rook(Team.BLACK, new BoardCoord(0, BLACK_BACKROW)));
            hSideBlackRook = (Rook)AddPieceToBoard(new Rook(Team.BLACK, new BoardCoord(7, BLACK_BACKROW)));

            for (int x = 0; x < BOARD_WIDTH; x++) {
                AddPieceToBoard(new Pawn(Team.WHITE, new BoardCoord(x, 0)));
                AddPieceToBoard(new Pawn(Team.WHITE, new BoardCoord(x, 1)));
                AddPieceToBoard(new Pawn(Team.WHITE, new BoardCoord(x, 2), false));
                AddPieceToBoard(new Pawn(Team.WHITE, new BoardCoord(x, 3), false));
                AddPieceToBoard(new Pawn(Team.BLACK, new BoardCoord(x, BLACK_PAWNROW)));

                if (x == 1 || x == BOARD_WIDTH - 2) {
                    AddPieceToBoard(new Knight(Team.BLACK, new BoardCoord(x, BLACK_BACKROW)));
                } else if (x == 2 || x == BOARD_WIDTH - 3) {
                    AddPieceToBoard(new Bishop(Team.BLACK, new BoardCoord(x, BLACK_BACKROW)));
                }
            }
        }

        public override List<BoardCoord> CalculateAvailableMoves(ChessPiece mover) {
            if(GetCurrentTeamTurn() == Team.WHITE) {
                return mover.CalculateTemplateMoves();
            }

            return base.CalculateAvailableMoves(mover);
        }
    }
}