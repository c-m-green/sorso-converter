using SolresolConverter.Converter;
using System.Globalization;
using static SolresolConverter.Analyzer.SolresolRecords;

namespace SolresolConverter.Analyzer
{
    internal class SorsoAnalyzer
    {
        public SorsoRec Analyze(string text)
        {
            const int maxSyllableLength = 5;
            // TODO Place these in a Constants class
            char[] sorsoConsonants = new char[] { 'd', 'r', 'm', 'f', 's', 'l', 's' };
            char[] sorsoVowels = new char[] { 'o', 'e', 'i', 'a', 'o', 'a', 'i' };
            char[] pluralAccents = new char[] { 'ó', 'é', 'í', 'á', 'ó', 'á', 'í' };
            char[] partOfSpeechAccents = new char[] { 'ô', 'ê', 'î', 'â', 'ô', 'â', 'î' };
            char[] partOfSpeechAltAccents = new char[] { 'ǒ', 'ě', 'ǐ', 'ǎ', 'ǒ', 'ǎ', 'ǐ' };
            char[] feminineAccents = new char[] { 'ō', 'ē', 'ī', 'ā', 'ō', 'ā', 'ī' };
            bool isValidConsonant(char ch)
            {
                return Array.Exists(sorsoConsonants, consonant => consonant == ch);
            }
            bool isExpectedConsonant(char ch, int idx)
            {
                try
                {
                    return sorsoConsonants[idx] == ch;
                }
                catch (IndexOutOfRangeException)
                {
                    return false;
                }
            }
            bool isValidVowel(char ch)
            {
                return Array.Exists(sorsoVowels, consonant => consonant == ch) ||
                        Array.Exists(pluralAccents, consonant => consonant == ch) ||
                        Array.Exists(partOfSpeechAccents, consonant => consonant == ch) ||
                        Array.Exists(partOfSpeechAltAccents, consonant => consonant == ch) ||
                        Array.Exists(feminineAccents, consonant => consonant == ch);
            }
            bool isExpectedVowel(char ch, int idx)
            {
                try
                {
                    return sorsoVowels[idx] == ch || pluralAccents[idx] == ch ||
                        partOfSpeechAccents[idx] == ch || partOfSpeechAltAccents[idx] == ch ||
                        feminineAccents[idx] == ch;
                }
                catch (IndexOutOfRangeException)
                {
                    return false;
                }
            }
            // Split up and analyze words
            string[] wordsIn = text.Split(new char[] { ' ', '-' });
            List<WordRec> wordRecs = new();
            foreach (string wordIn in wordsIn)
            {
                string[] currentWords = wordIn.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                for (int wordIdx = 0; wordIdx < currentWords.Length; wordIdx++)
                {
                    string currentWord = currentWords[wordIdx];
                    System.Diagnostics.Debug.WriteLine($"Reading {currentWord}...");
                    List<SyllableRec> syllableRecs = new();
                    string prefix = "", suffix = "";
                    SorsoSyllableDegree sorsoSyllableDegree;
                    int syllableIdx;
                    List<int> accentIndices = new();
                    List<int> invertedAccentIndices = new();
                    List<int> pluralIndices = new();
                    List<int> feminineIndices = new();
                    bool isAllCaps = true;
                    bool isCapitalized = false;
                    bool isValidSorso = true;
                    bool isPastPrefix = false;
                    void resetSyllable()
                    {
                        sorsoSyllableDegree = SorsoSyllableDegree.None;
                        syllableIdx = 0;
                        isCapitalized = false;
                    }
                    resetSyllable();
                    for (int charIdx = 0; charIdx < currentWord.Length; charIdx++)
                    {
                        char currentChar = char.ToLowerInvariant(currentWord[charIdx]);
                        if (isValidConsonant(currentChar))
                        {
                            isPastPrefix = true;
                            if (!string.IsNullOrEmpty(suffix))
                            {
                                // Found a punctuation mark in the middle of a word?
                                System.Diagnostics.Debug.WriteLine($"Unexpected extension of word: {currentChar} in {currentWord}");
                                isValidSorso = false;
                                resetSyllable();
                                continue;
                            }
                            if (syllableIdx == 0)
                            {
                                // Check capitalization of first actual valid letter
                                isCapitalized = char.GetUnicodeCategory(currentWord[charIdx]) == UnicodeCategory.UppercaseLetter;
                            }
                            isAllCaps = isAllCaps && isCapitalized && char.GetUnicodeCategory(currentWord[charIdx]) == UnicodeCategory.UppercaseLetter;
                            switch (syllableIdx)
                            {
                                case 0:
                                    // Can't tell sol from si yet
                                    if (currentChar == 's')
                                    {
                                        // Peek at the following char(s), accounting for a potential double consonant
                                        if (charIdx < currentWord.Length - 1 && isExpectedVowel(char.ToLowerInvariant(currentWord[charIdx + 1]), (int)SorsoSyllableDegree.Sol)
                                            || charIdx < currentWord.Length - 2 && isExpectedVowel(char.ToLowerInvariant(currentWord[charIdx + 2]), (int)SorsoSyllableDegree.Sol))
                                        {
                                            sorsoSyllableDegree = SorsoSyllableDegree.Sol;
                                            syllableIdx++;
                                        }
                                        else if (charIdx < currentWord.Length - 1 && isExpectedVowel(char.ToLowerInvariant(currentWord[charIdx + 1]), (int)SorsoSyllableDegree.Si)
                                            || charIdx < currentWord.Length - 2 && isExpectedVowel(char.ToLowerInvariant(currentWord[charIdx + 2]), (int)SorsoSyllableDegree.Si))
                                        {
                                            sorsoSyllableDegree = SorsoSyllableDegree.Si;
                                            syllableIdx++;
                                        }
                                        else
                                        {
                                            // Either the word ends before the syllable completes, or the vowel does not match.
                                            System.Diagnostics.Debug.WriteLine($"Possible v/c mismatch or premature word ending in {currentWord}");
                                            isValidSorso = false;
                                            resetSyllable();
                                        }
                                    }
                                    else
                                    {
                                        sorsoSyllableDegree = (SorsoSyllableDegree)Array.IndexOf(sorsoConsonants, currentChar);
                                        syllableIdx++;
                                    }
                                    break;
                                case 1:
                                    // Plural indicator if the consonants match.
                                    if (currentChar == sorsoConsonants[(int)sorsoSyllableDegree])
                                    {
                                        if (pluralIndices.Count > 0)
                                        {
                                            System.Diagnostics.Debug.WriteLine($"Too many plural indicators in {currentWord}");
                                            isValidSorso = false;
                                        }
                                        pluralIndices.Add(syllableRecs.Count);
                                        syllableIdx++;
                                    }
                                    else
                                    {
                                        System.Diagnostics.Debug.WriteLine($"Consecutive consonants do not match in syllable #{syllableRecs.Count + 1} of {currentWord}");
                                        isValidSorso = false;
                                        resetSyllable();
                                    }
                                    break;
                                default:
                                    System.Diagnostics.Debug.WriteLine($"Excessive consonants in syllable #{syllableRecs.Count + 1} of {currentWord}");
                                    isValidSorso = false;
                                    resetSyllable();
                                    break;
                            }
                        }
                        else if (isValidVowel(currentChar))
                        {
                            if (syllableIdx == 0 || !isPastPrefix || sorsoSyllableDegree == SorsoSyllableDegree.None)
                            {
                                // Started with a vowel?
                                System.Diagnostics.Debug.WriteLine($"Unexpected vowel encountered in syllable #{syllableRecs.Count + 1} of {currentWord}");
                                isValidSorso = false;
                                isPastPrefix = true;
                                resetSyllable();
                            }
                            else
                            {
                                isAllCaps = isAllCaps && isCapitalized && char.GetUnicodeCategory(currentWord[charIdx]) == UnicodeCategory.UppercaseLetter;
                                if (currentChar == pluralAccents[(int)sorsoSyllableDegree])
                                {
                                    if (pluralIndices.Count > 0)
                                    {
                                        System.Diagnostics.Debug.WriteLine($"Too many plural indicators in {currentWord}");
                                        isValidSorso = false;
                                    }
                                    pluralIndices.Add(syllableRecs.Count);
                                }
                                else if (currentChar == partOfSpeechAccents[(int)sorsoSyllableDegree])
                                {
                                    if (accentIndices.Count > 0 || invertedAccentIndices.Count > 0)
                                    {
                                        System.Diagnostics.Debug.WriteLine($"Too many part of speech indicators in {currentWord}");
                                        isValidSorso = false;
                                    }
                                    accentIndices.Add(syllableRecs.Count);
                                }
                                else if (currentChar == partOfSpeechAltAccents[(int)sorsoSyllableDegree])
                                {
                                    if (accentIndices.Count > 0 || invertedAccentIndices.Count > 0)
                                    {
                                        System.Diagnostics.Debug.WriteLine($"Too many part of speech indicators in {currentWord}");
                                        isValidSorso = false;
                                    }
                                    invertedAccentIndices.Add(syllableRecs.Count);
                                }
                                else if (currentChar == feminineAccents[(int)sorsoSyllableDegree])
                                {
                                    if (feminineIndices.Count > 0)
                                    {
                                        System.Diagnostics.Debug.WriteLine($"Too many feminine indicators in {currentWord}");
                                        isValidSorso = false;
                                    }
                                    feminineIndices.Add(syllableRecs.Count);
                                }

                                if (isExpectedConsonant(char.ToLowerInvariant(currentWord[charIdx - syllableIdx]), (int)sorsoSyllableDegree))
                                {
                                    // The vowel matches the consonant -- now look for the end of the syllable
                                    if (charIdx < currentWord.Length - 1 && isExpectedVowel(char.ToLowerInvariant(currentWord[charIdx + 1]), (int)sorsoSyllableDegree))
                                    {
                                        // Probably a feminine indicator (double vowel)
                                        if (isExpectedConsonant(char.ToLowerInvariant(currentWord[charIdx - 1]), (int)sorsoSyllableDegree))
                                        {
                                            if (feminineIndices.Count > 0)
                                            {
                                                System.Diagnostics.Debug.WriteLine($"Too many feminine indicators in {currentWord}");
                                                isValidSorso = false;
                                            }
                                            feminineIndices.Add(syllableRecs.Count);
                                            syllableIdx++;
                                        }
                                        else
                                        {
                                            System.Diagnostics.Debug.WriteLine($"Vowel issue in syllable #{syllableRecs.Count + 1} of {currentWord}");
                                            isValidSorso = false;
                                            resetSyllable();
                                        }
                                    }
                                    else
                                    {
                                        // The next char is either the end of the word or some other char.
                                        // See if it's the 'l' in "sol" or its variants
                                        if (sorsoSyllableDegree == SorsoSyllableDegree.Sol)
                                        {
                                            if (charIdx < currentWord.Length - 1 &&
                                                char.ToLowerInvariant(currentWord[charIdx + 1]) == 'l')
                                            {
                                                // Found Sol -- skip to the 'l' so it won't be digested in the next step
                                                charIdx++;
                                            }
                                            else
                                            {
                                                System.Diagnostics.Debug.WriteLine($"Truncated 'sol' at syllable #{syllableRecs.Count + 1} of {currentWord}");
                                                isValidSorso = false;
                                                resetSyllable();
                                                // Don't add this one.
                                                continue;
                                            }
                                        }
                                        syllableRecs.Add(new SyllableRec(sorsoSyllableDegree, isCapitalized, isAllCaps));
                                        resetSyllable();
                                    }
                                }
                                else
                                {
                                    // This is a malformed syllable.
                                    System.Diagnostics.Debug.WriteLine($"Mismatch encountered in syllable #{syllableRecs.Count + 1} of {currentWord}");
                                    isValidSorso = false;
                                    resetSyllable();
                                }
                            }

                        }
                        else
                        {
                            var unicodeCategory = char.GetUnicodeCategory(currentChar);
                            if (unicodeCategory == UnicodeCategory.ClosePunctuation || unicodeCategory == UnicodeCategory.FinalQuotePunctuation)
                            {
                                if (isPastPrefix)
                                {
                                    suffix = $"{suffix}{currentChar}";
                                }
                                else
                                {
                                    prefix = $"{prefix}{currentChar}";
                                }
                            }
                            else if (unicodeCategory == UnicodeCategory.InitialQuotePunctuation || unicodeCategory == UnicodeCategory.OpenPunctuation
                                || unicodeCategory == UnicodeCategory.OtherSymbol)
                            {
                                if (isPastPrefix)
                                {
                                    System.Diagnostics.Debug.WriteLine($"Unexpected character {currentChar} in word {currentWord}");
                                    isValidSorso = false;
                                }
                                else
                                {
                                    prefix = $"{prefix}{currentChar}";
                                }
                            }
                            else if (unicodeCategory == UnicodeCategory.Control || unicodeCategory == UnicodeCategory.OtherPunctuation)
                            {
                                if (isPastPrefix)
                                {
                                    suffix = $"{suffix}{currentChar}";
                                }
                                else
                                {
                                    prefix = $"{prefix}{currentChar}";
                                }
                            }
                            else
                            {
                                if (isPastPrefix)
                                {
                                    System.Diagnostics.Debug.WriteLine($"Unexpected character {currentChar} in word {currentWord}");
                                    isValidSorso = false;
                                }
                                else
                                {
                                    prefix = $"{prefix}{currentChar}";
                                }
                            }
                        }
                        // Last-ditch effort to catch wayward cases.
                        if (syllableIdx > maxSyllableLength - 1)
                        {
                            isValidSorso = false;
                            resetSyllable();
                        }
                    } // End syllable

                    if (wordIdx < currentWords.Length - 1)
                    {
                        suffix += "\r\n";
                    }

                    // Check for a broken syllable at the end.
                    if (syllableIdx > 0)
                    {
                        System.Diagnostics.Debug.WriteLine($"Unexpected end of word in {currentWord}");
                        isValidSorso = false;
                    }

                    // Check for illegal accent placement.
                    if (syllableRecs.Count > 4)
                    {
                        if (accentIndices.Count > 0 && accentIndices[0] > 1 && accentIndices[0] < syllableRecs.Count - 2
                            || invertedAccentIndices.Count > 0 && invertedAccentIndices[0] > 1 && invertedAccentIndices[0] < syllableRecs.Count - 2
                            || feminineIndices.Count > 0 && feminineIndices[0] > 1 && feminineIndices[0] < syllableRecs.Count - 2
                            || pluralIndices.Count > 0 && pluralIndices[0] > 1 && pluralIndices[0] < syllableRecs.Count - 2)
                        {
                            System.Diagnostics.Debug.WriteLine($"Invalid accent position in {currentWord}");
                            isValidSorso = false;
                        }
                    }

                    // Try detecting part of speech.
                    PartOfSpeech partOfSpeech = PartOfSpeech.VerbOrUnspecified;
                    if (isValidSorso && syllableRecs.Count >= 3)
                    {
                        if (accentIndices.Count > 0 || invertedAccentIndices.Count > 0)
                        {
                            partOfSpeech = (PartOfSpeech)(Math.Max(accentIndices.Count > 0 ? accentIndices[0] : -1,
                                invertedAccentIndices.Count > 0 ? invertedAccentIndices[0] : -1) - Math.Max(0, syllableRecs.Count - 4));
                        }
                        else if (accentIndices.Count == 0 && invertedAccentIndices.Count == 0
                            && pluralIndices.Count > 0 && pluralIndices[0] != syllableRecs.Count - 1)
                        {
                            partOfSpeech = (PartOfSpeech)(pluralIndices[0] - Math.Max(0, syllableRecs.Count - 4));
                        }
                        else if (accentIndices.Count == 0 && invertedAccentIndices.Count == 0
                            && feminineIndices.Count > 0 && feminineIndices[0] != syllableRecs.Count - 1)
                        {
                            partOfSpeech = (PartOfSpeech)(feminineIndices[0] - Math.Max(0, syllableRecs.Count - 4));
                        }

                        // Postfix for 3 letter words
                        if (syllableRecs.Count == 3 && (int)partOfSpeech > 0)
                        {
                            int partOfSpeechIdx = (int)partOfSpeech + 1;
                            partOfSpeech = (PartOfSpeech)partOfSpeechIdx;
                        }
                    }
                    else if (!isValidSorso)
                    {
                        System.Diagnostics.Debug.WriteLine($"WARN: {currentWord} is not valid Solresol input");
                    }
                    wordRecs.Add(new WordRec(syllableRecs, partOfSpeech, accentIndices.ToArray(), invertedAccentIndices.ToArray(), pluralIndices.ToArray(), feminineIndices.ToArray(), prefix, suffix, currentWord, isValidSorso));
                }
            } // End word

            return new SorsoRec(wordRecs, SolresolFormat.Sorso);
        }
    }
}
