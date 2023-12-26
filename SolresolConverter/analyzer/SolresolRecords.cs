using SolresolTranslator.Converter;

namespace SolresolTranslator.Analyzer
{
    internal static class SolresolRecords
    {
        public record class SyllableRec(SorsoSyllableDegree Syllable, bool IsCapitalized, bool IsAllCaps)
        {
            public virtual bool Equals(SyllableRec? other)
            {
                if (other == null)
                {
                    return false;
                }
                return Syllable == other.Syllable;
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }

            public override string? ToString()
            {
                return Syllable.ToString();
            }
        }
        public record class WordRec(List<SyllableRec> Syllables, PartOfSpeech PartOfSpeech, int AccentIdx, int InvertedAccentIdx, int PluralIdx, int FeminineIdx, string Prefix, string Suffix, string OriginalWord, bool IsValidSorso)
        {
            public virtual bool Equals(WordRec? other)
            {
                if (other == null)
                {
                    return false;
                }
                return Syllables.SequenceEqual(other.Syllables) && AccentIdx == other.AccentIdx && InvertedAccentIdx == other.InvertedAccentIdx && PluralIdx == other.PluralIdx && FeminineIdx == other.FeminineIdx;
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }

            public override string? ToString()
            {
                return IsValidSorso ? Syllables.ToString() : $"[{OriginalWord}]";
            }
        }

        public record class SorsoRec(List<WordRec> Words, SolresolFormat Source);
    }
}
