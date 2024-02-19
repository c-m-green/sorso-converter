using SolresolConverter.Converter;

namespace SolresolConverter.Analyzer
{
    public static class SolresolRecords
    {
        public record class SyllableRec(SorsoSyllableDegree Degree, bool IsCapitalized, bool IsAllCaps)
        {
            public virtual bool Equals(SyllableRec? other)
            {
                return other != null && Degree == other.Degree;
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }

            public override string? ToString()
            {
                return Degree.ToString();
            }
        }
        public record class WordRec(List<SyllableRec> Syllables, PartOfSpeech PartOfSpeech, int[] AccentIndices, int[] InvertedAccentIndices, int[] PluralIndices, int[] FeminineIndices, string Prefix, string Suffix, string OriginalWord, bool IsValidSorso)
        {
            public virtual bool Equals(WordRec? other)
            {
                return other != null && Syllables.SequenceEqual(other.Syllables) && AccentIndices.SequenceEqual(other.AccentIndices) && InvertedAccentIndices.SequenceEqual(other.InvertedAccentIndices) && PluralIndices.SequenceEqual(other.PluralIndices) && FeminineIndices.SequenceEqual(other.FeminineIndices);
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
