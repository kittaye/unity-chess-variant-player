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
        }

        public override string ToString() {
            return "Cheshire Cat Chess";
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

            // Try make the move
            if (MakeMove(mover, destination)) {
                Board.GetCoordInfo(oldPos).boardChunk.SetActive(false);

                if (mover is Pawn) {
                    ((Pawn)mover).validEnPassant = (mover.MoveCount == 1 && mover.GetRelativeBoardCoord(0, -1) != oldPos);
                    CheckPawnEnPassantCapture((Pawn)mover);
                    ChessPiece promotedPiece = CheckPawnPromotion((Pawn)mover);
                    if (promotedPiece != null) {
                        mover = promotedPiece;
                    }
                }
                return true;
            }
            return false;
        }
    }
}
