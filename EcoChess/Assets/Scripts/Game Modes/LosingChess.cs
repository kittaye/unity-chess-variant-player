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
    public class LosingChess : FIDERuleset {
        private bool canCaptureThisTurn;

        public LosingChess() : base(BOARD_WIDTH, BOARD_HEIGHT) {
            canCaptureThisTurn = false;
        }

        public override string ToString() {
            return "Losing Chess";
        }

        public override bool CheckWinState() {
            if (numConsecutiveCapturelessMoves == 100) {
                Debug.Log("No captures or pawn moves in 50 turns. Stalemate on " + GetCurrentTeamTurn().ToString() + "'s move!");
                return true;
            }

            if(GetPieces(GetCurrentTeamTurn()).TrueForAll(x => x.IsAlive == false)) {
                Debug.Log("Team " + GetCurrentTeamTurn().ToString() + " has lost all pieces -- Team " + GetCurrentTeamTurn().ToString() + " wins!");
                return true;
            }

            canCaptureThisTurn = CanCaptureAPiece();

            if (canCaptureThisTurn) {
                return false;
            } else {
                foreach (ChessPiece piece in GetPieces(currentTeamTurn)) {
                    if (piece.IsAlive) {
                        if (CalculateAvailableMoves(piece).Count > 0) {
                            return false;
                        }
                    }
                }
            }

            Debug.Log("Stalemate on " + GetCurrentTeamTurn().ToString() + "'s move -- Team " + GetCurrentTeamTurn().ToString() + " wins!");
            return true;
        }

        public override bool MovePiece(ChessPiece mover, BoardCoord destination) {
            BoardCoord oldPos = mover.GetBoardPosition();

            // Try make the move
            if (MakeMove(mover, destination)) {
                if (mover is Pawn) {
                    ((Pawn)mover).validEnPassant = (mover.MoveCount == 1 && mover.GetRelativeBoardCoord(0, -2) == oldPos);
                    CheckPawnEnPassantCapture((Pawn)mover);
                    CheckPawnPromotion((Pawn)mover);
                }
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
                BoardCoord enPassantMove = TryAddAvailableEnPassantMove(mover);
                if (enPassantMove != BoardCoord.NULL) {
                    availableMoves.Add(enPassantMove);
                }
            }
            return availableMoves;
        }

        private bool CanCaptureAPiece() {
            foreach (ChessPiece piece in GetPieces(currentTeamTurn)) {
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

        protected override BoardCoord TryAddAvailableEnPassantMove(ChessPiece mover) {
            const int LEFT = -1;
            const int RIGHT = 1;

            if (mover is Pawn && ((Pawn)mover).canEnPassantCapture) {
                for (int i = LEFT; i <= RIGHT; i += 2) {
                    BoardCoord coord = TryGetSpecificMove(mover, mover.GetRelativeBoardCoord(i, 0), threatOnly: true);
                    if (board.ContainsCoord(coord)) {
                        ChessPiece piece = board.GetCoordInfo(coord).occupier;
                        if (piece is Pawn && piece == lastMovedPiece && ((Pawn)piece).validEnPassant) {
                            ((Pawn)mover).enPassantTargets.Add((Pawn)piece);
                            return TryGetSpecificMove(mover, mover.GetRelativeBoardCoord(i, 1));
                        }
                    }
                }
            }
            return BoardCoord.NULL;
        }
    }
}