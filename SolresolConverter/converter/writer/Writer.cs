using static SolresolConverter.Analyzer.SolresolRecords;

namespace SolresolConverter.Writer
{
    internal abstract class Writer : IWriter
    {
        public abstract string Write(SorsoRec sorsoRec);
    }
}
