﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace ChessGameModes {
    /// <summary>
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
                "Invented in 1993",
                this.ToString() + " is a variant where pieces change teams upon capture, except for the king.",
                "Checkmate.",
                VariantHelpDetails.rule_None,
                "https://en.wikipedia.org/wiki/Andernach_chess"
            );
        }

        public override bool MovePiece(ChessPiece mover, BoardCoord destination) {
            int currentCaptures = mover.CaptureCount;

            if(base.MovePiece(mover, destination)) {
                if (mover != currentRoyalPiece && mover.CaptureCount != currentCaptures) {
                    KillPiece(mover);
                    RemovePieceFromTeam(mover);
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
