using System.Collections.Generic;

namespace ChessGameModes {
    /// <summary>
    /// Board layout: FIDE standard.
    /// </summary>
    public class Extinction : Chess {
        private readonly Piece[] pieces;
        private readonly Dictionary<Piece, int> whitePieceCounts;
        private readonly Dictionary<Piece, int> blackPieceCounts;

        public Extinction() : base() {
            pieces = new Piece[6] { Piece.King, Piece.Queen, Piece.Rook, Piece.Bishop, Piece.Knight, Piece.Pawn };

            whitePieceCounts = new Dictionary<Piece, int>(6) {
                { Piece.King, 1 }, { Piece.Queen, 1 }, { Piece.Rook, 2 }, { Piece.Bishop, 2 }, { Piece.Knight, 2 }, { Piece.Pawn, 8 }
            };
            blackPieceCounts = new Dictionary<Piece, int>(6) {
                { Piece.King, 1 }, { Piece.Queen, 1 }, { Piece.Rook, 2 }, { Piece.Bishop, 2 }, { Piece.Knight, 2 }, { Piece.Pawn, 8 }
            };
        }

        public override string ToString() {
            return "Extinction Chess";
        }

        public override VariantHelpDetails GetVariantHelpDetails() {
            return new VariantHelpDetails(
                this.ToString(),
                "Invented by R. Wayne Schmittberger (1985)",
                this.ToString() + " is a variant that involves capturing all of a particular piece.",
                "Capture all of the opposing team's set of unique pieces (pawns, knights, bishops, rooks, queen or king).",
                "- No royalty (no checks & castling is unrestricted by checks).\n" +
                "- Note: Pawns may promote to a king, in which capturing the additional king does not result in a game loss." +
                "- Note: If the last pawn on a team promotes, it is considered a game loss.",
                "https://en.wikipedia.org/wiki/Extinction_chess"
            );
        }

        public override bool CheckWinState() {
            if (GetCurrentTeamTurn() == Team.WHITE) {
                for (int i = 0; i < 6; i++) {
                    if(whitePieceCounts[pieces[i]] == 0) {
                        UIManager.Instance.LogCustom("Team WHITE's " + pieces[i].ToString().ToLower() + "s have all been eliminated -- Team BLACK wins!");
                        return true;
                    }
                }
            } else {
                for (int i = 0; i < 6; i++) {
                    if (blackPieceCounts[pieces[i]] == 0) {
                        UIManager.Instance.LogCustom("Team BLACK's " + pieces[i].ToString().ToLower() + "s have all been eliminated -- Team WHITE wins!");
                        return true;
                    }
                }
            }

            if (!TeamHasAnyMoves(GetCurrentTeamTurn())) {
                UIManager.Instance.LogStalemate(GetCurrentTeamTurn().ToString());
                return true;
            }

            if (CapturelessMovesLimit()) {
                return true;
            }

            return false;
        }

        public override void PopulateBoard() {
            AddNewPieceToBoard(Piece.King, Team.WHITE, new BoardCoord(4, WHITE_BACKROW));
            AddNewPieceToBoard(Piece.King, Team.BLACK, new BoardCoord(4, BLACK_BACKROW));

            AddNewPieceToBoard(Piece.Rook, Team.WHITE, new BoardCoord(0, WHITE_BACKROW));
            AddNewPieceToBoard(Piece.Rook, Team.BLACK, new BoardCoord(0, BLACK_BACKROW));
            AddNewPieceToBoard(Piece.Rook, Team.WHITE, new BoardCoord(7, WHITE_BACKROW));
            AddNewPieceToBoard(Piece.Rook, Team.BLACK, new BoardCoord(7, BLACK_BACKROW));

            AddNewPieceToBoard(Piece.Queen, Team.WHITE, new BoardCoord(3, WHITE_BACKROW));
            AddNewPieceToBoard(Piece.Queen, Team.BLACK, new BoardCoord(3, BLACK_BACKROW));

            for (int x = 0; x < BOARD_WIDTH; x++) {
                AddNewPieceToBoard(Piece.Pawn, Team.WHITE, new BoardCoord(x, WHITE_PAWNROW));
                AddNewPieceToBoard(Piece.Pawn, Team.BLACK, new BoardCoord(x, BLACK_PAWNROW));

                if (x == 1 || x == BOARD_WIDTH - 2) {
                    AddNewPieceToBoard(Piece.Knight, Team.WHITE, new BoardCoord(x, WHITE_BACKROW));
                    AddNewPieceToBoard(Piece.Knight, Team.BLACK, new BoardCoord(x, BLACK_BACKROW));
                } else if (x == 2 || x == BOARD_WIDTH - 3) {
                    AddNewPieceToBoard(Piece.Bishop, Team.WHITE, new BoardCoord(x, WHITE_BACKROW));
                    AddNewPieceToBoard(Piece.Bishop, Team.BLACK, new BoardCoord(x, BLACK_BACKROW));
                }
            }
        }

        protected override bool IsPieceInCheckAfterThisMove(ChessPiece pieceToCheck, ChessPiece mover, BoardCoord destination) {
            return false;
        }

        public override bool MovePiece(ChessPiece mover, BoardCoord destination) {
            ChessPiece capturedPiece = Board.GetCoordInfo(destination).GetAliveOccupier();

            // Try make the move
            if (MakeBaseMove(mover, destination)) {
                // Check castling moves
                if (IsRoyal(mover)) {
                    TryPerformCastlingMove(mover);
                } else if (mover is Pawn) {
                    TryPerformPawnEnPassantCapture((Pawn)mover);

                    ChessPiece promotedPiece = TryPerformPawnPromotion((Pawn)mover);
                    if (promotedPiece != null) {
                        if (GetCurrentTeamTurn() == Team.WHITE) {
                            whitePieceCounts[Piece.Pawn]--;
                            whitePieceCounts[SelectedPawnPromotion]++;
                        } else {
                            blackPieceCounts[Piece.Pawn]--;
                            blackPieceCounts[SelectedPawnPromotion]++;
                        }

                        mover = promotedPiece;
                    }
                }

                if(capturedPiece != null) {
                    if(GetCurrentTeamTurn() == Team.WHITE) {
                        blackPieceCounts[capturedPiece.GetPieceType()]--;
                    } else {
                        whitePieceCounts[capturedPiece.GetPieceType()]--;
                    }
                }

                return true;
            }
            return false;
        }
    }
}