using UnityEngine;
using System.Collections.Generic;

namespace ChessGameModes {
    /// <summary>
    /// Legan.cs is a chess variant that involves a custom board layout and unique pawn rules.
    /// 
    /// Winstate: Checkmate.
    /// Piece types: Orthodox.
    /// Piece rules: Pawns move/attack options are switched. Pawns promote on custom squares. No castling. No enpassant capture.
    /// Board layout: 
    ///     k n b r p . . .
    ///     b q p p . . . .
    ///     n p p . . . . .
    ///     r p . p . . . p
    ///     p . . . p . p R
    ///     . . . . . p p N
    ///     . . . . p p Q B
    ///     . . . p R B N K
    /// </summary>
    public class Legan : Chess {
        private List<BoardCoord> promotionSquares;

        public Legan() : base() {
            promotionSquares = new List<BoardCoord>(14);
            AddPromotionSquare("a8");
            AddPromotionSquare("b8");
            AddPromotionSquare("c8");
            AddPromotionSquare("d8");
            AddPromotionSquare("a7");
            AddPromotionSquare("a6");
            AddPromotionSquare("a5");

            AddPromotionSquare("e1");
            AddPromotionSquare("f1");
            AddPromotionSquare("g1");
            AddPromotionSquare("h1");
            AddPromotionSquare("h2");
            AddPromotionSquare("h3");
            AddPromotionSquare("h4");

            AllowCastling = false;
            AllowEnpassantCapture = false;
        }

        private void AddPromotionSquare(string algebraicKeyPosition) {
            BoardCoord coord;
            if (Board.TryGetCoordWithKey(algebraicKeyPosition, out coord)) {
                promotionSquares.Add(coord);
            }
        }

        public override string ToString() {
            return "Legan Chess";
        }

        public override void PopulateBoard() {
            currentRoyalPiece = (King)AddPieceToBoard(new King(Team.WHITE, "h1"));
            opposingRoyalPiece = (King)AddPieceToBoard(new King(Team.BLACK, "a8"));

            AddPieceToBoard(new Knight(Team.WHITE, "g1"));
            AddPieceToBoard(new Knight(Team.WHITE, "h3"));
            AddPieceToBoard(new Knight(Team.BLACK, "b8"));
            AddPieceToBoard(new Knight(Team.BLACK, "a6"));

            AddPieceToBoard(new Bishop(Team.WHITE, "h2"));
            AddPieceToBoard(new Bishop(Team.WHITE, "f1"));
            AddPieceToBoard(new Bishop(Team.BLACK, "c8"));
            AddPieceToBoard(new Bishop(Team.BLACK, "a7"));

            AddPieceToBoard(new Rook(Team.WHITE, "e1"));
            AddPieceToBoard(new Rook(Team.WHITE, "h4"));
            AddPieceToBoard(new Rook(Team.BLACK, "a5"));
            AddPieceToBoard(new Rook(Team.BLACK, "d8"));

            AddPieceToBoard(new Queen(Team.WHITE, "g2"));
            AddPieceToBoard(new Queen(Team.BLACK, "b7"));

            // White pawns
            AddPieceToBoard(new LeganPawn(Team.WHITE, "f2"));
            AddPieceToBoard(new LeganPawn(Team.WHITE, "g3"));
            AddPieceToBoard(new LeganPawn(Team.WHITE, "e4"));
            BoardCoord coord = new BoardCoord(3, 0);
            while (Board.ContainsCoord(coord)) {
                AddPieceToBoard(new LeganPawn(Team.WHITE, coord));
                coord.x++;
                coord.y++;
            }

            // Black pawns
            AddPieceToBoard(new LeganPawn(Team.BLACK, "b6"));
            AddPieceToBoard(new LeganPawn(Team.BLACK, "c7"));
            AddPieceToBoard(new LeganPawn(Team.BLACK, "d5"));
            coord = new BoardCoord(0, 3);
            while (Board.ContainsCoord(coord)) {
                AddPieceToBoard(new LeganPawn(Team.BLACK, coord));
                coord.x++;
                coord.y++;
            }
        }

        protected override bool CanPromote(Pawn mover, BoardCoord[] availableMoves) {
            for (int i = 0; i < availableMoves.Length; i++) {
                if (promotionSquares.Contains(availableMoves[i])) {
                    return true;
                }
            }
            return false;
        }

        protected override ChessPiece CheckPawnPromotion(Pawn mover) {
            if (promotionSquares.Contains(mover.GetBoardPosition())) {
                RemovePieceFromBoard(mover);
                RemovePieceFromActiveTeam(mover);
                return AddPieceToBoard(ChessPieceFactory.Create(SelectedPawnPromotion, mover.GetTeam(), mover.GetBoardPosition()));
            }
            return null;
        }
    }
}