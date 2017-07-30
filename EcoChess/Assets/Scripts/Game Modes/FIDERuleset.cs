using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChessGameModes {
    /// <summary>
    /// FIDERuleset.cs is the fully standardised ruleset for traditional chess.
    /// 
    /// Winstate: Checkmate.
    /// Piece types: Orthodox.
    /// Board layout:
    ///     r n b q k b n r
    ///     p p p p p p p p
    ///     . . . . . . . .
    ///     . . . . . . . .
    ///     . . . . . . . .
    ///     . . . . . . . .
    ///     p p p p p p p p
    ///     R N B Q K B N R
    /// </summary>
    public class FIDERuleset : Chess {
        public static event Action<bool> _DisplayPromotionUI;
        public static event Action<Piece[]> _OnPawnPromotionsChanged;
        public Piece[] pawnPromotionOptions { get; protected set; }
        public Piece selectedPawnPromotion { get; protected set; }

        protected const int BOARD_WIDTH = 8;
        protected const int BOARD_HEIGHT = 8;
        protected const int WHITE_BACKROW = 0;
        protected const int WHITE_PAWNROW = 1;
        protected int BLACK_BACKROW;
        protected int BLACK_PAWNROW;

        protected ChessPiece currentRoyalPiece;
        protected ChessPiece opposingRoyalPiece;

        protected Rook aSideWhiteRook;
        protected Rook hSideWhiteRook;
        protected Rook aSideBlackRook;
        protected Rook hSideBlackRook;

        protected bool checkingForCheck;
        protected List<ChessPiece> opposingTeamCheckThreats;

        public FIDERuleset(uint boardWidth, uint boardHeight) : base(boardWidth, boardHeight) {
            BLACK_BACKROW = board.GetHeight() - 1;
            BLACK_PAWNROW = board.GetHeight() - 2;
            currentRoyalPiece = opposingRoyalPiece = null;
            aSideWhiteRook = hSideWhiteRook = null;
            aSideBlackRook = hSideWhiteRook = null;
            opposingTeamCheckThreats = null;
            checkingForCheck = false;
            pawnPromotionOptions = new Piece[4] { Piece.Queen, Piece.Rook, Piece.Bishop, Piece.Knight };
            selectedPawnPromotion = Piece.Queen;
        }

        public FIDERuleset(uint boardWidth, uint boardHeight, Color primaryBoardColour, Color secondaryBoardColour) 
            : base(boardWidth, boardHeight, primaryBoardColour, secondaryBoardColour) {

            BLACK_BACKROW = board.GetHeight() - 1;
            BLACK_PAWNROW = board.GetHeight() - 2;
            currentRoyalPiece = opposingRoyalPiece = null;
            aSideWhiteRook = hSideWhiteRook = null;
            aSideBlackRook = hSideWhiteRook = null;
            opposingTeamCheckThreats = null;
            checkingForCheck = false;
            pawnPromotionOptions = new Piece[4] { Piece.Queen, Piece.Rook, Piece.Bishop, Piece.Knight };
            selectedPawnPromotion = Piece.Queen;
        }

        public FIDERuleset() : base(BOARD_WIDTH, BOARD_HEIGHT) {
            BLACK_BACKROW = board.GetHeight() - 1;
            BLACK_PAWNROW = board.GetHeight() - 2;
            currentRoyalPiece = opposingRoyalPiece = null;
            aSideWhiteRook = hSideWhiteRook = null;
            aSideBlackRook = hSideWhiteRook = null;
            opposingTeamCheckThreats = null;
            checkingForCheck = false;
            pawnPromotionOptions = new Piece[4] { Piece.Queen, Piece.Rook, Piece.Bishop, Piece.Knight };
            selectedPawnPromotion = Piece.Queen;
        }

        public FIDERuleset(Color primaryBoardColour, Color secondaryBoardColour) 
            : base(BOARD_WIDTH, BOARD_HEIGHT, primaryBoardColour, secondaryBoardColour) {

            BLACK_BACKROW = board.GetHeight() - 1;
            BLACK_PAWNROW = board.GetHeight() - 2;
            currentRoyalPiece = opposingRoyalPiece = null;
            aSideWhiteRook = hSideWhiteRook = null;
            aSideBlackRook = hSideWhiteRook = null;
            opposingTeamCheckThreats = null;
            checkingForCheck = false;
            pawnPromotionOptions = new Piece[4] { Piece.Queen, Piece.Rook, Piece.Bishop, Piece.Knight };
            selectedPawnPromotion = Piece.Queen;
        }

        public override string ToString() {
            return "FIDE";
        }

        public override void OnTurnComplete() {
            base.OnTurnComplete();

            ChessPiece temp = currentRoyalPiece;
            currentRoyalPiece = opposingRoyalPiece;
            opposingRoyalPiece = temp;
        }

        public override bool CheckWinState() {
            if(numConsecutiveCapturelessMoves == 100) {
                UIManager.Instance.Log("No captures or pawn moves in 50 turns. Stalemate on " + GetCurrentTeamTurn().ToString() + "'s move!");
                return true;
            }

            foreach (ChessPiece piece in GetPieces(GetCurrentTeamTurn())) {
                if (piece.IsAlive) {
                    if (CalculateAvailableMoves(piece).Count > 0) return false;
                }
            }

            if (IsPieceInCheck(currentRoyalPiece)) {
                UIManager.Instance.Log("Team " + GetCurrentTeamTurn().ToString() + " has been checkmated -- Team " + GetOpposingTeamTurn().ToString() + " wins!");
            } else {
                UIManager.Instance.Log("Stalemate on " + GetCurrentTeamTurn().ToString() + "'s move!");
            }
            return true;
        }

        public override void PopulateBoard() {
            currentRoyalPiece = (King)AddPieceToBoard(new King(Team.WHITE, new BoardCoord(4, WHITE_BACKROW)));
            opposingRoyalPiece = (King)AddPieceToBoard(new King(Team.BLACK, new BoardCoord(4, BLACK_BACKROW)));

            aSideWhiteRook = (Rook)AddPieceToBoard(new Rook(Team.WHITE, new BoardCoord(0, WHITE_BACKROW)));
            aSideBlackRook = (Rook)AddPieceToBoard(new Rook(Team.BLACK, new BoardCoord(0, BLACK_BACKROW)));
            hSideWhiteRook = (Rook)AddPieceToBoard(new Rook(Team.WHITE, new BoardCoord(7, WHITE_BACKROW)));
            hSideBlackRook = (Rook)AddPieceToBoard(new Rook(Team.BLACK, new BoardCoord(7, BLACK_BACKROW)));

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

        public override List<BoardCoord> CalculateAvailableMoves(ChessPiece mover) {
            BoardCoord[] templateMoves = mover.CalculateTemplateMoves().ToArray();
            List<BoardCoord> availableMoves = new List<BoardCoord>(templateMoves.Length);

            for (int i = 0; i < templateMoves.Length; i++) {
                if (IsPieceInCheckAfterThisMove(currentRoyalPiece, mover, templateMoves[i]) == false) {
                    availableMoves.Add(templateMoves[i]);
                }
            }

            if(mover is King && mover.MoveCount == 0) {
                availableMoves.AddRange(TryAddAvailableCastleMoves(mover));
            } else if (mover is Pawn) {
                BoardCoord enPassantMove = TryAddAvailableEnPassantMove((Pawn)mover);
                if(enPassantMove != BoardCoord.NULL) {
                    availableMoves.Add(enPassantMove);
                }
                if (checkingForCheck == false && CanPromote((Pawn)mover, availableMoves.ToArray())) {
                    OnDisplayPromotionUI(true);
                }
            }

            return availableMoves;
        }

        public override bool MovePiece(ChessPiece mover, BoardCoord destination) {
            BoardCoord oldPos = mover.GetBoardPosition();

            // Try make the move
            if (MakeMove(mover, destination)) {
                // Check castling moves
                if (mover is King && mover.MoveCount == 1) {
                    TryPerformCastlingRookMoves(mover);
                } else if (mover is Pawn) {
                    ((Pawn)mover).validEnPassant = (mover.MoveCount == 1 && mover.GetRelativeBoardCoord(0, -1) != oldPos);
                    CheckPawnEnPassantCapture((Pawn)mover);
                    ChessPiece promotedPiece = CheckPawnPromotion((Pawn)mover);
                    if(promotedPiece != null) {
                        mover = promotedPiece;
                    }
                }
                return true;
            }
            return false;
        }

        protected virtual void TryPerformCastlingRookMoves(ChessPiece mover) {
            if (mover.GetBoardPosition().x == 2) {
                if (mover.GetTeam() == Team.WHITE) {
                    aSideWhiteRook = (Rook)PerformCastle(aSideWhiteRook, new BoardCoord(3, mover.GetBoardPosition().y));
                } else {
                    aSideBlackRook = (Rook)PerformCastle(aSideBlackRook, new BoardCoord(3, mover.GetBoardPosition().y));
                }
            } else if (mover.GetBoardPosition().x == 6) {
                if (mover.GetTeam() == Team.WHITE) {
                    hSideWhiteRook = (Rook)PerformCastle(hSideWhiteRook, new BoardCoord(5, mover.GetBoardPosition().y));
                } else {
                    hSideBlackRook = (Rook)PerformCastle(hSideBlackRook, new BoardCoord(5, mover.GetBoardPosition().y));
                }
            }
        }

        protected virtual Pawn CheckPawnEnPassantCapture(Pawn mover) {
            if (board.ContainsCoord(mover.GetRelativeBoardCoord(0, -1)) && IsThreat(mover, mover.GetRelativeBoardCoord(0, -1))) {
                ChessPiece occupier = board.GetCoordInfo(mover.GetRelativeBoardCoord(0, -1)).occupier;
                if (occupier != null && occupier is Pawn && ((Pawn)occupier).validEnPassant) {
                    mover.CaptureCount++;
                    RemovePieceFromBoard(occupier);
                    return (Pawn)occupier;
                }
            }
            return null;
        }

        protected virtual bool CanPromote(Pawn mover, BoardCoord[] availableMoves) {
            for (int i = 0; i < availableMoves.Length; i++) {
                if (availableMoves[i].y == WHITE_BACKROW || availableMoves[i].y == BLACK_BACKROW) {
                    return true;
                }
            }
            return false;
        }

        protected virtual ChessPiece CheckPawnPromotion(Pawn mover) {
            if (mover.GetRelativeBoardCoord(0, 1).y < WHITE_BACKROW || mover.GetRelativeBoardCoord(0, 1).y > BLACK_BACKROW) {
                RemovePieceFromBoard(mover);
                RemovePieceFromActiveTeam(mover);
                return AddPieceToBoard(ChessPieceFactory.Create(selectedPawnPromotion, mover.GetTeam(), mover.GetBoardPosition()));
            }
            return null;
        }

        protected void SetPawnPromotionOptions(Piece[] pieces) {
            pawnPromotionOptions = pieces;
            if (_OnPawnPromotionsChanged != null) _OnPawnPromotionsChanged.Invoke(pawnPromotionOptions);
        }

        protected void OnDisplayPromotionUI(bool value) {
            if (_DisplayPromotionUI != null) _DisplayPromotionUI.Invoke(true);
        }

        public void SetPawnPromotionTo(Piece piece) {
            this.selectedPawnPromotion = piece;
        }

        protected ChessPiece PerformCastle(ChessPiece castlingPiece, BoardCoord castlingPieceNewPos) {
            if (AssertContainsCoord(castlingPieceNewPos)) {
                if (castlingPiece != null) {
                    RemovePieceFromBoard(castlingPiece);
                    RemovePieceFromActiveTeam(castlingPiece);
                    return AddPieceToBoard(ChessPieceFactory.Create(castlingPiece.GetPieceType(), castlingPiece.GetTeam(), castlingPieceNewPos));
                } else {
                    Debug.LogError("Reference to the castling piece should not be null! Ensure references were made when the piece was first created.");
                }
            }
            return null;
        }

        protected virtual bool IsPieceInCheckAfterThisMove(ChessPiece pieceToCheck, ChessPiece mover, BoardCoord dest) {
            if (AssertContainsCoord(dest)) {
                if (checkingForCheck) return false;

                // Temporarily simulate the move actually happening
                ChessPiece originalOccupier = board.GetCoordInfo(dest).occupier;
                ChessPiece originalLastMover;
                BoardCoord oldPos = mover.GetBoardPosition();
                SimulateMove(mover, dest, originalOccupier, out originalLastMover);

                ChessPiece occupier = null;
                if (mover is Pawn) {
                    occupier = CheckPawnEnPassantCapture((Pawn)mover);
                }

                // Check whether the piece is in check after this temporary move
                bool isInCheck = IsPieceInCheck(pieceToCheck);

                if (occupier != null) {
                    mover.CaptureCount--;
                    board.GetCoordInfo(occupier.GetBoardPosition()).occupier = occupier;
                    occupier.IsAlive = true;
                    occupier.gameObject.SetActive(true);
                }

                // Revert the temporary move back to normal
                RevertSimulatedMove(mover, dest, originalOccupier, originalLastMover, oldPos);

                return isInCheck;
            }
            return false;
        }

        protected virtual bool IsPieceInCheck(ChessPiece pieceToCheck) {
            if (checkingForCheck) return false;

            opposingTeamCheckThreats = GetAllPossibleCheckThreats(pieceToCheck);

            checkingForCheck = true;
            foreach (ChessPiece piece in opposingTeamCheckThreats) {
                if (CalculateAvailableMoves(piece).Contains(pieceToCheck.GetBoardPosition())) {
                    checkingForCheck = false;
                    return true;
                }
            }
            checkingForCheck = false;
            return false;
        }

        protected virtual List<ChessPiece> GetAllPossibleCheckThreats(ChessPiece pieceToCheck) {
            List<ChessPiece> possibleCheckThreats = new List<ChessPiece>();

            for (int i = (int)MoveDirection.Up; i <= (int)MoveDirection.DownRight; i++) {
                int xModifier, yModifier;
                GetMoveDirectionModifiers(pieceToCheck, (MoveDirection)i, out xModifier, out yModifier);
                BoardCoord coord = pieceToCheck.GetBoardPosition() + new BoardCoord(xModifier, yModifier);

                while (board.ContainsCoord(coord)) {
                    if (IsThreat(pieceToCheck, coord)) {
                        possibleCheckThreats.Add(board.GetCoordInfo(coord).occupier);
                    }
                    coord.x += xModifier;
                    coord.y += yModifier;
                }
            }

            GetPiecesOfType<Knight>(pieceToCheck.GetOpposingTeam()).ForEach(x => { possibleCheckThreats.Add(x); });

            return possibleCheckThreats;
        }

        protected virtual BoardCoord TryAddAvailableEnPassantMove(Pawn mover) {
            const int LEFT = -1;
            const int RIGHT = 1;

            if (mover.canEnPassantCapture) {
                for (int i = LEFT; i <= RIGHT; i += 2) {
                    BoardCoord coord = TryGetSpecificMove(mover, mover.GetRelativeBoardCoord(i, 0), threatOnly: true);
                    if (board.ContainsCoord(coord)) {
                        ChessPiece piece = board.GetCoordInfo(coord).occupier;
                        if (piece is Pawn && piece == lastMovedPiece && ((Pawn)piece).validEnPassant) {
                            if (IsPieceInCheckAfterThisMove(currentRoyalPiece, mover, mover.GetRelativeBoardCoord(i, 1)) == false) {
                                return TryGetSpecificMove(mover, mover.GetRelativeBoardCoord(i, 1));
                            }
                        }
                    }
                }
            }
            return BoardCoord.NULL;
        }

        protected virtual BoardCoord[] TryAddAvailableCastleMoves(ChessPiece king, bool canCastleLeftward = true, bool canCastleRightward = true) {
            const int LEFT = -1;
            const int RIGHT = 1;

            if (IsPieceInCheck(king) == false) {
                List<BoardCoord> castleMoves = new List<BoardCoord>(2);

                for (int i = LEFT; i <= RIGHT; i += 2) {
                    if (!canCastleLeftward && i == LEFT) continue;
                    if (!canCastleRightward && i == RIGHT) break;

                    int x = king.GetBoardPosition().x + i;
                    int y = king.GetBoardPosition().y;
                    BoardCoord coord = new BoardCoord(x, y);

                    while (board.ContainsCoord(coord)) {
                        ChessPiece occupier = board.GetCoordInfo(coord).occupier;
                        if (occupier != null) {
                            if (occupier is Rook && occupier.MoveCount == 0) {
                                if (IsPieceInCheckAfterThisMove(king, king, king.GetBoardPosition() + new BoardCoord(i, 0)) == false
                                    && IsPieceInCheckAfterThisMove(king, king, king.GetBoardPosition() + new BoardCoord(i * 2, 0)) == false) {
                                    castleMoves.Add(TryGetSpecificMove(king, king.GetBoardPosition() + new BoardCoord(i * 2, 0)));
                                }
                            }
                            break;
                        }
                        x += i;
                        coord = new BoardCoord(x, y);
                    }
                }
                return castleMoves.ToArray();
            }
            return new BoardCoord[0];
        }
    }
}
