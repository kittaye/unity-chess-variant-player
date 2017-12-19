using System.Collections.Generic;
using UnityEngine;

namespace ChessGameModes {
    /// <summary>
    /// GothicChess.cs is a chess variant with a different initial board layout on a 10x8 board.
    /// 
    /// Winstate: Checkmate.
    /// Piece types: Orthodox + Amazons, Empresses, Princesses.
    /// Board layout:
    ///     r n b q $ k ^ b n r
    ///     p p p p p p p p p p
    ///     . . . . . . . . . .
    ///     . . . . . . . . . .   $ = Empress
    ///     . . . . . . . . . .   ^ = Princess
    ///     . . . . . . . . . .
    ///     p p p p p p p p p p
    ///     R N B Q $ K ^ B N R
    /// </summary>
    public class GothicChess : FIDERuleset {
        private new const int BOARD_WIDTH = 10;

        public GothicChess() : base(BOARD_WIDTH, BOARD_HEIGHT) {
            pawnPromotionOptions = new Piece[6] { Piece.Queen, Piece.Empress, Piece.Princess, Piece.Rook, Piece.Bishop, Piece.Knight };
        }

        public override string ToString() {
            return "Gothic Chess";
        }

        public override void PopulateBoard() {
            currentRoyalPiece = (King)AddPieceToBoard(new King(Team.WHITE, "f1"));
            opposingRoyalPiece = (King)AddPieceToBoard(new King(Team.BLACK, "f8"));

            aSideWhiteRook = (Rook)AddPieceToBoard(new Rook(Team.WHITE, "a1"));
            aSideBlackRook = (Rook)AddPieceToBoard(new Rook(Team.BLACK, "a8"));
            hSideWhiteRook = (Rook)AddPieceToBoard(new Rook(Team.WHITE, "j1"));
            hSideBlackRook = (Rook)AddPieceToBoard(new Rook(Team.BLACK, "j8"));

            AddPieceToBoard(new Queen(Team.WHITE, "d1"));
            AddPieceToBoard(new Queen(Team.BLACK, "d8"));

            AddPieceToBoard(new Empress(Team.WHITE, "e1"));
            AddPieceToBoard(new Empress(Team.BLACK, "e8"));

            AddPieceToBoard(new Princess(Team.WHITE, "g1"));
            AddPieceToBoard(new Princess(Team.BLACK, "g8"));

            for (int x = 0; x < BOARD_WIDTH; x++) {
                AddPieceToBoard(new Pawn(Team.WHITE, new BoardCoord(x, WHITE_PAWNROW)));
                AddPieceToBoard(new Pawn(Team.BLACK, new BoardCoord(x, BLACK_PAWNROW)));

                if (x == 1 || x == BOARD_WIDTH - 2) {
                    AddPieceToBoard(new Knight(Team.WHITE, new BoardCoord(x, WHITE_BACKROW)));
                    AddPieceToBoard(new Knight(Team.BLACK, new BoardCoord(x, BLACK_BACKROW)));
                } else if (x == 2 || x == BOARD_WIDTH - 3) {
                    AddPieceToBoard(new Bishop(Team.WHITE, new BoardCoord(x, WHITE_BACKROW)));
                    AddPieceToBoard(new Bishop(Team.BLACK, new BoardCoord(x, BLACK_BACKROW)));
                }
            }
        }

        protected override List<ChessPiece> GetAllPossibleCheckThreats(ChessPiece pieceToCheck) {
            List<ChessPiece> possibleCheckThreats = base.GetAllPossibleCheckThreats(pieceToCheck);

            GetPiecesOfType<Empress>(pieceToCheck.GetOpposingTeam()).ForEach(x => { possibleCheckThreats.Add(x); });
            GetPiecesOfType<Princess>(pieceToCheck.GetOpposingTeam()).ForEach(x => { possibleCheckThreats.Add(x); });

            return possibleCheckThreats;
        }

        protected override void TryPerformCastlingRookMoves(ChessPiece mover) {
            if (mover.GetBoardPosition().x == 2) {
                if (mover.GetTeam() == Team.WHITE) {
                    aSideWhiteRook = (Rook)PerformCastle(aSideWhiteRook, new BoardCoord(3, mover.GetBoardPosition().y));
                } else {
                    aSideBlackRook = (Rook)PerformCastle(aSideBlackRook, new BoardCoord(3, mover.GetBoardPosition().y));
                }
            } else if (mover.GetBoardPosition().x == 8) {
                if (mover.GetTeam() == Team.WHITE) {
                    hSideWhiteRook = (Rook)PerformCastle(hSideWhiteRook, new BoardCoord(7, mover.GetBoardPosition().y));
                } else {
                    hSideBlackRook = (Rook)PerformCastle(hSideBlackRook, new BoardCoord(7, mover.GetBoardPosition().y));
                }
            }
        }

        protected override BoardCoord[] TryAddAvailableCastleMoves(ChessPiece king, bool canCastleLeftward = true, bool canCastleRightward = true) {
            const int LEFT = -1;
            const int RIGHT = 1;

            if (IsPieceInCheck(king) == false) {
                List<BoardCoord> castleMoves = new List<BoardCoord>(2);

                for (int i = LEFT; i <= RIGHT; i += 2) {
                    if (!canCastleLeftward && i == LEFT) continue;
                    if (!canCastleRightward && i == RIGHT) break;

                    int x = king.GetBoardPosition().x + i;
                    int y = king.GetBoardPosition().y;
                    BoardCoord coord = new BoardCoord(x, y);

                    while (board.ContainsCoord(coord)) {
                        ChessPiece occupier = board.GetCoordInfo(coord).occupier;
                        if (occupier != null) {
                            if (occupier is Rook && occupier.MoveCount == 0) {
                                if (IsPieceInCheckAfterThisMove(king, king, king.GetBoardPosition() + new BoardCoord(i, 0)) == false
                                    && IsPieceInCheckAfterThisMove(king, king, king.GetBoardPosition() + new BoardCoord(i * 2, 0)) == false
                                    && IsPieceInCheckAfterThisMove(king, king, king.GetBoardPosition() + new BoardCoord(i * 3, 0)) == false) {
                                    castleMoves.Add(TryGetSpecificMove(king, king.GetBoardPosition() + new BoardCoord(i * 3, 0)));
                                }
                            }
                            break;
                        }
                        x += i;
                        coord = new BoardCoord(x, y);
                    }
                }
                return castleMoves.ToArray();
            }
            return new BoardCoord[0];
        }
    }
}
