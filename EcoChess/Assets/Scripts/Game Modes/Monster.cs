
namespace ChessGameModes {
    /// <summary>
    /// Board layout:
    ///     r n b q k b n r
    ///     p p p p p p p p
    ///     . . . . . . . .
    ///     . . . . . . . .
    ///     . . . . . . . .
    ///     . . . . . . . .
    ///     . . p p p p . .
    ///     . . . . K . . .
    /// </summary>
    public class Monster : Chess {
        private bool isWhiteSecondMove = false;

        public Monster() : base() {
            NotationTurnDivider = 3;
        }

        public override string ToString() {
            return "Monster Chess";
        }

        public override VariantHelpDetails GetVariantHelpDetails() {
            return new VariantHelpDetails(
                this.ToString(),
                "Invented by ???",
                this.ToString() + " is a variant where white has only 4 pawns and a king, but can move twice per turn.",
                "Checkmate.",
                "- The white king may move into check and then out of check on the same turn.",
                "https://en.wikipedia.org/wiki/Monster_chess"
            );
        }

        public override void OnMoveComplete() {
            if (!isWhiteSecondMove && GetCurrentTeamTurn() == Team.WHITE) {
                isWhiteSecondMove = true;
            } else {
                base.OnMoveComplete();
                isWhiteSecondMove = false;
            }
        }

        public override void PopulateBoard() {
            currentRoyalPiece = (King)AddNewPieceToBoard(Piece.King, Team.WHITE, new BoardCoord(4, WHITE_BACKROW));
            opposingRoyalPiece = (King)AddNewPieceToBoard(Piece.King, Team.BLACK, new BoardCoord(4, BLACK_BACKROW));

            AddNewPieceToBoard(Piece.Rook, Team.BLACK, new BoardCoord(0, BLACK_BACKROW));
            AddNewPieceToBoard(Piece.Rook, Team.BLACK, new BoardCoord(7, BLACK_BACKROW));

            AddNewPieceToBoard(Piece.Queen, Team.BLACK, new BoardCoord(3, BLACK_BACKROW));

            for (int x = 0; x < BOARD_WIDTH; x++) {
                if (x > 1 && x < 6) {
                    AddNewPieceToBoard(Piece.Pawn, Team.WHITE, new BoardCoord(x, WHITE_PAWNROW));
                }
                AddNewPieceToBoard(Piece.Pawn, Team.BLACK, new BoardCoord(x, BLACK_PAWNROW));

                if (x == 1 || x == BOARD_WIDTH - 2) {
                    AddNewPieceToBoard(Piece.Knight, Team.BLACK, new BoardCoord(x, BLACK_BACKROW));
                } else if (x == 2 || x == BOARD_WIDTH - 3) {
                    AddNewPieceToBoard(Piece.Bishop, Team.BLACK, new BoardCoord(x, BLACK_BACKROW));
                }
            }
        }

        public override bool CheckWinState() {
            if (GetCurrentTeamTurn() == Team.WHITE && !isWhiteSecondMove) {
                return false;
            }

            if (isWhiteSecondMove && IsPieceInCheck(opposingRoyalPiece)) {
                UIManager.Instance.LogCheckmate(GetCurrentTeamTurn().ToString(), GetOpposingTeamTurn().ToString());
                return true;
            }

            return base.CheckWinState();
        }

        protected override bool IsPieceInCheck(ChessPiece pieceToCheck) {
            if (checkingForCheck) return false;

            checkingForCheck = true;
            if (pieceToCheck.GetTeam() == Team.BLACK) {
                foreach (ChessPiece piece in GetAlivePiecesOfType<ChessPiece>(Team.WHITE)) {
                    if (CalculateAvailableMoves(piece).Contains(pieceToCheck.GetBoardPosition())) {
                        checkingForCheck = false;
                        return true;
                    }
                }
            } else {
                if (isWhiteSecondMove) {
                    opposingTeamCheckThreats = GetAllPossibleCheckThreats(pieceToCheck);
                    foreach (ChessPiece piece in opposingTeamCheckThreats) {
                        if (CalculateAvailableMoves(piece).Contains(pieceToCheck.GetBoardPosition())) {
                            checkingForCheck = false;
                            return true;
                        }
                    }
                }
            }
            checkingForCheck = false;
            return false;
        }
    }
}
