using static SolresolTranslator.Analyzer.SolresolRecords;

namespace SolresolTranslator.Converter
{
    internal record class CompoundWordRec(SorsoRec ConstituentWords, PartOfSpeech? Override);
}
