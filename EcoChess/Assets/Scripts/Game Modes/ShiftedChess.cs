using UnityEngine;
using System.Collections.Generic;

namespace ChessGameModes {
    /// <summary>
    /// ShiftedChess.cs is a chess variant with an irregular board shape.
    /// 
    /// Winstate: Checkmate.
    /// Piece types: Orthodox.
    /// Board layout:
    ///           q b n r
    ///   r b n k p p p p
    ///   p p p p . . . .
    ///   . . . . . . . .
    ///   . . . .   . . . .
    ///     . . . . . . . .
    ///     . . . . p p p p
    ///     p p p p K B N R
    ///     R B N Q 
    /// </summary>  
    public class ShiftedChess : FIDERuleset {
        private new const int BOARD_WIDTH = 9;
        private new const int BOARD_HEIGHT = 9;

        public ShiftedChess() : base(BOARD_WIDTH, BOARD_HEIGHT) {
            board.RemoveBoardCoordinates(new string[]
            { "a1", "a2", "a3", "a4", "a9",
              "b9", "c9", "d9",
              "e5",
              "f1", "g1", "h1",
              "i1", "i6", "i7", "i8", "i9",
            });
        }

        public override string ToString() {
            return "Shifted Chess";
        }

        public override void PopulateBoard() {
            currentRoyalPiece = (King)AddPieceToBoard(new King(Team.WHITE, "f2"));
            opposingRoyalPiece = (King)AddPieceToBoard(new King(Team.BLACK, "d8"));

            AddPieceToBoard(new Queen(Team.WHITE, "e1"));
            AddPieceToBoard(new Queen(Team.BLACK, "e9"));

            AddPieceToBoard(new Bishop(Team.WHITE, "c1"));
            AddPieceToBoard(new Bishop(Team.BLACK, "b8"));
            AddPieceToBoard(new Bishop(Team.WHITE, "g2"));
            AddPieceToBoard(new Bishop(Team.BLACK, "f9"));

            AddPieceToBoard(new Knight(Team.WHITE, "d1"));
            AddPieceToBoard(new Knight(Team.BLACK, "c8"));
            AddPieceToBoard(new Knight(Team.WHITE, "h2"));
            AddPieceToBoard(new Knight(Team.BLACK, "g9"));

            AddPieceToBoard(new Rook(Team.WHITE, "b1"));
            aSideBlackRook = (Rook)AddPieceToBoard(new Rook(Team.BLACK, "a8"));
            hSideWhiteRook = (Rook)AddPieceToBoard(new Rook(Team.WHITE, "i2"));
            AddPieceToBoard(new Rook(Team.BLACK, "h9"));

            AddPieceToBoard(new Pawn(Team.WHITE, "b2"));
            AddPieceToBoard(new Pawn(Team.WHITE, "c2"));
            AddPieceToBoard(new Pawn(Team.WHITE, "d2"));
            AddPieceToBoard(new Pawn(Team.WHITE, "e2"));
            AddPieceToBoard(new Pawn(Team.WHITE, "f3"));
            AddPieceToBoard(new Pawn(Team.WHITE, "g3"));
            AddPieceToBoard(new Pawn(Team.WHITE, "h3"));
            AddPieceToBoard(new Pawn(Team.WHITE, "i3"));

            AddPieceToBoard(new Pawn(Team.BLACK, "a7"));
            AddPieceToBoard(new Pawn(Team.BLACK, "b7"));
            AddPieceToBoard(new Pawn(Team.BLACK, "c7"));
            AddPieceToBoard(new Pawn(Team.BLACK, "d7"));
            AddPieceToBoard(new Pawn(Team.BLACK, "e8"));
            AddPieceToBoard(new Pawn(Team.BLACK, "f8"));
            AddPieceToBoard(new Pawn(Team.BLACK, "g8"));
            AddPieceToBoard(new Pawn(Team.BLACK, "h8"));
        }

        public override List<BoardCoord> CalculateAvailableMoves(ChessPiece mover) {
            BoardCoord[] templateMoves = mover.CalculateTemplateMoves().ToArray();
            List<BoardCoord> availableMoves = new List<BoardCoord>(templateMoves.Length);

            for (int i = 0; i < templateMoves.Length; i++) {
                if (IsPieceInCheckAfterThisMove(currentRoyalPiece, mover, templateMoves[i]) == false) {
                    availableMoves.Add(templateMoves[i]);
                }
            }

            if (mover is King) {
                if (mover.GetTeam() == Team.WHITE) {
                    availableMoves.AddRange(TryAddAvailableCastleMoves(mover, false, true));
                } else {
                    availableMoves.AddRange(TryAddAvailableCastleMoves(mover, true, false));
                }
            } else if (mover is Pawn) {
                BoardCoord enPassantMove = TryAddAvailableEnPassantMove(mover);
                if (enPassantMove != BoardCoord.NULL) {
                    availableMoves.Add(enPassantMove);
                }
            }

            return availableMoves;
        }

        public override bool MovePiece(ChessPiece mover, BoardCoord destination) {
            BoardCoord oldPos = mover.GetBoardPosition();

            // Try make the move
            if (MakeMove(mover, destination)) {
                // Check castling moves
                if (mover is King && mover.MoveCount == 1) {
                    TryPerformCastlingRookMoves(mover, 1, 7, 2, 6);
                } else if (mover is Pawn) {
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