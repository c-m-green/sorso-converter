using static SolresolConverter.Analyzer.SolresolRecords;

namespace SolresolConverter.Converter
{
    internal record class CompoundWordRec(SorsoRec ConstituentWords, PartOfSpeech? Override);
}
