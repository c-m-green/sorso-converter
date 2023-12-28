using NUnit.Framework;
using SolresolConverter;
using SolresolConverter.Analyzer;
using SolresolConverter.Converter;

namespace SolresolConverterTests
{
    [TestFixture]
    [TestOf(typeof(InputAnalyzer))]
    public class WordAnalyzerTests
    {
        [TestCase("soda", SolresolFormat.Sorso)]
        [TestCase("sodo", SolresolFormat.Sorso)]
        public void WordAnalyzer_IncompleteSolAtStart_ReportsInvalid(string s, SolresolFormat src)
        {
            var rec = InputAnalyzer.ReadInputText(s, src);
            Assert.IsFalse(rec.Words[0].IsValidSorso);
        }

        [TestCase("dosodo", SolresolFormat.Sorso)]
        [TestCase("dasodo", SolresolFormat.Sorso)]
        [TestCase("dosoda", SolresolFormat.Sorso)]
        public void WordAnalyzer_IncompleteSolInMiddle_ReportsInvalid(string s, SolresolFormat src)
        {
            var rec = InputAnalyzer.ReadInputText(s, src);
            Assert.IsFalse(rec.Words[0].IsValidSorso);
        }

        [TestCase("doso", SolresolFormat.Sorso)]
        [TestCase("daso", SolresolFormat.Sorso)]
        public void WordAnalyzer_IncompleteSolAtEnd_ReportsInvalid(string s, SolresolFormat src)
        {
            var rec = InputAnalyzer.ReadInputText(s, src);
            Assert.IsFalse(rec.Words[0].IsValidSorso);
        }

        [TestCase("solla", SolresolFormat.Sorso, 0, 1)]
        [TestCase("dosolla", SolresolFormat.Sorso, 1, 2)]
        [TestCase("sollado", SolresolFormat.Sorso, 0, 1)]
        [TestCase("dosollado", SolresolFormat.Sorso, 1, 2)]
        public void WordAnalyzer_SolAgainstLa_ParsesAsExpected(string s, SolresolFormat src, int solIdx, int laIdx)
        {
            var rec = InputAnalyzer.ReadInputText(s, src);
            Assert.IsTrue(rec.Words[0].IsValidSorso);
            Assert.AreEqual(SorsoSyllableDegree.Sol, rec.Words[0].Syllables[solIdx].Syllable);
            Assert.AreEqual(SorsoSyllableDegree.La, rec.Words[0].Syllables[laIdx].Syllable);
        }

        [TestCase("sollla", SolresolFormat.Sorso)]
        [TestCase("dosollla", SolresolFormat.Sorso)]
        [TestCase("solllado", SolresolFormat.Sorso)]
        [TestCase("dosolllado", SolresolFormat.Sorso)]
        public void WordAnalyzer_ExtraL_ReportsValid(string s, SolresolFormat src)
        {
            var rec = InputAnalyzer.ReadInputText(s, src);
            Assert.IsTrue(rec.Words[0].IsValidSorso);
            Assert.AreNotEqual(-1, rec.Words[0].PluralIdx);
        }

        [TestCase("sidodo", SolresolFormat.Sorso, 0)]
        [TestCase("resila", SolresolFormat.Sorso, 1)]
        [TestCase("solmisi", SolresolFormat.Sorso, 2)]
        public void WordAnalyzer_DetectsSi_ParsesAsExpected(string s, SolresolFormat src, int siIdx)
        {
            var rec = InputAnalyzer.ReadInputText(s, src);
            Assert.IsTrue(rec.Words[0].IsValidSorso);
            Assert.AreEqual(SorsoSyllableDegree.Si, rec.Words[0].Syllables[siIdx].Syllable);
        }
    }
}