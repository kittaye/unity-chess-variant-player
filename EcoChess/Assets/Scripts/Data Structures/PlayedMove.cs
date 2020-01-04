using System.Collections;
using System.Collections.Generic;
using System.Text;

public class PlayedMove {
    public readonly ChessPiece mover;
    public readonly BoardCoord startPos;
    public readonly BoardCoord endPos;
    public ChessPiece capturedPiece;
    public BoardCoord capturedPiecePos;

    private string algebraicNotiation;

    private ChessGameModes.Chess chessGame;

    public PlayedMove(ChessPiece mover, BoardCoord startPos, BoardCoord endPos) {
        this.mover = mover;
        this.startPos = startPos;
        this.endPos = endPos;
        this.capturedPiece = null;
        this.capturedPiecePos = BoardCoord.NULL;

        algebraicNotiation = SetNotation();

        chessGame = GameManager.Instance.ChessGame;
    }

    public PlayedMove(ChessPiece mover, BoardCoord startPos, BoardCoord endPos, ChessPiece capturedPiece, BoardCoord capturedPiecePos) {
        this.mover = mover;
        this.startPos = startPos;
        this.endPos = endPos;
        this.capturedPiece = capturedPiece;
        this.capturedPiecePos = capturedPiecePos;

        algebraicNotiation = SetNotation();

        chessGame = GameManager.Instance.ChessGame;
    }

    public override string ToString() {
        return algebraicNotiation;
    }

    public string SetNotation() {
        StringBuilder moveNotation = new StringBuilder(null, 4);

        moveNotation.Append(mover.GetLetterNotation());

        if(capturedPiece != null) {
            if (mover is Pawn) {
                //moveNotation.Append(chessGame.Board.GetCoordInfo(startPos).algebraicKey[0]);
            }
            moveNotation.Append('x');
        }

        moveNotation.Append(chessGame.Board.GetCoordInfo(endPos).algebraicKey);

        return moveNotation.ToString();
    }

}
