using static SolresolTranslator.Analyzer.SolresolRecords;

namespace SolresolTranslator.Writer
{
    internal abstract class Writer : IWriter
    {
        public abstract string Write(SorsoRec sorsoRec);
    }
}
