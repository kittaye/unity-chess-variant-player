using System.Collections.Generic;
using UnityEngine;
using System;

public enum Team { WHITE, BLACK }
public enum MoveDirection { Up, Down, Left, Right, UpLeft, UpRight, DownLeft, DownRight }

public abstract class Chess {

    public static event Action<bool> _DisplayPromotionUI;
    public static event Action<Piece[]> _OnPawnPromotionsChanged;
    public Piece[] pawnPromotionOptions { get; protected set; }
    public Piece selectedPawnPromotion { get; protected set; }
    public Board board { get; private set; }
    public bool allowBoardFlipping;

    protected const int BOARD_WIDTH = 8;
    protected const int BOARD_HEIGHT = 8;
    protected const int WHITE_BACKROW = 0;
    protected const int WHITE_PAWNROW = 1;
    protected int BLACK_BACKROW;
    protected int BLACK_PAWNROW;
    protected Team currentTeamTurn;
    protected Team opposingTeamTurn;
    protected uint numConsecutiveCapturelessMoves { get; private set; }
    protected bool checkingForCheck;
    protected List<ChessPiece> opposingTeamCheckThreats;
    protected ChessPiece currentRoyalPiece;
    protected ChessPiece opposingRoyalPiece;
    protected Rook aSideWhiteRook;
    protected Rook hSideWhiteRook;
    protected Rook aSideBlackRook;
    protected Rook hSideBlackRook;

    private List<ChessPiece> whitePieces;
    private List<ChessPiece> blackPieces;
    private ChessPiece lastMovedWhitePiece;
    private ChessPiece lastMovedBlackPiece;

    public Chess(uint width, uint height) {
        board = new Board(width, height, new Color(0.9f, 0.9f, 0.9f), new Color(0.1f, 0.1f, 0.1f));
        whitePieces = new List<ChessPiece>();
        blackPieces = new List<ChessPiece>();
        lastMovedWhitePiece = null;
        lastMovedBlackPiece = null;
        currentTeamTurn = Team.WHITE;
        opposingTeamTurn = Team.BLACK;
        numConsecutiveCapturelessMoves = 0;
        allowBoardFlipping = true;
    }

    public Chess(uint width, uint height, Color primaryBoardColour, Color secondaryBoardColour) {
        board = new Board(width, height, primaryBoardColour, secondaryBoardColour);
        whitePieces = new List<ChessPiece>();
        blackPieces = new List<ChessPiece>();
        lastMovedWhitePiece = null;
        lastMovedBlackPiece = null;
        currentTeamTurn = Team.WHITE;
        opposingTeamTurn = Team.BLACK;
        numConsecutiveCapturelessMoves = 0;
        allowBoardFlipping = true;
    }

    /// <summary>
    /// Is called after the board is instantiated. Used to place the initial chess pieces on the board. 
    /// </summary>
    public abstract void PopulateBoard();

    /// <summary>
    /// Calculates the currently available moves for a selected piece.
    /// </summary>
    /// <param name="mover">Selected piece to calculate moves for.</param>
    /// <returns>A list of board coordinates for each move.</returns>
    public abstract List<BoardCoord> CalculateAvailableMoves(ChessPiece mover);

    /// <summary>
    /// Moves a selected piece to a destination.
    /// </summary>
    /// <param name="mover">Piece to move.</param>
    /// <param name="destination">Destination to move to.</param>
    /// <returns>True if the destination is an available move for this piece.</returns>
    public abstract bool MovePiece(ChessPiece mover, BoardCoord destination);

    /// <summary>
    /// Defines how the chess game is won.
    /// </summary>
    /// <returns>True if a team has won.</returns>
    public abstract bool CheckWinState();

    public Team GetCurrentTeamTurn() {
        return currentTeamTurn;
    }

    public Team GetOpposingTeamTurn() {
        return opposingTeamTurn;
    }

    public ChessPiece GetTeamLastMovedPiece(Team team) {
        if(team == Team.WHITE) {
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

    /// <summary>
    /// Used to update the pawn promotion options at any point during the game.
    /// </summary>
    /// <param name="pieces">Set of pieces to change the pawn promotion options to.</param>
    protected void SetPawnPromotionOptions(Piece[] pieces) {
        pawnPromotionOptions = pieces;
        if (_OnPawnPromotionsChanged != null) _OnPawnPromotionsChanged.Invoke(pawnPromotionOptions);
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
        this.selectedPawnPromotion = piece;
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
            if(piece is T && piece.IsAlive) {
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
            ChessPiece occupier = board.GetCoordInfo(coord).occupier;
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
            ChessPiece occupier = board.GetCoordInfo(coord).occupier;
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
            ChessPiece oldCoordOccupier = board.GetCoordInfo(previousPosition).occupier;
            if (oldCoordOccupier != null) {
                board.GetCoordInfo(previousPosition).occupier = null;
                if (IsThreat(oldCoordOccupier, newPosition)) {
                    RemovePieceFromBoard(board.GetCoordInfo(newPosition).occupier);
                }
                board.GetCoordInfo(newPosition).occupier = oldCoordOccupier;
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
    /// Called after a move is played. Switches the current and opposing teams around.
    /// </summary>
    public virtual void OnTurnComplete() {
        currentTeamTurn = (currentTeamTurn == Team.WHITE) ? Team.BLACK : Team.WHITE;
        opposingTeamTurn = (currentTeamTurn == Team.WHITE) ? Team.BLACK : Team.WHITE;
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
            board.GetCoordInfo(piece.GetBoardPosition()).occupier = null;
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
            board.GetCoordInfo(piece.GetBoardPosition()).occupier = piece;
            piece.IsAlive = true;
            GameManager.Instance.InstantiateChessPiece(piece);
            AddPieceToActiveTeam(piece);

            if (piece.GetTeam() == Team.BLACK && board.isFlipped) {
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
        } else if (board.GetCoordInfo(piece.GetBoardPosition()).occupier != null && board.GetCoordInfo(piece.GetBoardPosition()).occupier.IsAlive) {
            CoordInfo posInfo = board.GetCoordInfo(piece.GetBoardPosition());
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
        if (!board.ContainsCoord(coord)) {
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

            board.GetCoordInfo(mover.GetBoardPosition()).occupier = null;
            board.GetCoordInfo(dest).occupier = mover;
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
            board.GetCoordInfo(mover.GetBoardPosition()).occupier = mover;

            if (originalOccupier != null) {
                originalOccupier.IsAlive = true;
                board.GetCoordInfo(dest).occupier = originalOccupier;
            } else {
                board.GetCoordInfo(dest).occupier = null;
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
            if (xWrap) x = MathExtensions.mod(x, board.GetWidth());
            if (yWrap) y = MathExtensions.mod(y, board.GetHeight());
            coord = new BoardCoord(x, y);

            if (board.ContainsCoord(coord) == false) break;
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
            if (xWrap) x = MathExtensions.mod(x, board.GetWidth());
            if (yWrap) y = MathExtensions.mod(y, board.GetHeight());
            coord = new BoardCoord(x, y);

            if (board.ContainsCoord(coord) == false) break;
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
        if (board.ContainsCoord(destination)) {
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
