using UnityEngine;
using System.Collections.Generic;

namespace ChessGameModes {
    /// <summary>
    /// Balbo.cs is a chess variant with an irregular board shape.
    /// 
    /// Winstate: Checkmate.
    /// Piece types: Orthodox.
    /// Piece rules: Pawns promote differently on different files.
    /// Board layout:
    ///             q b k    
    ///           r n b n r     
    ///         p p p p p p p  
    ///       . . . . . . . . . 
    ///     . . . . . . . . . . .
    ///     . . . . . . . . . . .
    ///       . . . . . . . . . 
    ///         p p p p p p p
    ///           R N B N R   
    ///             Q B K 
    /// </summary>
    public class Balbo : Chess {
        private new const int BOARD_WIDTH = 11;
        private new const int BOARD_HEIGHT = 10;
        private new const int WHITE_PAWNROW = 2;
        private List<BoardCoord> promotionSquares;
        private Piece[] allPromotionOptions;
        private Piece[] limitedPromotionOptions;

        public Balbo() : base(BOARD_WIDTH, BOARD_HEIGHT) {
            Board.RemoveBoardCoordinates(new string[]
            { "a1", "a2", "a3", "a4", "a7", "a8", "a9", "a10",
              "b1", "b2", "b3", "b8", "b9", "b10",
              "c1", "c2", "c9", "c10",
              "d1", "d10",
              "h1", "h10",
              "i1", "i2", "i9", "i10",
              "j1", "j2", "j3", "j8", "j9", "j10",
              "k1", "k2", "k3", "k4", "k7", "k8", "k9", "k10",
            });

            BLACK_PAWNROW = Board.GetHeight() - 3;

            promotionSquares = new List<BoardCoord>(14);
            AddPromotionSquare("c3");
            AddPromotionSquare("c8");
            AddPromotionSquare("d2");
            AddPromotionSquare("d9");
            AddPromotionSquare("e1");
            AddPromotionSquare("e10");
            AddPromotionSquare("f1");
            AddPromotionSquare("f10");
            AddPromotionSquare("g1");
            AddPromotionSquare("g10");
            AddPromotionSquare("h2");
            AddPromotionSquare("h9");
            AddPromotionSquare("i3");
            AddPromotionSquare("i8");

            allPromotionOptions = PawnPromotionOptions;
            limitedPromotionOptions = new Piece[] { Piece.Bishop, Piece.Knight };
        }

        public override string ToString() {
            return "Balbo's Chess";
        }

        public override VariantHelpDetails GetVariantHelpDetails() {
            return new VariantHelpDetails(
                this.ToString(),
                "Created by G. Balbo (1974)",
                this.ToString() + " is a variant on an irregular board shape of 70 squares and one less pawn for both teams.",
                "Checkmate.",
                "- At the ends of the inner d and h files, pawns promote as normal.\n" +
                "- At the ends of the c and i files, pawns may only promote to a bishop or knight.\n" +
                "- At the ends of the outer four files, pawns may not promote.\n" +
                "- No castling.",
                "https://en.wikipedia.org/wiki/Balbo%27s_game"
            );
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

        protected override ChessPiece CheckPawnPromotion(Pawn mover, ref string moveNotation) {
            if (promotionSquares.Contains(mover.GetBoardPosition())) {
                KillPiece(mover);
                RemovePieceFromActiveTeam(mover);

                ChessPiece newPromotedPiece = ChessPieceFactory.Create(SelectedPawnPromotion, mover.GetTeam(), mover.GetBoardPosition());
                moveNotation += string.Format("={0}", newPromotedPiece.GetLetterNotation());

                return AddPieceToBoard(newPromotedPiece);
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

            if ((mover == currentRoyalPiece || mover == opposingRoyalPiece) && mover.MoveCount == 0) {
                availableMoves.AddRange(TryAddAvailableCastleMoves(mover, CastlerOptions));
            } else if (mover is Pawn) {
                BoardCoord enPassantMove = TryAddAvailableEnPassantMove((Pawn)mover);
                if (enPassantMove != BoardCoord.NULL) {
                    availableMoves.Add(enPassantMove);
                }
                if (checkingForCheck == false && CanPromote((Pawn)mover, availableMoves.ToArray())) {
                    // This is where the code differs from the base method. More specific pawn promotion mechanics.
                    SelectedPawnPromotion = Piece.Queen;
                    PawnPromotionOptions = allPromotionOptions;
                    for (int i = 0; i < availableMoves.Count; i++) {
                        if (availableMoves[i] == new BoardCoord(2, 2) || availableMoves[i] == new BoardCoord(2, 7)
                            || availableMoves[i] == new BoardCoord(8, 2) || availableMoves[i] == new BoardCoord(8, 7)) {
                            SelectedPawnPromotion = Piece.Bishop;
                            PawnPromotionOptions = limitedPromotionOptions;
                            break;
                        }
                    }
                    OnDisplayPromotionUI(true);
                }
            }

            return availableMoves;
        }

        public override void PopulateBoard() {
            currentRoyalPiece = (King)AddPieceToBoard(new King(Team.WHITE, new BoardCoord(6, WHITE_BACKROW)));
            opposingRoyalPiece = (King)AddPieceToBoard(new King(Team.BLACK, new BoardCoord(6, BLACK_BACKROW)));

            AddPieceToBoard(new Queen(Team.WHITE, new BoardCoord(4, WHITE_BACKROW)));
            AddPieceToBoard(new Queen(Team.BLACK, new BoardCoord(4, BLACK_BACKROW)));

            AddPieceToBoard(new Bishop(Team.WHITE, new BoardCoord(5, WHITE_BACKROW)));
            AddPieceToBoard(new Bishop(Team.BLACK, new BoardCoord(5, BLACK_BACKROW)));
            AddPieceToBoard(new Bishop(Team.WHITE, new BoardCoord(5, WHITE_BACKROW + 1)));
            AddPieceToBoard(new Bishop(Team.BLACK, new BoardCoord(5, BLACK_BACKROW - 1)));

            for (int x = 2; x < 9; x++) {
                AddPieceToBoard(new Pawn(Team.WHITE, new BoardCoord(x, WHITE_PAWNROW)));
                AddPieceToBoard(new Pawn(Team.BLACK, new BoardCoord(x, BLACK_PAWNROW)));

                if (x == 4 || x == 6) {
                    AddPieceToBoard(new Knight(Team.WHITE, new BoardCoord(x, WHITE_BACKROW + 1)));
                    AddPieceToBoard(new Knight(Team.BLACK, new BoardCoord(x, BLACK_BACKROW - 1)));
                } else if (x == 3 || x == 7) {
                    AddPieceToBoard(new Rook(Team.WHITE, new BoardCoord(x, WHITE_BACKROW + 1)));
                    AddPieceToBoard(new Rook(Team.BLACK, new BoardCoord(x, BLACK_BACKROW - 1)));
                }
            }
        }
    }
}