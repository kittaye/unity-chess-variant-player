using System.Collections.Generic;

namespace ChessGameModes {
    /// <summary>
    /// Board layout:
    ///     r n b q $ k ^ b n r
    ///     p p p p p p p p p p
    ///     . . . . . . . . . .
    ///     . . . . . . . . . .   $ = Empress
    ///     . . . . . . . . . .   ^ = Princess
    ///     . . . . . . . . . .
    ///     p p p p p p p p p p
    ///     R N B Q $ K ^ B N R
    /// </summary>
    public class GothicChess : Chess {
        private new const int BOARD_WIDTH = 10;

        public GothicChess() : base(BOARD_WIDTH, BOARD_HEIGHT) {
            PawnPromotionOptions = new Piece[6] { Piece.Queen, Piece.Empress, Piece.Princess, Piece.Rook, Piece.Bishop, Piece.Knight };
            castlingDistance = 3;
        }

        public override string ToString() {
            return "Gothic Chess";
        }

        public override VariantHelpDetails GetVariantHelpDetails() {
            return new VariantHelpDetails(
                this.ToString(),
                "Invented by Ed Trice (2000)",
                this.ToString() + " is a variant on a 10x8 board with empresses and princesses for both teams.",
                "Checkmate.",
                "- King moves three squares when castling.\n" +
                "- Pawns may also promote to an empress or princess.",
                "https://www.chessvariants.com/large.dir/gothicchess.html"
            );
        }

        public override void PopulateBoard() {
            currentRoyalPiece = (King)AddNewPieceToBoard(Piece.King, Team.WHITE, "f1");
            opposingRoyalPiece = (King)AddNewPieceToBoard(Piece.King, Team.BLACK, "f8");

            AddNewPieceToBoard(Piece.Rook, Team.WHITE, "a1");
            AddNewPieceToBoard(Piece.Rook, Team.BLACK, "a8");
            AddNewPieceToBoard(Piece.Rook, Team.WHITE, "j1");
            AddNewPieceToBoard(Piece.Rook, Team.BLACK, "j8");

            AddNewPieceToBoard(Piece.Queen, Team.WHITE, "d1");
            AddNewPieceToBoard(Piece.Queen, Team.BLACK, "d8");

            AddNewPieceToBoard(Piece.Empress, Team.WHITE, "e1");
            AddNewPieceToBoard(Piece.Empress, Team.BLACK, "e8");

            AddNewPieceToBoard(Piece.Princess, Team.WHITE, "g1");
            AddNewPieceToBoard(Piece.Princess, Team.BLACK, "g8");

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

        protected override List<ChessPiece> GetAllPossibleCheckThreats(ChessPiece pieceToCheck) {
            List<ChessPiece> possibleCheckThreats = base.GetAllPossibleCheckThreats(pieceToCheck);

            possibleCheckThreats.AddRange(GetAlivePiecesOfType<Princess>(pieceToCheck.GetOpposingTeam()));
            possibleCheckThreats.AddRange(GetAlivePiecesOfType<Empress>(pieceToCheck.GetOpposingTeam()));

            return possibleCheckThreats;
        }

        protected override bool TryPerformCastlingMove(ChessPiece mover) {
            if (mover.MoveCount == 1) {
                if (mover.GetBoardPosition().x == 2) {
                    ChessPiece castlingPiece = Board.GetCoordInfo(new BoardCoord(0, mover.GetBoardPosition().y)).GetAliveOccupier();
                    UpdatePiecePositionAndOccupance(castlingPiece, new BoardCoord(3, mover.GetBoardPosition().y));
                    SetLastMoveNotationToQueenSideCastle();
                    return true;

                } else if (mover.GetBoardPosition().x == 8) {
                    ChessPiece castlingPiece = Board.GetCoordInfo(new BoardCoord(BOARD_WIDTH - 1, mover.GetBoardPosition().y)).GetAliveOccupier();
                    UpdatePiecePositionAndOccupance(castlingPiece, new BoardCoord(7, mover.GetBoardPosition().y));
                    SetLastMoveNotationToKingSideCastle();
                    return true;
                }
            }
            return false;
        }
    }
}
