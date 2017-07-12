using System;
using UnityEngine;

namespace ChessGameModes {
    /// <summary>
    /// Atomic.cs is a chess variant that allows pieces to explode on capture.
    /// 
    /// Winstate: Checkmate OR King indirect capture.
    /// Piece types: Orthodox.
    /// Board layout: FIDE standard.
    /// </summary>
    public class Atomic : FIDERuleset {
        public Atomic() : base() { }

        public override string ToString() {
            return "Atomic";
        }

        public override bool CheckWinState() {
            if (numConsecutiveCapturelessMoves == 100) {
                Debug.Log("No captures or pawn moves in 50 turns. Stalemate on " + GetCurrentTeamTurn().ToString() + "'s move!");
                return true;
            }

            if (currentRoyalPiece.IsAlive == false) {
                Debug.Log("Team " + GetCurrentTeamTurn().ToString() + "'s king has been captured -- Team " + GetOpposingTeamTurn().ToString() + " wins!");
                return true;
            }

            foreach (ChessPiece piece in GetPieces(GetCurrentTeamTurn())) {
                if (piece.IsAlive) {
                    CalculateAvailableMoves(piece);
                    if (piece.GetAvailableMoves().Length > 0) return false;
                }
            }

            if (IsPieceInCheck(currentRoyalPiece)) {
                Debug.Log("Team " + GetCurrentTeamTurn().ToString() + " has been checkmated -- Team " + GetOpposingTeamTurn().ToString() + " wins!");
            } else {
                Debug.Log("Stalemate on " + GetCurrentTeamTurn().ToString() + "'s move!");
            }
            return true;
        }

        public override void CalculateAvailableMoves(ChessPiece mover) {
            mover.ClearAvailableMoves();
            mover.ClearTemplateMoves();

            mover.CalculateTemplateMoves();
            for (int i = 0; i < mover.GetTemplateMoves().Length; i++) {
                if (IsPieceInCheckAfterThisMove(currentRoyalPiece, mover, mover.GetTemplateMoves()[i])) {
                    continue;
                }
                if (IsThreat(mover, mover.GetTemplateMoves()[i]) == false) {
                    mover.AddToAvailableMoves(mover.GetTemplateMoves()[i]);
                    continue;
                }

                bool isValid = true;
                for (int x = -1; x <= 1 && isValid; x++) {
                    for (int y = -1; y <= 1; y++) {
                        BoardCoord coord = board.GetCoordInfo(mover.GetTemplateMoves()[i]).occupier.GetRelativeBoardCoord(x, y);
                        if (board.ContainsCoord(coord) && (x != 0 && y != 0) && board.GetCoordInfo(coord).occupier == currentRoyalPiece) {
                            isValid = false;
                            break;
                        }
                    }
                }
                if (isValid) mover.AddToAvailableMoves(mover.GetTemplateMoves()[i]);
            }

            if (mover is King) {
                AddAvailableCastleMoves((King)mover);
            } else if (mover is Pawn) {
                AddAvailableEnPassantMoves(mover);
            }
        }

        public override bool MovePiece(ChessPiece mover, BoardCoord destination) {
            BoardCoord oldPos = mover.GetBoardPosition();
            bool pieceCaptured = IsThreat(mover, destination);

            if (MakeMove(mover, destination)) {
                if (pieceCaptured) {
                    for (int x = -1; x <= 1; x++) {
                        for (int y = -1; y <= 1; y++) {
                            BoardCoord coord = mover.GetRelativeBoardCoord(x, y);
                            if (board.ContainsCoord(coord) && ((board.GetCoordInfo(coord).occupier is Pawn) == false) && coord != mover.GetBoardPosition()) {
                                RemovePieceFromBoard(board.GetCoordInfo(coord).occupier);
                            }
                        }
                    }
                    RemovePieceFromBoard(mover);
                } else {
                    if (mover is King && mover.MoveCount == 1) {
                        TryPerformCastlingRookMoves((King)mover);
                    } else if (mover is Pawn) {
                        ((Pawn)mover).validEnPassant = (mover.MoveCount == 1 && mover.GetRelativeBoardCoord(0, -2) == oldPos);
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

            foreach (ChessPiece piece in GetPieces(GetOpposingTeamTurn())) {
                if (piece.IsAlive) {
                    CalculateAvailableMoves(piece);
                    if (piece.CanMoveTo(king.GetBoardPosition()) && (piece is King) == false) {
                        return true;
                    }
                }
            }
            return false;
        }

        protected override void AddAvailableEnPassantMoves(ChessPiece mover) {
            const int LEFT = -1;
            const int RIGHT = 1;

            if (mover is Pawn && ((Pawn)mover).canEnPassantCapture) {
                for (int i = LEFT; i <= RIGHT; i += 2) {
                    BoardCoord coord = TryGetSpecificMove(mover, mover.GetRelativeBoardCoord(i, 0), threatOnly: true);
                    if (board.ContainsCoord(coord)) {
                        ChessPiece piece = board.GetCoordInfo(coord).occupier;
                        if (piece is Pawn && piece == lastMovedPiece && ((Pawn)piece).validEnPassant) {
                            ((Pawn)mover).enPassantTargets.Add((Pawn)piece);
                            if (IsPieceInCheckAfterThisMove(currentRoyalPiece, mover, mover.GetRelativeBoardCoord(i, 1)) == false) {
                                bool isValid = true;
                                for (int x = -1; x <= 1 && isValid; x++) {
                                    for (int y = -1; y <= 1; y++) {
                                        BoardCoord coord2 = board.GetCoordInfo(mover.GetTemplateMoves()[i]).occupier.GetRelativeBoardCoord(x, y);
                                        if (board.ContainsCoord(coord2) && (x != 0 && y != 0) && board.GetCoordInfo(coord2).occupier == currentRoyalPiece) {
                                            isValid = false;
                                            break;
                                        }
                                    }
                                }
                                if (isValid) mover.AddToAvailableMoves(TryGetSpecificMove(mover, mover.GetRelativeBoardCoord(i, 1)));
                            }
                        }
                    }
                }
            }
        }
    }
}
