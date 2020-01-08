using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VariantHelpDialog : MonoBehaviour
{
    public Text title;
    public Text subtitle;
    public Text summaryDescription;
    public Text winConditionDescription;
    public Text otherRulesDescription;
    public Text moreInfoLink;

    public void SetUIValues(VariantHelpDetails variantHelpDetails) {
        this.title.text = variantHelpDetails.title;
        this.subtitle.text = variantHelpDetails.subtitle;
        this.summaryDescription.text = variantHelpDetails.summary;
        this.winConditionDescription.text = variantHelpDetails.winCondition;
        this.otherRulesDescription.text = variantHelpDetails.otherRules;
        this.moreInfoLink.text = "More information at <i>" + variantHelpDetails.moreInfoLink + "</i>";
    }
}
