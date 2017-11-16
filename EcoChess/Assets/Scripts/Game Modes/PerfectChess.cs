using System.Collections.Generic;
using UnityEngine;

namespace ChessGameModes {
    /// <summary>
    /// PerfectChess.cs is a chess variant where all rook+bishop+knight combinations occur exactly once.
    /// 
    /// Winstate: Checkmate.
    /// Piece types: Orthodox + Amazons, Empresses, Princesses.
    /// Board layout:
    ///     $ ^ q a k b n r
    ///     p p p p p p p p
    ///     . . . . . . . .
    ///     . . . . . . . .     $ = Empress
    ///     . . . . . . . .     ^ = Princess
    ///     . . . . . . . .
    ///     p p p p p p p p
    ///     $ ^ Q A K B N R
    /// </summary>
    public class PerfectChess : FIDERuleset {
        private Empress aSideWhiteEmpress;
        private Empress aSideBlackEmpress;

        public PerfectChess() : base() {
            aSideWhiteEmpress = aSideBlackEmpress = null;

            selectedPawnPromotion = Piece.Amazon;
            pawnPromotionOptions = new Piece[7] { Piece.Amazon, Piece.Queen, Piece.Empress, Piece.Princess, Piece.Rook, Piece.Bishop, Piece.Knight };
        }

        public override string ToString() {
            return "Perfect Chess";
        }

        public override void PopulateBoard() {
            currentRoyalPiece = (King)AddPieceToBoard(new King(Team.WHITE, "e1"));
            opposingRoyalPiece = (King)AddPieceToBoard(new King(Team.BLACK, "e8"));

            aSideWhiteEmpress = (Empress)AddPieceToBoard(new Empress(Team.WHITE, "a1"));
            aSideBlackEmpress = (Empress)AddPieceToBoard(new Empress(Team.BLACK, "a8"));
            hSideWhiteRook = (Rook)AddPieceToBoard(new Rook(Team.WHITE, "h1"));
            hSideBlackRook = (Rook)AddPieceToBoard(new Rook(Team.BLACK, "h8"));

            AddPieceToBoard(new Princess(Team.WHITE, "b1"));
            AddPieceToBoard(new Princess(Team.BLACK, "b8"));

            AddPieceToBoard(new Queen(Team.WHITE, "c1"));
            AddPieceToBoard(new Queen(Team.BLACK, "c8"));

            AddPieceToBoard(new Amazon(Team.WHITE, "d1"));
            AddPieceToBoard(new Amazon(Team.BLACK, "d8"));

            AddPieceToBoard(new Bishop(Team.WHITE, "f1"));
            AddPieceToBoard(new Bishop(Team.BLACK, "f8"));

            AddPieceToBoard(new Knight(Team.WHITE, "g1"));
            AddPieceToBoard(new Knight(Team.BLACK, "g8"));

            for (int x = 0; x < BOARD_WIDTH; x++) {
                AddPieceToBoard(new Pawn(Team.WHITE, new BoardCoord(x, WHITE_PAWNROW)));
                AddPieceToBoard(new Pawn(Team.BLACK, new BoardCoord(x, BLACK_PAWNROW)));
            }
        }

        protected override List<ChessPiece> GetAllPossibleCheckThreats(ChessPiece pieceToCheck) {
            List<ChessPiece> possibleCheckThreats = base.GetAllPossibleCheckThreats(pieceToCheck);

            GetPiecesOfType<Amazon>(pieceToCheck.GetOpposingTeam()).ForEach(x => { possibleCheckThreats.Add(x); });
            GetPiecesOfType<Empress>(pieceToCheck.GetOpposingTeam()).ForEach(x => { possibleCheckThreats.Add(x); });
            GetPiecesOfType<Princess>(pieceToCheck.GetOpposingTeam()).ForEach(x => { possibleCheckThreats.Add(x); });

            return possibleCheckThreats;
        }

        protected override void TryPerformCastlingRookMoves(ChessPiece mover) {
            if (mover.GetBoardPosition().x == 2) {
                if (mover.GetTeam() == Team.WHITE) {
                    aSideWhiteEmpress = (Empress)PerformCastle(aSideWhiteEmpress, new BoardCoord(3, mover.GetBoardPosition().y));
                } else {
                    aSideBlackEmpress = (Empress)PerformCastle(aSideBlackEmpress, new BoardCoord(3, mover.GetBoardPosition().y));
                }
            } else if (mover.GetBoardPosition().x == 6) {
                if (mover.GetTeam() == Team.WHITE) {
                    hSideWhiteRook = (Rook)PerformCastle(hSideWhiteRook, new BoardCoord(5, mover.GetBoardPosition().y));
                } else {
                    hSideBlackRook = (Rook)PerformCastle(hSideBlackRook, new BoardCoord(5, mover.GetBoardPosition().y));
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
                            if ((occupier is Rook || occupier is Empress) && occupier.MoveCount == 0) {
                                if (IsPieceInCheckAfterThisMove(king, king, king.GetBoardPosition() + new BoardCoord(i, 0)) == false
                                    && IsPieceInCheckAfterThisMove(king, king, king.GetBoardPosition() + new BoardCoord(i * 2, 0)) == false) {
                                    castleMoves.Add(TryGetSpecificMove(king, king.GetBoardPosition() + new BoardCoord(i * 2, 0)));
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
