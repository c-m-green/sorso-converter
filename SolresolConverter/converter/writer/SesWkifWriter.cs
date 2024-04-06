using SolresolConverter.Analyzer;
using SolresolConverter.Converter;
using System.Globalization;
using System.Text;
using static SolresolConverter.Analyzer.SolresolRecords;
using static SolresolConverter.Converter.Converter;

namespace SolresolConverter.Writer
{
    internal class SesWkifWriter : Writer
    {
        public override string Write(SorsoRec sorsoRec)
        {
            return WriteText(sorsoRec, new List<CompoundWordRec>());
        }
        public string Write(SorsoRec sorsoRec, List<CompoundWord> compoundWords)
        {
            compoundWords.Sort((a, b) => b.ConstituentWords.Count - a.ConstituentWords.Count);
            List<CompoundWordRec> compoundWordRecs = new();
            foreach (var word in compoundWords)
            {
                var cmpdSorsoRec = InputAnalyzer.ReadInputText(string.Join(' ', word.ConstituentWords.ToArray()), SolresolFormat.Sorso);
                compoundWordRecs.Add(new CompoundWordRec(cmpdSorsoRec, word.PartOfSpeechOverride));
            }
            return WriteText(sorsoRec, compoundWordRecs);
        }
        private string WriteText(SorsoRec sorsoRec, List<CompoundWordRec> compoundWordRecs)
        {
            List<Tuple<int, int, PartOfSpeech?>> compoundWordBounds = new();

            foreach (var compoundWordRec in compoundWordRecs)
            {
                SorsoRec compoundWordParts = compoundWordRec.ConstituentWords;
                bool hasInvalidWord = false;
                for (int i = 0; i < compoundWordParts.Words.Count; i++)
                {
                    var word = compoundWordParts.Words[i];
                    // TODO Create const for the minimum syllable count.
                    if (!word.IsValidSorso
                        || i == 0 && word.Syllables.Count < 3
                        || i != 0 && word.Syllables.Count < 2)
                    {
                        hasInvalidWord = true;
                        break;
                    }
                }
                if (hasInvalidWord || compoundWordParts.Words.Count < 2)
                {
                    continue;
                }
                int startIdx = -1;
                int idx = 0;
                for (int sorsoWdRcIdx = 0; sorsoWdRcIdx < sorsoRec.Words.Count; sorsoWdRcIdx++)
                {
                    if (idx < compoundWordParts.Words.Count
                        && sorsoRec.Words[sorsoWdRcIdx].Equals(compoundWordParts.Words[idx])
                        && (idx == compoundWordParts.Words.Count - 1 || sorsoRec.Words[sorsoWdRcIdx].Suffix == "")
                        && sorsoRec.Words[sorsoWdRcIdx].IsValidSorso)
                    {
                        bool wordAlreadyUsed = false;
                        foreach (var bounds in compoundWordBounds)
                        {
                            if (idx >= bounds.Item1 && idx <= bounds.Item2)
                            {
                                wordAlreadyUsed = true;
                                break;
                            }
                        }
                        if (wordAlreadyUsed)
                        {
                            // This word (in the text) is already combined with something else.
                            idx = 0;
                            startIdx = -1;
                        }
                        else
                        {
                            if (startIdx == -1)
                            {
                                startIdx = sorsoWdRcIdx;
                                idx++;
                            }
                            else
                            {
                                if (sorsoWdRcIdx - startIdx == compoundWordParts.Words.Count - 1)
                                {
                                    compoundWordBounds.Add(new Tuple<int, int, PartOfSpeech?>(startIdx, sorsoWdRcIdx, compoundWordRec.Override));
                                    startIdx = -1;
                                    idx = 0;
                                } else
                                {
                                    idx++;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (idx > 0)
                        {
                            // Back up to try this one again with the beginning of the compound word.
                            sorsoWdRcIdx--;
                            idx = 0;
                            startIdx = -1;
                        }
                    }
                }
            }
            // TODO Place these in a Constants class
            char[] sesConsonants = new char[] { 'p', 'k', 'm', 'f', 's', 'l', 't' };
            char[] sesWkifAltConsonants = new char[] { 'b', 'g', 'n', 'v', 'z', 'r', 'd' };
            char[] sesWkifVowels = new char[] { 'o', 'e', 'ı', 'a', 'u', 'w', 'y' };
            char[] sesWkifAcuteAccentVowels = new char[] { 'ó', 'é', 'í', 'á', 'ú', 'ẃ', 'ý' };
            char[] sesWkifGraveAccentVowels = new char[] { 'ò', 'è', 'ì', 'à', 'ù', 'ẁ', 'ỳ' };
            char[] sesWkifConsonantsDottedUpper = new char[] { 'Ṗ', 'Ḳ', 'Ṁ', 'Ḟ', 'Ṡ', 'Ḷ', 'Ṫ' };
            char[] sesWkifConsonantsDottedLower = new char[] { 'ṗ', 'ḳ', 'ṁ', 'ḟ', 'ṡ', 'ḷ', 'ṫ' };
            char[] sesWkifAltConsonantsDottedUpper = new char[] { 'Ḃ', 'Ġ', 'Ṅ', 'Ṿ', 'Ż', 'Ṙ', 'Ḍ' };
            char[] sesWkifAltConsonantsDottedLower = new char[] { 'ḃ', 'ġ', 'ṅ', 'ṿ', 'ż', 'ṙ', 'ḍ' };
            char[] sesWkifVowelsDottedUpper = new char[] { 'Ȯ', 'Ė', 'İ', 'Ȧ', 'Ụ', 'Ẇ', 'Ẏ' };
            char[] sesWkifVowelsDottedLower = new char[] { 'ȯ', 'ė', 'i', 'ȧ', 'ụ', 'ẇ', 'ẏ' };
            char[] sesWkifDoubleAccents = new char[] { 'ô', 'ê', 'î', 'â', 'û', 'ŵ', 'ŷ' };

            StringBuilder textOut = new();
            StringBuilder sesWord = new();
            int firstWrd = -1;
            int lastWrd = -1;
            PartOfSpeech? partOfSpeechOverride = null;
            bool isVowel = false;
            void resetWord()
            {
                sesWord.Clear();
                isVowel = false;
                firstWrd = -1;
                lastWrd = -1;
                partOfSpeechOverride = null;
            }
            for (int wrdIdx = 0; wrdIdx < sorsoRec.Words.Count; wrdIdx++)
            {
                var currentWord = sorsoRec.Words[wrdIdx];

                string specialCase = CheckSpecialCases(currentWord);

                if (currentWord.IsValidSorso)
                {
                    if (firstWrd == -1)
                    {
                        foreach (var bounds in compoundWordBounds)
                        {
                            if (wrdIdx == bounds.Item1)
                            {
                                firstWrd = bounds.Item1;
                                lastWrd = bounds.Item2;
                                partOfSpeechOverride = bounds.Item3;
                                break;
                            }
                        }
                    }
                    bool inCompoundWord = firstWrd != -1;

                    // Determine whether to start the word off with a vowel.
                    if (inCompoundWord)
                    {
                        isVowel = wrdIdx == firstWrd && currentWord.Syllables.Count % 2 == 1;
                    }
                    else
                    {
                        if (currentWord.AccentIndices.Length == 0 && currentWord.InvertedAccentIndices.Length == 0
                            && currentWord.PluralIndices.Length > 0 && currentWord.PluralIndices[0] != currentWord.Syllables.Count - 1)
                        {
                            isVowel = currentWord.PluralIndices[0] % 2 == 0;
                        }
                        else if (currentWord.AccentIndices.Length == 0 && currentWord.InvertedAccentIndices.Length == 0
                            && currentWord.FeminineIndices.Length > 0 && currentWord.FeminineIndices[0] != currentWord.Syllables.Count - 1)
                        {
                            isVowel = currentWord.FeminineIndices[0] % 2 == 0;
                        }
                        else if (currentWord.AccentIndices.Length > 0 || currentWord.InvertedAccentIndices.Length > 0)
                        {
                            isVowel = Math.Max(currentWord.AccentIndices.Length > 0 ? currentWord.AccentIndices[0] : -1,
                                currentWord.InvertedAccentIndices.Length > 0 ? currentWord.InvertedAccentIndices[0] : -1) % 2 == 0;
                        }
                        isVowel = isVowel || currentWord.Syllables.Count == 1;
                    }

                    // Step through each syllable and add the appropriate character at each step.
                    for (int syllableIdx = 0; syllableIdx < currentWord.Syllables.Count; syllableIdx++)
                    {
                        int sesIdx = (int)currentWord.Syllables[syllableIdx].Degree;
                        var syllable = currentWord.Syllables[syllableIdx];
                        if (inCompoundWord)
                        {
                            // Compound word letters (dots or plain)
                            bool hasNoRegAccent = currentWord.AccentIndices.Length == 0 && currentWord.InvertedAccentIndices.Length == 0;
                            if (currentWord.AccentIndices.Length > 0 && syllableIdx == currentWord.AccentIndices[0]
                                || currentWord.InvertedAccentIndices.Length > 0 && syllableIdx == currentWord.InvertedAccentIndices[0]
                                || hasNoRegAccent && currentWord.PluralIndices.Length > 0 && syllableIdx == currentWord.PluralIndices[0]
                                || hasNoRegAccent && currentWord.FeminineIndices.Length > 0 && syllableIdx == currentWord.FeminineIndices[0])
                            {
                                if (syllableIdx == 0 && wrdIdx != firstWrd)
                                {
                                    // Should never be a vowel here
                                    sesWord.Append(syllable.IsCapitalized ? sesWkifAltConsonantsDottedUpper[sesIdx] : sesWkifAltConsonantsDottedLower[sesIdx]);
                                }
                                else
                                {
                                    if (isVowel)
                                    {
                                        sesWord.Append(syllable.IsCapitalized ? sesWkifVowelsDottedUpper[sesIdx] : sesWkifVowelsDottedLower[sesIdx]);
                                    }
                                    else
                                    {
                                        sesWord.Append(syllable.IsCapitalized ? sesWkifConsonantsDottedUpper[sesIdx] : sesWkifConsonantsDottedLower[sesIdx]);
                                    }
                                }
                            }
                            else
                            {
                                if (syllableIdx == 0 && wrdIdx != firstWrd)
                                {
                                    // Should never be a vowel here
                                    sesWord.Append(sesWkifAltConsonants[sesIdx]);
                                }
                                else
                                {
                                    sesWord.Append(isVowel ? sesWkifVowels[sesIdx] : sesConsonants[sesIdx]);
                                }
                            }

                            // Compound word capital letter(s)
                            // NOTE This has no effect on certain chars (the ones with both upper and lower arrays)
                            if (syllable.IsCapitalized)
                            {
                                sesWord[syllableIdx] = char.ToUpperInvariant(sesWord[syllableIdx]);
                            }

                            // Add part of speech accent once the full compound word is assembled
                            if (syllableIdx == currentWord.Syllables.Count - 1
                                && wrdIdx == lastWrd)
                            {
                                int replacementIndex = -1;
                                PartOfSpeech finalPartOfSpeech = partOfSpeechOverride ?? sorsoRec.Words[firstWrd].PartOfSpeech;
                                switch (finalPartOfSpeech)
                                {
                                    case PartOfSpeech.Noun:
                                        replacementIndex = sorsoRec.Words[firstWrd].Syllables.Count % 2 == 0 ? 1 : 0;
                                        break;
                                    case PartOfSpeech.AgentNoun:
                                        replacementIndex = sorsoRec.Words[firstWrd].Syllables.Count % 2 == 0 ? 3 : 2;
                                        break;
                                    case PartOfSpeech.Adjective:
                                        int search = sesWord.Length - 3;
                                        while (search > 1)
                                        {
                                            if (Array.IndexOf(sesWkifVowels, sesWord[search]) != -1
                                                || Array.IndexOf(sesWkifVowelsDottedLower, sesWord[search]) != -1
                                                || Array.IndexOf(sesWkifVowelsDottedUpper, sesWord[search]) != -1)
                                            {
                                                replacementIndex = search;
                                                break;
                                            }
                                            search--;
                                        }
                                        break;
                                    case PartOfSpeech.Adverb:
                                        replacementIndex = syllableIdx % 2 == 0 ? sesWord.Length - 2 : sesWord.Length - 1;
                                        break;
                                }
                                if (replacementIndex != -1)
                                {
                                    int vowelIdx = Array.IndexOf(sesWkifVowelsDottedLower, sesWord[replacementIndex]);
                                    if (vowelIdx != -1)
                                    {
                                        sesWord[replacementIndex] = sesWkifDoubleAccents[vowelIdx];
                                    }
                                    else
                                    {
                                        vowelIdx = Array.IndexOf(sesWkifVowelsDottedUpper, sesWord[replacementIndex]);
                                        if (vowelIdx != -1)
                                        {
                                            sesWord[replacementIndex] = char.ToUpper(sesWkifDoubleAccents[vowelIdx]);
                                        }
                                        else
                                        {
                                            vowelIdx = Array.IndexOf(sesWkifVowels, char.ToLower(sesWord[replacementIndex]));
                                            if (vowelIdx != -1)
                                            {
                                                sesWord[replacementIndex] = char.GetUnicodeCategory(sesWord[replacementIndex]) == UnicodeCategory.UppercaseLetter
                                                    ? char.ToUpper(sesWkifAcuteAccentVowels[vowelIdx])
                                                    : sesWkifAcuteAccentVowels[vowelIdx];
                                            } else
                                            {
                                                // TODO Log this
                                            }
                                        }
                                    }
                                }

                                // Add compound word endings, suffix
                                if (sorsoRec.Words[firstWrd].FeminineIndices.Length > 0)
                                {
                                    sesWord.Append(syllable.IsAllCaps ? "NU" : "nu");
                                }
                                if (sorsoRec.Words[firstWrd].PluralIndices.Length > 0)
                                {
                                    if (isVowel || sorsoRec.Words[firstWrd].FeminineIndices.Length > 0)
                                    {
                                        sesWord.Append(syllable.IsAllCaps ? 'Ð' : 'ð');
                                    }
                                    else
                                    {
                                        sesWord[^1] = syllable.IsCapitalized ? char.ToUpperInvariant(sesWkifAltConsonants[sesIdx]) : sesWkifAltConsonants[sesIdx];
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (currentWord.AccentIndices.Length > 0 && currentWord.AccentIndices[0] == syllableIdx
                                || currentWord.InvertedAccentIndices.Length > 0 && currentWord.InvertedAccentIndices[0] == syllableIdx
                                || currentWord.PluralIndices.Length > 0 && currentWord.PluralIndices[0] == syllableIdx && syllableIdx < currentWord.Syllables.Count - 1
                                || currentWord.FeminineIndices.Length > 0 && currentWord.FeminineIndices[0] == syllableIdx && syllableIdx < currentWord.Syllables.Count - 1)
                            {
                                sesWord.Append(currentWord.InvertedAccentIndices.Length > 0 && currentWord.InvertedAccentIndices[0] == syllableIdx ? sesWkifGraveAccentVowels[sesIdx] : sesWkifAcuteAccentVowels[sesIdx]);
                            }
                            else
                            {
                                sesWord.Append(isVowel ? sesWkifVowels[sesIdx] : sesConsonants[sesIdx]);
                            }
                            if (syllable.IsCapitalized)
                            {
                                sesWord[syllableIdx] = char.ToUpperInvariant(sesWord[syllableIdx]);
                            }
                            if (syllableIdx == currentWord.Syllables.Count - 1)
                            {
                                if (currentWord.FeminineIndices.Length > 0)
                                {
                                    sesWord.Append(syllable.IsAllCaps ? "NU" : "nu");
                                }
                                if (currentWord.PluralIndices.Length > 0)
                                {
                                    if (isVowel || currentWord.FeminineIndices.Length > 0)
                                    {
                                        sesWord.Append(syllable.IsAllCaps ? 'Ð' : 'ð');
                                    }
                                    else
                                    {
                                        sesWord[syllableIdx] = syllable.IsCapitalized ? char.ToUpperInvariant(sesWkifAltConsonants[sesIdx]) : sesWkifAltConsonants[sesIdx];
                                    }
                                }
                            }
                        }
                        isVowel = !isVowel;
                    }
                    if (!inCompoundWord || wrdIdx == lastWrd)
                    {
                        if (!string.IsNullOrEmpty(specialCase))
                        {
                            sesWord.Clear();
                            sesWord.Append(specialCase);
                        }
                        if (inCompoundWord)
                        {
                            sesWord.Insert(0, sorsoRec.Words[firstWrd].Prefix);
                            if (firstWrd > 0 && !sorsoRec.Words[firstWrd - 1].Suffix.EndsWith('\r'))
                            {
                                sesWord.Insert(0, " ");
                            }
                        }
                        else
                        {
                            sesWord.Insert(0, currentWord.Prefix);
                            if (wrdIdx > 0 && !sorsoRec.Words[wrdIdx - 1].Suffix.EndsWith('\r'))
                            {
                                sesWord.Insert(0, " ");
                            }
                        }
                        sesWord.Append(currentWord.Suffix);
                        textOut.Append(sesWord);
                        resetWord();
                    }
                    else
                    {
                        isVowel = !isVowel;
                    }
                }
                else
                {
                    // Don't bother with invalid words.
                    textOut.Append(wrdIdx == 0 ? "---" : " ---");
                }
            }
            return textOut.ToString();
        }

        private string CheckSpecialCases(WordRec input)
        {
            if (input.Syllables.Count == 1 && input.Syllables[0].Degree == SorsoSyllableDegree.Sol && input.FeminineIndices.Length > 0 && input.FeminineIndices[0] == 0)
            {
                return input.Syllables[0].IsCapitalized || input.Syllables[0].IsAllCaps ? "Ū" : "ū";
            }
            return "";
        }
    }
}
