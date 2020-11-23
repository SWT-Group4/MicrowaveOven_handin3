using NUnit.Framework;
using NSubstitute;
using Microwave.Classes.Controllers;
using Microwave.Classes.Interfaces;
using Microwave.Classes.Boundary;

namespace Microwave.Test.Integration
{
    [TestFixture]
    public class IT5_CookController
    {
        // Fakes
        private IUserInterface _stubUi;
        private IOutput _stubOutput;
        private ITimer _fakeTimer;

        // Real objects
        private PowerTube _powerTube;
        private Display _display;

        // System Under Test
        private CookController _sut;

        [SetUp]
        public void Setup()
        {
            // Fakes
            _stubUi = Substitute.For<IUserInterface>();
            _fakeTimer = Substitute.For<ITimer>();
            _stubOutput = Substitute.For<IOutput>();

            // Real Objects
            _powerTube = new PowerTube(_stubOutput);
            _display = new Display(_stubOutput);

            // System Under Test
            _sut = new CookController(
                _fakeTimer,
                _display,
                _powerTube,
                _stubUi);
        }

        // TC-01
        [Test]
        public void StartCooking_ValidParameters_DoesNotThrow()
        {
            // Arrange
            int inputPower = 350;
            int timerSetting = 2000;

            // Act & Assert
            Assert.That(() => _sut.StartCooking(
                inputPower,
                timerSetting), Throws.Nothing);
        }

        // TC-02
        [Test]
        public void StartCooking_PowerTubeIsOn_OutputsCorrectPowerPct()
        {
            // Arrange
            int inputPower = 350;
            int timerSetting = 2000;
            int expectedPowerPercentage = 50;

            // Act
            _sut.StartCooking(inputPower, timerSetting);

            // Assert output shows 50%
            _stubOutput.Received().OutputLine(
                Arg.Is<string>(
                    str => 
                        str.Contains($"{expectedPowerPercentage}")));
        }

        // TC-03
        [Test]
        public void StartCooking_TimerIsSet_DisplayOutputsCorrectTime()
        {
            // Arrange
            int inputPower = 350;
            int minutes = 60 * 1000;
            int seconds = 1000;
            int timerSetting = (10*minutes)+(15*seconds);

            _fakeTimer.TimeRemaining.Returns(timerSetting);

            // Act
            _sut.StartCooking(inputPower, 2500);
            _fakeTimer.TimerTick += Raise.Event();
            
            // Assert output shows 10:15
            _stubOutput.Received().OutputLine(
                Arg.Is<string>(str => str.Contains("10:15")));
        }
        
        // TC-04
        [Test]
        public void StartCooking_TimerExpires_OutputPowerTubeTurnsOff()
        {
            // Arrange
            int inputPower = 350;
            int timerSetting = 1000;

            // Act
            _sut.StartCooking(inputPower, timerSetting);
            _fakeTimer.Expired += Raise.Event();

            // Assert
           _stubOutput.Received().OutputLine(
                Arg.Is<string>(
                    str => str.Contains("PowerTube turned off")));
        }

    }
}


