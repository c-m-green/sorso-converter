using static SolresolConverter.Analyzer.SolresolRecords;

namespace SolresolConverter.Analyzer
{
    public static class InputAnalyzer
    {      
        public static SorsoRec ReadInputText(string text, SolresolFormat src)
        {
            return src switch
            {
                SolresolFormat.Sorso => new SorsoAnalyzer().Analyze(text),
                _ => throw new NotImplementedException(),
            };
        }
    }
}
