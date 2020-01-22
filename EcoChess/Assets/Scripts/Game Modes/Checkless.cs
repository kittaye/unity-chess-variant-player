using System.Collections.Generic;
using UnityEngine;

namespace ChessGameModes {
    /// <summary>
    /// Board layout: FIDE standard.
    /// </summary>
    public class Checkless : Chess {
        private bool checkingForCheckmate;

        public Checkless() : base() {
            checkingForCheckmate = false;
        }

        public override string ToString() {
            return "Checkless Chess";
        }

        public override VariantHelpDetails GetVariantHelpDetails() {
            return new VariantHelpDetails(
                this.ToString(),
                "Invented in the 1800s",
                this.ToString() + " is a variant that prohibits checks against the king except for checkmate.",
                "Checkmate.",
                VariantHelpDetails.rule_None,
                "https://www.chessvariants.com/usualeq.dir/checklss.html"
            );
        }

        public override List<BoardCoord> CalculateAvailableMoves(ChessPiece mover) {
            BoardCoord[] templateMoves = mover.CalculateTemplateMoves().ToArray();
            List<BoardCoord> availableMoves = new List<BoardCoord>(templateMoves.Length);

            // This is where the code differs from the base method. Disallows moves that place the opposing king in check, except checkmate.
            for (int i = 0; i < templateMoves.Length; i++) {
                if (IsPieceInCheckAfterThisMove(currentRoyalPiece, mover, templateMoves[i]) == false) {
                    if (IsPieceInCheckAfterThisMove(opposingRoyalPiece, mover, templateMoves[i]) == false) {
                        availableMoves.Add(templateMoves[i]);
                    } else if (IsPieceInCheckMateAfterThisMove(opposingRoyalPiece, mover, templateMoves[i])){
                        availableMoves.Add(templateMoves[i]);
                    }
                }
            }

            if ((mover == currentRoyalPiece || mover == opposingRoyalPiece) && mover.MoveCount == 0) {
                availableMoves.AddRange(TryAddAvailableCastleMoves(mover, CastlerOptions));
            } else if (mover is Pawn) {
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

        private bool IsPieceInCheckMateAfterThisMove(ChessPiece pieceToCheck, ChessPiece mover, BoardCoord dest) {
            if (AssertContainsCoord(dest)) {
                if (checkingForCheckmate) return false;

                // Temporarily simulate the move actually happening
                ChessPiece originalOccupier = Board.GetCoordInfo(dest).occupier;
                ChessPiece originalLastMover;
                BoardCoord oldPos = mover.GetBoardPosition();
                SimulateMove(mover, dest, originalOccupier, out originalLastMover);

                ChessPiece occupier = null;
                if (mover is Pawn) {
                    string dummy = string.Empty;
                    occupier = CheckPawnEnPassantCapture((Pawn)mover, oldPos, ref dummy);
                }

                // Check whether the piece is checkmated after this temporary move
                bool hasAnyMoves = false;
                checkingForCheckmate = true;
                foreach (ChessPiece piece in GetPieces(pieceToCheck.GetTeam())) {
                    if (piece.IsAlive && hasAnyMoves == false) {
                        BoardCoord[] availableMoves = CalculateAvailableMoves(piece).ToArray();
                        for (int i = 0; i < availableMoves.Length; i++) {
                            if (IsPieceInCheckAfterThisMove(pieceToCheck, piece, availableMoves[i])) {
                                continue;
                            } else {
                                hasAnyMoves = true;
                            }
                        }
                    }
                }
                checkingForCheckmate = false;

                if (occupier != null) {
                    Board.GetCoordInfo(occupier.GetBoardPosition()).occupier = occupier;
                    occupier.IsAlive = true;
                    occupier.gameObject.SetActive(true);
                }

                // Revert the temporary move back to normal
                RevertSimulatedMove(mover, dest, originalOccupier, originalLastMover, oldPos);

                return hasAnyMoves == false;
            }
            return false;
        }
    }
}