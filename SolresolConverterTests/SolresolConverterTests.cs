using NUnit.Framework;
using SolresolConverter;
using SolresolConverter.Analyzer;

namespace SolresolConverterTests
{
    [TestFixture]
    [TestOf(typeof(InputAnalyzer))]
    public class WordAnalyzerTests
    {
        [TestCase("soda", SolresolFormat.Sorso)]
        [TestCase("solda", SolresolFormat.Sorso)]
        [TestCase("sodo", SolresolFormat.Sorso)]
        [TestCase("doso", SolresolFormat.Sorso)]
        public void FirstTest_GonnaAddMoreLater_ThisOneShouldWorkTho(string input, SolresolFormat src)
        {
            var rec = InputAnalyzer.ReadInputText(input, src);
            Assert.IsFalse(rec.Words[0].IsValidSorso);
        }
    }
}