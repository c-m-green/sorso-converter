using SolresolTranslator.Analyzer;
using SolresolTranslator.Writer;

namespace SolresolTranslator.Converter
{
    public static class SolresolConverter
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
            return dest switch
            {
                SolresolFormat.SesCmgreen => new SesWkifWriter().Write(WordAnalyzer.ReadInputText(text, src), compoundWords ?? new List<CompoundWord>()),
                _ => throw new NotImplementedException(),
            };
        }
    }
}
