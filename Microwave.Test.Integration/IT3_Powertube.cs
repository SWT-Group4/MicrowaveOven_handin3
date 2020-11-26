using System;
using NUnit.Framework;
using Microwave.Classes.Boundary;
using Microwave.Classes.Controllers;
using Microwave.Classes.Interfaces;

namespace Microwave.Test.Integration
{
    [TestFixture]
    public class IT3_Powertube
    {
        private IPowerTube _powerTube;
        private IOutput _output;

        [SetUp]
        public void SetUp()
        {
            _output = new Output();
            _powerTube = new PowerTube(_output);
        }

        [Test]
        public void OnTurnOn_MessageWithPowerOf42_IsMade()
        {
            _powerTube.TurnOn(42);

        }

        [Test]
        public void OnTurnOn_IfAlreadyOn_ExceptionThrown()
        {
            _powerTube.TurnOn(42);
            Assert.Throws<ApplicationException>
                (() => _powerTube.TurnOn(42));
        }

        [TestCase(101)]
        [TestCase(-1)]
        [TestCase(0)]
        public void OnTurnOn_IfValueOutOfRange_ExceptionThrown(int power)
        {
            Assert.Throws<ArgumentOutOfRangeException>
                (() => _powerTube.TurnOn(power));
        }

        [Test]
        public void OnTurnOff_IfNotTurnedOn_NoMessageIsMade()
        {
            _powerTube.TurnOff();
        }

        [Test]
        public void OnTurnOff_IfTurnedOn_TwoMessagesIsMade()
        {
            _powerTube.TurnOn(42);
            _powerTube.TurnOff();
        }
    }
}