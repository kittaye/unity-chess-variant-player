using System.Collections.Generic;
using System;
using System.Text;

public enum Team { WHITE, BLACK, NONE }
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
        public static event Action<bool> OnDisplayPromotionOptions;

        public event Action<ChessPiece, BoardCoord> OnChessPieceMoved;
        public event Action<ChessPiece> OnChessPieceCaptured;
        public event Action<ChessPiece> OnUndoChessPieceCapture;
        public event Action<ChessPiece> OnChessPieceCreated;
        public event Action<ChessPiece> OnChessPieceDestroyed;

        public event Action<string> OnLogInfoMessage;
        public event Action<string> OnLogErrorMessage;

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
        private Stack<GameStateSnapshot> gameStateHistory;

        public Chess() {
            Board = new Board(BOARD_WIDTH, BOARD_HEIGHT, new BoardColour(0.9f, 0.9f, 0.9f), new BoardColour(0.1f, 0.1f, 0.1f));
            Init();
        }

        public Chess(uint width, uint height) {
            Board = new Board(width, height, new BoardColour(0.9f, 0.9f, 0.9f), new BoardColour(0.1f, 0.1f, 0.1f));
            Init();
        }

        public Chess(uint width, uint height, BoardColour primaryBoardColour, BoardColour secondaryBoardColour) {
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
            gameStateHistory = new Stack<GameStateSnapshot>(30);
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
            currentRoyalPiece = (King)AddNewPieceToBoard(Piece.King, Team.WHITE, new BoardCoord(4, WHITE_BACKROW));
            opposingRoyalPiece = (King)AddNewPieceToBoard(Piece.King, Team.BLACK, new BoardCoord(4, BLACK_BACKROW));

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

        protected void RaiseEventOnLogInfoMessage(string message) {
            OnLogInfoMessage?.Invoke(message);
        }

        protected void RaiseEventOnLogErrorMessage(string message) {
            OnLogErrorMessage?.Invoke(message);
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
            if (MakeBaseMove(mover, destination)) {
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

            // Alert listeners that the piece has moved.
            OnChessPieceMoved?.Invoke(piece, newPosition);
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
                    BoardCoord sidewaysCoord = mover.GetRelativeBoardCoord(i, 0);

                    if (Board.ContainsCoord(sidewaysCoord) && mover.IsThreatTowards(sidewaysCoord)) {
                        ChessPiece piece = Board.GetCoordInfo(sidewaysCoord).GetAliveOccupier();

                        if (piece is Pawn && CheckEnPassantVulnerability((Pawn)piece)) {
                            BoardCoord enpassantCoord = mover.GetRelativeBoardCoord(i, 1);

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

        /// <summary>
        /// Checks if the pawn is a valid candidate for being en passant captured.
        /// </summary>
        protected bool CheckEnPassantVulnerability(Pawn piece) {
            BoardCoord oldPos = piece.StateHistory[piece.StateHistory.Count - 1].position;
            return (piece.MoveCount == 1 && piece == GetLastMovedPiece(piece.GetTeam()) && piece.GetRelativeBoardCoord(0, -1) != oldPos);
        }

        /// <summary>
        /// Called in MovePiece. If an enpassant move was made, enpassant capture is performed.
        /// </summary>
        /// <param name="moveNotation">A reference to the current move notation.</param>
        /// <returns>The piece that was removed.</returns>
        protected virtual Pawn TryPerformPawnEnPassantCapture(Pawn mover) {
            BoardCoord oldPos = mover.StateHistory[mover.StateHistory.Count - 1].position;
            BoardCoord newPos = mover.GetBoardPosition();

            if (Board.ContainsCoord(mover.GetRelativeBoardCoord(0, -1)) && mover.IsThreatTowards(mover.GetRelativeBoardCoord(0, -1))) {
                ChessPiece occupier = Board.GetCoordInfo(mover.GetRelativeBoardCoord(0, -1)).GetAliveOccupier();
                if (occupier != null && occupier is Pawn && CheckEnPassantVulnerability((Pawn)occupier)) {
                    mover.CaptureCount++;
                    CapturePiece(occupier);

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
                CapturePiece(mover);

                ChessPiece newPromotedPiece = AddNewPieceToBoard(SelectedPawnPromotion, mover.GetTeam(), mover.GetBoardPosition());

                AddPromotionToLastMoveNotation(newPromotedPiece.GetLetterNotation());
                return newPromotedPiece;
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
            if (Board.ContainsCoord(destination)) {
                if (checkingForCheck) return false;

                bool isInCheck = false;

                if (SimulateMove(mover, destination)) {
                    isInCheck = IsPieceInCheck(pieceToCheck);
                    RevertSimulatedMove();
                }

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
                BoardCoord coordStep = pieceToCheck.GetCoordStepInDirection((MoveDirection)i, true);
                BoardCoord coord = pieceToCheck.GetBoardPosition() + coordStep;

                while (Board.ContainsCoord(coord)) {
                    if (pieceToCheck.IsThreatTowards(coord)) {
                        possibleCheckThreats.Add(Board.GetCoordInfo(coord).GetAliveOccupier());
                    }
                    coord += coordStep;
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
                                    BoardCoord castlingCoord = castler.GetBoardPosition() + new BoardCoord(i * castlingDistance, 0);
                                    if (Board.ContainsCoord(castlingCoord) && castler.IsAllyTowards(castlingCoord) == false) {
                                        castleMoves.Add(castlingCoord);
                                    }
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
            if (checkingSimulatedMove == false) {
                string moveNotation = GameMoveNotations.Pop();
                moveNotation += "+";
                GameMoveNotations.Push(moveNotation);
            }
        }

        protected void AddCheckmateToLastMoveNotation() {
            if (checkingSimulatedMove == false) {
                string moveNotation = GameMoveNotations.Pop();
                moveNotation += "#";
                GameMoveNotations.Push(moveNotation);
            }
        }

        protected void AddPromotionToLastMoveNotation(string promotedPieceNotation) {
            if (checkingSimulatedMove == false) {
                string moveNotation = GameMoveNotations.Pop();
                moveNotation += string.Format("={0}", promotedPieceNotation);
                GameMoveNotations.Push(moveNotation);
            }
        }

        protected void SetLastMoveNotationToEnPassant(BoardCoord oldPos, BoardCoord newPos) {
            if (checkingSimulatedMove == false) {
                string moveNotation = GameMoveNotations.Pop();
                moveNotation = Board.GetCoordInfo(oldPos).file + "x" + Board.GetCoordInfo(newPos).algebraicKey + "e.p.";
                GameMoveNotations.Push(moveNotation);
            }
        }

        protected void SetLastMoveNotationToKingSideCastle() {
            if (checkingSimulatedMove == false) {
                string moveNotation = GameMoveNotations.Pop();
                moveNotation = "O-O";
                GameMoveNotations.Push(moveNotation);
            }
        }

        protected void SetLastMoveNotationToQueenSideCastle() {
            if (checkingSimulatedMove == false) {
                string moveNotation = GameMoveNotations.Pop();
                moveNotation = "O-O-O";
                GameMoveNotations.Push(moveNotation);
            }
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
            OnDisplayPromotionOptions?.Invoke(true);
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
        /// Directly moves a chess piece from it's current position to the destination and adds it's move notation. 
        /// Ignores promotions, enpassant, and castling rules. 
        /// If rules are desired, use the virtual method MovePiece for more flexible behaviour. 
        /// </summary>
        protected bool MakeBaseMove(ChessPiece mover, BoardCoord destination) {
            if (Board.ContainsCoord(destination)) {
                bool capturingThreat = mover.IsThreatTowards(destination);

                mover.MoveCount++;
                SetLastMovedPiece(mover);

                if (capturingThreat) {
                    CapturePiece(Board.GetCoordInfo(destination).GetAliveOccupier());
                    mover.CaptureCount++;
                } else {
                    if ((mover is Pawn) == false) {
                        numConsecutiveCapturelessMoves++;
                    }
                }

                // Determine base move notation for this move. Skipped for simulated moves.
                // This condition is also needed to avoid a stack overflow when checking for check in simulated moves.
                if (checkingSimulatedMove == false) {
                    string moveNotation = DetermineBaseNotationOfMove(mover, destination, capturingThreat);
                    GameMoveNotations.Push(moveNotation);
                }

                // Physically move the piece.
                UpdatePiecePositionAndOccupance(mover, destination);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Determines the base move notation for a move. 
        /// Does not include enpassant, castling, promotion, check, or checkmate notation.
        /// Those notations are appended at a higher level.
        /// </summary>
        private string DetermineBaseNotationOfMove(ChessPiece mover, BoardCoord destination, bool capturingThreat) {
            BoardCoord previousPosition = mover.GetBoardPosition();
            StringBuilder moveNotation = new StringBuilder(null, 4);

            // Example move: Queen takes Bishop on e4 with check.

            moveNotation.Append(mover.GetLetterNotation()); // Notation: Q
            moveNotation.Append(ResolveMoveNotationAmbiguity(mover, destination)); // Notation: Q (No ambiguity)
            
            if (capturingThreat) {
                if (mover is Pawn) {
                    moveNotation.Append(Board.GetCoordInfo(previousPosition).file);
                }
                moveNotation.Append('x'); // Notation: Qx
            }
            
            moveNotation.Append(Board.GetCoordInfo(destination).algebraicKey); // Notation: Qxe4

            // As this is only the base move notation (Qxe4), the final notation including the check (Qxe4+) will be appended later.
            return moveNotation.ToString();
        }

        /// <summary>
        /// Resolves notation ambiguity between same-type pieces that can move to the same destination.
        /// </summary>
        /// <returns>The resolved notation to be appended.</returns>
        public string ResolveMoveNotationAmbiguity(ChessPiece mover, BoardCoord destination) {
            int numAmbiguousMovers = 0;
            bool atLeastOneFileMatched = false;
            bool atLeastOneRankMatched = false;

            foreach (ChessPiece piece in GetPiecesOfType<ChessPiece>(mover.GetTeam())) {
                // Ensure piece is not the mover but the same type of piece as the mover.
                if (piece != mover && piece.GetPieceType() == mover.GetPieceType()) {
                    // Check if the piece could potentially move to the same destination as the mover.
                    if (piece.CalculateTemplateMoves().Contains(destination)) {
                        // Check if that move would be valid.
                        if (IsPieceInCheckAfterThisMove(currentRoyalPiece, piece, destination) == false) {
                            // If so, this is an ambigious mover.
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
                return Board.GetCoordInfo(mover.GetBoardPosition()).file;

            } else if (atLeastOneFileMatched && !atLeastOneRankMatched) {
                return Board.GetCoordInfo(mover.GetBoardPosition()).rank;

            } else if (atLeastOneFileMatched && atLeastOneRankMatched) {
                // If both file and rank matched, that means there at least 2 other movers, so the whole key is required.
                return Board.GetCoordInfo(mover.GetBoardPosition()).algebraicKey;

            } else {
                return string.Empty;
            }
        }

        /// <summary>
        /// Returns the turn number for each team's moves in a turn.
        /// </summary>
        public int GetNotationTurn() {
            return (int)Math.Ceiling((double)GameMoveNotations.Count / (double)NotationTurnDivider);
        }

        /// <summary>
        /// Updates all pieces' state histories to be up to date with the total move count. Is called by GameManager when a move is completed.
        /// </summary>
        public virtual void IncrementGameAndPieceStateHistory() {
            gameStateHistory.Push(new GameStateSnapshot(currentTeamTurn, opposingTeamTurn, currentRoyalPiece, opposingRoyalPiece,
                GetLastMovedPiece(opposingTeamTurn), GetLastMovedPiece(currentTeamTurn), (uint)GetNumConseqCapturelessMoves()));

            foreach (ChessPiece piece in GetPiecesOfType<ChessPiece>()) {
                piece.UpdateStateHistory();
            }
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
        /// Captures a chess piece. All we do is hide it from the board and set it to not alive.
        /// </summary>
        protected void CapturePiece(ChessPiece piece) {
            piece.IsAlive = false;

            OnChessPieceCaptured?.Invoke(piece);
        }

        private void RewindGameStateToPreviousMove() {
            if (gameStateHistory.Count > 1) {
                gameStateHistory.Pop();

                GameStateSnapshot gameStateToRestore = gameStateHistory.Peek();

                currentTeamTurn = gameStateToRestore.currentTeam;
                opposingTeamTurn = gameStateToRestore.opposingTeam;

                currentRoyalPiece = gameStateToRestore.currentRoyalPiece;
                opposingRoyalPiece = gameStateToRestore.opposingRoyalPiece;

                SetLastMovedPiece(gameStateToRestore.lastMovedWhitePiece);
                SetLastMovedPiece(gameStateToRestore.lastMovedBlackPiece);

                numConsecutiveCapturelessMoves = gameStateToRestore.numConsequtiveCapturelessMoves;
            }
        }

        private void RewindPieceStateToPreviousMove(ChessPiece piece) {
            if (piece.StateHistory.Count > 1) {
                piece.StateHistory.RemoveAt(piece.StateHistory.Count - 1);

                PieceStateSnapshot pieceStateToRestore = piece.StateHistory[piece.StateHistory.Count - 1];

                // If the piece is being brought back to life, alert any listeners to take care of the visuals.
                if (!piece.IsAlive && pieceStateToRestore.wasAlive) {
                    OnUndoChessPieceCapture?.Invoke(piece);
                }

                piece.IsAlive = pieceStateToRestore.wasAlive;

                UpdatePiecePositionAndOccupance(piece, pieceStateToRestore.position);

                piece.MoveCount = pieceStateToRestore.moveCount;
                piece.CaptureCount = pieceStateToRestore.captureCount;

            } else if (piece.StateHistory.Count == 1 && piece.IsAlive) {
                // This occurs for promoted pieces. A promoted pawn is considered a new piece, so once the new piece has no states left,
                // we completely remove it from the game and bring back the pawn.
                DestroyPieceFromBoard(piece);
            }
        }

        /// <summary>
        /// Reverts all chess pieces' states and overarching game state to the previous move.
        /// </summary>
        public virtual bool UndoLastMove() {
            if (gameStateHistory.Count > 1) {
                RewindGameStateToPreviousMove();

                foreach (ChessPiece piece in GetPiecesOfType<ChessPiece>()) {
                    RewindPieceStateToPreviousMove(piece);
                }

                if (checkingSimulatedMove == false) {
                    GameMoveNotations.Pop();
                }

                return true;
            }
            return false;
        }

        /// <summary>
        /// Adds a chess piece to the game board and assigns it to a team based on it's own team value.
        /// </summary>
        /// <returns>Returns the chess piece added to the game board.</returns>
        protected ChessPiece AddNewPieceToBoard(Piece pieceType, Team team, BoardCoord position) {
            if (Board.ContainsCoord(position)) {
                ChessPiece piece = ChessPieceFactory.Create(pieceType, team, position, Board);

                if (Board.GetCoordInfo(piece.GetBoardPosition()).GetAliveOccupier() != null && Board.GetCoordInfo(piece.GetBoardPosition()).GetAliveOccupier().IsAlive) {
                    CoordInfo posInfo = Board.GetCoordInfo(piece.GetBoardPosition());

                    RaiseEventOnLogErrorMessage(string.Format("OCCUPIED EXCEPTION :: {0} failed to instantiate because a {1} is already at it's position! Location: {3}.",
                        piece.ToString(), posInfo.GetAliveOccupier().ToString(), posInfo.algebraicKey, piece.GetBoardPosition().ToString()));

                } else {
                    Board.GetCoordInfo(piece.GetBoardPosition()).AddOccupier(piece);
                    piece.IsAlive = true;
                    AddPieceToTeam(piece);

                    // Alert listeners that this chess piece has been created.
                    OnChessPieceCreated?.Invoke(piece);

                    return piece;
                }
            }
            return null;
        }

        protected ChessPiece AddNewPieceToBoard(Piece pieceType, Team team, string algebraicKey) {
            if (Board.TryGetCoordWithKey(algebraicKey, out BoardCoord position)) {
                return AddNewPieceToBoard(pieceType, team, position);
            }
            return null;
        }

        /// <summary>
        /// Completely destroys a chess piece. Only used when a promoted piece is undo'd back to a pawn.
        /// </summary>
        /// <param name="piece"></param>
        private void DestroyPieceFromBoard(ChessPiece piece) {
            CapturePiece(piece);
            RemovePieceFromTeam(piece);

            // Alert listeners that this chess piece has been destroyed.
            OnChessPieceDestroyed?.Invoke(piece);
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
        protected bool SimulateMove(ChessPiece mover, BoardCoord destination) {
            checkingSimulatedMove = true;

            if (MovePiece(mover, destination)) {
                OnMoveComplete();
                IncrementGameAndPieceStateHistory();
                return true;
            }

            checkingSimulatedMove = false;
            return false;
        }

        /// <summary>
        /// Reverts a simulated move. NOTE: Must be preceeded by SimulateMove.
        /// </summary>
        protected void RevertSimulatedMove() {
            UndoLastMove();

            checkingSimulatedMove = false;
        }
    }
}
