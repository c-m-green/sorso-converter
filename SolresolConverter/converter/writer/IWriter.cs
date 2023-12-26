using static SolresolConverter.Analyzer.SolresolRecords;

namespace SolresolConverter.Writer
{
    internal interface IWriter
    {
        string Write(SorsoRec sorsoRec);
    }
}
