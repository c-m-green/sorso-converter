using NUnit.Framework;
using SolresolConverter.Converter;
using SolresolConverter;
using static SolresolConverter.Converter.Converter;

namespace SolresolConverterTests
{
    [TestFixture]
    [TestOf(typeof(Converter))]
    public class SesWkifWriterTests
    {
        // TODO Phase these out in favor of more targeted tests.
        [TestCaseSource(nameof(BasicCases))]
        public void BasicTest(string input, string expected, string[] compoundWords)
        {
            List<CompoundWord> words = new();
            foreach (var str in compoundWords)
            {
                words.Add(new CompoundWord(str));
            }
            Assert.AreEqual(expected, ConvertSolresol(input, SolresolFormat.Sorso, SolresolFormat.SesCmgreen, words));
        }

        public static object[] BasicCases =
        {
            new object[] { "Famisol mimilare, solre sîlala sirêre fasi fasidôla…", "Fıs mıle, se ŷlwdėk fy atól…", new string[] { "sîlala sirêre" } },
            new object[] { "Solsol dosolfala fare dore sol fasifa domifare faredomi.", "Su pufw fe pe u fyf pıfevepı.", new string[] { "domifare faredomi" } },
            new object[] { "Mifásila, misolrela!", "Mátwð, mukw!", Array.Empty<string>() },
            new object[] { "Dore domiremi mísolredo refamîdo.", "Pe pıkı íseb efíp.", Array.Empty<string>() },
            new object[] { "Dofaladô, mīremi, do solfalafa dore.", "Paló, íkınu, o sala pe.", Array.Empty<string>() }
        };

        [TestCase("doremi fasol", PartOfSpeech.Noun, "ókıvu")]
        [TestCase("doremi fasol", PartOfSpeech.Adjective, "okívu")]
        [TestCase("doremi fasol", PartOfSpeech.Adverb, "okıvú")]
        public void ConverterToSesWkif_ShortCompoundWord_OverridesPartOfSpeech(string input, PartOfSpeech partOfSpeech, string expected)
        {
            List<CompoundWord> compoundWords = new()
            {
                new CompoundWord(input, partOfSpeech)
            };
            Assert.AreEqual(expected, ConvertSolresol(input, SolresolFormat.Sorso, SolresolFormat.SesCmgreen, compoundWords));
        }

        [TestCase("doremi fasolla", PartOfSpeech.Noun, "ókıvul")]
        [TestCase("doremi fasolla", PartOfSpeech.Adjective, "okívul")]
        [TestCase("doremi fasolla", PartOfSpeech.Adverb, "okıvúl")]
        public void ConverterToSesWkif_ShortCompoundWordConsonant_OverridesPartOfSpeech(string input, PartOfSpeech partOfSpeech, string expected)
        {
            List<CompoundWord> compoundWords = new()
            {
                new CompoundWord(input, partOfSpeech)
            };
            Assert.AreEqual(expected, ConvertSolresol(input, SolresolFormat.Sorso, SolresolFormat.SesCmgreen, compoundWords));
        }

        [TestCase("doremifa solla", PartOfSpeech.Noun, "pémazw")]
        [TestCase("doremifa solla", PartOfSpeech.Adjective, "pemázw")]
        [TestCase("doremifa solla", PartOfSpeech.Adverb, "pemazẃ")]
        public void ConverterToSesWkif_MedCompoundWordConsonant_OverridesPartOfSpeech(string input, PartOfSpeech partOfSpeech, string expected)
        {
            List<CompoundWord> compoundWords = new()
            {
                new CompoundWord(input, partOfSpeech)
            };
            Assert.AreEqual(expected, ConvertSolresol(input, SolresolFormat.Sorso, SolresolFormat.SesCmgreen, compoundWords));
        }

        [TestCase("dôremifa solla", PartOfSpeech.Noun, "ṗémazw")]
        [TestCase("dôremifa solla", PartOfSpeech.Adjective, "ṗemázw")]
        [TestCase("dôremifa solla", PartOfSpeech.Adverb, "ṗemazẃ")]
        public void ConverterToSesWkif_MedCompoundWordVowel_OverridesPartOfSpeech(string input, PartOfSpeech partOfSpeech, string expected)
        {
            List<CompoundWord> compoundWords = new()
            {
                new CompoundWord(input, partOfSpeech)
            };
            Assert.AreEqual(expected, ConvertSolresol(input, SolresolFormat.Sorso, SolresolFormat.SesCmgreen, compoundWords));
        }

        [TestCase("doremifa sollasi", PartOfSpeech.Noun, "pémazwt")]
        [TestCase("doremifa sollasi", PartOfSpeech.Adjective, "pemázwt")]
        [TestCase("doremifa sollasi", PartOfSpeech.Adverb, "pemazẃt")]
        public void ConverterToSesWkif_MedLgCompoundWordConsonant_OverridesPartOfSpeech(string input, PartOfSpeech partOfSpeech, string expected)
        {
            List<CompoundWord> compoundWords = new()
            {
                new CompoundWord(input, partOfSpeech)
            };
            Assert.AreEqual(expected, ConvertSolresol(input, SolresolFormat.Sorso, SolresolFormat.SesCmgreen, compoundWords));
        }

        [TestCase("dôremifa sollasi", PartOfSpeech.Noun, "ṗémazwt")]
        [TestCase("dôremifa sollasi", PartOfSpeech.Adjective, "ṗemázwt")]
        [TestCase("dôremifa sollasi", PartOfSpeech.Adverb, "ṗemazẃt")]
        public void ConverterToSesWkif_MedLgCompoundWordVowel_OverridesPartOfSpeech(string input, PartOfSpeech partOfSpeech, string expected)
        {
            List<CompoundWord> compoundWords = new()
            {
                new CompoundWord(input, partOfSpeech)
            };
            Assert.AreEqual(expected, ConvertSolresol(input, SolresolFormat.Sorso, SolresolFormat.SesCmgreen, compoundWords));
        }

        [TestCase("dǒremi", "òkı")]
        [TestCase("dorěmi", "pèm")]
        [TestCase("doremǐ", "okì")]
        [TestCase("dôremi", "ókı")]
        [TestCase("dorêmi", "pém")]
        [TestCase("doremî", "okí")]
        public void ConverterToSesWkif_AccentDirection_IsRepresentedInOutput(string input, string expected)
        {
            Assert.AreEqual(expected, ConvertSolresol(input, SolresolFormat.Sorso, SolresolFormat.SesCmgreen));
        }
    }
}