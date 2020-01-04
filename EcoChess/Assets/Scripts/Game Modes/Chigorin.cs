﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace ChessGameModes {
    /// <summary>
    /// Chigorin.cs is a chess variant with knights and an empress for team white; bishops and a queen for team black.
    /// 
    /// Winstate: Checkmate.
    /// Piece types: Orthodox + Empress.
    /// Board layout:
    ///     r b b q k b b r
    ///     p p p p p p p p
    ///     . . . . . . . .
    ///     . . . . . . . .     $ = Empress
    ///     . . . . . . . .
    ///     . . . . . . . .
    ///     p p p p p p p p
    ///     R N N $ K N N R
    /// </summary>
    public class Chigorin : Chess {
        private Piece[] whitePromotionOptions;
        private Piece[] blackPromotionOptions;

        public Chigorin() : base() {
            whitePromotionOptions = new Piece[] { Piece.Empress, Piece.Knight };
            blackPromotionOptions = new Piece[] { Piece.Queen, Piece.Bishop };
        }

        public override string ToString() {
            return "Chigorin Chess";
        }

        public override void OnMoveComplete() {
            base.OnMoveComplete();
            if (GetCurrentTeamTurn() == Team.WHITE) {
                SelectedPawnPromotion = Piece.Empress;
                PawnPromotionOptions = whitePromotionOptions;
            } else {
                SelectedPawnPromotion = Piece.Queen;
                PawnPromotionOptions = blackPromotionOptions;
            }
        }

        public override void PopulateBoard() {
            currentRoyalPiece = (King)AddPieceToBoard(new King(Team.WHITE, new BoardCoord(4, WHITE_BACKROW)));
            opposingRoyalPiece = (King)AddPieceToBoard(new King(Team.BLACK, new BoardCoord(4, BLACK_BACKROW)));

            AddPieceToBoard(new Rook(Team.WHITE, new BoardCoord(0, WHITE_BACKROW)));
            AddPieceToBoard(new Rook(Team.BLACK, new BoardCoord(0, BLACK_BACKROW)));
            AddPieceToBoard(new Rook(Team.WHITE, new BoardCoord(7, WHITE_BACKROW)));
            AddPieceToBoard(new Rook(Team.BLACK, new BoardCoord(7, BLACK_BACKROW)));

            AddPieceToBoard(new Knight(Team.WHITE, new BoardCoord(1, WHITE_BACKROW)));
            AddPieceToBoard(new Knight(Team.WHITE, new BoardCoord(2, WHITE_BACKROW)));
            AddPieceToBoard(new Knight(Team.WHITE, new BoardCoord(5, WHITE_BACKROW)));
            AddPieceToBoard(new Knight(Team.WHITE, new BoardCoord(6, WHITE_BACKROW)));

            AddPieceToBoard(new Bishop(Team.BLACK, new BoardCoord(1, BLACK_BACKROW)));
            AddPieceToBoard(new Bishop(Team.BLACK, new BoardCoord(2, BLACK_BACKROW)));
            AddPieceToBoard(new Bishop(Team.BLACK, new BoardCoord(5, BLACK_BACKROW)));
            AddPieceToBoard(new Bishop(Team.BLACK, new BoardCoord(6, BLACK_BACKROW)));

            AddPieceToBoard(new Empress(Team.WHITE, new BoardCoord(3, WHITE_BACKROW)));
            AddPieceToBoard(new Queen(Team.BLACK, new BoardCoord(3, BLACK_BACKROW)));

            for (int x = 0; x < BOARD_WIDTH; x++) {
                AddPieceToBoard(new Pawn(Team.WHITE, new BoardCoord(x, WHITE_PAWNROW)));
                AddPieceToBoard(new Pawn(Team.BLACK, new BoardCoord(x, BLACK_PAWNROW)));
            }
        }
    }
}
