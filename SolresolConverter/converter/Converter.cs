using SolresolConverter.Analyzer;
using SolresolConverter.Writer;
using static SolresolConverter.Analyzer.SolresolRecords;

namespace SolresolConverter.Converter
{
    public static class Converter
    {
        public struct CompoundWord
        {
            public List<string> ConstituentWords;
            public PartOfSpeech? PartOfSpeechOverride;

            public CompoundWord(List<string> constituentWords)
            {
                ConstituentWords = constituentWords;
                PartOfSpeechOverride = null;
            }

            public CompoundWord(string constituentWords)
            {
                ConstituentWords = new List<string>(constituentWords.Split(' ', '-'));
                PartOfSpeechOverride = null;
            }

            public CompoundWord(List<string> constituentWords, PartOfSpeech partOfSpeechOverride)
            {
                ConstituentWords = constituentWords;
                PartOfSpeechOverride = partOfSpeechOverride;
            }

            public CompoundWord(string constituentWords, PartOfSpeech partOfSpeechOverride)
            {
                ConstituentWords = new List<string>(constituentWords.Split(' ', '-'));
                PartOfSpeechOverride = partOfSpeechOverride;
            }
        }
        public static string ConvertSolresol(string text, SolresolFormat src, SolresolFormat dest, List<CompoundWord>? compoundWords = null)
        {
            var sorsoRec = InputAnalyzer.ReadInputText(text, src);
            return ConvertSolresol(sorsoRec, dest, compoundWords);
        }

        public static string ConvertSolresol(SorsoRec sorsoRec, SolresolFormat dest, List<CompoundWord>? compoundWords = null)
        {
            return dest switch
            {
                SolresolFormat.Ses => new SesWriter().Write(sorsoRec),
                SolresolFormat.SesCmgreen => new SesWkifWriter().Write(sorsoRec, compoundWords ?? new List<CompoundWord>()),
                _ => throw new NotImplementedException(),
            };
        }
    }
}
