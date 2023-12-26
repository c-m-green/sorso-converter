using static SolresolConverter.Analyzer.SolresolRecords;

namespace SolresolConverter.Analyzer
{
    internal abstract class Analyzer : IAnalyzer
    {
        public abstract SorsoRec Analyze(string text, SolresolFormat src);
    }
}
