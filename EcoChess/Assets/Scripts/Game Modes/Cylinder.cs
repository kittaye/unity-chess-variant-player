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

            for (int i = (int)MoveDirection.Up; i <= (int)MoveDirection.DownRight; i++) {
                int xModifier, yModifier;
                GetMoveDirectionModifiers(pieceToCheck, (MoveDirection)i, out xModifier, out yModifier);
                BoardCoord coord = pieceToCheck.GetBoardPosition() + new BoardCoord(xModifier, yModifier);

                int failsafe = 0;
                while (pieceToCheck.GetBoardPosition() != coord || failsafe < 9) {
                    if (Board.ContainsCoord(coord) == false) break;

                    if (IsThreat(pieceToCheck, coord)) {
                        possibleCheckThreats.Add(Board.GetCoordInfo(coord).occupier);
                    }
                    coord.x += xModifier;
                    coord.y += yModifier;
                    coord.x = MathExtensions.mod(coord.x, BOARD_WIDTH);
                    failsafe++;
                }
            }

            foreach (Knight knight in GetAllPiecesOfType<Knight>()) {
                if (IsThreat(pieceToCheck, knight.GetBoardPosition())) {
                    possibleCheckThreats.Add(knight);
                }
            }

            return possibleCheckThreats;
        }

        protected override BoardCoord[] TryAddAvailableEnPassantMoves(Pawn mover) {
            const int LEFT = -1;
            const int RIGHT = 1;
            List<BoardCoord> enpassantMoves = new List<BoardCoord>(1);

            if (mover.canEnPassantCapture) {
                for (int i = LEFT; i <= RIGHT; i += 2) {
                    int modulusRelativeX = MathExtensions.mod(i, BOARD_WIDTH);
                    BoardCoord coord = TryGetSpecificMove(mover, mover.GetRelativeBoardCoord(modulusRelativeX, 0), threatOnly: true);
                    if (Board.ContainsCoord(coord)) {
                        ChessPiece piece = Board.GetCoordInfo(coord).occupier;
                        if (piece is Pawn && piece == GetLastMovedOpposingPiece(mover) && ((Pawn)piece).validEnPassant) {
                            if (IsPieceInCheckAfterThisMove(currentRoyalPiece, mover, mover.GetRelativeBoardCoord(modulusRelativeX, 1)) == false) {
                                enpassantMoves.Add(TryGetSpecificMove(mover, mover.GetRelativeBoardCoord(modulusRelativeX, 1)));
                            }
                        }
                    }
                }
            }
            return enpassantMoves.ToArray();
        }
    }
}
