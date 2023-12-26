using static SolresolConverter.Analyzer.SolresolRecords;

namespace SolresolConverter.Analyzer
{
    internal interface IAnalyzer
    {
        SorsoRec Analyze(string text, SolresolFormat src);
    }
}
