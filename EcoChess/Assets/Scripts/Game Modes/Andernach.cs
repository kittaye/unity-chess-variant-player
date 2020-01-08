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

        public override VariantHelpDetails GetVariantHelpDetails() {
            return new VariantHelpDetails(
                this.ToString(),
                "Created in 1993",
                this.ToString() + " is a variant where pieces change teams upon capture, except for the king.",
                "Checkmate.",
                "None.",
                "https://en.wikipedia.org/wiki/Andernach_chess"
            );
        }

        public override bool MovePiece(ChessPiece mover, BoardCoord destination) {
            int currentCaptures = mover.CaptureCount;

            if(base.MovePiece(mover, destination)) {
                if (mover != currentRoyalPiece && mover.CaptureCount != currentCaptures) {
                    KillPiece(mover);
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
