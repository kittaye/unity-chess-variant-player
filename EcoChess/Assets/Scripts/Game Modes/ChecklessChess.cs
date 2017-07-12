﻿using System.Collections.Generic;
using UnityEngine;

namespace ChessGameModes {
    /// <summary>
    /// ChecklessChess.cs is a chess variant that prohibits checks against the king except checkmate.
    /// 
    /// Winstate: Checkmate.
    /// Piece types: Orthodox.
    /// Board layout: FIDE standard.
    /// </summary>
    public class ChecklessChess : FIDERuleset {
        private bool checkingForCheckmate;

        public ChecklessChess() : base() {
            checkingForCheckmate = false;
        }

        public override string ToString() {
            return "Checkless Chess";
        }

        public override void CalculateAvailableMoves(ChessPiece mover) {
            mover.ClearAvailableMoves();
            mover.ClearTemplateMoves();

            mover.CalculateTemplateMoves();
            BoardCoord[] templateMoves = mover.GetTemplateMoves();
            for (int i = 0; i < templateMoves.Length; i++) {
                if (IsPieceInCheckAfterThisMove(currentRoyalPiece, mover, templateMoves[i]) == false) {
                    if (IsPieceInCheckAfterThisMove(opposingRoyalPiece, mover, templateMoves[i]) == false) {
                        mover.AddToAvailableMoves(templateMoves[i]);
                    } else if (IsPieceInCheckMateAfterThisMove(opposingRoyalPiece, mover, templateMoves[i])){
                        //mover.AddToAvailableMoves(templateMoves[i]);
                    }
                }
            }

            if (mover is King) {
                AddAvailableCastleMoves(mover);
            } else if (mover is Pawn) {
                AddAvailableEnPassantMoves(mover);
            }
        }

        private bool IsPieceInCheckMateAfterThisMove(ChessPiece pieceToCheck, ChessPiece mover, BoardCoord dest) {
            if (AssertContainsCoord(dest)) {
                if (checkingForCheckmate) return false;

                // Temporarily simulate the move actually happening
                ChessPiece originalOccupier = board.GetCoordInfo(dest).occupier;
                ChessPiece originalLastMover;
                BoardCoord oldPos = mover.GetBoardPosition();
                SimulateMove(mover, dest, originalOccupier, out originalLastMover);

                ChessPiece occupier = null;
                if (mover is Pawn) {
                    occupier = CheckPawnEnPassantCapture((Pawn)mover);
                }

                // Check whether the piece is checkmated after this temporary move
                bool hasAnyMoves = false;
                checkingForCheckmate = true;
                foreach (ChessPiece piece in GetPieces(pieceToCheck.GetTeam())) {
                    if (piece.IsAlive) {
                        CalculateAvailableMoves(piece);
                        if (piece.GetAvailableMoves().Length > 0) hasAnyMoves = true;
                    }
                }
                checkingForCheckmate = false;

                if (occupier != null) {
                    board.GetCoordInfo(occupier.GetBoardPosition()).occupier = occupier;
                    occupier.IsAlive = true;
                    occupier.gameObject.SetActive(true);
                }

                // Revert the temporary move back to normal
                RevertSimulatedMove(mover, dest, originalOccupier, originalLastMover, oldPos);

                return hasAnyMoves;
            }
            return false;
        }
    }
}