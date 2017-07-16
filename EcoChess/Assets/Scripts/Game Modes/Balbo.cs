using UnityEngine;

namespace ChessGameModes {
    /// <summary>
    /// Balbo.cs is a chess variant with an irregular board shape.
    /// 
    /// Winstate: Checkmate.
    /// Piece types: Orthodox.
    /// Piece rules: pawn
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
    public class Balbo : FIDERuleset {
        private new const int BOARD_WIDTH = 11;
        private new const int BOARD_HEIGHT = 10;
        private new const int WHITE_PAWNROW = 2;

        public Balbo() : base(BOARD_WIDTH, BOARD_HEIGHT) {
            BLACK_PAWNROW = board.GetHeight() - 3;

            board.RemoveBoardCoordinates(new string[]
            { "a1", "a2", "a3", "a4", "a7", "a8", "a9", "a10",
              "b1", "b2", "b3", "b8", "b9", "b10",
              "c1", "c2", "c9", "c10",
              "d1", "d10",
              "h1", "h10",
              "i1", "i2", "i9", "i10",
              "j1", "j2", "j3", "j8", "j9", "j10",
              "k1", "k2", "k3", "k4", "k7", "k8", "k9", "k10",
            });
        }

        public override string ToString() {
            return "Balbo's chess";
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