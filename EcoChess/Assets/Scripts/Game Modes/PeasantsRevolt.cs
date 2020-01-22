using System;
using UnityEngine;

namespace ChessGameModes {
    /// <summary>
    /// Board layout:
    ///     . n n . k . n .
    ///     . . . . p . . .
    ///     . . . . . . . .
    ///     . . . . . . . .
    ///     . . . . . . . .
    ///     . . . . . . . .
    ///     p p p p p p p p
    ///     . . . . K . . .
    /// </summary>
    public class PeasantsRevolt : Chess {
        public PeasantsRevolt() : base() { }

        public override string ToString() {
            return "Peasants' Revolt";
        }

        public override VariantHelpDetails GetVariantHelpDetails() {
            return new VariantHelpDetails(
                this.ToString(),
                "Invented by George Whelon",
                this.ToString() + " is a variant with a standard army of pawns for white and 3 knights and a pawn for black.",
                "Checkmate.",
                VariantHelpDetails.rule_None,
                "https://www.chessvariants.com/large.dir/peasantrevolt.html"
            );
        }

        public override void PopulateBoard() {
            currentRoyalPiece = (King)AddPieceToBoard(new King(Team.WHITE, new BoardCoord(4, WHITE_BACKROW)));
            opposingRoyalPiece = (King)AddPieceToBoard(new King(Team.BLACK, new BoardCoord(4, BLACK_BACKROW)));

            AddPieceToBoard(new Pawn(Team.BLACK, new BoardCoord(4, BLACK_PAWNROW)));

            for (int x = 0; x < BOARD_WIDTH; x++) {
                AddPieceToBoard(new Pawn(Team.WHITE, new BoardCoord(x, WHITE_PAWNROW)));

                if (x == 1 || x == 2 || x == BOARD_WIDTH - 2) {
                    AddPieceToBoard(new Knight(Team.BLACK, new BoardCoord(x, BLACK_BACKROW)));
                }
            }
        }
    }
}
