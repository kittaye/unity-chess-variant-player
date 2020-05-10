using System.Collections.Generic;

namespace ChessGameModes {
    /// <summary>
    /// Board layout:
    ///     $ ^ q a k b n r
    ///     p p p p p p p p
    ///     . . . . . . . .
    ///     . . . . . . . .     $ = Empress
    ///     . . . . . . . .     ^ = Princess
    ///     . . . . . . . .
    ///     p p p p p p p p
    ///     $ ^ Q A K B N R
    /// </summary>
    public class PerfectChess : Chess {

        public PerfectChess() : base() {
            SelectedPawnPromotion = Piece.Amazon;
            PawnPromotionOptions = new Piece[7] { Piece.Amazon, Piece.Queen, Piece.Empress, Piece.Princess, Piece.Rook, Piece.Bishop, Piece.Knight };
            CastlerOptions = new Piece[] { Piece.Rook, Piece.Empress };
        }

        public override string ToString() {
            return "Perfect Chess";
        }

        public override VariantHelpDetails GetVariantHelpDetails() {
            return new VariantHelpDetails(
                this.ToString(),
                "Invented by Köksal Karakus (2000)",
                this.ToString() + " is a variant involving amazon, empress, and princess fairy pieces on both teams.",
                "Checkmate.",
                "- Kings move three squares when castling.\n" +
                "- Kings may a-side castle with the empress.\n" +
                "- Pawns may also promote to an amazon, empress, or princess.",
                "https://www.chessvariants.com/diffmove.dir/perfectchess.html"
            );
        }

        public override void PopulateBoard() {
            currentRoyalPiece = (King)AddNewPieceToBoard(Piece.King, Team.WHITE, "e1");
            opposingRoyalPiece = (King)AddNewPieceToBoard(Piece.King, Team.BLACK, "e8");

            AddNewPieceToBoard(Piece.Empress, Team.WHITE, "a1");
            AddNewPieceToBoard(Piece.Empress, Team.BLACK, "a8");
            AddNewPieceToBoard(Piece.Rook, Team.WHITE, "h1");
            AddNewPieceToBoard(Piece.Rook, Team.BLACK, "h8");

            AddNewPieceToBoard(Piece.Princess, Team.WHITE, "b1");
            AddNewPieceToBoard(Piece.Princess, Team.BLACK, "b8");

            AddNewPieceToBoard(Piece.Queen, Team.WHITE, "c1");
            AddNewPieceToBoard(Piece.Queen, Team.BLACK, "c8");

            AddNewPieceToBoard(Piece.Amazon, Team.WHITE, "d1");
            AddNewPieceToBoard(Piece.Amazon, Team.BLACK, "d8");

            AddNewPieceToBoard(Piece.Bishop, Team.WHITE, "f1");
            AddNewPieceToBoard(Piece.Bishop, Team.BLACK, "f8");

            AddNewPieceToBoard(Piece.Knight, Team.WHITE, "g1");
            AddNewPieceToBoard(Piece.Knight, Team.BLACK, "g8");

            for (int x = 0; x < BOARD_WIDTH; x++) {
                AddNewPieceToBoard(Piece.Pawn, Team.WHITE, new BoardCoord(x, WHITE_PAWNROW));
                AddNewPieceToBoard(Piece.Pawn, Team.BLACK, new BoardCoord(x, BLACK_PAWNROW));
            }
        }

        protected override List<ChessPiece> GetAllPossibleCheckThreats(ChessPiece pieceToCheck) {
            List<ChessPiece> possibleCheckThreats = base.GetAllPossibleCheckThreats(pieceToCheck);

            possibleCheckThreats.AddRange(GetAlivePiecesOfType<Amazon>(pieceToCheck.GetOpposingTeam()));
            possibleCheckThreats.AddRange(GetAlivePiecesOfType<Empress>(pieceToCheck.GetOpposingTeam()));
            possibleCheckThreats.AddRange(GetAlivePiecesOfType<Princess>(pieceToCheck.GetOpposingTeam()));

            return possibleCheckThreats;
        }

        protected override bool TryPerformCastlingMove(ChessPiece mover) {
            if (mover.MoveCount == 1) {
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
            }
            return false;
        }
    }
}
