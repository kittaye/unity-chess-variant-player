using System.Collections.Generic;
using UnityEngine;

namespace ChessGameModes {
    /// <summary>
    /// Weak.cs is a chess variant with a custom initial board layout.
    /// 
    /// Winstate: Checkmate.
    /// Piece types: Orthodox.
    /// Board layout:
    ///     n n n n k n n n
    ///     p p p p p p p p
    ///     . . p . . p . .
    ///     . p p p p p p .
    ///     . . . . . . . .
    ///     . . . . . . . .
    ///     p p p p p p p p
    ///     R N B Q K B N R
    /// </summary>
    public class Weak : FIDERuleset {
        public Weak() : base() {
        }

        public override string ToString() {
            return "Weak!";
        }

        public override void OnTurnComplete() {
            base.OnTurnComplete();
            if(currentTeamTurn == Team.WHITE) {
                selectedPawnPromotion = Piece.Queen;
            } else {
                selectedPawnPromotion = Piece.Knight;
            }
        }

        public override void PopulateBoard() {
            currentRoyalPiece = (King)AddPieceToBoard(new King(Team.WHITE, new BoardCoord(4, WHITE_BACKROW)));
            opposingRoyalPiece = (King)AddPieceToBoard(new King(Team.BLACK, new BoardCoord(4, BLACK_BACKROW)));

            AddPieceToBoard(new Queen(Team.WHITE, "d1"));
            AddPieceToBoard(new Bishop(Team.WHITE, "c1"));
            AddPieceToBoard(new Bishop(Team.WHITE, "f1"));
            AddPieceToBoard(new Knight(Team.WHITE, "b1"));
            AddPieceToBoard(new Knight(Team.WHITE, "g1"));
            aSideWhiteRook = (Rook)AddPieceToBoard(new Rook(Team.WHITE, "a1"));
            hSideWhiteRook = (Rook)AddPieceToBoard(new Rook(Team.WHITE, "h1"));

            AddPieceToBoard(new Pawn(Team.BLACK, "c6", initialMoveLimit: 1));
            AddPieceToBoard(new Pawn(Team.BLACK, "f6", initialMoveLimit: 1));

            for (int x = 0; x < BOARD_WIDTH; x++) {
                AddPieceToBoard(new Pawn(Team.WHITE, new BoardCoord(x, WHITE_PAWNROW)));
                AddPieceToBoard(new Pawn(Team.BLACK, new BoardCoord(x, BLACK_PAWNROW)));

                if(x != 4) {
                    AddPieceToBoard(new Knight(Team.BLACK, new BoardCoord(x, BLACK_BACKROW)));
                }
                if(x >= 1 && x <= BOARD_WIDTH - 2) {
                    AddPieceToBoard(new Pawn(Team.BLACK, new BoardCoord(x, BLACK_PAWNROW - 2), initialMoveLimit: 1));
                }
            }
        }

        public override List<BoardCoord> CalculateAvailableMoves(ChessPiece mover) {
            BoardCoord[] templateMoves = mover.CalculateTemplateMoves().ToArray();
            List<BoardCoord> availableMoves = new List<BoardCoord>(templateMoves.Length);

            for (int i = 0; i < templateMoves.Length; i++) {
                if (IsPieceInCheckAfterThisMove(currentRoyalPiece, mover, templateMoves[i]) == false) {
                    availableMoves.Add(templateMoves[i]);
                }
            }

            if (mover is King && mover.MoveCount == 0) {
                availableMoves.AddRange(TryAddAvailableCastleMoves(mover));
            } else if (mover is Pawn) {
                BoardCoord enPassantMove = TryAddAvailableEnPassantMove((Pawn)mover);
                if (enPassantMove != BoardCoord.NULL) {
                    availableMoves.Add(enPassantMove);
                }
                if (checkingForCheck == false && currentTeamTurn == Team.WHITE && CanPromote((Pawn)mover, availableMoves.ToArray())) {
                    OnDisplayPromotionUI(true);
                }
            }

            return availableMoves;
        }
    }
}
