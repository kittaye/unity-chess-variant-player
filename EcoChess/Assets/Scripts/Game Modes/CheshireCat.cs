using System.Collections.Generic;

namespace ChessGameModes {
    /// <summary>
    /// Board layout: FIDE standard.
    /// </summary>
    public class CheshireCat : Chess {
        private HashSet<BoardCoord> vanishedSquares;

        public CheshireCat() : base() {
            vanishedSquares = new HashSet<BoardCoord>();
            AllowCastling = false;
        }

        public override string ToString() {
            return "Cheshire Cat Chess";
        }

        public override VariantHelpDetails GetVariantHelpDetails() {
            return new VariantHelpDetails(
                this.ToString(),
                "Invented by V. R. Parton (1970)",
                this.ToString() + " is a variant where the square a piece moves from vanishes.",
                "Checkmate.",
                "- Pieces can move over, but not onto, vanished squares.\n" +
                "- The king's first move behaves like a queen.\n" +
                VariantHelpDetails.rule_NoCastling,
                "https://www.chessvariants.com/boardrules.dir/cheshir.html"
            );
        }

        public override List<BoardCoord> CalculateAvailableMoves(ChessPiece mover) {
            BoardCoord[] templateMoves;

            if (mover is King && mover.MoveCount == 0) {
                List<BoardCoord> kingFirstMoves = new List<BoardCoord>();
                for (int i = 0; i <= 7; i++) {
                    kingFirstMoves.AddRange(mover.TryGetDirectionalTemplateMoves((MoveDirection)i));
                }
                templateMoves = kingFirstMoves.ToArray();
            } else {
                templateMoves = mover.CalculateTemplateMoves().ToArray();
            }

            List<BoardCoord> availableMoves = new List<BoardCoord>(templateMoves.Length);

            for (int i = 0; i < templateMoves.Length; i++) {
                if (!IsSquareVanished(templateMoves[i]) && !IsPieceInCheckAfterThisMove(currentRoyalPiece, mover, templateMoves[i])) {
                    availableMoves.Add(templateMoves[i]);
                }
            }

            // This code remains the same from the base method.
            if (mover is Pawn) {
                availableMoves.AddRange(TryAddAvailableEnPassantMoves((Pawn)mover));
            }

            return availableMoves;
        }

        public override bool MovePiece(ChessPiece mover, BoardCoord destination) {
            BoardCoord oldPos = mover.GetBoardPosition();

            if (base.MovePiece(mover, destination)) {
                VanishSquare(oldPos);
                return true;
            }

            return false;
        }

        public override bool UndoLastMove() {
            if (base.UndoLastMove()) {
                foreach (ChessPiece piece in GetAlivePiecesOfType<ChessPiece>()) {
                    UndoVanishedSquare(piece.GetBoardPosition());
                }

                return true;
            }
            return false;
        }

        private bool VanishSquare(BoardCoord coord) {
            if (!IsSquareVanished(coord)) {
                vanishedSquares.Add(coord);
                Board.RaiseEventHideBoardChunkObject(Board.GetCoordInfo(coord).graphicalObject);
                return true;
            }
            return false;
        }

        private bool UndoVanishedSquare(BoardCoord coord) {
            if (vanishedSquares.Remove(coord)) {
                Board.RaiseEventShowBoardChunkObject(Board.GetCoordInfo(coord).graphicalObject);
                return true;
            }
            return false;
        }

        private bool IsSquareVanished(BoardCoord coord) {
            return vanishedSquares.Contains(coord);
        }
    }
}
