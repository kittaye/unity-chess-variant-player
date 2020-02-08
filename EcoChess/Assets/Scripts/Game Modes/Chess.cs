using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

public enum Team { WHITE, BLACK }
public enum MoveDirection { Up, Down, Left, Right, UpLeft, UpRight, DownLeft, DownRight }

namespace ChessGameModes {
    /// <summary>
    /// This is the base class for all other chess gamemodes.
    /// 
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

        public Board Board { get; private set; }

        // Use these properties to change the behaviour of base virtual methods without needing to override them.
        // Useful when the chess variant is only slightly different from base chess rules.
        public bool AllowPawnPromotion { get; protected set; }
        public Piece[] PawnPromotionOptions { get; protected set; }
        public Piece SelectedPawnPromotion { get; protected set; }
        public bool AllowCastling { get; protected set; }
        public Piece[] CastlerOptions { get; protected set; }
        public bool AllowEnpassantCapture { get; protected set; }

        public int NotationTurnDivider { get; protected set; }
        public Stack<string> GameMoveNotations { get; protected set; }

        protected const int BOARD_WIDTH = 8;
        protected const int BOARD_HEIGHT = 8;
        protected const int WHITE_BACKROW = 0;
        protected const int WHITE_PAWNROW = 1;
        protected int BLACK_BACKROW;
        protected int BLACK_PAWNROW;
        protected int castlingDistance;
        protected bool checkingForCheck;
        protected List<ChessPiece> opposingTeamCheckThreats;
        protected ChessPiece currentRoyalPiece;
        protected ChessPiece opposingRoyalPiece;

        private bool checkingSimulatedMove;
        private uint numConsecutiveCapturelessMoves;
        private Team currentTeamTurn;
        private Team opposingTeamTurn;
        private List<ChessPiece> whitePieces;
        private List<ChessPiece> blackPieces;
        private ChessPiece lastMovedWhitePiece;
        private ChessPiece lastMovedBlackPiece;
        private Stack<TeamTurnState> teamTurnStateHistory;

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

        /// <summary>
        /// Initialise all variables to match basic chess rules.
        /// </summary>
        private void Init() {
            whitePieces = new List<ChessPiece>();
            blackPieces = new List<ChessPiece>();
            numConsecutiveCapturelessMoves = 0;
            castlingDistance = 2;
            Board.allowFlipping = false;
            checkingForCheck = false;
            checkingSimulatedMove = false;

            AllowCastling = true;
            AllowEnpassantCapture = true;
            AllowPawnPromotion = true;
            CastlerOptions = new Piece[] { Piece.Rook };
            PawnPromotionOptions = new Piece[4] { Piece.Queen, Piece.Rook, Piece.Bishop, Piece.Knight };
            SelectedPawnPromotion = Piece.Queen;

            NotationTurnDivider = 2;
            GameMoveNotations = new Stack<string>(30);

            BLACK_BACKROW = Board.GetHeight() - 1;
            BLACK_PAWNROW = Board.GetHeight() - 2;

            currentTeamTurn = Team.WHITE;
            opposingTeamTurn = Team.BLACK;
            currentRoyalPiece = null;
            opposingRoyalPiece = null;
            lastMovedWhitePiece = null;
            lastMovedBlackPiece = null;
            opposingTeamCheckThreats = null;
            teamTurnStateHistory = new Stack<TeamTurnState>(30);
        }

        public override string ToString() {
            return "Traditional Chess";
        }

        public virtual VariantHelpDetails GetVariantHelpDetails() {
            return new VariantHelpDetails(
                this.ToString(),
                "Standardized in the 19th century",
                this.ToString() + " is the FIDE standardised ruleset for chess.",
                "Checkmate.",
                VariantHelpDetails.rule_None,
                "https://en.wikipedia.org/wiki/Chess"
            );
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

        /// <summary>
        /// Defines the captureless move limit rule.
        /// </summary>
        /// <returns>True if 50 turns (50 * 2 moves in FIDE chess) passes without a single capture.</returns>
        public bool CapturelessMovesLimit() {
            if (numConsecutiveCapturelessMoves >= 50 * NotationTurnDivider) {
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

        /// <summary>
        /// From the piece's template moves, get those that would not put the current royal piece in check.
        /// </summary>
        protected List<BoardCoord> GetLegalTemplateMoves(ChessPiece mover) {
            List<BoardCoord> legalMoves = new List<BoardCoord>();
            BoardCoord[] templateMoves = mover.CalculateTemplateMoves().ToArray();

            for (int i = 0; i < templateMoves.Length; i++) {
                if (IsPieceInCheckAfterThisMove(currentRoyalPiece, mover, templateMoves[i]) == false) {
                    legalMoves.Add(templateMoves[i]);
                }
            }

            return legalMoves;
        }

        /// <summary>
        /// Calculates the currently available moves for a selected piece.
        /// </summary>
        public virtual List<BoardCoord> CalculateAvailableMoves(ChessPiece mover) {
            List<BoardCoord> availableMoves = new List<BoardCoord>();

            availableMoves.AddRange(GetLegalTemplateMoves(mover));

            if (IsRoyal(mover)) {
                if (AllowCastling) {
                    availableMoves.AddRange(TryAddAvailableCastleMoves(mover, CastlerOptions, castlingDistance));
                }
            } else if (mover is Pawn) {
                if (AllowEnpassantCapture) {
                    availableMoves.AddRange(TryAddAvailableEnPassantMoves((Pawn)mover));
                }
            }

            return availableMoves;
        }

        protected bool IsRoyal(ChessPiece piece) {
            return piece == currentRoyalPiece || piece == opposingRoyalPiece;
        }

        /// <summary>
        /// Moves a selected piece to a destination.
        /// </summary>
        /// <returns>True if the destination is an available move for this piece.</returns>
        public virtual bool MovePiece(ChessPiece mover, BoardCoord destination) {
            // Make the move. If successful, try perform special cases.
            if (MakeDirectMove(mover, destination)) {
                if (IsRoyal(mover)) {
                    if (AllowCastling) {
                        TryPerformCastlingMove(mover);
                    }
                }

                if (mover is Pawn) {
                    if (AllowEnpassantCapture) {
                        TryPerformPawnEnPassantCapture((Pawn)mover);
                    }

                    if (AllowPawnPromotion) {
                        TryPerformPawnPromotion((Pawn)mover);
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
        /// <param name="moveNotation">A reference to the current move notation.</param>
        /// <returns>True if the castle is successful.</returns>
        protected virtual bool TryPerformCastlingMove(ChessPiece mover) {
            if (mover.MoveCount == 1) {
                // If the king moved to the left to castle, grab the rook on the left-side of the board to castle with and move it.
                if (mover.GetBoardPosition().x == 2) {
                    ChessPiece castlingPiece = Board.GetCoordInfo(new BoardCoord(0, mover.GetBoardPosition().y)).GetAliveOccupier();
                    UpdatePiecePositionAndOccupance(castlingPiece, new BoardCoord(3, mover.GetBoardPosition().y));
                    SetLastMoveNotationToQueenSideCastle();
                    return true;

                    // Else the king moved right, so grab the right rook instead.
                } else if (mover.GetBoardPosition().x == 6) {
                    ChessPiece castlingPiece = Board.GetCoordInfo(new BoardCoord(BOARD_WIDTH - 1, mover.GetBoardPosition().y)).GetAliveOccupier();
                    UpdatePiecePositionAndOccupance(castlingPiece, new BoardCoord(5, mover.GetBoardPosition().y));
                    SetLastMoveNotationToKingSideCastle();
                    return true;
                }
            }
            return false;
        }

        protected void UpdatePiecePositionAndOccupance(ChessPiece piece, BoardCoord newPosition) {
            BoardCoord previousPosition = piece.GetBoardPosition();

            UpdateSquareOccupiers(piece, previousPosition, newPosition);

            piece.SetBoardPosition(newPosition);
            piece.gameObject.transform.position = newPosition;
        }

        /// <summary>
        /// Called in CalculateAvailableMoves. If an enpassant move is available, returns the enpassant move.
        /// In most cases there is only one enpassant move. The exception is in Monster chess, where a black pawn could
        /// perform en passant on two different white pawns.
        /// TODO: allow multiple enpassant options (Monster variant)
        /// </summary>
        protected virtual BoardCoord[] TryAddAvailableEnPassantMoves(Pawn mover) {
            const int LEFT = -1;
            const int RIGHT = 1;
            List<BoardCoord> enpassantMoves = new List<BoardCoord>(1);

            if (mover.canEnPassantCapture) {
                for (int i = LEFT; i <= RIGHT; i += 2) {
                    BoardCoord coord = TryGetSpecificMove(mover, mover.GetRelativeBoardCoord(i, 0), threatOnly: true);
                    if (Board.ContainsCoord(coord)) {
                        ChessPiece piece = Board.GetCoordInfo(coord).GetAliveOccupier();
                        if (piece is Pawn && CheckEnPassantVulnerability((Pawn)piece)) {
                            if (IsPieceInCheckAfterThisMove(currentRoyalPiece, mover, mover.GetRelativeBoardCoord(i, 1)) == false) {
                                enpassantMoves.Add(TryGetSpecificMove(mover, mover.GetRelativeBoardCoord(i, 1)));
                            }
                        }
                    }
                }
            }
            return enpassantMoves.ToArray();
        }

        /// <summary>
        /// Checks if the pawn is a valid candidate for being en passant captured.
        /// </summary>
        protected bool CheckEnPassantVulnerability(Pawn piece) {
            BoardCoord oldPos = piece.MoveStateHistory[GameMoveNotations.Count - 1].position;
            return (piece.MoveCount == 1 && piece == GetLastMovedPiece(piece.GetTeam()) && piece.GetRelativeBoardCoord(0, -1) != oldPos);
        }

        /// <summary>
        /// Called in MovePiece. If an enpassant move was made, enpassant capture is performed.
        /// </summary>
        /// <param name="moveNotation">A reference to the current move notation.</param>
        /// <returns>The piece that was removed.</returns>
        protected virtual Pawn TryPerformPawnEnPassantCapture(Pawn mover) {
            BoardCoord oldPos = mover.MoveStateHistory[GameMoveNotations.Count - 1].position;
            BoardCoord newPos = mover.GetBoardPosition();

            if (Board.ContainsCoord(mover.GetRelativeBoardCoord(0, -1)) && IsThreat(mover, mover.GetRelativeBoardCoord(0, -1))) {
                ChessPiece occupier = Board.GetCoordInfo(mover.GetRelativeBoardCoord(0, -1)).GetAliveOccupier();
                if (occupier != null && occupier is Pawn && CheckEnPassantVulnerability((Pawn)occupier)) {
                    mover.CaptureCount++;
                    KillPiece(occupier);

                    SetLastMoveNotationToEnPassant(oldPos, newPos);
                    return (Pawn)occupier;
                }
            }
            return null;
        }

        /// <summary>
        /// Called in CalculateAvailableMoves. Determines if a pawn can promote on their move.
        /// </summary>
        /// <param name="availableMoves">The pawn's available moves to check.</param>
        protected bool IsPromotionMoveAvailable(Pawn mover, BoardCoord[] availableMoves) {
            for (int i = 0; i < availableMoves.Length; i++) {
                if (IsAPromotionMove(availableMoves[i])) {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Called in MovePiece. If a promoting move was made, the pawn is removed from the board and replaced with the selected pawn promotion.
        /// TODO: The only variant using this virtual method is Sovereign chess. Maybe find a way to remove this?
        /// </summary>
        protected virtual ChessPiece TryPerformPawnPromotion(Pawn mover) {
            if (PerformedAPromotionMove(mover)) {
                KillPiece(mover);

                ChessPiece newPromotedPiece = ChessPieceFactory.Create(SelectedPawnPromotion, mover.GetTeam(), mover.GetBoardPosition());

                AddPromotionToLastMoveNotation(newPromotedPiece.GetLetterNotation());
                return AddPieceToBoard(newPromotedPiece);
            }
            return null;
        }

        /// <summary>
        /// Determines whether the move is a potential promotion move. Override this to change the condition for potential promotion moves.
        /// </summary>
        protected virtual bool IsAPromotionMove(BoardCoord move) {
            return move.y == WHITE_BACKROW || move.y == BLACK_BACKROW;
        }

        /// <summary>
        /// Determines whether the mover should be promoted. Override this to change the condition for piece promotion.
        /// </summary>
        protected virtual bool PerformedAPromotionMove(Pawn mover) {
            return mover.GetRelativeBoardCoord(0, 1).y < WHITE_BACKROW || mover.GetRelativeBoardCoord(0, 1).y > BLACK_BACKROW;
        }

        /// <summary>
        /// Simulates a move, checks whether the piece-to-check is in check, then reverts the simulated move.
        /// </summary>
        protected virtual bool IsPieceInCheckAfterThisMove(ChessPiece pieceToCheck, ChessPiece mover, BoardCoord destination) {
            if (AssertContainsCoord(destination)) {
                if (checkingForCheck) return false;

                SimulateMove(mover, destination);

                bool isInCheck = IsPieceInCheck(pieceToCheck);

                RevertSimulatedMove();

                return isInCheck;
            }
            return false;
        }

        /// <summary>
        /// Determines whether the piece-to-check is currently in check.
        /// </summary>
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
        /// <returns>A list of chess pieces that can check the piece-to-check.</returns>
        protected virtual List<ChessPiece> GetAllPossibleCheckThreats(ChessPiece pieceToCheck) {
            List<ChessPiece> possibleCheckThreats = new List<ChessPiece>();

            for (int i = (int)MoveDirection.Up; i <= (int)MoveDirection.DownRight; i++) {
                int xModifier, yModifier;
                GetMoveDirectionModifiers(pieceToCheck, (MoveDirection)i, out xModifier, out yModifier);
                BoardCoord coord = pieceToCheck.GetBoardPosition() + new BoardCoord(xModifier, yModifier);

                while (Board.ContainsCoord(coord)) {
                    if (IsThreat(pieceToCheck, coord)) {
                        possibleCheckThreats.Add(Board.GetCoordInfo(coord).GetAliveOccupier());
                    }
                    coord.x += xModifier;
                    coord.y += yModifier;
                }
            }

            possibleCheckThreats.AddRange(GetAlivePiecesOfType<Knight>(pieceToCheck.GetOpposingTeam()));

            return possibleCheckThreats;
        }

        public virtual void DisplayPromotionOptionsUIIfCanPromote(ChessPiece mover, BoardCoord[] availableMoves) {
            if (IsPromotionMoveAvailable((Pawn)mover, availableMoves)) {
                OnDisplayPromotionUI(true);
            }
        }

        /// <summary>
        /// Called in CalculateAvailableMoves. Determines if a castling move can be made for a chess piece.
        /// </summary>
        /// <param name="castler">Moving piece.</param>
        /// <param name="castleTypes">Which types of pieces can the moving piece castle with?</param>
        /// <param name="castlingDistance">How many squares should the piece move when castling?</param>
        /// <param name="canCastleLeftward">Can the piece castle leftwards?</param>
        /// <param name="canCastleRightward">Can the piece castle rightwards?</param>
        /// <returns>A list of board coordinates for available castle moves.</returns>
        protected virtual BoardCoord[] TryAddAvailableCastleMoves(ChessPiece castler, Piece[] castlerOptions, int castlingDistance = 2, bool canCastleLeftward = true, bool canCastleRightward = true) {
            const int LEFT = -1;
            const int RIGHT = 1;

            if(castler.MoveCount > 0 || castlerOptions.Length == 0) {
                return new BoardCoord[0];
            }

            if (IsPieceInCheck(castler) == false) {
                List<BoardCoord> castleMoves = new List<BoardCoord>(2);

                for (int i = LEFT; i <= RIGHT; i += 2) {
                    if (!canCastleLeftward && i == LEFT) continue;
                    if (!canCastleRightward && i == RIGHT) break;

                    int x = castler.GetBoardPosition().x + i;
                    int y = castler.GetBoardPosition().y;
                    BoardCoord coord = new BoardCoord(x, y);

                    while (Board.ContainsCoord(coord)) {
                        ChessPiece occupier = Board.GetCoordInfo(coord).GetAliveOccupier();
                        if (occupier != null) {
                            bool validCastler = false;
                            for (int j = 0; j < castlerOptions.Length; j++) {
                                if(occupier.GetPieceType() == castlerOptions[j]) {
                                    validCastler = true;
                                    break;
                                }
                            }

                            if (validCastler && occupier.MoveCount == 0) {
                                bool inCheck = false;
                                for (int k = 1; k <= castlingDistance; k++) {
                                    if (IsPieceInCheckAfterThisMove(castler, castler, castler.GetBoardPosition() + new BoardCoord(i * k, 0))) {
                                        inCheck = true;
                                        break;
                                    }
                                }

                                if (!inCheck) {
                                    castleMoves.Add(TryGetSpecificMove(castler, castler.GetBoardPosition() + new BoardCoord(i * castlingDistance, 0)));
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
        protected bool TeamHasAnyMoves(Team team) {
            foreach (ChessPiece piece in GetPiecesOfType<ChessPiece>(team)) {
                if (piece.IsAlive) {
                    if (CalculateAvailableMoves(piece).Count > 0) {
                        return true;
                    }
                }
            }
            return false;
        }

        protected void AddCheckToLastMoveNotation() {
            string moveNotation = GameMoveNotations.Pop();
            moveNotation += "+";
            GameMoveNotations.Push(moveNotation);
        }

        protected void AddCheckmateToLastMoveNotation() {
            string moveNotation = GameMoveNotations.Pop();
            moveNotation += "#";
            GameMoveNotations.Push(moveNotation);
        }

        protected void AddPromotionToLastMoveNotation(string promotedPieceNotation) {
            string moveNotation = GameMoveNotations.Pop();
            moveNotation += string.Format("={0}", promotedPieceNotation);
            GameMoveNotations.Push(moveNotation);
        }

        protected void SetLastMoveNotationToEnPassant(BoardCoord oldPos, BoardCoord newPos) {
            string moveNotation = GameMoveNotations.Pop();
            moveNotation = Board.GetCoordInfo(oldPos).file + "x" + Board.GetCoordInfo(newPos).algebraicKey + "e.p.";
            GameMoveNotations.Push(moveNotation);
        }

        protected void SetLastMoveNotationToKingSideCastle() {
            string moveNotation = GameMoveNotations.Pop();
            moveNotation = "O-O";
            GameMoveNotations.Push(moveNotation);
        }

        protected void SetLastMoveNotationToQueenSideCastle() {
            string moveNotation = GameMoveNotations.Pop();
            moveNotation = "O-O-O";
            GameMoveNotations.Push(moveNotation);
        }

        /// <summary>
        /// Defines how the chess game is won.
        /// </summary>
        /// <returns>True if a team has won.</returns>
        public virtual bool CheckWinState() {
            bool isThreateningCheck = IsPieceInCheck(currentRoyalPiece);

            if (!TeamHasAnyMoves(GetCurrentTeamTurn())) {
                if (isThreateningCheck) {
                    AddCheckmateToLastMoveNotation();
                    UIManager.Instance.LogCheckmate(GetOpposingTeamTurn().ToString(), GetCurrentTeamTurn().ToString());
                } else {
                    UIManager.Instance.LogStalemate(GetCurrentTeamTurn().ToString());
                }
                return true;

            } else if (isThreateningCheck) {
                AddCheckToLastMoveNotation();
            }

            if (CapturelessMovesLimit()) {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the last moved piece from a specific team.
        /// </summary>
        public ChessPiece GetLastMovedPiece(Team team) {
            if (team == Team.WHITE) {
                return lastMovedWhitePiece;
            } else {
                return lastMovedBlackPiece;
            }
        }

        /// <summary>
        /// Sets the last moved piece. This will update the last mover of the piece's team.
        /// </summary>
        public void SetLastMovedPiece(ChessPiece piece) {
            if (piece != null) {
                if (piece.GetTeam() == Team.WHITE) {
                    lastMovedWhitePiece = piece;
                } else {
                    lastMovedBlackPiece = piece;
                }
            }
        }

        /// <summary>
        /// Gets the last moved opposing team's piece based on the mover's team.
        /// </summary>
        /// <param name="mover">Piece to determine the opposing team.</param>
        /// <returns>The last moved piece from the opposing team based on the mover's team.</returns>
        protected ChessPiece GetLastMovedOpposingPiece(ChessPiece mover) {
            if (mover.GetTeam() == Team.WHITE) {
                return GetLastMovedPiece(Team.BLACK);
            } else {
                return GetLastMovedPiece(Team.WHITE);
            }
        }

        /// <summary>
        /// Display the UI for showing pawn promotion options to choose from.
        /// </summary>
        protected void OnDisplayPromotionUI(bool value) {
            if (_DisplayPromotionUI != null) _DisplayPromotionUI.Invoke(true);
        }

        /// <summary>
        /// Set the current pawn promotion option to a specified piece.
        /// </summary>
        public virtual void SetPawnPromotionTo(Piece piece) {
            this.SelectedPawnPromotion = piece;
        }

        public List<T> GetPiecesOfType<T>() where T : ChessPiece {
            List<T> selectionOfPieces = new List<T>(whitePieces.Count + blackPieces.Count);

            selectionOfPieces.AddRange(GetPiecesOfType<T>(Team.WHITE));
            selectionOfPieces.AddRange(GetPiecesOfType<T>(Team.BLACK));

            return selectionOfPieces;
        }

        public List<T> GetAlivePiecesOfType<T>() where T : ChessPiece {
            List<T> selectionOfPieces = new List<T>();

            selectionOfPieces.AddRange(GetAlivePiecesOfType<T>(Team.WHITE));
            selectionOfPieces.AddRange(GetAlivePiecesOfType<T>(Team.BLACK));

            return selectionOfPieces;
        }

        public List<T> GetPiecesOfType<T>(Team team) where T : ChessPiece {
            List<ChessPiece> selectedArmy = GetPiecesFrom(team);
            return GetPiecesOfType<T>(selectedArmy);
        }

        public List<T> GetAlivePiecesOfType<T>(Team team) where T : ChessPiece {
            List<ChessPiece> selectedArmy = GetPiecesFrom(team);
            return GetAlivePiecesOfType<T>(selectedArmy);
        }

        private List<T> GetPiecesOfType<T>(List<ChessPiece> pieceCollection) where T : ChessPiece {
            List<T> selectionOfPieces = new List<T>();

            foreach (ChessPiece piece in pieceCollection) {
                if (piece is T) {
                    selectionOfPieces.Add((T)piece);
                }
            }
            return selectionOfPieces;
        }

        private List<T> GetAlivePiecesOfType<T>(List<ChessPiece> pieceCollection) where T : ChessPiece {
            List<T> selectionOfPieces = new List<T>();

            foreach (ChessPiece piece in pieceCollection) {
                if (piece is T && piece.IsAlive) {
                    selectionOfPieces.Add((T)piece);
                }
            }
            return selectionOfPieces;
        }

        private List<ChessPiece> GetPiecesFrom(Team team) {
            List<ChessPiece> selectedArmy = new List<ChessPiece>();

            if (team == Team.WHITE) {
                selectedArmy = whitePieces;
            } else {
                selectedArmy = blackPieces;
            }

            return selectedArmy;
        }

        protected virtual bool IsAlly(ChessPiece mover, BoardCoord coord) {
            ChessPiece occupier = Board.GetCoordInfo(coord).GetAliveOccupier();
            if (occupier != null) {
                return !IsThreat(mover, occupier);
            }
            return false;
        }

        protected bool IsAlly(ChessPiece mover, ChessPiece target) {
            return !IsThreat(mover, target);
        }

        protected virtual bool IsThreat(ChessPiece mover, BoardCoord coord) {
            ChessPiece occupier = Board.GetCoordInfo(coord).GetAliveOccupier();
            if (occupier != null) {
                return IsThreat(mover, occupier);
            }
            return false;
        }

        protected bool IsThreat(ChessPiece mover, ChessPiece target) {
            return target.GetTeam() != mover.GetTeam();
        }

        /// <summary>
        /// Returns a string that describes who's turn it is currently.
        /// </summary>
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
        protected void AddPieceToTeam(ChessPiece piece) {
            if (piece.GetTeam() == Team.WHITE) {
                whitePieces.Add(piece);
            } else {
                blackPieces.Add(piece);
            }
        }

        /// <summary>
        /// Removes a chess piece from it's team.
        /// </summary>
        /// <returns>True if the removal was successful.</returns>
        protected bool RemovePieceFromTeam(ChessPiece piece) {
            if (piece.GetTeam() == Team.WHITE) {
                return whitePieces.Remove(piece);
            } else {
                return blackPieces.Remove(piece);
            }
        }

        /// <summary>
        /// Used to update the occupiers of affected squares after a move is played.
        /// </summary>
        private void UpdateSquareOccupiers(ChessPiece piece, BoardCoord previousPosition, BoardCoord newPosition) {
            Board.GetCoordInfo(previousPosition).RemoveOccupier(piece);
            Board.GetCoordInfo(newPosition).AddOccupier(piece);
        }

        /// <summary>
        /// Directly moves a chess piece from it's current position to the destination. Ignores promotions, enpassant, and castling rules. 
        /// If rules are desired, use the virtual method MovePiece for more flexible behaviour. 
        /// Ensure that this method is always called for moving pieces.
        /// </summary>
        protected bool MakeDirectMove(ChessPiece mover, BoardCoord destination) {
            if (AssertContainsCoord(destination)) {
                StringBuilder moveNotation = new StringBuilder(null, 4);
                BoardCoord previousPosition = mover.GetBoardPosition();
                bool attackingThreat = IsThreat(mover, destination);

                mover.MoveCount++;
                SetLastMovedPiece(mover);

                moveNotation.Append(mover.GetLetterNotation());

                // This condition is needed to avoid a stack overflow when checking for check.
                if (checkingSimulatedMove == false) {
                    moveNotation.Append(ResolveMoveNotationAmbiguity(mover, destination));
                }

                if (attackingThreat) {
                    KillPiece(Board.GetCoordInfo(destination).GetAliveOccupier());

                    if (mover is Pawn) {
                        moveNotation.Append(Board.GetCoordInfo(previousPosition).file);
                    }
                    moveNotation.Append('x');
                    mover.CaptureCount++;
                }

                numConsecutiveCapturelessMoves = (attackingThreat == false && (mover is Pawn) == false) ? numConsecutiveCapturelessMoves + 1 : 0;

                moveNotation.Append(Board.GetCoordInfo(destination).algebraicKey);

                // Physically move the piece.
                UpdatePiecePositionAndOccupance(mover, destination);

                GameMoveNotations.Push(moveNotation.ToString());
                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns the turn number for each team's moves in a turn.
        /// </summary>
        public int GetNotationTurn() {
            return Mathf.CeilToInt((float)GameMoveNotations.Count / NotationTurnDivider);
        }

        /// <summary>
        /// Updates all pieces' state histories to be up to date with the total move count. Is called by GameManager when a move is completed.
        /// </summary>
        public void UpdateGameStateHistory() {
            teamTurnStateHistory.Push(new TeamTurnState(currentTeamTurn, opposingTeamTurn, currentRoyalPiece, opposingRoyalPiece,
                GetLastMovedPiece(opposingTeamTurn), GetLastMovedPiece(currentTeamTurn), (uint)GetNumConseqCapturelessMoves()));

            foreach (ChessPiece piece in GetPiecesOfType<ChessPiece>()) {
                piece.UpdatePieceMoveStateHistory(GameMoveNotations.Count);
            }
        }

        /// <summary>
        /// Resolves notation ambiguity between same-type pieces that can move to the same destination.
        /// </summary>
        /// <returns>The resolved notation to be appended.</returns>
        public virtual string ResolveMoveNotationAmbiguity(ChessPiece mover, BoardCoord destination) {
            string result = string.Empty;

            int numAmbiguousMovers = 0;
            bool atLeastOneFileMatched = false;
            bool atLeastOneRankMatched = false;

            foreach (ChessPiece piece in GetPiecesOfType<ChessPiece>(mover.GetTeam())) {
                if (piece != mover && piece.GetPieceType() == mover.GetPieceType()) {
                    if (piece.CalculateTemplateMoves().Contains(destination)) {
                        if (IsPieceInCheckAfterThisMove(currentRoyalPiece, piece, destination) == false) {
                            // If so, perform file/rank comparisons.
                            numAmbiguousMovers++;

                            // If the files are the same...
                            if (Board.GetCoordInfo(mover.GetBoardPosition()).file == Board.GetCoordInfo(piece.GetBoardPosition()).file) {
                                atLeastOneFileMatched = true;
                            }
                            // If the ranks are the same...
                            if (Board.GetCoordInfo(mover.GetBoardPosition()).rank == Board.GetCoordInfo(piece.GetBoardPosition()).rank) {
                                atLeastOneRankMatched = true;
                            }
                        }
                    }
                }
            }

            if ((atLeastOneRankMatched && !atLeastOneFileMatched) || (numAmbiguousMovers > 0 && !atLeastOneRankMatched && !atLeastOneFileMatched)) {
                result = Board.GetCoordInfo(mover.GetBoardPosition()).file;
            } else if (atLeastOneFileMatched && !atLeastOneRankMatched) {
                result = Board.GetCoordInfo(mover.GetBoardPosition()).rank;
            } else if (atLeastOneFileMatched && atLeastOneRankMatched) {
                // If both file and rank matched, that means there at least 2 other movers, so the whole key is required.
                result = Board.GetCoordInfo(mover.GetBoardPosition()).algebraicKey;
            }

            return result;
        }

        /// <summary>
        /// Called after a move is played.
        /// </summary>
        public virtual void OnMoveComplete() {
            currentTeamTurn = (currentTeamTurn == Team.WHITE) ? Team.BLACK : Team.WHITE;
            opposingTeamTurn = (currentTeamTurn == Team.WHITE) ? Team.BLACK : Team.WHITE;

            SwapCurrentAndOpposingRoyaltyPieces();

            SelectedPawnPromotion = Piece.Queen;
        }

        protected virtual void SwapCurrentAndOpposingRoyaltyPieces() {
            if (currentRoyalPiece != null && opposingRoyalPiece != null) {
                ChessPiece temp = currentRoyalPiece;
                currentRoyalPiece = opposingRoyalPiece;
                opposingRoyalPiece = temp;
            }
        }

        /// <summary>
        /// Kills a chess piece and hides it from the board.
        /// </summary>
        /// <returns>True if the kill was successful.</returns>
        protected bool KillPiece(ChessPiece piece) {
            if (piece != null) {
                piece.gameObject.SetActive(false);
                piece.IsAlive = false;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Reverts all chess pieces' states to the previous move.
        /// </summary>
        public virtual void UndoLastMove() {
            // Remove last game notation that was undone.
            GameMoveNotations.Pop();

            // Undo current game status.
            if (teamTurnStateHistory.Count > 1) {
                teamTurnStateHistory.Pop();
                TeamTurnState teamTurnStateToRestore = teamTurnStateHistory.Peek();
                currentTeamTurn = teamTurnStateToRestore.currentTeam;
                opposingTeamTurn = teamTurnStateToRestore.opposingTeam;
                currentRoyalPiece = teamTurnStateToRestore.currentRoyalPiece;
                opposingRoyalPiece = teamTurnStateToRestore.opposingRoyalPiece;
                SetLastMovedPiece(teamTurnStateToRestore.lastMovedWhitePiece);
                SetLastMovedPiece(teamTurnStateToRestore.lastMovedBlackPiece);
                numConsecutiveCapturelessMoves = teamTurnStateToRestore.numConsequtiveCapturelessMoves;
            }

            foreach (ChessPiece piece in GetPiecesOfType<ChessPiece>()) {
                if (piece.MoveStateHistory.Count > 1) {
                    // Remove piece state to be undone.
                    piece.MoveStateHistory.Remove(GameMoveNotations.Count + 1);

                    PieceMoveState pieceMoveStateToRestore = piece.MoveStateHistory[GameMoveNotations.Count];

                    // Undo alive status.
                    piece.IsAlive = pieceMoveStateToRestore.wasAlive;
                    piece.gameObject.SetActive(piece.IsAlive);

                    // Undo position & occupance.
                    UpdatePiecePositionAndOccupance(piece, pieceMoveStateToRestore.position);

                    // Undo move/capture counts.
                    piece.MoveCount = pieceMoveStateToRestore.moveCount;
                    piece.CaptureCount = pieceMoveStateToRestore.captureCount;

                } else if (piece.MoveStateHistory.Count == 1 && piece.IsAlive) {
                    // This occurs for promoted pieces. A promoted pawn is considered a new piece, so once the new piece has no states left,
                    // we completely remove it from the game and bring back the pawn.
                    DestoryPieceFromBoard(piece);
                }
            }
        }

        /// <summary>
        /// Adds a chess piece to the game board and assigns it to a team based on it's own team value.
        /// </summary>
        /// <returns>Returns the chess piece added to the game board.</returns>
        protected ChessPiece AddPieceToBoard(ChessPiece piece) {
            if (CheckValidPlacement(piece)) {
                Board.GetCoordInfo(piece.GetBoardPosition()).AddOccupier(piece);
                piece.IsAlive = true;
                GameManager.Instance.InstantiateChessPiece(piece);
                AddPieceToTeam(piece);

                if (piece.GetTeam() == Team.BLACK && Board.isFlipped) {
                    piece.gameObject.transform.Rotate(new Vector3(0, 0, 180));
                }
                return piece;
            }
            return null;
        }

        /// <summary>
        /// Completely destroys a chess piece. Only used when a promoted piece is undo'd back to a pawn.
        /// </summary>
        /// <param name="piece"></param>
        private void DestoryPieceFromBoard(ChessPiece piece) {
            KillPiece(piece);
            RemovePieceFromTeam(piece);
            GameManager.Instance.DestroyChessPiece(piece);
        }

        /// <summary>
        /// Used to ensure the chess piece added to the game board has been placed in a valid position.
        /// </summary>
        private bool CheckValidPlacement(ChessPiece piece) {
            if (AssertContainsCoord(piece.GetBoardPosition()) == false) {
                return false;
            } else if (Board.GetCoordInfo(piece.GetBoardPosition()).GetAliveOccupier() != null && Board.GetCoordInfo(piece.GetBoardPosition()).GetAliveOccupier().IsAlive) {
                CoordInfo posInfo = Board.GetCoordInfo(piece.GetBoardPosition());
                Debug.LogErrorFormat("OCCUPIED EXCEPTION :: " +
                    "{0} failed to instantiate because a {1} is already at it's position! Location: {3}."
                    , piece.ToString(), posInfo.GetAliveOccupier().ToString(), posInfo.algebraicKey, piece.GetBoardPosition().ToString());
                return false;
            }
            return true;
        }

        /// <summary>
        /// Assert that the current game board contains a specified coordinate.
        /// </summary>
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
        public virtual bool IsMoversTurn(ChessPiece mover) {
            return mover.GetTeam() == currentTeamTurn;
        }

        /// <summary>
        /// Simulates a move being played. NOTE: Must be followed by RevertSimulatedMove.
        /// </summary>
        protected void SimulateMove(ChessPiece mover, BoardCoord destination) {
            checkingSimulatedMove = true;

            MovePiece(mover, destination);
            OnMoveComplete();
            UpdateGameStateHistory();
        }

        /// <summary>
        /// Reverts a simulated move. NOTE: Must be preceeded by SimulateMove.
        /// </summary>
        protected void RevertSimulatedMove() {
            checkingSimulatedMove = false;

            UndoLastMove();
        }

        /// <summary>
        /// Gets a list of moves that a chess piece can move to. This method is used to build up a chess piece's list of template moves.
        /// </summary>
        /// <param name="moveCap">Number of squares to test before stopping (0 = unbounded).</param>
        /// <param name="threatAttackLimit">Number of threats to test before stopping (0 = unbounded).</param>
        /// <param name="threatsOnly">Only get attacking moves?</param>
        /// <param name="teamSensitive">Is the move direction relative to the team or to the game board?</param>
        /// <returns></returns>
        public BoardCoord[] TryGetDirectionalMoves(ChessPiece mover, MoveDirection direction, uint moveCap = 0, uint threatAttackLimit = 1, bool threatsOnly = false, bool teamSensitive = true) {
            int xStep;
            int yStep;
            GetMoveDirectionModifiers(mover, direction, out xStep, out yStep, teamSensitive);

            return GetMovesInStepPattern(mover, xStep, yStep, moveCap, threatAttackLimit, threatsOnly);
        }

        /// <summary>
        /// Gets a list of moves that a chess piece can move to. This method is used to build up a chess piece's list of template moves.
        /// </summary>
        /// <param name="xVariance">Custom direction's x step from the mover's position.</param>
        /// <param name="yVariance">Custom direction's y step from the mover's position.</param>
        /// <param name="moveCap">Number of squares to test before stopping (0 = unbounded).</param>
        /// <param name="threatAttackLimit">Number of threats to test before stopping (0 = unbounded).</param>
        /// <param name="threatsOnly">Only get attacking moves?</param>
        /// <returns></returns>
        public BoardCoord[] TryGetDirectionalMoves(ChessPiece mover, int xVariance, int yVariance, uint moveCap = 0, uint threatAttackLimit = 1, bool threatsOnly = false) {
            int xStep = mover.TeamSensitiveMove(xVariance);
            int yStep = mover.TeamSensitiveMove(yVariance);

            return GetMovesInStepPattern(mover, xStep, yStep, moveCap, threatAttackLimit, threatsOnly);
        }

        /// <summary>
        /// Calculates to see if a specific move for a chess piece can be made.
        /// </summary>
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
        /// Loops indefinitely in a step pattern to get moves for a piece.
        /// </summary>
        private BoardCoord[] GetMovesInStepPattern(ChessPiece mover, int xStep, int yStep, uint cap = 0, uint threatAttackLimit = 1, bool threatsOnly = false) {
            int x = mover.GetBoardPosition().x;
            int y = mover.GetBoardPosition().y;
            uint iter = 0;
            uint threats = 0;
            BoardCoord coord;
            List<BoardCoord> moves = new List<BoardCoord>();

            while (true) {
                iter++;
                x += xStep;
                y += yStep;
                if (mover.HasXWrapping) x = MathExtensions.mod(x, Board.GetWidth());
                if (mover.HasYWrapping) y = MathExtensions.mod(y, Board.GetHeight());
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
        /// Returns the x and y step values for a specific move direction.
        /// </summary>
        /// <param name="teamSensitive">Is the move direction relative to the team or to the game board?</param>
        public void GetMoveDirectionModifiers(ChessPiece mover, MoveDirection direction, out int xModifier, out int yModifier, bool teamSensitive = true) {
            switch (direction) {
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
