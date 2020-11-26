using NUnit.Framework;
//using NSubstitute;
using Microwave.Classes.Boundary;
using Microwave.Classes.Controllers;
using Microwave.Classes.Interfaces;

namespace Microwave.Test.Integration
{
    [TestFixture]
    public class IT2_Display
    {
        private IDisplay _display;
        private IOutput _output;

        [SetUp]
        public void SetUp()
        {
            _output = new Output();
            _display = new Display(_output);
        }

        [Test]
        public void OnShowTime_OutputWith42MinAnd42Sec_IsMade()
        {
            _display.ShowTime(42, 42);
        }

        [Test]
        public void OnShowPower_OutputOf42W_IsMade()
        {
            _display.ShowPower(42);
        }

        [Test]
        public void OnClear_OutputMessage_WithClearedIsMade()
        {
            _display.Clear();
        }
    }
}