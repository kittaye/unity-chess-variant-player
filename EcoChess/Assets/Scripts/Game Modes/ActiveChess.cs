﻿
namespace ChessGameModes {
    /// <summary>
    /// Board layout:
    ///     r n b k q b n r q
    ///     p p p p p p p p p
    ///     . . . . . . . . .
    ///     . . . . . . . . .
    ///     . . . . . . . . .
    ///     . . . . . . . . .
    ///     p p p p p p p p p
    ///     R N B Q K B N R Q
    /// </summary>
    public class ActiveChess : Chess {
        private new const int BOARD_WIDTH = 9;
        public ActiveChess() : base(BOARD_WIDTH, BOARD_HEIGHT) {
        }

        public override string ToString() {
            return "Active Chess";
        }

        public override VariantHelpDetails GetVariantHelpDetails() {
            return new VariantHelpDetails(
                this.ToString(),
                "Invented by G. Kuzmichov (1989)",
                this.ToString() + " is a variant on a 9x8 board with an extra queen and pawn for both teams.",
                "Checkmate.",
                VariantHelpDetails.rule_None,
                "https://greenchess.net/rules.php?v=active"
            );
        }

        public override void PopulateBoard() {
            base.PopulateBoard();

            AddNewPieceToBoard(Piece.Pawn, Team.WHITE, new BoardCoord(BOARD_WIDTH - 1, WHITE_PAWNROW));
            AddNewPieceToBoard(Piece.Pawn, Team.BLACK, new BoardCoord(BOARD_WIDTH - 1, BLACK_PAWNROW));
            AddNewPieceToBoard(Piece.Queen, Team.WHITE, new BoardCoord(BOARD_WIDTH - 1, WHITE_BACKROW));
            AddNewPieceToBoard(Piece.Queen, Team.BLACK, new BoardCoord(BOARD_WIDTH - 1, BLACK_BACKROW));
        }
    }
}
