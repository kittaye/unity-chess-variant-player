using System.Collections.Generic;
using UnityEngine;

namespace ChessGameModes {
    /// <summary>
    /// Atomic.cs is a chess variant that allows pieces to explode on capture.
    /// 
    /// Winstate: Checkmate OR King indirect capture.
    /// Piece types: Orthodox.
    /// Board layout: FIDE standard.
    /// </summary>
    public class Atomic : Chess {
        public Atomic() : base() { }

        public override string ToString() {
            return "Atomic Chess";
        }

        public override bool CheckWinState() {
            if (currentRoyalPiece.IsAlive == false) {
                UIManager.Instance.LogCustom("Team " + GetCurrentTeamTurn().ToString() + "'s king has been captured -- Team " + GetOpposingTeamTurn().ToString() + " wins!");
                return true;
            }

            return base.CheckWinState();
        }

        public override List<BoardCoord> CalculateAvailableMoves(ChessPiece mover) {
            BoardCoord[] templateMoves = mover.CalculateTemplateMoves().ToArray();
            List<BoardCoord> availableMoves = new List<BoardCoord>(templateMoves.Length);

            for (int i = 0; i < templateMoves.Length; i++) {
                if (IsPieceInCheckAfterThisMove(currentRoyalPiece, mover, templateMoves[i])) {
                    continue;
                }
                if (IsThreat(mover, templateMoves[i]) == false) {
                    availableMoves.Add(templateMoves[i]);
                    continue;
                }

                bool isValid = true;
                for (int x = -1; x <= 1 && isValid; x++) {
                    for (int y = -1; y <= 1; y++) {
                        BoardCoord coord = Board.GetCoordInfo(templateMoves[i]).occupier.GetRelativeBoardCoord(x, y);
                        if (Board.ContainsCoord(coord) && (x != 0 && y != 0) && Board.GetCoordInfo(coord).occupier == currentRoyalPiece) {
                            isValid = false;
                            break;
                        }
                    }
                }
                if (isValid) availableMoves.Add(templateMoves[i]);
            }

            if (mover is King && mover.MoveCount == 0) {
                availableMoves.AddRange(TryAddAvailableCastleMoves(mover, CastlerOptions));
            } else if (mover is Pawn) {
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

        public override bool MovePiece(ChessPiece mover, BoardCoord destination) {
            BoardCoord oldPos = mover.GetBoardPosition();
            bool pieceCaptured = IsThreat(mover, destination);

            if (MakeMove(mover, destination)) {
                if (pieceCaptured) {
                    for (int x = -1; x <= 1; x++) {
                        for (int y = -1; y <= 1; y++) {
                            BoardCoord coord = mover.GetRelativeBoardCoord(x, y);
                            if (Board.ContainsCoord(coord) && ((Board.GetCoordInfo(coord).occupier is Pawn) == false) && coord != mover.GetBoardPosition()) {
                                RemovePieceFromBoard(Board.GetCoordInfo(coord).occupier);
                            }
                        }
                    }
                    RemovePieceFromBoard(mover);
                } else {
                    if (mover == currentRoyalPiece && mover.MoveCount == 1) {
                        TryPerformCastlingRookMoves((King)mover);
                    } else if (mover is Pawn) {
                        ((Pawn)mover).validEnPassant = (mover.MoveCount == 1 && mover.GetRelativeBoardCoord(0, -1) != oldPos);
                        CheckPawnEnPassantCapture((Pawn)mover);
                        CheckPawnPromotion((Pawn)mover);
                    }
                }
                return true;
            }
            return false;
        }

        protected override bool IsPieceInCheck(ChessPiece king) {
            if (checkingForCheck) return false;

            opposingTeamCheckThreats = GetAllPossibleCheckThreats(king);

            checkingForCheck = true;
            foreach (ChessPiece piece in opposingTeamCheckThreats) {
                if (piece.IsAlive) {
                    if ((piece is King) == false && CalculateAvailableMoves(piece).Contains(king.GetBoardPosition())) {
                        checkingForCheck = false;
                        return true;
                    }
                }
            }
            checkingForCheck = false;
            return false;
        }

        protected override BoardCoord TryAddAvailableEnPassantMove(Pawn mover) {
            const int LEFT = -1;
            const int RIGHT = 1;

            if (mover.canEnPassantCapture) {
                for (int i = LEFT; i <= RIGHT; i += 2) {
                    BoardCoord coord = TryGetSpecificMove(mover, mover.GetRelativeBoardCoord(i, 0), threatOnly: true);
                    if (Board.ContainsCoord(coord)) {
                        ChessPiece piece = Board.GetCoordInfo(coord).occupier;
                        if (piece is Pawn && piece == GetLastMovedOpposingPiece(mover) && ((Pawn)piece).validEnPassant) {
                            if (IsPieceInCheckAfterThisMove(currentRoyalPiece, mover, mover.GetRelativeBoardCoord(i, 1)) == false) {
                                bool isValid = true;
                                for (int x = -1; x <= 1 && isValid; x++) {
                                    for (int y = -1; y <= 1; y++) {
                                        BoardCoord coord2 = piece.GetRelativeBoardCoord(x, y);
                                        if (Board.ContainsCoord(coord2) && (x != 0 && y != 0) && Board.GetCoordInfo(coord2).occupier == currentRoyalPiece) {
                                            isValid = false;
                                            break;
                                        }
                                    }
                                }
                                if (isValid) return TryGetSpecificMove(mover, mover.GetRelativeBoardCoord(i, 1));
                            }
                        }
                    }
                }
            }
            return BoardCoord.NULL;
        }
    }
}
