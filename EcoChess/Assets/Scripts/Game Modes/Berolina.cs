using System.Collections.Generic;

namespace ChessGameModes {
    /// <summary>
    /// Board layout: FIDE standard.
    /// </summary>
    public class Berolina : Chess {
        public Berolina() : base() {
        }

        public override string ToString() {
            return "Berolina Chess";
        }

        public override VariantHelpDetails GetVariantHelpDetails() {
            return new VariantHelpDetails(
                this.ToString(),
                "Invented by Edmund Nebermann (1926)",
                this.ToString() + " is a variant where pawns are replaced with berolina pawns.",
                "Checkmate.",
                VariantHelpDetails.rule_None,
                "https://en.wikipedia.org/wiki/Berolina_chess"
            );
        }

        public override void PopulateBoard() {
            currentRoyalPiece = (King)AddNewPieceToBoard(Piece.King, Team.WHITE, new BoardCoord(4, WHITE_BACKROW));
            opposingRoyalPiece = (King)AddNewPieceToBoard(Piece.King, Team.BLACK, new BoardCoord(4, BLACK_BACKROW));

            AddNewPieceToBoard(Piece.Rook, Team.WHITE, new BoardCoord(0, WHITE_BACKROW));
            AddNewPieceToBoard(Piece.Rook, Team.BLACK, new BoardCoord(0, BLACK_BACKROW));
            AddNewPieceToBoard(Piece.Rook, Team.WHITE, new BoardCoord(7, WHITE_BACKROW));
            AddNewPieceToBoard(Piece.Rook, Team.BLACK, new BoardCoord(7, BLACK_BACKROW));

            AddNewPieceToBoard(Piece.Queen, Team.WHITE, new BoardCoord(3, WHITE_BACKROW));
            AddNewPieceToBoard(Piece.Queen, Team.BLACK, new BoardCoord(3, BLACK_BACKROW));

            for (int x = 0; x < BOARD_WIDTH; x++) {
                AddNewPieceToBoard(Piece.BerolinaPawn, Team.WHITE, new BoardCoord(x, WHITE_PAWNROW));
                AddNewPieceToBoard(Piece.BerolinaPawn, Team.BLACK, new BoardCoord(x, BLACK_PAWNROW));

                if (x == 1 || x == BOARD_WIDTH - 2) {
                    AddNewPieceToBoard(Piece.Knight, Team.WHITE, new BoardCoord(x, WHITE_BACKROW));
                    AddNewPieceToBoard(Piece.Knight, Team.BLACK, new BoardCoord(x, BLACK_BACKROW));
                } else if (x == 2 || x == BOARD_WIDTH - 3) {
                    AddNewPieceToBoard(Piece.Bishop, Team.WHITE, new BoardCoord(x, WHITE_BACKROW));
                    AddNewPieceToBoard(Piece.Bishop, Team.BLACK, new BoardCoord(x, BLACK_BACKROW));
                }
            }
        }

        protected override BoardCoord[] TryAddAvailableEnPassantMoves(Pawn mover) {
            const int LEFT = -1;
            const int RIGHT = 1;
            List<BoardCoord> enpassantMoves = new List<BoardCoord>(1);

            if (mover.canEnPassantCapture) {
                for (int i = LEFT; i <= RIGHT; i += 2) {
                    BoardCoord sidewaysCoord = mover.GetRelativeBoardCoord(i, 0);

                    if (Board.ContainsCoord(sidewaysCoord) && mover.IsThreatTowards(sidewaysCoord)) {
                        ChessPiece piece = Board.GetCoordInfo(sidewaysCoord).GetAliveOccupier();

                        if (piece is Pawn && CheckEnPassantVulnerability((Pawn)piece)) {
                            BoardCoord enpassantCoord = mover.GetRelativeBoardCoord(0, 1);

                            if (Board.ContainsCoord(enpassantCoord)) {
                                if (IsPieceInCheckAfterThisMove(currentRoyalPiece, mover, enpassantCoord) == false) {
                                    enpassantMoves.Add(enpassantCoord);
                                }
                            }
                        }
                    }
                }
            }
            return enpassantMoves.ToArray();
        }

        protected override Pawn TryPerformPawnEnPassantCapture(Pawn mover) {
            const int LEFT = -1;
            const int RIGHT = 1;

            BoardCoord oldPos = mover.StateHistory[mover.StateHistory.Count - 1].position;
            BoardCoord newPos = mover.GetBoardPosition();

            for (int i = LEFT; i <= RIGHT; i += 2) {
                if (Board.ContainsCoord(mover.GetRelativeBoardCoord(i, -1)) && mover.IsThreatTowards(mover.GetRelativeBoardCoord(i, -1))) {
                    ChessPiece occupier = Board.GetCoordInfo(mover.GetRelativeBoardCoord(i, -1)).GetAliveOccupier();
                    if (occupier != null && occupier is Pawn && CheckEnPassantVulnerability((Pawn)occupier)) {
                        mover.CaptureCount++;
                        CapturePiece(occupier);

                        SetLastMoveNotationToEnPassant(oldPos, newPos);
                        return (Pawn)occupier;
                    }
                }
            }
            return null;
        }
    }
}
