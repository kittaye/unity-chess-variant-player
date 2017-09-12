using System.Collections.Generic;
using UnityEngine;

public enum Team { WHITE, BLACK }
public enum MoveDirection { Up, Down, Left, Right, UpLeft, UpRight, DownLeft, DownRight }

public abstract class Chess {
    public ChessPiece lastMovedPiece { get; protected set; }
    public Board board { get; private set; }

    private List<ChessPiece> whitePieces;
    private List<ChessPiece> blackPieces;
    protected Team currentTeamTurn;
    protected Team opposingTeamTurn;
    protected uint numConsecutiveCapturelessMoves { get; private set; }

    public Chess(uint width, uint height) {
        board = new Board(width, height, new Color(0.9f, 0.9f, 0.9f), new Color(0.1f, 0.1f, 0.1f));
        whitePieces = new List<ChessPiece>();
        blackPieces = new List<ChessPiece>();
        lastMovedPiece = null;
        currentTeamTurn = Team.WHITE;
        opposingTeamTurn = Team.BLACK;
        numConsecutiveCapturelessMoves = 0;
    }

    public Chess(uint width, uint height, Color primaryBoardColour, Color secondaryBoardColour) {
        board = new Board(width, height, primaryBoardColour, secondaryBoardColour);
        whitePieces = new List<ChessPiece>();
        blackPieces = new List<ChessPiece>();
        lastMovedPiece = null;
        currentTeamTurn = Team.WHITE;
        opposingTeamTurn = Team.BLACK;
        numConsecutiveCapturelessMoves = 0;
    }

    public abstract void PopulateBoard();

    public abstract List<BoardCoord> CalculateAvailableMoves(ChessPiece mover);

    public abstract bool MovePiece(ChessPiece mover, BoardCoord destination);

    public abstract bool CheckWinState();

    public Team GetCurrentTeamTurn() {
        return currentTeamTurn;
    }

    public Team GetOpposingTeamTurn() {
        return opposingTeamTurn;
    }

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

    public virtual string GetCurrentTurnLabel() {
        if (currentTeamTurn == Team.WHITE) {
            return "White's move";
        } else {
            return "Black's move";
        }
    }

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
    /// <returns></returns>
    protected bool MakeMove(ChessPiece mover, BoardCoord destination) {
        BoardCoord previousPosition = mover.GetBoardPosition();
        bool wasThreat = IsThreat(mover, destination);

        if (mover.MakeMoveTo(destination)) {
            if (wasThreat) mover.CaptureCount++;
            numConsecutiveCapturelessMoves = (wasThreat == false && (mover is Pawn) == false) ? numConsecutiveCapturelessMoves + 1 : 0;
            UpdateSquareOccupiers(previousPosition, mover.GetBoardPosition());
            lastMovedPiece = mover;
            return true;
        }
        return false;
    }

    public virtual void OnTurnComplete() {
        currentTeamTurn = (currentTeamTurn == Team.WHITE) ? Team.BLACK : Team.WHITE;
        opposingTeamTurn = (currentTeamTurn == Team.WHITE) ? Team.BLACK : Team.WHITE;
    }

    protected bool RemovePieceFromBoard(ChessPiece piece) {
        if (piece != null) {
            piece.gameObject.SetActive(false);
            piece.IsAlive = false;
            board.GetCoordInfo(piece.GetBoardPosition()).occupier = null;
            return true;
        }
        return false;
    }

    protected bool RemovePieceFromActiveTeam(ChessPiece piece) {
        if (piece.GetTeam() == Team.WHITE) {
            return whitePieces.Remove(piece);
        } else {
            return blackPieces.Remove(piece);
        }
    }

    protected void AddPieceToActiveTeam(ChessPiece piece) {
        if (piece.GetTeam() == Team.WHITE) {
            whitePieces.Add(piece);
        } else {
            blackPieces.Add(piece);
        }
    }

    protected ChessPiece AddPieceToBoard(ChessPiece piece) {
        if (CheckValidPlacement(piece)) {
            board.GetCoordInfo(piece.GetBoardPosition()).occupier = piece;
            piece.IsAlive = true;
            GameManager.Instance.InstantiateChessPiece(piece);
            if (piece.GetTeam() == Team.WHITE) {
                whitePieces.Add(piece);
            } else {
                blackPieces.Add(piece);
            }
            return piece;
        }
        return null;
    }

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

    public bool AssertContainsCoord(BoardCoord coord) {
        if (!board.ContainsCoord(coord)) {
            Debug.LogErrorFormat("ERROR: {0} is not a valid position on the GameBoard!", coord.ToString());
            return false;
        }
        return true;
    }

    public virtual bool IsMoversTurn(ChessPiece mover) {
        return mover.GetTeam() == currentTeamTurn;
    }

    protected void SimulateMove(ChessPiece mover, BoardCoord dest, ChessPiece originalOccupier, out ChessPiece originalLastMover) {
        originalLastMover = null;
        if (AssertContainsCoord(dest)) {
            if (lastMovedPiece != null) originalLastMover = lastMovedPiece;
            lastMovedPiece = mover;
            if (originalOccupier != null) originalOccupier.IsAlive = false;
            board.GetCoordInfo(mover.GetBoardPosition()).occupier = null;
            board.GetCoordInfo(dest).occupier = mover;
            mover.SetBoardPosition(dest);
        }
    }

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
            lastMovedPiece = originalLastMover;
        }
    }

    public BoardCoord[] TryGetDirectionalMoves(ChessPiece mover, MoveDirection dir, uint cap = 0, uint threatAttackLimit = 1, bool threatsOnly = false, bool teamSensitive = true) {
        int x = mover.GetBoardPosition().x;
        int y = mover.GetBoardPosition().y;
        int xModifier;
        int yModifier;
        GetMoveDirectionModifiers(mover, dir, out xModifier, out yModifier, teamSensitive);

        uint iter = 0;
        uint threats = 0;
        BoardCoord coord;
        List<BoardCoord> moves = new List<BoardCoord>();
        while (true) {
            iter++;
            x += xModifier;
            y += yModifier;
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

    public BoardCoord[] TryGetCustomDirectionalMoves(ChessPiece mover, int xVariance, int yVariance, uint cap = 0, bool threatsOnly = false) {
        int x = mover.GetBoardPosition().x;
        int y = mover.GetBoardPosition().y;
        int xModifier = mover.TeamSensitiveMove(xVariance);
        int yModifier = mover.TeamSensitiveMove(yVariance);

        uint iter = 0;
        BoardCoord coord;
        List<BoardCoord> moves = new List<BoardCoord>();
        while (true) {
            iter++;
            x += xModifier;
            y += yModifier;
            coord = new BoardCoord(x, y);

            if (board.ContainsCoord(coord) == false) break;
            if (IsAlly(mover, coord)) break;
            if (IsThreat(mover, coord) == false && threatsOnly) break;
            moves.Add(coord);

            if (iter == cap) break;
        }
        return moves.ToArray();
    }

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
