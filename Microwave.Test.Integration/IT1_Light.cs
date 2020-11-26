using NUnit.Framework;
//using NSubstitute;
using Microwave.Classes.Boundary;
using Microwave.Classes.Controllers;
using Microwave.Classes.Interfaces;

namespace Microwave.Test.Integration
{
    [TestFixture]
    public class IT1_Light
    {

        private ILight _light;
        private IOutput _output;

        [SetUp]
        public void SetUp()
        {   
            _output = new Output();
            _light = new Light(_output);
        }

        [Test]
        public void OnTurnOnLights_WhenLightsOff_OutputIsMade()
        {
            _light.TurnOn();
        }

        [Test]
        public void OnTurnOnLights_WhenLightsOn_OutputIsMadeOnce()
        {
            _light.TurnOn();
            _light.TurnOn();
        }

        [Test]
        public void OnTurnOffLights_WhenLightsOff_OutputIsNotMade()
        {
            _light.TurnOff();
        }

        [Test]
        public void OnTurnOffLights_WhenLightsOn_OutputIsMade()
        {
            _light.TurnOn();
            _light.TurnOff();
        }
    }
}