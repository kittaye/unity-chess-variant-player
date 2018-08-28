using System.Collections.Generic;
using UnityEngine;
using System;

public enum Team { WHITE, BLACK }
public enum MoveDirection { Up, Down, Left, Right, UpLeft, UpRight, DownLeft, DownRight }

namespace ChessGameModes {
    /// <summary>
    /// Chess.cs is the fully standardised ruleset for traditional chess. 
    /// This is also the base class for all other gamemodes.
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
    public class Chess {

        public static event Action<bool> _DisplayPromotionUI;
        public static event Action<Piece[]> _OnPawnPromotionsChanged;
        public Piece[] PawnPromotionOptions { get; protected set; }
        public Piece SelectedPawnPromotion { get; protected set; }
        public Board Board { get; private set; }
        public bool allowBoardFlipping;

        protected const int BOARD_WIDTH = 8;
        protected const int BOARD_HEIGHT = 8;
        protected const int WHITE_BACKROW = 0;
        protected const int WHITE_PAWNROW = 1;
        protected int BLACK_BACKROW;
        protected int BLACK_PAWNROW;
        protected bool checkingForCheck;
        protected List<ChessPiece> opposingTeamCheckThreats;
        protected ChessPiece currentRoyalPiece;
        protected ChessPiece opposingRoyalPiece;
        protected Rook aSideWhiteRook;
        protected Rook hSideWhiteRook;
        protected Rook aSideBlackRook;
        protected Rook hSideBlackRook;

        private uint numConsecutiveCapturelessMoves;
        private Team currentTeamTurn;
        private Team opposingTeamTurn;
        private List<ChessPiece> whitePieces;
        private List<ChessPiece> blackPieces;
        private ChessPiece lastMovedWhitePiece;
        private ChessPiece lastMovedBlackPiece;

        public Chess() {
            Board = new Board(BOARD_WIDTH, BOARD_HEIGHT, new Color(0.9f, 0.9f, 0.9f), new Color(0.1f, 0.1f, 0.1f));
            Init();
        }

        public Chess(uint width, uint height) {
            Board = new Board(width, height, new Color(0.9f, 0.9f, 0.9f), new Color(0.1f, 0.1f, 0.1f));
            Init();
        }

        public Chess(uint width, uint height, Color primaryBoardColour, Color secondaryBoardColour) {
            Board = new Board(width, height, primaryBoardColour, secondaryBoardColour);
            Init();
        }

        private void Init() {
            whitePieces = new List<ChessPiece>();
            blackPieces = new List<ChessPiece>();
            lastMovedWhitePiece = null;
            lastMovedBlackPiece = null;
            currentTeamTurn = Team.WHITE;
            opposingTeamTurn = Team.BLACK;
            numConsecutiveCapturelessMoves = 0;
            allowBoardFlipping = false;

            BLACK_BACKROW = Board.GetHeight() - 1;
            BLACK_PAWNROW = Board.GetHeight() - 2;
            currentRoyalPiece = opposingRoyalPiece = null;
            aSideWhiteRook = hSideWhiteRook = null;
            aSideBlackRook = hSideWhiteRook = null;
            opposingTeamCheckThreats = null;
            checkingForCheck = false;
            PawnPromotionOptions = new Piece[4] { Piece.Queen, Piece.Rook, Piece.Bishop, Piece.Knight };
            SelectedPawnPromotion = Piece.Queen;
        }

        public override string ToString() {
            return "Traditional Chess";
        }

        public Team GetCurrentTeamTurn() {
            return currentTeamTurn;
        }

        public Team GetOpposingTeamTurn() {
            return opposingTeamTurn;
        }

        public int GetNumConseqCapturelessMoves() {
            return (int)numConsecutiveCapturelessMoves;
        }

        public bool CapturelessMovesLimit() {
            if (numConsecutiveCapturelessMoves >= 100) {
                UIManager.Instance.LogCapturelessLimit(GetCurrentTeamTurn().ToString());
                return true;
            }
            return false;
        }

        /// <summary>
        /// Is called after the board is instantiated. Used to place the initial chess pieces on the board. 
        /// </summary>
        public virtual void PopulateBoard() {
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

        /// <summary>
        /// Calculates the currently available moves for a selected piece.
        /// </summary>
        /// <param name="mover">Selected piece to calculate moves for.</param>
        /// <returns>A list of board coordinates for each move.</returns>
        public virtual List<BoardCoord> CalculateAvailableMoves(ChessPiece mover) {
            BoardCoord[] templateMoves = mover.CalculateTemplateMoves().ToArray();
            List<BoardCoord> availableMoves = new List<BoardCoord>(templateMoves.Length);

            for (int i = 0; i < templateMoves.Length; i++) {
                if (IsPieceInCheckAfterThisMove(currentRoyalPiece, mover, templateMoves[i]) == false) {
                    availableMoves.Add(templateMoves[i]);
                }
            }

            if (mover is King && mover.MoveCount == 0) {
                availableMoves.AddRange(TryAddAvailableCastleMoves(mover));
            } else if (mover is Pawn) {
                BoardCoord enPassantMove = TryAddAvailableEnPassantMove((Pawn)mover);
                if (enPassantMove != BoardCoord.NULL) {
                    availableMoves.Add(enPassantMove);
                }
                if (checkingForCheck == false && CanPromote((Pawn)mover, availableMoves.ToArray())) {
                    OnDisplayPromotionUI(true);
                }
            }

            return availableMoves;
        }

        /// <summary>
        /// Moves a selected piece to a destination.
        /// </summary>
        /// <param name="mover">Piece to move.</param>
        /// <param name="destination">Destination to move to.</param>
        /// <returns>True if the destination is an available move for this piece.</returns>
        public virtual bool MovePiece(ChessPiece mover, BoardCoord destination) {
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
                    if (promotedPiece != null) {
                        mover = promotedPiece;
                    }
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Called in MovePiece. If a castling move was played, this method will perform the castle.
        /// </summary>
        /// <param name="mover">Piece to perform the castling move.</param>
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

        /// <summary>
        /// Performs the castling move. Should be called within TryPerformCastlingRookMoves.
        /// </summary>
        /// <param name="castlingPiece">Piece to castle with.</param>
        /// <param name="castlingPieceNewPos">Castling piece's final position.</param>
        /// <returns></returns>
        protected virtual ChessPiece PerformCastle(ChessPiece castlingPiece, BoardCoord castlingPieceNewPos) {
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

        /// <summary>
        /// Called in MovePiece. If an enpassant move was made, enpassant capture is performed.
        /// </summary>
        /// <param name="mover">Moving piece.</param>
        /// <returns></returns>
        protected virtual Pawn CheckPawnEnPassantCapture(Pawn mover) {
            if (Board.ContainsCoord(mover.GetRelativeBoardCoord(0, -1)) && IsThreat(mover, mover.GetRelativeBoardCoord(0, -1))) {
                ChessPiece occupier = Board.GetCoordInfo(mover.GetRelativeBoardCoord(0, -1)).occupier;
                if (occupier != null && occupier is Pawn && occupier == LastMovedOpposingPiece(mover) && ((Pawn)occupier).validEnPassant) {
                    mover.CaptureCount++;
                    RemovePieceFromBoard(occupier);
                    return (Pawn)occupier;
                }
            }
            return null;
        }

        /// <summary>
        /// Called in CalculateAvailableMoves. Determines if a pawn can promote on their move.
        /// </summary>
        /// <param name="mover">Moving pawn.</param>
        /// <param name="availableMoves">The pawn's available moves to check.</param>
        /// <returns></returns>
        protected virtual bool CanPromote(Pawn mover, BoardCoord[] availableMoves) {
            for (int i = 0; i < availableMoves.Length; i++) {
                if (availableMoves[i].y == WHITE_BACKROW || availableMoves[i].y == BLACK_BACKROW) {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Called in MovePiece. If a promoting move was made, the pawn is removed from the board and replaced with the selected pawn promotion.
        /// </summary>
        /// <param name="mover">Moving pawn.</param>
        /// <returns></returns>
        protected virtual ChessPiece CheckPawnPromotion(Pawn mover) {
            if (mover.GetRelativeBoardCoord(0, 1).y < WHITE_BACKROW || mover.GetRelativeBoardCoord(0, 1).y > BLACK_BACKROW) {
                RemovePieceFromBoard(mover);
                RemovePieceFromActiveTeam(mover);
                return AddPieceToBoard(ChessPieceFactory.Create(SelectedPawnPromotion, mover.GetTeam(), mover.GetBoardPosition()));
            }
            return null;
        }

        /// <summary>
        /// Simulates a move, checks whether the piece-to-check is in check, then reverts the simulated move.
        /// </summary>
        /// <param name="pieceToCheck">Piece to check (usually the current or opposing royal piece).</param>
        /// <param name="mover">Piece to move.</param>
        /// <param name="dest">Destination to move to.</param>
        /// <returns>True if the piece-to-check is in check after the simulated move.</returns>
        protected virtual bool IsPieceInCheckAfterThisMove(ChessPiece pieceToCheck, ChessPiece mover, BoardCoord dest) {
            if (AssertContainsCoord(dest)) {
                if (checkingForCheck) return false;

                // Temporarily simulate the move actually happening
                ChessPiece originalOccupier = Board.GetCoordInfo(dest).occupier;
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
                    Board.GetCoordInfo(occupier.GetBoardPosition()).occupier = occupier;
                    occupier.IsAlive = true;
                    occupier.gameObject.SetActive(true);
                }

                // Revert the temporary move back to normal
                RevertSimulatedMove(mover, dest, originalOccupier, originalLastMover, oldPos);

                return isInCheck;
            }
            return false;
        }

        /// <summary>
        /// Determines whether the piece-to-check is currently in check.
        /// </summary>
        /// <param name="pieceToCheck">Piece to check (usually the current or opposing royal piece).</param>
        /// <returns>True if the piece-to-check is in check.</returns>
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

        /// <summary>
        /// Gets all possible check threats against the piece-to-check. This should vary between game-modes 
        /// that involve pieces that don't move directionally.
        /// </summary>
        /// <param name="pieceToCheck">Piece to check (usually the current or opposing royal piece).</param>
        /// <returns>A list of chess pieces that can check the piece-to-check.</returns>
        protected virtual List<ChessPiece> GetAllPossibleCheckThreats(ChessPiece pieceToCheck) {
            List<ChessPiece> possibleCheckThreats = new List<ChessPiece>();

            for (int i = (int)MoveDirection.Up; i <= (int)MoveDirection.DownRight; i++) {
                int xModifier, yModifier;
                GetMoveDirectionModifiers(pieceToCheck, (MoveDirection)i, out xModifier, out yModifier);
                BoardCoord coord = pieceToCheck.GetBoardPosition() + new BoardCoord(xModifier, yModifier);

                while (Board.ContainsCoord(coord)) {
                    if (IsThreat(pieceToCheck, coord)) {
                        possibleCheckThreats.Add(Board.GetCoordInfo(coord).occupier);
                    }
                    coord.x += xModifier;
                    coord.y += yModifier;
                }
            }

            foreach (Knight knight in GetPiecesOfType<Knight>()) {
                if (IsThreat(pieceToCheck, knight.GetBoardPosition())) {
                    possibleCheckThreats.Add(knight);
                }
            }

            return possibleCheckThreats;
        }

        /// <summary>
        /// Called in CalculateAvailableMoves. If an enpassant move is available, returns the enpassant move.
        /// </summary>
        /// <param name="mover">Moving pawn.</param>
        /// <returns>Enpassant coordinate, otherwise null.</returns>
        protected virtual BoardCoord TryAddAvailableEnPassantMove(Pawn mover) {
            const int LEFT = -1;
            const int RIGHT = 1;

            if (mover.canEnPassantCapture) {
                for (int i = LEFT; i <= RIGHT; i += 2) {
                    BoardCoord coord = TryGetSpecificMove(mover, mover.GetRelativeBoardCoord(i, 0), threatOnly: true);
                    if (Board.ContainsCoord(coord)) {
                        ChessPiece piece = Board.GetCoordInfo(coord).occupier;
                        if (piece is Pawn && piece == LastMovedOpposingPiece(mover) && ((Pawn)piece).validEnPassant) {
                            if (IsPieceInCheckAfterThisMove(currentRoyalPiece, mover, mover.GetRelativeBoardCoord(i, 1)) == false) {
                                return TryGetSpecificMove(mover, mover.GetRelativeBoardCoord(i, 1));
                            }
                        }
                    }
                }
            }
            return BoardCoord.NULL;
        }

        /// <summary>
        /// Called in CalculateAvailableMoves. Determines if a castling move can be made for a chess piece.
        /// </summary>
        /// <param name="king">Moving piece.</param>
        /// <param name="canCastleLeftward">Can the piece castle leftwards?</param>
        /// <param name="canCastleRightward">Can the piece castle rightwards?</param>
        /// <returns>A list of board coordinates for available castle moves.</returns>
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

                    while (Board.ContainsCoord(coord)) {
                        ChessPiece occupier = Board.GetCoordInfo(coord).occupier;
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

        /// <summary>
        /// Checks whether any piece on the team is able to move.
        /// </summary>
        /// <param name="team">Team to check.</param>
        /// <returns>True if a piece is able to move.</returns>
        protected bool TeamHasAnyMoves(Team team) {
            foreach (ChessPiece piece in GetPieces(GetCurrentTeamTurn())) {
                if (piece.IsAlive) {
                    if (CalculateAvailableMoves(piece).Count > 0) {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Defines how the chess game is won.
        /// </summary>
        /// <returns>True if a team has won.</returns>
        public virtual bool CheckWinState() {
            if (!TeamHasAnyMoves(GetCurrentTeamTurn())) {
                if (IsPieceInCheck(currentRoyalPiece)) {
                    UIManager.Instance.LogCheckmate(GetOpposingTeamTurn().ToString(), GetCurrentTeamTurn().ToString());
                } else {
                    UIManager.Instance.LogStalemate(GetCurrentTeamTurn().ToString());
                }
                return true;
            }

            if (CapturelessMovesLimit()) {
                return true;
            }

            return false;
        }

        public ChessPiece GetTeamLastMovedPiece(Team team) {
            if (team == Team.WHITE) {
                return lastMovedWhitePiece;
            } else {
                return lastMovedBlackPiece;
            }
        }

        public void SetTeamLastMovedPiece(ChessPiece piece) {
            if (piece != null) {
                if (piece.GetTeam() == Team.WHITE) {
                    lastMovedWhitePiece = piece;
                } else {
                    lastMovedBlackPiece = piece;
                }
            }
        }

        protected ChessPiece LastMovedOpposingPiece(ChessPiece mover) {
            if (mover.GetTeam() == Team.WHITE) {
                return GetTeamLastMovedPiece(Team.BLACK);
            } else {
                return GetTeamLastMovedPiece(Team.WHITE);
            }
        }

        /// <summary>
        /// Used to update the pawn promotion options at any point during the game.
        /// </summary>
        /// <param name="pieces">Set of pieces to change the pawn promotion options to.</param>
        protected void SetPawnPromotionOptions(Piece[] pieces) {
            PawnPromotionOptions = pieces;
            if (_OnPawnPromotionsChanged != null) _OnPawnPromotionsChanged.Invoke(PawnPromotionOptions);
        }

        /// <summary>
        /// Display the UI for showing pawn promotion options to choose from.
        /// </summary>
        /// <param name="value"></param>
        protected void OnDisplayPromotionUI(bool value) {
            if (_DisplayPromotionUI != null) _DisplayPromotionUI.Invoke(true);
        }

        /// <summary>
        /// Set the current pawn promotion option to a specified piece.
        /// </summary>
        /// <param name="piece">Piece to set the pawn promotion option to.</param>
        public virtual void SetPawnPromotionTo(Piece piece) {
            this.SelectedPawnPromotion = piece;
        }

        /// <summary>
        /// Gets a list of a specific type of chess pieces in the game.
        /// </summary>
        /// <typeparam name="T">Type of chess piece to retrieve.</typeparam>
        /// <param name="team">Team to get pieces from.</param>
        /// <param name="aliveOnly">Should only pieces that are currently on the board be retrieved?</param>
        /// <returns>A list of T chess pieces.</returns>
        public List<T> GetPiecesOfType<T>(Team team, bool aliveOnly = true) where T : ChessPiece {
            List<T> selectionOfPieces = new List<T>();
            switch (team) {
                case Team.WHITE:
                    foreach (ChessPiece piece in whitePieces) {
                        if (piece is T && piece.IsAlive) {
                            selectionOfPieces.Add((T)piece);
                        }
                    }
                    break;
                case Team.BLACK:
                    foreach (ChessPiece piece in blackPieces) {
                        if (piece is T && piece.IsAlive) {
                            selectionOfPieces.Add((T)piece);
                        }
                    }
                    break;
            }
            return selectionOfPieces;
        }

        /// <summary>
        /// Gets a list of a specific type of chess pieces in the game.
        /// </summary>
        /// <typeparam name="T">Type of chess piece to retrieve.</typeparam>
        /// <param name="aliveOnly">Should only pieces that are currently on the board be retrieved?</param>
        /// <returns>A list of T chess pieces.</returns>
        public List<T> GetPiecesOfType<T>(bool aliveOnly = true) where T : ChessPiece {
            List<T> selectionOfPieces = new List<T>();
            foreach (ChessPiece piece in whitePieces) {
                if (piece is T && piece.IsAlive) {
                    selectionOfPieces.Add((T)piece);
                }
            }
            foreach (ChessPiece piece in blackPieces) {
                if (piece is T && piece.IsAlive) {
                    selectionOfPieces.Add((T)piece);
                }
            }
            return selectionOfPieces;
        }

        /// <summary>
        /// Gets a list of all chess pieces in the current game.
        /// </summary>
        /// <param name="aliveOnly">Should only pieces that are currently on the board be retrieved?</param>
        /// <returns>A list of all chess pieces in the current game.</returns>
        public List<ChessPiece> GetPieces(bool aliveOnly = true) {
            List<ChessPiece> pieces = new List<ChessPiece>(whitePieces.Count + blackPieces.Count);
            if (aliveOnly) {
                whitePieces.ForEach(x => { if (x.IsAlive) pieces.Add(x); });
                blackPieces.ForEach(x => { if (x.IsAlive) pieces.Add(x); });
            } else {
                pieces.AddRange(whitePieces);
                pieces.AddRange(blackPieces);
            }
            return pieces;
        }

        /// <summary>
        /// Gets a list of all chess pieces in the current game.
        /// </summary>
        /// <param name="team">Team to get pieces from.</param>
        /// <param name="aliveOnly">Should only pieces that are currently on the board be retrieved?</param>
        /// <returns>A list of all chess pieces in the current game.</returns>
        public List<ChessPiece> GetPieces(Team team, bool aliveOnly = true) {
            List<ChessPiece> pieces = new List<ChessPiece>();
            if (aliveOnly) {
                if (team == Team.WHITE) {
                    whitePieces.ForEach(x => { if (x.IsAlive) pieces.Add(x); });
                } else {
                    blackPieces.ForEach(x => { if (x.IsAlive) pieces.Add(x); });
                }
                return pieces;
            } else {
                if (team == Team.WHITE) {
                    return new List<ChessPiece>(whitePieces);
                }
                return new List<ChessPiece>(blackPieces);
            }
        }

        /// <summary>
        /// Determines whether or not a specific coord is considered an ally against the specified chess piece.
        /// </summary>
        /// <param name="mover">Piece to compare against.</param>
        /// <param name="coord">Board coordinate to test.</param>
        /// <returns>True if the specified square is an ally to mover.</returns>
        protected virtual bool IsAlly(ChessPiece mover, BoardCoord coord) {
            if (AssertContainsCoord(coord)) {
                ChessPiece occupier = Board.GetCoordInfo(coord).occupier;
                if (occupier != null) {
                    return occupier.GetTeam() == mover.GetTeam();
                } else {
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// Determines whether or not a specific coord is considered a threat against the specified chess piece.
        /// </summary>
        /// <param name="mover">Piece to compare against.</param>
        /// <param name="coord">Board coordinate to test.</param>
        /// <returns>True if the specified square is a threat to mover.</returns>
        protected virtual bool IsThreat(ChessPiece mover, BoardCoord coord) {
            if (AssertContainsCoord(coord)) {
                ChessPiece occupier = Board.GetCoordInfo(coord).occupier;
                if (occupier != null) {
                    return occupier.GetTeam() != mover.GetTeam();
                } else {
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns a string that describes who's turn it is currently.
        /// </summary>
        /// <returns></returns>
        public virtual string GetCurrentTurnLabel() {
            if (currentTeamTurn == Team.WHITE) {
                return "White's move";
            } else {
                return "Black's move";
            }
        }

        /// <summary>
        /// Adds a chess piece to a team based on it's own team value.
        /// </summary>
        /// <param name="piece">Piece to add.</param>
        protected void AddPieceToActiveTeam(ChessPiece piece) {
            if (piece.GetTeam() == Team.WHITE) {
                whitePieces.Add(piece);
            } else {
                blackPieces.Add(piece);
            }
        }

        /// <summary>
        /// Removes a chess piece from it's team.
        /// </summary>
        /// <param name="piece">Piece to remove.</param>
        /// <returns>True if the removal was successful.</returns>
        protected bool RemovePieceFromActiveTeam(ChessPiece piece) {
            if (piece.GetTeam() == Team.WHITE) {
                return whitePieces.Remove(piece);
            } else {
                return blackPieces.Remove(piece);
            }
        }

        /// <summary>
        /// Used to update the occupiers of affected squares after a move is played.
        /// </summary>
        /// <param name="previousPosition"></param>
        /// <param name="newPosition"></param>
        private void UpdateSquareOccupiers(BoardCoord previousPosition, BoardCoord newPosition) {
            if (AssertContainsCoord(previousPosition) && AssertContainsCoord(newPosition)) {
                ChessPiece oldCoordOccupier = Board.GetCoordInfo(previousPosition).occupier;
                if (oldCoordOccupier != null) {
                    Board.GetCoordInfo(previousPosition).occupier = null;
                    if (IsThreat(oldCoordOccupier, newPosition)) {
                        RemovePieceFromBoard(Board.GetCoordInfo(newPosition).occupier);
                    }
                    Board.GetCoordInfo(newPosition).occupier = oldCoordOccupier;
                }
            }
        }

        /// <summary>
        /// Moves a chess piece from it's current position to the destination.
        /// </summary>
        /// <param name="mover"></param>
        /// <param name="destination"></param>
        /// <returns>Returns true if the move was successful.</returns>
        protected bool MakeMove(ChessPiece mover, BoardCoord destination) {
            BoardCoord previousPosition = mover.GetBoardPosition();
            bool wasThreat = IsThreat(mover, destination);

            if (mover.MakeMoveTo(destination)) {
                if (wasThreat) mover.CaptureCount++;
                numConsecutiveCapturelessMoves = (wasThreat == false && (mover is Pawn) == false) ? numConsecutiveCapturelessMoves + 1 : 0;
                UpdateSquareOccupiers(previousPosition, mover.GetBoardPosition());
                SetTeamLastMovedPiece(mover);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Called after a move is played. Switches the current and opposing teams around,
        /// resets the pawn promotion value, and switches the current and opposing royal pieces around.
        /// </summary>
        public virtual void OnTurnComplete() {
            currentTeamTurn = (currentTeamTurn == Team.WHITE) ? Team.BLACK : Team.WHITE;
            opposingTeamTurn = (currentTeamTurn == Team.WHITE) ? Team.BLACK : Team.WHITE;

            ChessPiece temp = currentRoyalPiece;
            currentRoyalPiece = opposingRoyalPiece;
            opposingRoyalPiece = temp;

            SelectedPawnPromotion = Piece.Queen;
        }

        /// <summary>
        /// Removes a chess piece from the board.
        /// </summary>
        /// <param name="piece">Piece to remove.</param>
        /// <returns>True if the removal was successful.</returns>
        protected bool RemovePieceFromBoard(ChessPiece piece) {
            if (piece != null) {
                piece.gameObject.SetActive(false);
                piece.IsAlive = false;
                Board.GetCoordInfo(piece.GetBoardPosition()).occupier = null;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Adds a chess piece to the game board and assigns it to a team based on it's own team value.
        /// </summary>
        /// <param name="piece">Piece to add.</param>
        /// <returns>Returns the chess piece added to the game board.</returns>
        protected ChessPiece AddPieceToBoard(ChessPiece piece) {
            if (CheckValidPlacement(piece)) {
                Board.GetCoordInfo(piece.GetBoardPosition()).occupier = piece;
                piece.IsAlive = true;
                GameManager.Instance.InstantiateChessPiece(piece);
                AddPieceToActiveTeam(piece);

                if (piece.GetTeam() == Team.BLACK && Board.isFlipped) {
                    piece.gameObject.transform.Rotate(new Vector3(0, 0, 180));
                }
                return piece;
            }
            return null;
        }

        /// <summary>
        /// Used to ensure the chess piece added to the game board has been placed in a valid position.
        /// </summary>
        /// <param name="piece">Piece to check.</param>
        /// <returns>True if valid placement.</returns>
        private bool CheckValidPlacement(ChessPiece piece) {
            if (AssertContainsCoord(piece.GetBoardPosition()) == false) {
                return false;
            } else if (Board.GetCoordInfo(piece.GetBoardPosition()).occupier != null && Board.GetCoordInfo(piece.GetBoardPosition()).occupier.IsAlive) {
                CoordInfo posInfo = Board.GetCoordInfo(piece.GetBoardPosition());
                Debug.LogErrorFormat("OCCUPIED EXCEPTION :: " +
                    "{0} failed to instantiate because a {1} is already at it's position! Location: {3}."
                    , piece.ToString(), posInfo.occupier.ToString(), posInfo.algebraicKey, piece.GetBoardPosition().ToString());
                return false;
            }
            return true;
        }

        /// <summary>
        /// Assert that the current game board contains a specified coordinate.
        /// </summary>
        /// <param name="coord">Coordinate to check.</param>
        /// <returns>True if game board contains the coordinate.</returns>
        public bool AssertContainsCoord(BoardCoord coord) {
            if (!Board.ContainsCoord(coord)) {
                Debug.LogErrorFormat("ERROR: {0} is not a valid position on the GameBoard!", coord.ToString());
                return false;
            }
            return true;
        }

        /// <summary>
        /// Determines whether a selected chess piece is it's team's turn to move.
        /// </summary>
        /// <param name="mover"></param>
        /// <returns></returns>
        public virtual bool IsMoversTurn(ChessPiece mover) {
            return mover.GetTeam() == currentTeamTurn;
        }

        /// <summary>
        /// Simulates a move being played. NOTE: Must be followed by RevertSimulatedMove.
        /// </summary>
        /// <param name="mover">Piece to move.</param>
        /// <param name="dest">Destination to move to.</param>
        /// <param name="originalOccupier">The occupier at the destination prior to this simulated move.</param>
        /// <param name="originalLastMover">The last moved piece prior to this simulated move.</param>
        protected void SimulateMove(ChessPiece mover, BoardCoord dest, ChessPiece originalOccupier, out ChessPiece originalLastMover) {
            originalLastMover = null;
            if (AssertContainsCoord(dest)) {
                originalLastMover = GetTeamLastMovedPiece(mover.GetTeam());
                SetTeamLastMovedPiece(mover);

                if (originalOccupier != null) {
                    originalOccupier.IsAlive = false;
                }

                Board.GetCoordInfo(mover.GetBoardPosition()).occupier = null;
                Board.GetCoordInfo(dest).occupier = mover;
                mover.SetBoardPosition(dest);
            }
        }

        /// <summary>
        /// Reverts a simulated move. NOTE: Must be preceeded by SimulateMove.
        /// </summary>
        /// <param name="mover">Piece to move.</param>
        /// <param name="dest">Destination to move to.</param>
        /// <param name="originalOccupier">The occupier at the destination prior to this simulated move.</param>
        /// <param name="originalLastMover">The last moved piece prior to this simulated move.</param>
        /// <param name="oldPos">The position of the mover prior to this simulated move.</param>
        protected void RevertSimulatedMove(ChessPiece mover, BoardCoord dest, ChessPiece originalOccupier, ChessPiece originalLastMover, BoardCoord oldPos) {
            if (AssertContainsCoord(dest)) {
                mover.SetBoardPosition(oldPos);
                Board.GetCoordInfo(mover.GetBoardPosition()).occupier = mover;

                if (originalOccupier != null) {
                    originalOccupier.IsAlive = true;
                    Board.GetCoordInfo(dest).occupier = originalOccupier;
                } else {
                    Board.GetCoordInfo(dest).occupier = null;
                }

                SetTeamLastMovedPiece(originalLastMover);
            }
        }

        /// <summary>
        /// Gets a list of moves that a chess piece can move to. This method is used to build up a chess piece's list of template moves.
        /// </summary>
        /// <param name="mover">Piece to calculate moves for.</param>
        /// <param name="dir">Move direction to test moves for.</param>
        /// <param name="cap">Number of squares to test before stopping (0 = unbounded).</param>
        /// <param name="threatAttackLimit">Number of threats to test before stopping (0 = unbounded).</param>
        /// <param name="threatsOnly">Only get attacking moves?</param>
        /// <param name="teamSensitive">Is the move direction relative to the team or to the game board?</param>
        /// <returns></returns>
        public BoardCoord[] TryGetDirectionalMoves(ChessPiece mover, MoveDirection dir, uint cap = 0, uint threatAttackLimit = 1, bool threatsOnly = false, bool teamSensitive = true) {
            int x = mover.GetBoardPosition().x;
            int y = mover.GetBoardPosition().y;
            int xModifier;
            int yModifier;
            GetMoveDirectionModifiers(mover, dir, out xModifier, out yModifier, teamSensitive);

            bool xWrap = mover.hasXWrapping;
            bool yWrap = mover.hasYWrapping;

            uint iter = 0;
            uint threats = 0;
            BoardCoord coord;
            List<BoardCoord> moves = new List<BoardCoord>();
            while (true) {
                iter++;
                x += xModifier;
                y += yModifier;
                if (xWrap) x = MathExtensions.mod(x, Board.GetWidth());
                if (yWrap) y = MathExtensions.mod(y, Board.GetHeight());
                coord = new BoardCoord(x, y);

                if (Board.ContainsCoord(coord) == false) break;
                if (IsAlly(mover, coord)) break;
                if (IsThreat(mover, coord)) {
                    if (threatAttackLimit == 0) break;
                    moves.Add(coord);
                    if (++threats == threatAttackLimit) break;
                } else {
                    if (threatsOnly) break;
                    moves.Add(coord);
                }

                if (iter == cap) break;
            }
            return moves.ToArray();
        }

        /// <summary>
        /// Gets a list of moves that a chess piece can move to. This method is used to build up a chess piece's list of template moves.
        /// </summary>
        /// <param name="mover">Piece to calculate moves for.</param>
        /// <param name="xVariance">Custom direction's x step from the mover's position.</param>
        /// <param name="yVariance">Custom direction's y step from the mover's position.</param>
        /// <param name="cap">Number of squares to test before stopping (0 = unbounded).</param>
        /// <param name="threatsOnly">Only get attacking moves?</param>
        /// <returns></returns>
        public BoardCoord[] TryGetCustomDirectionalMoves(ChessPiece mover, int xVariance, int yVariance, uint cap = 0, bool threatsOnly = false) {
            int x = mover.GetBoardPosition().x;
            int y = mover.GetBoardPosition().y;
            int xModifier = mover.TeamSensitiveMove(xVariance);
            int yModifier = mover.TeamSensitiveMove(yVariance);

            bool xWrap = mover.hasXWrapping;
            bool yWrap = mover.hasYWrapping;

            uint iter = 0;
            BoardCoord coord;
            List<BoardCoord> moves = new List<BoardCoord>();
            while (true) {
                iter++;
                x += xModifier;
                y += yModifier;
                if (xWrap) x = MathExtensions.mod(x, Board.GetWidth());
                if (yWrap) y = MathExtensions.mod(y, Board.GetHeight());
                coord = new BoardCoord(x, y);

                if (Board.ContainsCoord(coord) == false) break;
                if (IsAlly(mover, coord)) break;
                if (IsThreat(mover, coord) == false && threatsOnly) break;
                moves.Add(coord);

                if (iter == cap) break;
            }
            return moves.ToArray();
        }

        /// <summary>
        /// Calculates to see if a specific move for a chess piece can be made.
        /// </summary>
        /// <param name="mover">Piece to move.</param>
        /// <param name="destination">Destination to move to.</param>
        /// <param name="threatOnly">Destination occupier must be a threat?</param>
        /// <returns>The coordinate that the piece can move to; otherwise NULL.</returns>
        public BoardCoord TryGetSpecificMove(ChessPiece mover, BoardCoord destination, bool threatOnly = false) {
            if (Board.ContainsCoord(destination)) {
                if (threatOnly && (IsThreat(mover, destination) == false)) {
                    return BoardCoord.NULL;
                } else if (IsAlly(mover, destination) == false) {
                    return destination;
                }
            }
            return BoardCoord.NULL;
        }

        /// <summary>
        /// Returns the x and y step values for a specific move direction.
        /// </summary>
        /// <param name="mover">Piece to move.</param>
        /// <param name="dir">Move direction to test.</param>
        /// <param name="xModifier">X step value to be returned.</param>
        /// <param name="yModifier">Y step value to be returned.</param>
        /// <param name="teamSensitive">Is the move direction relative to the team or to the game board?</param>
        public void GetMoveDirectionModifiers(ChessPiece mover, MoveDirection dir, out int xModifier, out int yModifier, bool teamSensitive = true) {
            switch (dir) {
                case MoveDirection.Up:
                    xModifier = 0;
                    yModifier = (teamSensitive) ? mover.TeamSensitiveMove(1) : 1;
                    break;
                case MoveDirection.Down:
                    xModifier = 0;
                    yModifier = (teamSensitive) ? mover.TeamSensitiveMove(-1) : -1;
                    break;
                case MoveDirection.Left:
                    xModifier = (teamSensitive) ? mover.TeamSensitiveMove(-1) : -1;
                    yModifier = 0;
                    break;
                case MoveDirection.Right:
                    xModifier = (teamSensitive) ? mover.TeamSensitiveMove(1) : 1;
                    yModifier = 0;
                    break;
                case MoveDirection.UpLeft:
                    xModifier = (teamSensitive) ? mover.TeamSensitiveMove(-1) : -1;
                    yModifier = (teamSensitive) ? mover.TeamSensitiveMove(1) : 1;
                    break;
                case MoveDirection.UpRight:
                    xModifier = (teamSensitive) ? mover.TeamSensitiveMove(1) : 1;
                    yModifier = (teamSensitive) ? mover.TeamSensitiveMove(1) : 1;
                    break;
                case MoveDirection.DownLeft:
                    xModifier = (teamSensitive) ? mover.TeamSensitiveMove(-1) : -1;
                    yModifier = (teamSensitive) ? mover.TeamSensitiveMove(-1) : -1;
                    break;
                case MoveDirection.DownRight:
                    xModifier = (teamSensitive) ? mover.TeamSensitiveMove(1) : 1;
                    yModifier = (teamSensitive) ? mover.TeamSensitiveMove(-1) : -1;
                    break;
                default:
                    xModifier = 0;
                    yModifier = 0;
                    break;
            }
        }
    }
}
