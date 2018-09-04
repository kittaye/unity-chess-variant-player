using System.Collections.Generic;
using UnityEngine;

namespace ChessGameModes {
    /// <summary>
    /// Extinction.cs is a chess variant with a custom winstate.
    /// 
    /// Winstate: Elimination of all of a type of chess piece.
    /// Piece types: Orthodox.
    /// Piece rules: King isn't royal -- can castle unrestricted by checks.
    /// Board layout: Orthodox.
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
            AddPieceToBoard(new King(Team.WHITE, new BoardCoord(4, WHITE_BACKROW)));
            AddPieceToBoard(new King(Team.BLACK, new BoardCoord(4, BLACK_BACKROW)));

            AddPieceToBoard(new Rook(Team.WHITE, new BoardCoord(0, WHITE_BACKROW)));
            AddPieceToBoard(new Rook(Team.BLACK, new BoardCoord(0, BLACK_BACKROW)));
            AddPieceToBoard(new Rook(Team.WHITE, new BoardCoord(7, WHITE_BACKROW)));
            AddPieceToBoard(new Rook(Team.BLACK, new BoardCoord(7, BLACK_BACKROW)));

            AddPieceToBoard(new Queen(Team.WHITE, new BoardCoord(3, WHITE_BACKROW)));
            AddPieceToBoard(new Queen(Team.BLACK, new BoardCoord(3, BLACK_BACKROW)));

            for (int x = 0; x < BOARD_WIDTH; x++) {
                AddPieceToBoard(new Pawn(Team.WHITE, new BoardCoord(x, WHITE_PAWNROW)));
                AddPieceToBoard(new Pawn(Team.BLACK, new BoardCoord(x, BLACK_PAWNROW)));

                if (x == 1 || x == BOARD_WIDTH - 2) {
                    AddPieceToBoard(new Knight(Team.WHITE, new BoardCoord(x, WHITE_BACKROW)));
                    AddPieceToBoard(new Knight(Team.BLACK, new BoardCoord(x, BLACK_BACKROW)));
                } else if (x == 2 || x == BOARD_WIDTH - 3) {
                    AddPieceToBoard(new Bishop(Team.WHITE, new BoardCoord(x, WHITE_BACKROW)));
                    AddPieceToBoard(new Bishop(Team.BLACK, new BoardCoord(x, BLACK_BACKROW)));
                }
            }
        }

        protected override bool IsPieceInCheckAfterThisMove(ChessPiece pieceToCheck, ChessPiece mover, BoardCoord dest) {
            return false;
        }

        public override bool MovePiece(ChessPiece mover, BoardCoord destination) {
            BoardCoord oldPos = mover.GetBoardPosition();
            ChessPiece capturedPiece = Board.GetCoordInfo(destination).occupier;

            // Try make the move
            string moveNotation = MakeDirectMove(mover, destination);
            if (moveNotation != null) {
                // Check castling moves
                if (mover == currentRoyalPiece && mover.MoveCount == 1) {
                    TryPerformCastlingRookMoves(mover, ref moveNotation);
                } else if (mover is Pawn) {
                    ((Pawn)mover).validEnPassant = (mover.MoveCount == 1 && mover.GetRelativeBoardCoord(0, -1) != oldPos);
                    CheckPawnEnPassantCapture((Pawn)mover);
                    ChessPiece promotedPiece = CheckPawnPromotion((Pawn)mover);
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

                GetMoveNotations.Push(moveNotation);
                return true;
            }
            return false;
        }
    }
}