using System.Collections.Generic;

namespace ChessGameModes {
    /// <summary>
    /// Board layout:
    ///           q b n r
    ///   r b n k p p p p
    ///   p p p p . . . .
    ///   . . . . . . . .
    ///   . . . .   . . . .
    ///     . . . . . . . .
    ///     . . . . p p p p
    ///     p p p p K B N R
    ///     R B N Q 
    /// </summary>  
    public class ShiftedChess : Chess {
        private new const int BOARD_WIDTH = 9;
        private new const int BOARD_HEIGHT = 9;

        public ShiftedChess() : base(BOARD_WIDTH, BOARD_HEIGHT) {
            Board.RemoveAndDestroyBoardCoordinates(new string[]
            { "a1", "a2", "a3", "a4", "a9",
              "b9", "c9", "d9",
              "e5",
              "f1", "g1", "h1",
              "i1", "i6", "i7", "i8", "i9",
            });
        }

        public override string ToString() {
            return "Shifted Chess";
        }

        public override VariantHelpDetails GetVariantHelpDetails() {
            return new VariantHelpDetails(
                this.ToString(),
                "Invented by Robert J. Bell",
                this.ToString() + " is a variant with an irregular board shape (9x9).",
                "Checkmate.",
                "- Kings may only castle king-side.",
                "https://greenchess.net/rules.php?v=shifted-hv"
            );
        }

        public override void PopulateBoard() {
            currentRoyalPiece = (King)AddNewPieceToBoard(Piece.King, Team.WHITE, "f2");
            opposingRoyalPiece = (King)AddNewPieceToBoard(Piece.King, Team.BLACK, "d8");

            AddNewPieceToBoard(Piece.Queen, Team.WHITE, "e1");
            AddNewPieceToBoard(Piece.Queen, Team.BLACK, "e9");

            AddNewPieceToBoard(Piece.Bishop, Team.WHITE, "c1");
            AddNewPieceToBoard(Piece.Bishop, Team.BLACK, "b8");
            AddNewPieceToBoard(Piece.Bishop, Team.WHITE, "g2");
            AddNewPieceToBoard(Piece.Bishop, Team.BLACK, "f9");

            AddNewPieceToBoard(Piece.Knight, Team.WHITE, "d1");
            AddNewPieceToBoard(Piece.Knight, Team.BLACK, "c8");
            AddNewPieceToBoard(Piece.Knight, Team.WHITE, "h2");
            AddNewPieceToBoard(Piece.Knight, Team.BLACK, "g9");

            AddNewPieceToBoard(Piece.Rook, Team.WHITE, "b1");
            AddNewPieceToBoard(Piece.Rook, Team.BLACK, "a8");
            AddNewPieceToBoard(Piece.Rook, Team.WHITE, "i2");
            AddNewPieceToBoard(Piece.Rook, Team.BLACK, "h9");

            AddNewPieceToBoard(Piece.Pawn, Team.WHITE, "b2");
            AddNewPieceToBoard(Piece.Pawn, Team.WHITE, "c2");
            AddNewPieceToBoard(Piece.Pawn, Team.WHITE, "d2");
            AddNewPieceToBoard(Piece.Pawn, Team.WHITE, "e2");
            AddNewPieceToBoard(Piece.Pawn, Team.WHITE, "f3");
            AddNewPieceToBoard(Piece.Pawn, Team.WHITE, "g3");
            AddNewPieceToBoard(Piece.Pawn, Team.WHITE, "h3");
            AddNewPieceToBoard(Piece.Pawn, Team.WHITE, "i3");

            AddNewPieceToBoard(Piece.Pawn, Team.BLACK, "a7");
            AddNewPieceToBoard(Piece.Pawn, Team.BLACK, "b7");
            AddNewPieceToBoard(Piece.Pawn, Team.BLACK, "c7");
            AddNewPieceToBoard(Piece.Pawn, Team.BLACK, "d7");
            AddNewPieceToBoard(Piece.Pawn, Team.BLACK, "e8");
            AddNewPieceToBoard(Piece.Pawn, Team.BLACK, "f8");
            AddNewPieceToBoard(Piece.Pawn, Team.BLACK, "g8");
            AddNewPieceToBoard(Piece.Pawn, Team.BLACK, "h8");
        }

        protected override bool IsAPromotionMove(BoardCoord move) {
            if (move.x <= 3) {
                if (move.y == WHITE_BACKROW || move.y == BLACK_BACKROW - 1) {
                    return true;
                }
            } else if (move.x >= 5) {
                if (move.y == WHITE_BACKROW + 1 || move.y == BLACK_BACKROW) {
                    return true;
                }
            } else {
                if (move == new BoardCoord(4, WHITE_BACKROW) || move == new BoardCoord(4, BLACK_BACKROW)) {
                    return true;
                }
            }
            return false;
        }

        public override List<BoardCoord> CalculateAvailableMoves(ChessPiece mover) {
            List<BoardCoord> availableMoves = new List<BoardCoord>();

            availableMoves.AddRange(GetLegalTemplateMoves(mover));

            if (IsRoyal(mover)) {
                if (mover.GetTeam() == Team.WHITE) {
                    availableMoves.AddRange(TryAddAvailableCastleMoves(mover, CastlerOptions, canCastleLeftward: false));
                } else {
                    availableMoves.AddRange(TryAddAvailableCastleMoves(mover, CastlerOptions, canCastleRightward: false));
                }
            } else if (mover is Pawn) {
                availableMoves.AddRange(TryAddAvailableEnPassantMoves((Pawn)mover));
            }

            return availableMoves;
        }

        protected override bool TryPerformCastlingMove(ChessPiece mover) {
            if (mover.MoveCount == 1) {
                if (mover.GetBoardPosition().x == 1) {
                    ChessPiece castlingPiece = Board.GetCoordInfo(new BoardCoord(0, mover.GetBoardPosition().y)).GetAliveOccupier();
                    UpdatePiecePositionAndOccupance(castlingPiece, new BoardCoord(2, mover.GetBoardPosition().y));
                    SetLastMoveNotationToQueenSideCastle();
                    return true;

                } else if (mover.GetBoardPosition().x == 7) {
                    ChessPiece castlingPiece = Board.GetCoordInfo(new BoardCoord(BOARD_WIDTH - 1, mover.GetBoardPosition().y)).GetAliveOccupier();
                    UpdatePiecePositionAndOccupance(castlingPiece, new BoardCoord(6, mover.GetBoardPosition().y));
                    SetLastMoveNotationToKingSideCastle();
                    return true;
                }
            }
            return false;
        }
    }
}