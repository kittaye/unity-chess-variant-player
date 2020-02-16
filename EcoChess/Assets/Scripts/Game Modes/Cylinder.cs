using System.Collections.Generic;
using UnityEngine;

namespace ChessGameModes {
    /// <summary>
    /// Board layout: FIDE standard.
    /// </summary>
    public class Cylinder : Chess {

        public Cylinder() : base() {
            AllowCastling = false;
        }

        public override string ToString() {
            return "Cylinder Chess";
        }

        public override VariantHelpDetails GetVariantHelpDetails() {
            return new VariantHelpDetails(
                this.ToString(),
                "Invented in the 1800s",
                this.ToString() + " is a variant where the board wraps around horizontally, allowing pieces to travel from one side to the other.",
                "Checkmate.",
                "- No null moves (where a piece's destination is the same as their current position).\n" + 
                VariantHelpDetails.rule_NoCastling,
                "https://en.wikipedia.org/wiki/Cylinder_chess"
            );
        }

        public override void PopulateBoard() {
            currentRoyalPiece = (King)AddPieceToBoard(new King(Team.WHITE, new BoardCoord(4, WHITE_BACKROW), true, false));
            opposingRoyalPiece = (King)AddPieceToBoard(new King(Team.BLACK, new BoardCoord(4, BLACK_BACKROW), true, false));

            AddPieceToBoard(new Rook(Team.WHITE, new BoardCoord(0, WHITE_BACKROW), true, false));
            AddPieceToBoard(new Rook(Team.BLACK, new BoardCoord(0, BLACK_BACKROW), true, false));
            AddPieceToBoard(new Rook(Team.WHITE, new BoardCoord(7, WHITE_BACKROW), true, false));
            AddPieceToBoard(new Rook(Team.BLACK, new BoardCoord(7, BLACK_BACKROW), true, false));

            AddPieceToBoard(new Queen(Team.WHITE, new BoardCoord(3, WHITE_BACKROW), true, false));
            AddPieceToBoard(new Queen(Team.BLACK, new BoardCoord(3, BLACK_BACKROW), true, false));

            for (int x = 0; x < BOARD_WIDTH; x++) {
                AddPieceToBoard(new Pawn(Team.WHITE, new BoardCoord(x, WHITE_PAWNROW), true, false));
                AddPieceToBoard(new Pawn(Team.BLACK, new BoardCoord(x, BLACK_PAWNROW), true, false));

                if (x == 1 || x == BOARD_WIDTH - 2) {
                    AddPieceToBoard(new Knight(Team.WHITE, new BoardCoord(x, WHITE_BACKROW), true, false));
                    AddPieceToBoard(new Knight(Team.BLACK, new BoardCoord(x, BLACK_BACKROW), true, false));
                } else if (x == 2 || x == BOARD_WIDTH - 3) {
                    AddPieceToBoard(new Bishop(Team.WHITE, new BoardCoord(x, WHITE_BACKROW), true, false));
                    AddPieceToBoard(new Bishop(Team.BLACK, new BoardCoord(x, BLACK_BACKROW), true, false));
                }
            }
        }

        protected override List<ChessPiece> GetAllPossibleCheckThreats(ChessPiece pieceToCheck) {
            List<ChessPiece> possibleCheckThreats = new List<ChessPiece>();

            for (int i = 0; i <= 7; i++) {
                BoardCoord coordStep = GetCoordStepInDirection(pieceToCheck, (MoveDirection)i, true);
                BoardCoord coord = pieceToCheck.GetBoardPosition() + coordStep;

                int failsafe = 0;
                while (pieceToCheck.GetBoardPosition() != coord || failsafe < 9) {
                    if (Board.ContainsCoord(coord) == false) break;

                    if (IsThreat(pieceToCheck, coord)) {
                        possibleCheckThreats.Add(Board.GetCoordInfo(coord).GetAliveOccupier());
                    }
                    coord += coordStep;
                    coord.x = MathExtensions.mod(coord.x, BOARD_WIDTH);
                    failsafe++;
                }
            }

            possibleCheckThreats.AddRange(GetAlivePiecesOfType<Knight>(pieceToCheck.GetOpposingTeam()));

            return possibleCheckThreats;
        }

        protected override BoardCoord[] TryAddAvailableEnPassantMoves(Pawn mover) {
            const int LEFT = -1;
            const int RIGHT = 1;
            List<BoardCoord> enpassantMoves = new List<BoardCoord>(1);

            if (mover.canEnPassantCapture) {
                for (int i = LEFT; i <= RIGHT; i += 2) {
                    int modulusRelativeX = MathExtensions.mod(i, BOARD_WIDTH);
                    BoardCoord sidewaysCoord = mover.GetRelativeBoardCoord(modulusRelativeX, 0);

                    if (Board.ContainsCoord(sidewaysCoord) && IsThreat(mover, sidewaysCoord)) {
                        ChessPiece piece = Board.GetCoordInfo(sidewaysCoord).GetAliveOccupier();

                        if (piece is Pawn && CheckEnPassantVulnerability((Pawn)piece)) {
                            BoardCoord enpassantCoord = mover.GetRelativeBoardCoord(modulusRelativeX, 1);

                            if (Board.ContainsCoord(enpassantCoord)) {
                                if (IsPieceInCheckAfterThisMove(currentRoyalPiece, mover, enpassantCoord) == false) {
                                    enpassantMoves.Add(enpassantCoord);
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
