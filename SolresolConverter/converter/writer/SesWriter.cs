using System.Text;
using static SolresolConverter.Analyzer.SolresolRecords;

namespace SolresolConverter.Writer
{
    internal class SesWriter : Writer
    {
        public override string Write(SorsoRec sorsoRec)
        {
            // TODO This looks goofy...
            string[,] sesSyllables = new string[7,2];
            sesSyllables[0, 0] = "p";
            sesSyllables[0, 1] = "o";
            sesSyllables[1, 0] = "k";
            sesSyllables[1, 1] = "e";
            sesSyllables[2, 0] = "m";
            sesSyllables[2, 1] = "i";
            sesSyllables[3, 0] = "f";
            sesSyllables[3, 1] = "a";
            sesSyllables[4, 0] = "s";
            sesSyllables[4, 1] = "u";
            sesSyllables[5, 0] = "l";
            sesSyllables[5, 1] = "au";
            sesSyllables[6, 0] = "t";
            sesSyllables[6, 1] = "ai";

            StringBuilder strOut = new();

            foreach (var word in sorsoRec.Words)
            {
                bool isVowel = word.Syllables.Count == 1;
                bool isAllCapsWord = true;
                bool firstLetterCapsOnly = false;
                StringBuilder sesWord = new();
                for (int syllableIdx = 0; syllableIdx < word.Syllables.Count; syllableIdx++)
                {
                    var syllable = word.Syllables[syllableIdx];
                    StringBuilder syllableBuilder = new(sesSyllables[(int)syllable.Degree, isVowel ? 1 : 0]);
                    if (syllable.IsAllCaps || syllable.IsCapitalized)
                    {
                        for (int i = 0; i < syllableBuilder.Length; i++)
                        {
                            syllableBuilder[i] = char.ToUpper(syllableBuilder[i]);
                        }
                        firstLetterCapsOnly = syllableIdx == 0;
                        isAllCapsWord = isAllCapsWord && syllable.IsAllCaps;
                    } else
                    {
                        isAllCapsWord = false;
                    }
                    sesWord.Append(syllableBuilder);
                    isVowel = !isVowel;
                }
                if (word.PluralIndices.Length > 0)
                {
                    if (isAllCapsWord)
                    {
                        sesWord.Insert(0, "PAU ");
                    } else
                    {
                        if (firstLetterCapsOnly)
                        {
                            sesWord[0] = char.ToLower(sesWord[0]);
                            sesWord.Insert(0, "Pau ");
                        } else
                        {
                            sesWord.Insert(0, "pau ");
                        }
                    }
                }
                if (word.FeminineIndices.Length > 0)
                {
                    sesWord.Append(isAllCapsWord ? " MU" : " mu");
                }
                sesWord.Insert(0, word.Prefix);
                sesWord.Append(word.Suffix);
                if (strOut.Length > 0)
                {
                    strOut.Append(' ');
                }
                strOut.Append(sesWord);
            }
            return strOut.ToString();
        }
    }
}
