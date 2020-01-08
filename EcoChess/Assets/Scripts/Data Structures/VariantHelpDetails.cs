using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VariantHelpDetails
{
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
