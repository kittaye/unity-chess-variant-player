using System.Collections.Generic;
using UnityEngine;

namespace ChessGameModes {
    /// <summary>
    /// CheshireCat.cs is a chess variant where the square a piece moved from is removed.
    /// 
    /// Winstate: Checkmate.
    /// Piece types: Orthodox.
    /// Piece rules: No castling, piece can move over - but not onto - removed squares. King's first move moves like a queen.
    /// Board layout: Orthodox.
    /// </summary>
    public class CheshireCat : Chess {

        public CheshireCat() : base() {
            AllowCastling = false;
        }

        public override string ToString() {
            return "Cheshire Cat Chess";
        }

        public override VariantHelpDetails GetVariantHelpDetails() {
            return new VariantHelpDetails(
                this.ToString(),
                "Created by V. R. Parton (1970)",
                this.ToString() + " is a variant where the square a piece moves from vanishes.",
                "Checkmate.",
                "- Pieces can move over, but not onto, vanished squares.\n" +
                "- The king's first move behaves like a queen.\n" +
                "- No castling.",
                "https://www.chessvariants.com/boardrules.dir/cheshir.html"
            );
        }

        public override List<BoardCoord> CalculateAvailableMoves(ChessPiece mover) {
            BoardCoord[] templateMoves;

            if (mover is King && mover.MoveCount == 0) {
                List<BoardCoord> kingFirstMoves = new List<BoardCoord>();
                for (int i = 0; i <= 7; i++) {
                    kingFirstMoves.AddRange(TryGetDirectionalMoves(mover, (MoveDirection)i));
                }
                templateMoves = kingFirstMoves.ToArray();
            } else {
                templateMoves = mover.CalculateTemplateMoves().ToArray();
            }

            List<BoardCoord> availableMoves = new List<BoardCoord>(templateMoves.Length);

            for (int i = 0; i < templateMoves.Length; i++) {
                if (Board.GetCoordInfo(templateMoves[i]).boardChunk.activeInHierarchy 
                    && IsPieceInCheckAfterThisMove(currentRoyalPiece, mover, templateMoves[i]) == false) {
                    availableMoves.Add(templateMoves[i]);
                }
            }

            // This code remains the same from the base method.
            if (mover is Pawn) {
                BoardCoord enPassantMove = TryAddAvailableEnPassantMove((Pawn)mover);
                if (enPassantMove != BoardCoord.NULL) {
                    availableMoves.Add(enPassantMove);
                }
                if (checkingForCheck == false && CanPromote((Pawn)mover, availableMoves.ToArray())) {
                    OnDisplayPromotionUI(true);
                }
            }

            return availableMoves;
        }

        public override bool MovePiece(ChessPiece mover, BoardCoord destination) {
            BoardCoord oldPos = mover.GetBoardPosition();

            if (base.MovePiece(mover, destination)) {
                Board.GetCoordInfo(oldPos).boardChunk.SetActive(false);
                return true;
            }

            return false;
        }
    }
}
