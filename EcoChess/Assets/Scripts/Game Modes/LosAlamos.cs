using System.Collections;
using System.Collections.Generic;

namespace ChessGameModes {
    /// <summary>
    /// LosAlamos.cs is a chess variant on a 6x6 board with no bishops.
    /// 
    /// Winstate: Checkmate.
    /// Piece types: Orthodox.
    /// Piece rules: No castling, no pawn double moves, no enpassant.
    /// Board layout:
    ///     r n q k n r
    ///     p p p p p p
    ///     . . . . . .
    ///     . . . . . .
    ///     p p p p p p
    ///     R N Q K N R
    /// </summary>
    public class LosAlamos : Chess {
        private new const int BOARD_WIDTH = 6;
        private new const int BOARD_HEIGHT = 6;

        public LosAlamos() : base(BOARD_WIDTH, BOARD_HEIGHT) {
            PawnPromotionOptions = new Piece[3] { Piece.Queen, Piece.Rook, Piece.Knight };
        }

        public override string ToString() {
            return "Los Alamos Chess";
        }

        public override void PopulateBoard() {
            currentRoyalPiece = (King)AddPieceToBoard(new King(Team.WHITE, "d1"));
            opposingRoyalPiece = (King)AddPieceToBoard(new King(Team.BLACK, "d6"));

            AddPieceToBoard(new Queen(Team.WHITE, "c1"));
            AddPieceToBoard(new Queen(Team.BLACK, "c6"));

            aSideWhiteRook = (Rook)AddPieceToBoard(new Rook(Team.WHITE, new BoardCoord(0, WHITE_BACKROW)));
            hSideWhiteRook = (Rook)AddPieceToBoard(new Rook(Team.WHITE, new BoardCoord(BOARD_WIDTH - 1, WHITE_BACKROW)));
            aSideBlackRook = (Rook)AddPieceToBoard(new Rook(Team.BLACK, new BoardCoord(0, BLACK_BACKROW)));
            hSideBlackRook = (Rook)AddPieceToBoard(new Rook(Team.BLACK, new BoardCoord(BOARD_WIDTH - 1, BLACK_BACKROW)));

            for (int x = 0; x < BOARD_WIDTH; x++) {
                AddPieceToBoard(new Pawn(Team.WHITE, new BoardCoord(x, WHITE_PAWNROW), initialMoveLimit: 1));
                AddPieceToBoard(new Pawn(Team.BLACK, new BoardCoord(x, BLACK_PAWNROW), initialMoveLimit: 1));

                if (x == 1 || x == BOARD_WIDTH - 2) {
                    AddPieceToBoard(new Knight(Team.WHITE, new BoardCoord(x, WHITE_BACKROW)));
                    AddPieceToBoard(new Knight(Team.BLACK, new BoardCoord(x, BLACK_BACKROW)));
                }
            }
        }

        public override bool MovePiece(ChessPiece mover, BoardCoord destination) {
            // Try make the move
            if (MakeMove(mover, destination)) {
                if (mover is Pawn) {
                    ChessPiece promotedPiece = CheckPawnPromotion((Pawn)mover);
                    if (promotedPiece != null) {
                        mover = promotedPiece;
                    }
                }
                return true;
            }
            return false;
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
                if (checkingForCheck == false && CanPromote((Pawn)mover, availableMoves.ToArray())) {
                    OnDisplayPromotionUI(true);
                }
            }

            return availableMoves;
        }
    }
}
