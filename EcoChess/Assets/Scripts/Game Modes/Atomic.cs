﻿using System.Collections.Generic;
using UnityEngine;

namespace ChessGameModes {
    /// <summary>
    /// Board layout: FIDE standard.
    /// </summary>
    public class Atomic : Chess {
        public Atomic() : base() { }

        public override string ToString() {
            return "Atomic Chess";
        }

        public override VariantHelpDetails GetVariantHelpDetails() {
            return new VariantHelpDetails(
                this.ToString(),
                "Invented in 1995",
                this.ToString() + " is a variant where pieces explode on capture, removing all surrounding pieces in a 3x3 grid except for pawns.",
                "Checkmate, or king indirect capture.",
                VariantHelpDetails.rule_None,
                "https://en.wikipedia.org/wiki/Atomic_chess"
            );
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
                        BoardCoord coord = Board.GetCoordInfo(templateMoves[i]).GetAliveOccupier().GetRelativeBoardCoord(x, y);
                        if (Board.ContainsCoord(coord) && (x != 0 && y != 0) && Board.GetCoordInfo(coord).GetAliveOccupier() == currentRoyalPiece) {
                            isValid = false;
                            break;
                        }
                    }
                }
                if (isValid) availableMoves.Add(templateMoves[i]);
            }

            if (IsRoyal(mover)) {
                availableMoves.AddRange(TryAddAvailableCastleMoves(mover, CastlerOptions));
            } else if (mover is Pawn) {
                availableMoves.AddRange(TryAddAvailableEnPassantMoves((Pawn)mover));
            }

            return availableMoves;
        }

        public override bool MovePiece(ChessPiece mover, BoardCoord destination) {
            bool pieceCaptured = IsThreat(mover, destination);

            // Try make the move.
            if (MakeDirectMove(mover, destination)) {
                if (pieceCaptured) {
                    for (int x = -1; x <= 1; x++) {
                        for (int y = -1; y <= 1; y++) {
                            BoardCoord coord = mover.GetRelativeBoardCoord(x, y);
                            if (Board.ContainsCoord(coord) && ((Board.GetCoordInfo(coord).GetAliveOccupier() is Pawn) == false) && coord != mover.GetBoardPosition()) {
                                KillPiece(Board.GetCoordInfo(coord).GetAliveOccupier());
                            }
                        }
                    }
                    KillPiece(mover);
                } else {
                    if (IsRoyal(mover)) {
                        TryPerformCastlingMove(mover);
                    } else if (mover is Pawn) {
                        TryPerformPawnEnPassantCapture((Pawn)mover);
                        TryPerformPawnPromotion((Pawn)mover);
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

        protected override BoardCoord[] TryAddAvailableEnPassantMoves(Pawn mover) {
            const int LEFT = -1;
            const int RIGHT = 1;
            List<BoardCoord> enpassantMoves = new List<BoardCoord>(1);

            if (mover.canEnPassantCapture) {
                for (int i = LEFT; i <= RIGHT; i += 2) {
                    BoardCoord coord = TryGetSpecificMove(mover, mover.GetRelativeBoardCoord(i, 0), threatOnly: true);
                    if (Board.ContainsCoord(coord)) {
                        ChessPiece piece = Board.GetCoordInfo(coord).GetAliveOccupier();
                        if (piece is Pawn && CheckEnPassantVulnerability((Pawn)piece)) {
                            if (IsPieceInCheckAfterThisMove(currentRoyalPiece, mover, mover.GetRelativeBoardCoord(i, 1)) == false) {
                                bool isValid = true;
                                for (int x = -1; x <= 1 && isValid; x++) {
                                    for (int y = -1; y <= 1; y++) {
                                        BoardCoord coord2 = piece.GetRelativeBoardCoord(x, y);
                                        if (Board.ContainsCoord(coord2) && (x != 0 && y != 0) && Board.GetCoordInfo(coord2).GetAliveOccupier() == currentRoyalPiece) {
                                            isValid = false;
                                            break;
                                        }
                                    }
                                }
                                if (isValid) {
                                    enpassantMoves.Add(TryGetSpecificMove(mover, mover.GetRelativeBoardCoord(i, 1)));
                                }
                            }
                        }
                    }
                }
            }
            return enpassantMoves.ToArray();
        }
    }
}
