using System.Collections.Generic;

namespace ChessGameModes {
    /// <summary>
    /// Board layout:
    ///     . . . . . . . .
    ///     . . . . . . . .
    ///     . . . . . . . .
    ///     . . . . . . . .
    ///     . . . . . . . .
    ///     . . . . . . . .
    ///     k r b n N B R K
    ///     q r b n N B R Q
    /// </summary>
    public class RacingKings : Chess {
        public RacingKings() : base(BOARD_WIDTH, BOARD_HEIGHT) {
            AllowCastling = false;
            AllowEnpassantCapture = false;
            AllowPawnPromotion = false;
        }

        public override string ToString() {
            return "Racing Kings";
        }

        public override VariantHelpDetails GetVariantHelpDetails() {
            return new VariantHelpDetails(
                this.ToString(),
                "Invented by Vernon R. Parton (1961)",
                this.ToString() + " is a variant where check is forbidden, and instead teams must race their kings to the end of the board.",
                "King reaches the 8th rank.",
                VariantHelpDetails.rule_NoCastling + "\n" +
                VariantHelpDetails.rule_NoEnpassantCapture + "\n" +
                VariantHelpDetails.rule_NoPawnPromotion,
                "https://www.chessvariants.com/diffobjective.dir/racing.html"
            );
        }

        public override void PopulateBoard() {
            opposingRoyalPiece = (King)AddNewPieceToBoard(Piece.King, Team.BLACK, new BoardCoord(0, 1));
            currentRoyalPiece = (King)AddNewPieceToBoard(Piece.King, Team.WHITE, new BoardCoord(7, 1));

            AddNewPieceToBoard(Piece.Queen, Team.BLACK, new BoardCoord(0, 0));
            AddNewPieceToBoard(Piece.Queen, Team.WHITE, new BoardCoord(7, 0));

            AddNewPieceToBoard(Piece.Rook, Team.BLACK, new BoardCoord(1, 0));
            AddNewPieceToBoard(Piece.Rook, Team.BLACK, new BoardCoord(1, 1));
            AddNewPieceToBoard(Piece.Rook, Team.WHITE, new BoardCoord(6, 0));
            AddNewPieceToBoard(Piece.Rook, Team.WHITE, new BoardCoord(6, 1));

            AddNewPieceToBoard(Piece.Bishop, Team.BLACK, new BoardCoord(2, 0));
            AddNewPieceToBoard(Piece.Bishop, Team.BLACK, new BoardCoord(2, 1));
            AddNewPieceToBoard(Piece.Bishop, Team.WHITE, new BoardCoord(5, 0));
            AddNewPieceToBoard(Piece.Bishop, Team.WHITE, new BoardCoord(5, 1));

            AddNewPieceToBoard(Piece.Knight, Team.BLACK, new BoardCoord(3, 0));
            AddNewPieceToBoard(Piece.Knight, Team.BLACK, new BoardCoord(3, 1));
            AddNewPieceToBoard(Piece.Knight, Team.WHITE, new BoardCoord(4, 0));
            AddNewPieceToBoard(Piece.Knight, Team.WHITE, new BoardCoord(4, 1));
        }

        public override bool CheckWinState() {
            if(opposingRoyalPiece.GetBoardPosition().y == BOARD_HEIGHT - 1) {
                BoardCoord stalemateSquare1 = new BoardCoord(currentRoyalPiece.GetBoardPosition().x, BOARD_HEIGHT - 1);
                BoardCoord stalemateSquare2 = new BoardCoord(currentRoyalPiece.GetBoardPosition().x + 1, BOARD_HEIGHT - 1);
                BoardCoord stalemateSquare3 = new BoardCoord(currentRoyalPiece.GetBoardPosition().x - 1, BOARD_HEIGHT - 1);

                List<BoardCoord> kingMoves = CalculateAvailableMoves(currentRoyalPiece);
                if (kingMoves.Contains(stalemateSquare1) || kingMoves.Contains(stalemateSquare2) || kingMoves.Contains(stalemateSquare3)) {
                    UIManager.Instance.LogCustom("Stalemate after team " + GetOpposingTeamTurn().ToString() + "'s move! (Team " + GetCurrentTeamTurn().ToString()
                        + " is able to reach the eighth rank on this turn)  -- Draw!");
                    return true;
                } else {
                    UIManager.Instance.LogCustom("Team " + GetOpposingTeamTurn().ToString() + "'s king has reached the eighth rank -- Team "
                        + GetOpposingTeamTurn().ToString() + "wins!");
                    return true;
                }
            }

            if(!TeamHasAnyMoves(GetCurrentTeamTurn())) {
                UIManager.Instance.LogStalemate(GetCurrentTeamTurn().ToString());
                return true;
            }

            if (CapturelessMovesLimit()) {
                return true;
            }

            return false;
        }

        public override bool MovePiece(ChessPiece mover, BoardCoord destination) {
            return MakeBaseMove(mover, destination);
        }

        public override List<BoardCoord> CalculateAvailableMoves(ChessPiece mover) {
            BoardCoord[] templateMoves = mover.CalculateTemplateMoves().ToArray();
            List<BoardCoord> availableMoves = new List<BoardCoord>(templateMoves.Length);

            for (int i = 0; i < templateMoves.Length; i++) {
               if (IsAKingInCheckAfterThisMove(mover, templateMoves[i]) == false) {
                    availableMoves.Add(templateMoves[i]);
               }
            }
            return availableMoves;
        }

        private bool IsAKingInCheckAfterThisMove(ChessPiece mover, BoardCoord destination) {
            if (Board.ContainsCoord(destination)) {
                SimulateMove(mover, destination);

                bool kingChecked = IsPieceInCheck(currentRoyalPiece) || IsPieceInCheck(opposingRoyalPiece);

                RevertSimulatedMove();

                return kingChecked;
            }
            return false;
        }
    }
}
