namespace HostUtility;

public static class RulesetManager
{
    public static string CurrentRuleSet = string.Empty;
    public static bool HasRuleset => !CurrentRuleSet.IsNullOrWhiteSpace();
}