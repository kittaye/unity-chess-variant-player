﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace ChessGameModes {
    /// <summary>
    /// Berolina.cs is a chess variant where pawns are replaced with berolina pawns.
    /// 
    /// Winstate: Checkmate.
    /// Piece types: Orthodox + Berolina Pawn.
    /// Board layout: FIDE standard.
    /// </summary>
    public class Berolina : FIDERuleset {
        public Berolina() : base() {
        }

        public override string ToString() {
            return "Berolina Chess";
        }

        public override void PopulateBoard() {
            currentRoyalPiece = (King)AddPieceToBoard(new King(Team.WHITE, new BoardCoord(4, WHITE_BACKROW)));
            opposingRoyalPiece = (King)AddPieceToBoard(new King(Team.BLACK, new BoardCoord(4, BLACK_BACKROW)));

            aSideWhiteRook = (Rook)AddPieceToBoard(new Rook(Team.WHITE, new BoardCoord(0, WHITE_BACKROW)));
            aSideBlackRook = (Rook)AddPieceToBoard(new Rook(Team.BLACK, new BoardCoord(0, BLACK_BACKROW)));
            hSideWhiteRook = (Rook)AddPieceToBoard(new Rook(Team.WHITE, new BoardCoord(7, WHITE_BACKROW)));
            hSideBlackRook = (Rook)AddPieceToBoard(new Rook(Team.BLACK, new BoardCoord(7, BLACK_BACKROW)));

            AddPieceToBoard(new Queen(Team.WHITE, new BoardCoord(3, WHITE_BACKROW)));
            AddPieceToBoard(new Queen(Team.BLACK, new BoardCoord(3, BLACK_BACKROW)));

            for (int x = 0; x < BOARD_WIDTH; x++) {
                AddPieceToBoard(new BerolinaPawn(Team.WHITE, new BoardCoord(x, WHITE_PAWNROW)));
                AddPieceToBoard(new BerolinaPawn(Team.BLACK, new BoardCoord(x, BLACK_PAWNROW)));

                if (x == 1 || x == BOARD_WIDTH - 2) {
                    AddPieceToBoard(new Knight(Team.WHITE, new BoardCoord(x, WHITE_BACKROW)));
                    AddPieceToBoard(new Knight(Team.BLACK, new BoardCoord(x, BLACK_BACKROW)));
                } else if (x == 2 || x == BOARD_WIDTH - 3) {
                    AddPieceToBoard(new Bishop(Team.WHITE, new BoardCoord(x, WHITE_BACKROW)));
                    AddPieceToBoard(new Bishop(Team.BLACK, new BoardCoord(x, BLACK_BACKROW)));
                }
            }
        }

        protected override BoardCoord TryAddAvailableEnPassantMove(Pawn mover) {
            const int LEFT = -1;
            const int RIGHT = 1;

            if (mover.canEnPassantCapture) {
                for (int i = LEFT; i <= RIGHT; i += 2) {
                    BoardCoord coord = TryGetSpecificMove(mover, mover.GetRelativeBoardCoord(i, 0), threatOnly: true);
                    if (board.ContainsCoord(coord)) {
                        ChessPiece piece = board.GetCoordInfo(coord).occupier;
                        if (piece is Pawn && piece == LastMovedOpposingPiece(mover) && ((Pawn)piece).validEnPassant) {
                            if (IsPieceInCheckAfterThisMove(currentRoyalPiece, mover, mover.GetRelativeBoardCoord(0, 1)) == false) {
                                return TryGetSpecificMove(mover, mover.GetRelativeBoardCoord(0, 1));
                            }
                        }
                    }
                }
            }
            return BoardCoord.NULL;
        }

        protected override Pawn CheckPawnEnPassantCapture(Pawn mover) {
            const int LEFT = -1;
            const int RIGHT = 1;

            for (int i = LEFT; i <= RIGHT; i += 2) {
                if (board.ContainsCoord(mover.GetRelativeBoardCoord(i, -1)) && IsThreat(mover, mover.GetRelativeBoardCoord(i, -1))) {
                    ChessPiece occupier = board.GetCoordInfo(mover.GetRelativeBoardCoord(i, -1)).occupier;
                    if (occupier != null && occupier is Pawn && ((Pawn)occupier).validEnPassant) {
                        mover.CaptureCount++;
                        RemovePieceFromBoard(occupier);
                        return (Pawn)occupier;
                    }
                }
            }
            return null;
        }
    }
}
