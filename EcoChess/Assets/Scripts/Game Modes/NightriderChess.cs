using System.Collections.Generic;

namespace ChessGameModes {
    /// <summary>
    /// Board layout:
    ///     r # b q k b n r
    ///     p p p p p p p p
    ///     . . . . . . . .
    ///     . . . . . . . .     # = Nightrider
    ///     . . . . . . . .
    ///     . . . . . . . .
    ///     p p p p p p p p
    ///     R # B Q K B N R
    /// </summary>
    public class NightriderChess : Chess {
        public NightriderChess() : base() {
            PawnPromotionOptions = new Piece[5] { Piece.Queen, Piece.Rook, Piece.Bishop, Piece.Knight, Piece.Nightrider };
        }

        public override string ToString() {
            return "Nightrider Chess";
        }

        public override VariantHelpDetails GetVariantHelpDetails() {
            return new VariantHelpDetails(
                this.ToString(),
                "Invented by T. R. Dawson (1925)",
                this.ToString() + " is a variant with the nightrider fairy piece replacing one of the knights on both teams.",
                "Checkmate.",
                "- Pawns may also promote to a nightrider.",
                "https://greenchess.net/rules.php?piece=nightrider"
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

            AddNewPieceToBoard(Piece.Nightrider, Team.WHITE, new BoardCoord(1, WHITE_BACKROW));
            AddNewPieceToBoard(Piece.Nightrider, Team.BLACK, new BoardCoord(1, BLACK_BACKROW));
            AddNewPieceToBoard(Piece.Knight, Team.WHITE, new BoardCoord(6, WHITE_BACKROW));
            AddNewPieceToBoard(Piece.Knight, Team.BLACK, new BoardCoord(6, BLACK_BACKROW));

            for (int x = 0; x < BOARD_WIDTH; x++) {
                AddNewPieceToBoard(Piece.Pawn, Team.WHITE, new BoardCoord(x, WHITE_PAWNROW));
                AddNewPieceToBoard(Piece.Pawn, Team.BLACK, new BoardCoord(x, BLACK_PAWNROW));

                if (x == 2 || x == BOARD_WIDTH - 3) {
                    AddNewPieceToBoard(Piece.Bishop, Team.WHITE, new BoardCoord(x, WHITE_BACKROW));
                    AddNewPieceToBoard(Piece.Bishop, Team.BLACK, new BoardCoord(x, BLACK_BACKROW));
                }
            }
        }

        protected override List<ChessPiece> GetAllPossibleCheckThreats(ChessPiece pieceToCheck) {
            List<ChessPiece> possibleCheckThreats = base.GetAllPossibleCheckThreats(pieceToCheck);

            possibleCheckThreats.AddRange(GetAlivePiecesOfType<Nightrider>(pieceToCheck.GetOpposingTeam()));

            return possibleCheckThreats;
        }
    }
}
