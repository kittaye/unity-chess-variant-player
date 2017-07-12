using System.Collections;
using System.Collections.Generic;

namespace ChessGameModes {
    /// <summary>
    /// Silverman4x5.cs is a chess variant with a smaller board layout.
    /// 
    /// Winstate: Checkmate.
    /// Piece types: Orthodox.
    /// Piece rules: No castling, no pawn double moves.
    /// Board layout:
    ///     k n b r
    ///     p . . .
    ///     . . . .
    ///     . . . p
    ///     R B N K
    /// </summary>
    public class Microchess : FIDERuleset {
        private new const int BOARD_WIDTH = 4;
        private new const int BOARD_HEIGHT = 5;

        public Microchess() : base(BOARD_WIDTH, BOARD_HEIGHT) { }

        public override string ToString() {
            return "Microchess";
        }

        public override void PopulateBoard() {
            opposingRoyalPiece = (King)AddPieceToBoard(new King(Team.BLACK, new BoardCoord(0, BLACK_BACKROW)));
            currentRoyalPiece = (King)AddPieceToBoard(new King(Team.WHITE, new BoardCoord(3, WHITE_BACKROW)));

            AddPieceToBoard(new Pawn(Team.BLACK, new BoardCoord(0, BLACK_PAWNROW), initialMoveLimit: 1));
            aSideWhiteRook = (Rook)AddPieceToBoard(new Rook(Team.WHITE, new BoardCoord(0, WHITE_BACKROW)));

            AddPieceToBoard(new Bishop(Team.WHITE, new BoardCoord(1, WHITE_BACKROW)));
            AddPieceToBoard(new Knight(Team.BLACK, new BoardCoord(1, BLACK_BACKROW)));

            AddPieceToBoard(new Knight(Team.WHITE, new BoardCoord(2, WHITE_BACKROW)));
            AddPieceToBoard(new Bishop(Team.BLACK, new BoardCoord(2, BLACK_BACKROW)));

            AddPieceToBoard(new Pawn(Team.WHITE, new BoardCoord(3, WHITE_PAWNROW), initialMoveLimit: 1));
            hSideBlackRook = (Rook)AddPieceToBoard(new Rook(Team.BLACK, new BoardCoord(3, BLACK_BACKROW)));
        }

        public override bool MovePiece(ChessPiece mover, BoardCoord destination) {
            BoardCoord oldPos = mover.GetBoardPosition();

            // Try make the move
            if (MakeMove(mover, destination)) {
                // Check castling moves
                if (mover is King && mover.MoveCount == 1) {
                    if (mover.GetBoardPosition() == new BoardCoord(1, WHITE_BACKROW)) {
                        aSideWhiteRook = PerformCastle(aSideWhiteRook, new BoardCoord(2, WHITE_BACKROW));
                    } else if (mover.GetBoardPosition() == new BoardCoord(2, BLACK_BACKROW)) {
                        hSideBlackRook = PerformCastle(hSideBlackRook, new BoardCoord(1, BLACK_BACKROW));
                    }
                } else if (mover is Pawn) {
                    ((Pawn)mover).validEnPassant = (mover.MoveCount == 1 && mover.GetRelativeBoardCoord(0, -2) == oldPos);
                    CheckPawnEnPassantCapture((Pawn)mover);
                    CheckPawnPromotion((Pawn)mover);
                }
                return true;
            }
            return false;
        }

        public override void CalculateAvailableMoves(ChessPiece mover) {
            mover.ClearAvailableMoves();
            mover.ClearTemplateMoves();

            mover.CalculateTemplateMoves();
            BoardCoord[] templateMoves = mover.GetTemplateMoves();
            for (int i = 0; i < templateMoves.Length; i++) {
                if (IsPieceInCheckAfterThisMove(currentRoyalPiece, mover, templateMoves[i]) == false) {
                    mover.AddToAvailableMoves(templateMoves[i]);
                }
            }

            if (mover is King) {
                if (mover.GetTeam() == Team.WHITE) {
                    AddAvailableCastleMoves(mover, true, false);
                } else {
                    AddAvailableCastleMoves(mover, false, true);
                }
            }
        }
    }
}
