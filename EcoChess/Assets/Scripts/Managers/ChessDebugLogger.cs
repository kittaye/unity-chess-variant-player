using UnityEngine;
using ChessGameModes;

public static class ChessDebugLogger
{
    public static void InitLogListeners(Chess chessGame)
    {
        chessGame.OnLogInfoMessage += ChessGame_OnLogInfoMessage;
        chessGame.OnLogErrorMessage += ChessGame_OnLogErrorMessage;
        chessGame.Board.OnLogErrorMessage += ChessBoard_OnLogErrorMessage;
    }

    private static void ChessGame_OnLogInfoMessage(string message) {
        LogInfo(message);
    }

    private static void ChessBoard_OnLogErrorMessage(string message) {
        LogError(message);
    }

    private static void ChessGame_OnLogErrorMessage(string message) {
        LogError(message);
    }

    private static void LogError(string error) {
        Debug.LogError(error);
    }

    private static void LogInfo(string info) {
        Debug.Log(info);
    }
}
