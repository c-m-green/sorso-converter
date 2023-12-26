using static SolresolTranslator.Analyzer.SolresolRecords;

namespace SolresolTranslator.Writer
{
    internal interface IWriter
    {
        string Write(SorsoRec sorsoRec);
    }
}
