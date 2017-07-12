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
                        CalculateAvailableMoves(piece);
                        if (piece.GetAvailableMoves().Length > 0) {
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

        public override void CalculateAvailableMoves(ChessPiece mover) {
            mover.ClearAvailableMoves();
            mover.ClearTemplateMoves();

            mover.CalculateTemplateMoves();

            if (canCaptureThisTurn) {
                BoardCoord[] templateMoves = mover.GetTemplateMoves();
                for (int i = 0; i < templateMoves.Length; i++) {
                    if (IsThreat(mover, templateMoves[i])) {
                        mover.AddToAvailableMoves(templateMoves[i]);
                    }
                }
            } else {
                mover.AddToAvailableMoves(mover.GetTemplateMoves());
            }

            if (mover is Pawn) {
                AddAvailableEnPassantMoves(mover);
            }
        }

        private bool CanCaptureAPiece() {
            foreach (ChessPiece piece in GetPieces(currentTeamTurn)) {
                if (piece.IsAlive) {
                    piece.ClearTemplateMoves();
                    piece.CalculateTemplateMoves();
                    BoardCoord[] templateMoves = piece.GetTemplateMoves();

                    for (int i = 0; i < templateMoves.Length; i++) {
                        if (IsThreat(piece, templateMoves[i])) {
                            return true;
                        }
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
                            mover.AddToAvailableMoves(TryGetSpecificMove(mover, mover.GetRelativeBoardCoord(i, 1)));
                        }
                    }
                }
            }
        }
    }
}