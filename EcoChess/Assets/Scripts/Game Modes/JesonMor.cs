using UnityEngine;
using System.Collections.Generic;

namespace ChessGameModes {
    /// <summary>
    /// Board layout: 
    ///     n n n n n n n n n
    ///     . . . . . . . . .
    ///     . . . . . . . . .
    ///     . . . . . . . . .
    ///     . . . . . . . . .
    ///     . . . . . . . . .
    ///     . . . . . . . . .
    ///     . . . . . . . . .
    ///     N N N N N N N N N
    /// </summary>
    public class JesonMor : Chess {
        protected new const int BOARD_WIDTH = 9;
        protected new const int BOARD_HEIGHT = 9;

        private readonly BoardCoord centerSquare = new BoardCoord(4, 4);
        private bool gameFinished = false;

        public JesonMor() : base(BOARD_WIDTH, BOARD_HEIGHT) {
        }

        public override string ToString() {
            return "Jeson Mor";
        }

        public override VariantHelpDetails GetVariantHelpDetails() {
            return new VariantHelpDetails(
                this.ToString(),
                "Invented in Mongolia",
                this.ToString() + " is a variant on a 9x9 board with nine knights for both teams and an alternative win condition.",
                "Capture all of the opposing team's pieces OR move a knight off of the central e5 square.",
                "- Note: A knight on the e5 square may be captured.",
                "https://en.wikipedia.org/wiki/Jeson_Mor"
            );
        }

        public override void PopulateBoard() {
            for(int i = 0; i < BOARD_WIDTH; i++) {
                AddPieceToBoard(new Knight(Team.WHITE, new BoardCoord(i, 0)));
                AddPieceToBoard(new Knight(Team.BLACK, new BoardCoord(i, BOARD_HEIGHT - 1)));
            }
        }

        public override bool CheckWinState() {
            if (gameFinished) {
                UIManager.Instance.LogCustom("Team " + GetOpposingTeamTurn().ToString() + " has left e5 -- Team " + GetOpposingTeamTurn().ToString() + " wins!");
                return true;
            } else if (GetAlivePiecesOfType<ChessPiece>(GetCurrentTeamTurn()).Count == 0) {
                UIManager.Instance.LogCustom("Team " + GetOpposingTeamTurn().ToString() + " wins by elimination!");
                return true;
            }

            if (CapturelessMovesLimit()) {
                return true;
            }

            return false;
        }

        protected override bool IsPieceInCheckAfterThisMove(ChessPiece pieceToCheck, ChessPiece mover, BoardCoord destination) {
            return false;
        }

        public override List<BoardCoord> CalculateAvailableMoves(ChessPiece mover) {
            return mover.CalculateTemplateMoves();
        }

        public override bool MovePiece(ChessPiece mover, BoardCoord destination) {
            BoardCoord oldPos = mover.GetBoardPosition();

            string moveNotation = MakeDirectMove(mover, destination);
            if (moveNotation != null) {
                if (oldPos == centerSquare) {
                    gameFinished = true;
                }
                GameMoveNotations.Push(moveNotation);
                return true;
            }
            return false;
        }
    }
}