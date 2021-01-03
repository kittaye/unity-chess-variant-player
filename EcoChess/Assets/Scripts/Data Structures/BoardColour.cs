
public struct BoardColour
{
    public float r;
    public float g;
    public float b;
    public float a;

    public BoardColour(float r, float g, float b, float a) {
        this.r = r;
        this.g = g;
        this.b = b;
        this.a = a;
    }

    public BoardColour(float r, float g, float b) {
        this.r = r;
        this.g = g;
        this.b = b;
        this.a = 1;
    }
}
