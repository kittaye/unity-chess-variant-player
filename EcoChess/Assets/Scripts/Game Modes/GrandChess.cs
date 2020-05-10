using System.Collections.Generic;

namespace ChessGameModes {
    /// <summary>
    /// Board layout: 
    ///     r . . . . . . . . r
    ///     . n b q k $ ^ b n .
    ///     p p p p p p p p p p
    ///     . . . . . . . . . .     $ = Empress
    ///     . . . . . . . . . .     ^ = Princess
    ///     . . . . . . . . . .
    ///     . . . . . . . . . .
    ///     p p p p p p p p p p
    ///     . N B Q K $ ^ B N .
    ///     R . . . . . . . . R
    /// </summary>
    public class GrandChess : Chess {
        private new const int BOARD_WIDTH = 10;
        private new const int BOARD_HEIGHT = 10;
        private new const int WHITE_PAWNROW = 2;

        public GrandChess() : base(BOARD_WIDTH, BOARD_HEIGHT) {
            PawnPromotionOptions = new Piece[6] { Piece.Queen, Piece.Empress, Piece.Princess, Piece.Rook, Piece.Bishop, Piece.Knight };
            BLACK_PAWNROW = Board.GetHeight() - 3;
            AllowCastling = false;
        }

        public override string ToString() {
            return "Grand Chess";
        }

        public override VariantHelpDetails GetVariantHelpDetails() {
            return new VariantHelpDetails(
                this.ToString(),
                "Invented by Christian Freeling (1984)",
                this.ToString() + " is a variant on a 10x10 board with empresses and princesses.",
                "Checkmate.",
                "- Pawns may also promote to an empress or princess.\n" + 
                VariantHelpDetails.rule_NoCastling,
                "https://en.wikipedia.org/wiki/Grand_chess"
            );
        }

        public override void PopulateBoard() {
            currentRoyalPiece = (King)AddNewPieceToBoard(Piece.King, Team.WHITE, new BoardCoord(4, WHITE_BACKROW + 1));
            opposingRoyalPiece = (King)AddNewPieceToBoard(Piece.King, Team.BLACK, new BoardCoord(4, BLACK_BACKROW - 1));

            AddNewPieceToBoard(Piece.Rook, Team.WHITE, new BoardCoord(0, WHITE_BACKROW));
            AddNewPieceToBoard(Piece.Rook, Team.BLACK, new BoardCoord(0, BLACK_BACKROW));
            AddNewPieceToBoard(Piece.Rook, Team.WHITE, new BoardCoord(9, WHITE_BACKROW));
            AddNewPieceToBoard(Piece.Rook, Team.BLACK, new BoardCoord(9, BLACK_BACKROW));

            AddNewPieceToBoard(Piece.Queen, Team.WHITE, new BoardCoord(3, WHITE_BACKROW + 1));
            AddNewPieceToBoard(Piece.Queen, Team.BLACK, new BoardCoord(3, BLACK_BACKROW - 1));

            AddNewPieceToBoard(Piece.Empress, Team.WHITE, new BoardCoord(5, WHITE_BACKROW + 1));
            AddNewPieceToBoard(Piece.Empress, Team.BLACK, new BoardCoord(5, BLACK_BACKROW - 1));

            AddNewPieceToBoard(Piece.Princess, Team.WHITE, new BoardCoord(6, WHITE_BACKROW + 1));
            AddNewPieceToBoard(Piece.Princess, Team.BLACK, new BoardCoord(6, BLACK_BACKROW - 1));

            for (int x = 0; x < BOARD_WIDTH; x++) {
                AddNewPieceToBoard(Piece.Pawn, Team.WHITE, new BoardCoord(x, WHITE_PAWNROW));
                AddNewPieceToBoard(Piece.Pawn, Team.BLACK, new BoardCoord(x, BLACK_PAWNROW));

                if (x == 1 || x == BOARD_WIDTH - 2) {
                    AddNewPieceToBoard(Piece.Knight, Team.WHITE, new BoardCoord(x, WHITE_BACKROW + 1));
                    AddNewPieceToBoard(Piece.Knight, Team.BLACK, new BoardCoord(x, BLACK_BACKROW - 1));
                } else if (x == 2 || x == BOARD_WIDTH - 3) {
                    AddNewPieceToBoard(Piece.Bishop, Team.WHITE, new BoardCoord(x, WHITE_BACKROW + 1));
                    AddNewPieceToBoard(Piece.Bishop, Team.BLACK, new BoardCoord(x, BLACK_BACKROW - 1));
                }
            }
        }

        protected override List<ChessPiece> GetAllPossibleCheckThreats(ChessPiece pieceToCheck) {
            List<ChessPiece> possibleCheckThreats = base.GetAllPossibleCheckThreats(pieceToCheck);

            possibleCheckThreats.AddRange(GetAlivePiecesOfType<Empress>(pieceToCheck.GetOpposingTeam()));
            possibleCheckThreats.AddRange(GetAlivePiecesOfType<Princess>(pieceToCheck.GetOpposingTeam()));

            return possibleCheckThreats;
        }
    }
}