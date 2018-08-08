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
            if (numConsecutiveCapturelessMoves == 100) {
                UIManager.Instance.Log("No captures or pawn moves in 50 turns. Stalemate on " + GetCurrentTeamTurn().ToString() + "'s move!");
                return true;
            }

            if (currentRoyalPiece.IsAlive == false) {
                UIManager.Instance.Log("Team " + GetCurrentTeamTurn().ToString() + "'s king has been captured -- Team " + GetOpposingTeamTurn().ToString() + " wins!");
                return true;
            }

            foreach (ChessPiece piece in GetPieces(GetCurrentTeamTurn())) {
                if (piece.IsAlive) {
                    if (CalculateAvailableMoves(piece).Count > 0) return false;
                }
            }

            if (IsPieceInCheck(currentRoyalPiece)) {
                UIManager.Instance.Log("Team " + GetCurrentTeamTurn().ToString() + " has been checkmated -- Team " + GetOpposingTeamTurn().ToString() + " wins!");
            } else {
                UIManager.Instance.Log("Stalemate on " + GetCurrentTeamTurn().ToString() + "'s move!");
            }
            return true;
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
                        BoardCoord coord = board.GetCoordInfo(templateMoves[i]).occupier.GetRelativeBoardCoord(x, y);
                        if (board.ContainsCoord(coord) && (x != 0 && y != 0) && board.GetCoordInfo(coord).occupier == currentRoyalPiece) {
                            isValid = false;
                            break;
                        }
                    }
                }
                if (isValid) availableMoves.Add(templateMoves[i]);
            }

            if (mover is King && mover.MoveCount == 0) {
                availableMoves.AddRange(TryAddAvailableCastleMoves((King)mover));
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
                    if (board.ContainsCoord(coord)) {
                        ChessPiece piece = board.GetCoordInfo(coord).occupier;
                        if (piece is Pawn && piece == LastMovedOpposingPiece(mover) && ((Pawn)piece).validEnPassant) {
                            if (IsPieceInCheckAfterThisMove(currentRoyalPiece, mover, mover.GetRelativeBoardCoord(i, 1)) == false) {
                                bool isValid = true;
                                for (int x = -1; x <= 1 && isValid; x++) {
                                    for (int y = -1; y <= 1; y++) {
                                        BoardCoord coord2 = piece.GetRelativeBoardCoord(x, y);
                                        if (board.ContainsCoord(coord2) && (x != 0 && y != 0) && board.GetCoordInfo(coord2).occupier == currentRoyalPiece) {
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
