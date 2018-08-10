using System;
using System.Collections.Generic;
using UnityEngine;

namespace ChessGameModes {
    /// <summary>
    /// Andernach.cs is a chess variant where pieces change teams upon capture.
    /// 
    /// Winstate: Checkmate.
    /// Piece types: Orthodox.
    /// Board layout: FIDE standard.
    /// </summary>
    public class Andernach : Chess {
        public Andernach() : base() {
        }

        public override string ToString() {
            return "Andernach Chess";
        }

        public override bool MovePiece(ChessPiece mover, BoardCoord destination) {
            int currentCaptures = mover.CaptureCount;

            if(base.MovePiece(mover, destination)) {
                if (((mover is King) == false) && mover.CaptureCount != currentCaptures) {
                    RemovePieceFromBoard(mover);
                    RemovePieceFromActiveTeam(mover);
                    if(mover is Pawn) {
                        AddPieceToBoard(new Pawn(mover.GetOpposingTeam(), destination, initialMoveLimit: 1));
                    } else {
                        AddPieceToBoard(ChessPieceFactory.Create(mover.GetPieceType(), mover.GetOpposingTeam(), destination));
                    }
                }
                return true;
            }
            return false;
        }
    }
}
