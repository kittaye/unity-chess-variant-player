using System.Collections.Generic;
using System;
using System.Text;

public class Board {
    public event Action<string> OnLogErrorMessage;

    public event Action<object> OnShowBoardChunkObject;
    public event Action<object> OnHideBoardChunkObject;
    public event Action<object> OnDestroyBoardChunkObject;

    public bool allowFlipping;
    public bool isFlipped;

    private readonly uint boardWidth;
    private readonly uint boardHeight;
    private readonly Dictionary<BoardCoord, CoordInfo> coordinates;
    private char[] boardLetters;
    private string[] boardNumbers;
    private const int MAX_DIM = 26;

    public readonly BoardColour primaryBoardColour;
    public readonly BoardColour secondaryBoardColour;


    public Board(uint width, uint height, BoardColour primaryBoardColour, BoardColour secondaryBoardColour) {
        coordinates = new Dictionary<BoardCoord, CoordInfo>();

        boardWidth = Math.Min(width, MAX_DIM);
        boardHeight = Math.Min(height, MAX_DIM);

        allowFlipping = false;
        isFlipped = false;

        this.primaryBoardColour = primaryBoardColour;
        this.secondaryBoardColour = secondaryBoardColour;

        GenerateBoardCoordinateValues();

        for (int y = 0; y < this.GetHeight(); y++) {
            for (int x = 0; x < this.GetWidth(); x++) {
                coordinates.Add(new BoardCoord(x, y), new CoordInfo(boardLetters[x] + boardNumbers[y]));
            }
        }
    }

    private void GenerateBoardCoordinateValues() {
        StringBuilder letters = new StringBuilder(MAX_DIM);
        StringBuilder numbers = new StringBuilder(MAX_DIM);

        int nextLetter = 'a';
        for (int i = 0; i < letters.Capacity; i++) {
            letters.Append((char)nextLetter++);
            numbers.Append((i + 1) + ",");
        }
        boardLetters = letters.ToString().ToCharArray();
        boardNumbers = numbers.ToString().Split(new char[] { ',' });
    }

    public bool TryGetCoordWithKey(string algebraicKey, out BoardCoord coord) {
        coord = new BoardCoord();

        foreach (KeyValuePair<BoardCoord, CoordInfo> pair in coordinates) {
            if (pair.Value.algebraicKey.Equals(algebraicKey)) {
                coord = pair.Key;
                return true;
            }
        }
        OnLogErrorMessage?.Invoke(string.Format("Couldn't retrieve a BoardCoord because the GameBoard doesn't contain key: " + algebraicKey + "!"));
        return false;
    }

    public CoordInfo GetCoordInfo(BoardCoord coord) {
        return coordinates[coord];
    }

    public CoordInfo GetCoordInfo(string algebraicKey) {
        if (TryGetCoordWithKey(algebraicKey, out BoardCoord coord)) {
            return coordinates[coord];
        }
        OnLogErrorMessage?.Invoke(string.Format("ERROR: The GameBoard does not contain a CoordInfo for coordinate key: {0}", algebraicKey));
        return null;
    }

    public void RaiseEventShowBoardChunkObject(object boardChunkObject) {
        OnShowBoardChunkObject?.Invoke(boardChunkObject);
    }

    public void RaiseEventHideBoardChunkObject(object boardChunkObject) {
        OnHideBoardChunkObject?.Invoke(boardChunkObject);
    }

    public int GetHeight() {
        return (int)boardHeight;
    }

    public int GetWidth() {
        return (int)boardWidth;
    }

    public bool ContainsCoord(BoardCoord coord) {
        return coordinates.ContainsKey(coord);
    }

    public bool ContainsCoord(string coordKey) {
        return TryGetCoordWithKey(coordKey, out BoardCoord coord);
    }

    public void RemoveAndDestroyBoardCoordinates(string[] coordKeys) {
        for (int i = 0; i < coordKeys.Length; i++) {
            RemoveAndDestroyBoardCoordinate(coordKeys[i]);
        }
    }

    public bool RemoveAndDestroyBoardCoordinate(string coordKey) {
        if (TryGetCoordWithKey(coordKey, out BoardCoord coord)) {
            return RemoveAndDestroyBoardCoordinate(coord);
        }
        return false;
    }

    public void RemoveAndDestroyBoardCoordinates(BoardCoord[] coords) {
        for (int i = 0; i < coords.Length; i++) {
            RemoveAndDestroyBoardCoordinate(coords[i]);
        }
    }

    public bool RemoveAndDestroyBoardCoordinate(BoardCoord coord) {
        if (ContainsCoord(coord)) {
            OnDestroyBoardChunkObject?.Invoke(GetCoordInfo(coord).graphicalObject);
            coordinates.Remove(coord);
            return true;
        }
        return false;
    }

    public bool SetCustomBoardAlgebraicKey(string coordToChange, string newAlgebraicKey) {
        if (TryGetCoordWithKey(coordToChange, out BoardCoord coord)) {
            return SetCustomBoardAlgebraicKey(coord, newAlgebraicKey);
        }
        return false;
    }

    public bool SetCustomBoardAlgebraicKey(BoardCoord coordToChange, string newAlgebraicKey) {
        if (ContainsCoord(coordToChange)) {
            coordinates[coordToChange].algebraicKey = newAlgebraicKey;
            return true;
        }
        return false;
    }

    public void SetCustomBoardAlgebraicKeys(string coordKeyToStartFrom, int stopAfterXPos, int stopAfterYPos) {
        if (TryGetCoordWithKey(coordKeyToStartFrom, out BoardCoord coordToStartFrom)) {
            SetCustomBoardAlgebraicKeys(coordToStartFrom, stopAfterXPos, stopAfterYPos);
        }
    }

    public void SetCustomBoardAlgebraicKeys(BoardCoord coordToStartFrom, int stopAfterXPos, int stopAfterYPos) {
        if (stopAfterXPos >= MAX_DIM || stopAfterYPos >= MAX_DIM || coordToStartFrom.x >= MAX_DIM || coordToStartFrom.y >= MAX_DIM) {
            OnLogErrorMessage?.Invoke(string.Format("Board dimensions greater than {0} are not allowed.", MAX_DIM));
        } else if (coordToStartFrom.x < 0 || coordToStartFrom.y < 0) {
            OnLogErrorMessage?.Invoke(string.Format("Starting boardcoord values are undefined! Do not use negative numbers."));
        }

        for (int y = coordToStartFrom.y; y <= stopAfterYPos; y++) {
            for (int x = coordToStartFrom.x; x <= stopAfterXPos; x++) {
                SetCustomBoardAlgebraicKey(new BoardCoord(x, y), boardLetters[x - coordToStartFrom.x] + boardNumbers[y - coordToStartFrom.y]);
            }
        }
    }
}
