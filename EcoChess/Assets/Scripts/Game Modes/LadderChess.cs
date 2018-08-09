using UnityEngine;
using System.Collections.Generic;

namespace ChessGameModes {
    /// <summary>
    /// LadderChess.cs is a chess variant with an irregular board shape (5x12).
    /// 
    /// Winstate: Checkmate.
    /// Piece types: Orthodox.
    /// Piece rules: No castling.
    /// Board layout:
    ///             K
    ///           Q p
    ///         B p .
    ///       N p . .
    ///     R p . . . 
    ///     p . . . .
    ///     . . . . p
    ///     . . . p R
    ///     . . p N 
    ///     . p B
    ///     p Q   
    ///     K 
    /// </summary>
    public class LadderChess : Chess {
        private new const int BOARD_WIDTH = 5;
        private new const int BOARD_HEIGHT = 12;
        private List<BoardCoord> promotionSquares;

        public LadderChess() : base(BOARD_WIDTH, BOARD_HEIGHT) {
            Board.RemoveBoardCoordinates(new string[]
            { "b1", "c1", "d1", "e1",
              "c2", "d2", "e2",
              "d3", "e3",
              "e4",
              "a12", "b12","c12","d12",
              "a11", "b11","c11",
              "a10","b10",
              "a9"
            });

            promotionSquares = new List<BoardCoord>(10);
            AddPromotionSquare("a1");
            AddPromotionSquare("b2");
            AddPromotionSquare("c3");
            AddPromotionSquare("d4");
            AddPromotionSquare("e5");
            AddPromotionSquare("a8");
            AddPromotionSquare("b9");
            AddPromotionSquare("c10");
            AddPromotionSquare("d11");
            AddPromotionSquare("e12");
        }

        public override string ToString() {
            return "Ladder Chess";
        }

        private void AddPromotionSquare(string algebraicKeyPosition) {
            BoardCoord coord;
            if (Board.TryGetCoordWithKey(algebraicKeyPosition, out coord)) {
                promotionSquares.Add(coord);
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

        public override List<BoardCoord> CalculateAvailableMoves(ChessPiece mover) {
            BoardCoord[] templateMoves = mover.CalculateTemplateMoves().ToArray();
            List<BoardCoord> availableMoves = new List<BoardCoord>(templateMoves.Length);

            for (int i = 0; i < templateMoves.Length; i++) {
                if (IsPieceInCheckAfterThisMove(currentRoyalPiece, mover, templateMoves[i]) == false) {
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

        public override void PopulateBoard() {
            currentRoyalPiece = (King)AddPieceToBoard(new King(Team.WHITE, "a1"));
            opposingRoyalPiece = (King)AddPieceToBoard(new King(Team.BLACK, "e12"));

            AddPromotionSquare("a1");
            AddPromotionSquare("b2");
            AddPromotionSquare("c3");
            AddPromotionSquare("d4");
            AddPromotionSquare("e5");
            AddPromotionSquare("a8");
            AddPromotionSquare("b9");
            AddPromotionSquare("c10");
            AddPromotionSquare("d11");
            AddPromotionSquare("e12");

            AddPieceToBoard(new Queen(Team.WHITE, "b2"));
            AddPieceToBoard(new Queen(Team.BLACK, "d11"));

            AddPieceToBoard(new Bishop(Team.WHITE, "c3"));
            AddPieceToBoard(new Bishop(Team.BLACK, "c10"));

            AddPieceToBoard(new Knight(Team.WHITE, "d4"));
            AddPieceToBoard(new Knight(Team.BLACK, "b9"));

            AddPieceToBoard(new Rook(Team.WHITE, "e5"));
            AddPieceToBoard(new Rook(Team.BLACK, "a8"));

            for (int x = 0; x < BOARD_WIDTH; x++) {
                AddPieceToBoard(new Pawn(Team.WHITE, new BoardCoord(x, x + 1)));
                AddPieceToBoard(new Pawn(Team.BLACK, new BoardCoord(x, x + 6)));
            }
        }
    }
}