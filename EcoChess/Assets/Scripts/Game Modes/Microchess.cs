using System.Collections;
using System.Collections.Generic;

namespace ChessGameModes {
    /// <summary>
    /// Silverman4x5.cs is a chess variant with a smaller board layout.
    /// 
    /// Winstate: Checkmate.
    /// Piece types: Orthodox.
    /// Piece rules: No pawn double moves, no enpassant.
    /// Board layout:
    ///     k n b r
    ///     p . . .
    ///     . . . .
    ///     . . . p
    ///     R B N K
    /// </summary>
    public class Microchess : Chess {
        private new const int BOARD_WIDTH = 4;
        private new const int BOARD_HEIGHT = 5;

        public Microchess() : base(BOARD_WIDTH, BOARD_HEIGHT) {
            AllowEnpassantCapture = false;
        }

        public override string ToString() {
            return "Microchess";
        }

        public override void PopulateBoard() {
            opposingRoyalPiece = (King)AddPieceToBoard(new King(Team.BLACK, new BoardCoord(0, BLACK_BACKROW)));
            currentRoyalPiece = (King)AddPieceToBoard(new King(Team.WHITE, new BoardCoord(3, WHITE_BACKROW)));

            AddPieceToBoard(new Pawn(Team.BLACK, new BoardCoord(0, BLACK_PAWNROW), initialMoveLimit: 1));
            AddPieceToBoard(new Rook(Team.WHITE, new BoardCoord(0, WHITE_BACKROW)));

            AddPieceToBoard(new Bishop(Team.WHITE, new BoardCoord(1, WHITE_BACKROW)));
            AddPieceToBoard(new Knight(Team.BLACK, new BoardCoord(1, BLACK_BACKROW)));

            AddPieceToBoard(new Knight(Team.WHITE, new BoardCoord(2, WHITE_BACKROW)));
            AddPieceToBoard(new Bishop(Team.BLACK, new BoardCoord(2, BLACK_BACKROW)));

            AddPieceToBoard(new Pawn(Team.WHITE, new BoardCoord(3, WHITE_PAWNROW), initialMoveLimit: 1));
            AddPieceToBoard(new Rook(Team.BLACK, new BoardCoord(3, BLACK_BACKROW)));
        }

        public override List<BoardCoord> CalculateAvailableMoves(ChessPiece mover) {
            BoardCoord[] templateMoves = mover.CalculateTemplateMoves().ToArray();
            List<BoardCoord> availableMoves = new List<BoardCoord>(templateMoves.Length);

            for (int i = 0; i < templateMoves.Length; i++) {
                if (IsPieceInCheckAfterThisMove(currentRoyalPiece, mover, templateMoves[i]) == false) {
                    availableMoves.Add(templateMoves[i]);
                }
            }

            if (mover is King && mover.MoveCount == 0) {
                if (mover.GetTeam() == Team.WHITE) {
                    availableMoves.AddRange(TryAddAvailableCastleMoves(mover, CastlerOptions, canCastleRightward: false));
                } else {
                    availableMoves.AddRange(TryAddAvailableCastleMoves(mover, CastlerOptions, canCastleLeftward: false));
                }
            }
            return availableMoves;
        }

        protected override bool TryPerformCastlingRookMoves(ChessPiece mover, ref string moveNotation) {
            if (mover.MoveCount == 1 && mover == currentRoyalPiece) {

                if (mover.GetBoardPosition() == new BoardCoord(1, WHITE_BACKROW)) {
                    ChessPiece castlingPiece = Board.GetCoordInfo(new BoardCoord(0, mover.GetBoardPosition().y)).occupier;
                    MakeDirectMove(castlingPiece, new BoardCoord(2, mover.GetBoardPosition().y), false);
                    moveNotation = "O-O";
                    return true;

                } else if (mover.GetBoardPosition() == new BoardCoord(2, BLACK_BACKROW)) {
                    ChessPiece castlingPiece = Board.GetCoordInfo(new BoardCoord(BOARD_WIDTH - 1, mover.GetBoardPosition().y)).occupier;
                    MakeDirectMove(castlingPiece, new BoardCoord(1, mover.GetBoardPosition().y), false);
                    moveNotation = "O-O";
                    return true;
                }
            }
            return false;
        }
    }
}
