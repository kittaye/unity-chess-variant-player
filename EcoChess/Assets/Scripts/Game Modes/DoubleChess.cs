using System.Collections.Generic;

namespace ChessGameModes {
    /// <summary>
    /// Board layout:
    ///     r n b q k b n r r n b q k b n r
    ///     p p p p p p p p p p p p p p p p
    ///     . . . . . . . . . . . . . . . .
    ///     . . . . . . . . . . . . . . . .
    ///     . . . . . . . . . . . . . . . .
    ///     . . . . . . . . . . . . . . . .
    ///     . . . . . . . . . . . . . . . .
    ///     . . . . . . . . . . . . . . . .
    ///     . . . . . . . . . . . . . . . .
    ///     . . . . . . . . . . . . . . . .
    ///     p p p p p p p p p p p p p p p p
    ///     R N B Q K B N R R N B Q K B N R
    /// </summary>
    public class DoubleChess : Chess {
        private new const int BOARD_WIDTH = 16;
        private new const int BOARD_HEIGHT = 12;
        private King secondCurrentKing;
        private King secondOpposingKing;

        public DoubleChess() : base(BOARD_WIDTH, BOARD_HEIGHT) {
            secondCurrentKing = secondOpposingKing = null;
        }

        public override string ToString() {
            return "Double-Chess";
        }

        public override VariantHelpDetails GetVariantHelpDetails() {
            return new VariantHelpDetails(
                this.ToString(),
                "Invented by Julian Hayward (1916)",
                this.ToString() + " is a variant on a 12x16 board with an additional king for each team.",
                "Checkmate.",
                "- a-side king castles a-side, p-side king castles p-side.\n" +
                "- Pawns may move up to 4 squares on the initial move.",
                "https://en.wikipedia.org/wiki/Double_chess"
            );
        }

        protected override void SwapCurrentAndOpposingRoyaltyPieces() {
            base.SwapCurrentAndOpposingRoyaltyPieces();

            King temp = secondCurrentKing;
            secondCurrentKing = secondOpposingKing;
            secondOpposingKing = temp;
        }

        public override void PopulateBoard() {
            currentRoyalPiece = (King)AddNewPieceToBoard(Piece.King, Team.WHITE, new BoardCoord(4, WHITE_BACKROW));
            opposingRoyalPiece = (King)AddNewPieceToBoard(Piece.King, Team.BLACK, new BoardCoord(4, BLACK_BACKROW));
            secondCurrentKing = (King)AddNewPieceToBoard(Piece.King, Team.WHITE, new BoardCoord(12, WHITE_BACKROW));
            secondOpposingKing = (King)AddNewPieceToBoard(Piece.King, Team.BLACK, new BoardCoord(12, BLACK_BACKROW));

            AddNewPieceToBoard(Piece.Rook, Team.WHITE, new BoardCoord(0, WHITE_BACKROW));
            AddNewPieceToBoard(Piece.Rook, Team.BLACK, new BoardCoord(0, BLACK_BACKROW));
            AddNewPieceToBoard(Piece.Rook, Team.WHITE, new BoardCoord(15, WHITE_BACKROW));
            AddNewPieceToBoard(Piece.Rook, Team.BLACK, new BoardCoord(15, BLACK_BACKROW));

            for (int x = 0; x < BOARD_WIDTH; x++) {
                ((Pawn)AddNewPieceToBoard(Piece.Pawn, Team.WHITE, new BoardCoord(x, WHITE_PAWNROW))).initialMoveLimit = 4;
                ((Pawn)AddNewPieceToBoard(Piece.Pawn, Team.BLACK, new BoardCoord(x, BLACK_PAWNROW))).initialMoveLimit = 4;

                if (x == 1 || x == 6 || x == 9 || x == 14) {
                    AddNewPieceToBoard(Piece.Knight, Team.WHITE, new BoardCoord(x, WHITE_BACKROW));
                    AddNewPieceToBoard(Piece.Knight, Team.BLACK, new BoardCoord(x, BLACK_BACKROW));
                } else if (x == 2 || x == 5 || x == 10 || x == 13) {
                    AddNewPieceToBoard(Piece.Bishop, Team.WHITE, new BoardCoord(x, WHITE_BACKROW));
                    AddNewPieceToBoard(Piece.Bishop, Team.BLACK, new BoardCoord(x, BLACK_BACKROW));
                } else if (x == 3 || x == 11) {
                    AddNewPieceToBoard(Piece.Queen, Team.WHITE, new BoardCoord(x, WHITE_BACKROW));
                    AddNewPieceToBoard(Piece.Queen, Team.BLACK, new BoardCoord(x, BLACK_BACKROW));
                } else if (x == 7 || x == 8) {
                    AddNewPieceToBoard(Piece.Rook, Team.WHITE, new BoardCoord(x, WHITE_BACKROW));
                    AddNewPieceToBoard(Piece.Rook, Team.BLACK, new BoardCoord(x, BLACK_BACKROW));
                }
            }
        }

        protected override bool TryPerformCastlingMove(ChessPiece mover) {
            if (mover.MoveCount == 1) {
                if (mover.GetBoardPosition().x == 2) {
                    ChessPiece castlingPiece = Board.GetCoordInfo(new BoardCoord(0, mover.GetBoardPosition().y)).GetAliveOccupier();
                    UpdatePiecePositionAndOccupance(castlingPiece, new BoardCoord(3, mover.GetBoardPosition().y));
                    SetLastMoveNotationToQueenSideCastle();
                    return true;

                } else if (mover.GetBoardPosition().x == 14) {
                    ChessPiece castlingPiece = Board.GetCoordInfo(new BoardCoord(BOARD_WIDTH - 1, mover.GetBoardPosition().y)).GetAliveOccupier();
                    UpdatePiecePositionAndOccupance(castlingPiece, new BoardCoord(13, mover.GetBoardPosition().y));
                    SetLastMoveNotationToKingSideCastle();
                    return true;
                }
            }
            return false;
        }

        public override List<BoardCoord> CalculateAvailableMoves(ChessPiece mover) {
            BoardCoord[] templateMoves = mover.CalculateTemplateMoves().ToArray();
            List<BoardCoord> availableMoves = new List<BoardCoord>(templateMoves.Length);

            for (int i = 0; i < templateMoves.Length; i++) {
                if (IsPieceInCheckAfterThisMove(currentRoyalPiece, mover, templateMoves[i]) == false
                    && IsPieceInCheckAfterThisMove(secondCurrentKing, mover, templateMoves[i]) == false) {
                    availableMoves.Add(templateMoves[i]);
                }
            }

            if (IsRoyal(mover)) {
                if (mover == currentRoyalPiece) {
                    availableMoves.AddRange(TryAddAvailableCastleMoves(mover, CastlerOptions, canCastleRightward: false));
                } else if (mover == secondCurrentKing) {
                    availableMoves.AddRange(TryAddAvailableCastleMoves(mover, CastlerOptions, canCastleLeftward: false));
                }
            } else if (mover is Pawn) {
                availableMoves.AddRange(TryAddAvailableEnPassantMoves((Pawn)mover));
            }
            return availableMoves;
        }

        protected override BoardCoord[] TryAddAvailableEnPassantMoves(Pawn mover) {
            const int LEFT = -1;
            const int RIGHT = 1;
            List<BoardCoord> enpassantMoves = new List<BoardCoord>(1);

            if (mover.canEnPassantCapture) {
                for (int i = LEFT; i <= RIGHT; i += 2) {
                    int y = 0;

                    while(Board.ContainsCoord(mover.GetRelativeBoardCoord(i, y))) {
                        BoardCoord sidewaysCoord = mover.GetRelativeBoardCoord(i, y);

                        if (Board.ContainsCoord(sidewaysCoord) && mover.IsThreatTowards(sidewaysCoord)) {
                            ChessPiece piece = Board.GetCoordInfo(sidewaysCoord).GetAliveOccupier();

                            if (piece != null) {
                                if (piece is Pawn && CheckEnPassantVulnerability((Pawn)piece)) {
                                    BoardCoord enpassantCoord = mover.GetRelativeBoardCoord(i, 1);

                                    if (Board.ContainsCoord(enpassantCoord)) {
                                        if (IsPieceInCheckAfterThisMove(currentRoyalPiece, mover, enpassantCoord) == false) {
                                            enpassantMoves.Add(enpassantCoord);
                                        }
                                    }
                                } else {
                                    break;
                                }
                            }
                        }
                        y--;
                    }
                }
            }
            return enpassantMoves.ToArray();
        }

        protected override Pawn TryPerformPawnEnPassantCapture(Pawn mover) {
            BoardCoord oldPos = mover.StateHistory[GameMoveNotations.Count - 1].position;
            BoardCoord newPos = mover.GetBoardPosition();
            int y = -1;

            while (Board.ContainsCoord(mover.GetRelativeBoardCoord(0, y))) {
                ChessPiece occupier = Board.GetCoordInfo(mover.GetRelativeBoardCoord(0, y)).GetAliveOccupier();

                if (occupier != null) {
                    if (mover.IsThreatTowards(occupier)) {
                        if (occupier is Pawn && CheckEnPassantVulnerability((Pawn)occupier)) {
                            mover.CaptureCount++;
                            CapturePiece(occupier);

                            SetLastMoveNotationToEnPassant(oldPos, newPos);
                            return (Pawn)occupier;
                        } else {
                            return null;
                        }
                    } else {
                        return null;
                    }
                }
                y--;
            }
            return null;
        }
    }
}
