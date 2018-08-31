using System;
using System.Collections.Generic;
using UnityEngine;

namespace ChessGameModes {
    /// <summary>
    /// LosingChess.cs is a chess variant that makes captures compulsory.
    /// 
    /// Winstate: Lose all pieces or be stalemated.
    /// Piece types: Orthodox.
    /// Piece rules: no castling, kings have no royalty, pawns may promote to kings.
    /// Board layout: FIDE standard.
    /// </summary>
    public class LosingChess : Chess {
        private bool canCaptureThisTurn;

        public LosingChess() : base(BOARD_WIDTH, BOARD_HEIGHT) {
            canCaptureThisTurn = false;
            AllowCastling = false;
        }

        public override string ToString() {
            return "Losing Chess";
        }

        public override bool CheckWinState() {
            if (GetPieces(GetCurrentTeamTurn()).TrueForAll(x => x.IsAlive == false)) {
                UIManager.Instance.LogCustom("Team " + GetCurrentTeamTurn().ToString() + " has lost all pieces -- Team " + GetCurrentTeamTurn().ToString() + " wins!");
                return true;
            }

            canCaptureThisTurn = CanCaptureAPiece();

            if (canCaptureThisTurn) {
                return false;
            }

            if (!TeamHasAnyMoves(GetCurrentTeamTurn())) {
                UIManager.Instance.LogStalemate(GetCurrentTeamTurn().ToString());
                return true;
            }

            if (CapturelessMovesLimit()) {
                return true;
            }

            return false;
        }

        public override List<BoardCoord> CalculateAvailableMoves(ChessPiece mover) {
            BoardCoord[] templateMoves = mover.CalculateTemplateMoves().ToArray();
            List<BoardCoord> availableMoves = new List<BoardCoord>(templateMoves.Length);

            if (canCaptureThisTurn) {
                for (int i = 0; i < templateMoves.Length; i++) {
                    if (IsThreat(mover, templateMoves[i])) {
                        availableMoves.Add(templateMoves[i]);
                    }
                }
            } else {
                availableMoves.AddRange(templateMoves);
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

        private bool CanCaptureAPiece() {
            foreach (ChessPiece piece in GetPieces(GetCurrentTeamTurn())) {
                if (piece.IsAlive) {
                    BoardCoord[] templateMoves = piece.CalculateTemplateMoves().ToArray();
                    for (int i = 0; i < templateMoves.Length; i++) {
                        if (IsThreat(piece, templateMoves[i])) {
                            return true;
                        }
                    }
                }
            }
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
                            return TryGetSpecificMove(mover, mover.GetRelativeBoardCoord(i, 1));
                        }
                    }
                }
            }
            return BoardCoord.NULL;
        }
    }
}