
namespace ChessGameModes {
    /// <summary>
    /// Board layout:
    ///     r n b k q b n r
    ///     p p p p p p p p
    ///     . . . . . . . .
    ///     . . . . . . . .
    ///     . . . . . . . .
    ///     . . . . . . . .
    ///     p p p p p p p p
    ///     R N B Q K B N R
    /// </summary>
    public class ReversedRoyals : Chess {
        public ReversedRoyals() : base() {
        }

        public override string ToString() {
            return "Reversed Royals Chess";
        }

        public override VariantHelpDetails GetVariantHelpDetails() {
            return new VariantHelpDetails(
                this.ToString(),
                "Invented by ???",
                this.ToString() + " is a variant where the king and queen's positions are swapped.",
                "Checkmate.",
                VariantHelpDetails.rule_None,
                "https://greenchess.net/rules.php?v=reversed-royals"
            );
        }

        public override void PopulateBoard() {
            currentRoyalPiece = (King)AddNewPieceToBoard(Piece.King, Team.WHITE, new BoardCoord(4, WHITE_BACKROW));
            opposingRoyalPiece = (King)AddNewPieceToBoard(Piece.King, Team.BLACK, new BoardCoord(3, BLACK_BACKROW));

            AddNewPieceToBoard(Piece.Rook, Team.WHITE, new BoardCoord(0, WHITE_BACKROW));
            AddNewPieceToBoard(Piece.Rook, Team.BLACK, new BoardCoord(0, BLACK_BACKROW));
            AddNewPieceToBoard(Piece.Rook, Team.WHITE, new BoardCoord(7, WHITE_BACKROW));
            AddNewPieceToBoard(Piece.Rook, Team.BLACK, new BoardCoord(7, BLACK_BACKROW));

            AddNewPieceToBoard(Piece.Queen, Team.WHITE, new BoardCoord(3, WHITE_BACKROW));
            AddNewPieceToBoard(Piece.Queen, Team.BLACK, new BoardCoord(4, BLACK_BACKROW));

            for (int x = 0; x < BOARD_WIDTH; x++) {
                AddNewPieceToBoard(Piece.Pawn, Team.WHITE, new BoardCoord(x, WHITE_PAWNROW));
                AddNewPieceToBoard(Piece.Pawn, Team.BLACK, new BoardCoord(x, BLACK_PAWNROW));

                if (x == 1 || x == BOARD_WIDTH - 2) {
                    AddNewPieceToBoard(Piece.Knight, Team.WHITE, new BoardCoord(x, WHITE_BACKROW));
                    AddNewPieceToBoard(Piece.Knight, Team.BLACK, new BoardCoord(x, BLACK_BACKROW));
                } else if (x == 2 || x == BOARD_WIDTH - 3) {
                    AddNewPieceToBoard(Piece.Bishop, Team.WHITE, new BoardCoord(x, WHITE_BACKROW));
                    AddNewPieceToBoard(Piece.Bishop, Team.BLACK, new BoardCoord(x, BLACK_BACKROW));
                }
            }
        }

        protected override bool TryPerformCastlingMove(ChessPiece mover) {
            if (mover.MoveCount == 1) {
                if (mover.GetTeam() == Team.WHITE) {
                    if (mover.GetBoardPosition().x == 2) {
                        ChessPiece castlingPiece = Board.GetCoordInfo(new BoardCoord(0, mover.GetBoardPosition().y)).GetAliveOccupier();
                        UpdatePiecePositionAndOccupance(castlingPiece, new BoardCoord(3, mover.GetBoardPosition().y));
                        SetLastMoveNotationToQueenSideCastle();
                        return true;

                    } else if (mover.GetBoardPosition().x == 6) {
                        ChessPiece castlingPiece = Board.GetCoordInfo(new BoardCoord(BOARD_WIDTH - 1, mover.GetBoardPosition().y)).GetAliveOccupier();
                        UpdatePiecePositionAndOccupance(castlingPiece, new BoardCoord(5, mover.GetBoardPosition().y));
                        SetLastMoveNotationToKingSideCastle();
                        return true;
                    }
                } else {
                    if (mover.GetBoardPosition().x == 1) {
                        ChessPiece castlingPiece = Board.GetCoordInfo(new BoardCoord(0, mover.GetBoardPosition().y)).GetAliveOccupier();
                        UpdatePiecePositionAndOccupance(castlingPiece, new BoardCoord(2, mover.GetBoardPosition().y));
                        SetLastMoveNotationToKingSideCastle();
                        return true;

                    } else if (mover.GetBoardPosition().x == 5) {
                        ChessPiece castlingPiece = Board.GetCoordInfo(new BoardCoord(BOARD_WIDTH - 1, mover.GetBoardPosition().y)).GetAliveOccupier();
                        UpdatePiecePositionAndOccupance(castlingPiece, new BoardCoord(4, mover.GetBoardPosition().y));
                        SetLastMoveNotationToQueenSideCastle();
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
