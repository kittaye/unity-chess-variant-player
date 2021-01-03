
public class VariantHelpDetails
{
    public static readonly string rule_NoCastling  = "- No castling.";
    public static readonly string rule_NoEnpassantCapture  = "- No enpassant capture.";
    public static readonly string rule_NoPawnDoubleMove  = "- No pawn double-move.";
    public static readonly string rule_NoPawnPromotion  = "- No pawn promotion.";
    public static readonly string rule_None  = "None.";

    public string title;
    public string subtitle;
    public string summary;
    public string winCondition;
    public string otherRules;
    public string moreInfoLink;

    private VariantHelpDetails() { }

    public VariantHelpDetails(string title, string subtitle, string summary, string winCondition, string otherRules, string infoLink) {
        this.title = title;
        this.subtitle = subtitle;
        this.summary = summary;
        this.winCondition = winCondition;
        this.otherRules = otherRules;
        this.moreInfoLink = infoLink;
    }
}
