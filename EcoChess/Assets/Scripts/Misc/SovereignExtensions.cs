using UnityEngine;

public enum ColourName { Red, Blue, Yellow, Green, Pink, Purple, Grey, Silver, Orange, Lightblue, White, Black }

public static class SovereignExtensions {
    private static readonly Color Pink = new Color(1, 0.45f, 0.71f);
    private static readonly Color Purple = new Color(0.6f, 0, 0.6f);
    private static readonly Color Silver = new Color(0.85f, 0.85f, 0.85f);
    private static readonly Color Orange = new Color(0.9f, 0.58f, 0);
    private static readonly Color Lightblue = new Color(0.447f, 0.77f, 0.98f);

    public static ColourName GetColourName(Color colour) {
        if(colour == Color.red) {
            return ColourName.Red;
        } else if (colour == Color.blue) {
            return ColourName.Blue;
        } else if (colour == Color.yellow) {
            return ColourName.Yellow;
        } else if (colour == Color.green) {
            return ColourName.Green;
        } else if (colour == Pink) {
            return ColourName.Pink;
        } else if (colour == Purple) {
            return ColourName.Purple;
        } else if (colour == Color.grey) {
            return ColourName.Grey;
        } else if (colour == Silver) {
            return ColourName.Silver;
        } else if (colour == Orange) {
            return ColourName.Orange;
        } else if (colour == Lightblue) {
            return ColourName.Lightblue;
        } else if (colour == Color.white) {
            return ColourName.White;
        } else {
            return ColourName.Black;
        }
    }

    public static Color GetColour(ColourName colour) {
        if (colour == ColourName.Red) {
            return Color.red;
        } else if (colour == ColourName.Blue) {
            return Color.blue;
        } else if (colour == ColourName.Yellow) {
            return Color.yellow;
        } else if (colour == ColourName.Green) {
            return Color.green;
        } else if (colour == ColourName.Pink) {
            return Pink;
        } else if (colour == ColourName.Purple) {
            return Purple;
        } else if (colour == ColourName.Grey) {
            return Color.grey;
        } else if (colour == ColourName.Silver) {
            return Silver;
        } else if (colour == ColourName.Orange) {
            return Orange;
        } else if (colour == ColourName.Lightblue) {
            return Lightblue;
        } else if (colour == ColourName.White) {
            return Color.white;
        } else {
            return Color.black;
        }
    }
}
