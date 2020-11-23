using NUnit.Framework;
using NSubstitute;
using Microwave.Classes.Controllers;
using Microwave.Classes.Interfaces;
using Timer = Microwave.Classes.Boundary.Timer;

namespace Microwave.Test.Integration
{
    [TestFixture]
    public class IT4_CookController
    {
        // Fakes
        private IUserInterface _stubUi;
        private IPowerTube _stubPowerTube;
        private IDisplay _stubDisplay;
        private ILight _stubLight;

        // Real objects
        private Timer _timer;
        private Timer _refTimer;

        // System Under Test
        private CookController _sut;

        [SetUp]
        public void Setup()
        {
            // Fakes
            _stubUi = Substitute.For<IUserInterface>();
            _stubPowerTube = Substitute.For<IPowerTube>();
            _stubDisplay = Substitute.For<IDisplay>();
            _stubLight = Substitute.For<ILight>();

            // Real objects
            _timer = new Timer();
            _refTimer = new Timer();

            // System Under Test
            _sut = new CookController(_timer, _stubDisplay, _stubPowerTube, _stubUi);
        }

        // TC-01
        [Test] public void StartCooking_TimerStarts_PowerTubeTurnsOn()
        {
            // Arrange
            int inputPower = 350;
            int expectedPercentage = 50;
            int timerSetting = 2000;
            // Ensure test only executes once
            bool oneShot = true;

            // Act
            _sut.StartCooking(inputPower, timerSetting);
            // ref timer has 100ms extra to ensure the sut timer expires first
            _refTimer.Start(timerSetting + 100);

            while (_refTimer.TimeRemaining > 0)
            {
                if (oneShot)
                {
                    oneShot = false;

                    // Assert Power "consumption" in tube is expectedPercentage
                    _stubPowerTube.Received().TurnOn(expectedPercentage);
                }
            }
        }
        
        // TC-02
        [Test]
        public void StartCooking_ValidParameters_DisplayShowCorrectTime()
        {
            // Arrange
            int expectedMinutes = 0;
            int expectedSeconds = 3;
            int timerSetting = 5000;
            // Ensure test only executes once
            bool oneShot = true;

            // Act
            _sut.StartCooking(50, timerSetting);
            _refTimer.Start(timerSetting);

            while (_refTimer.TimeRemaining > 0)
            {
                // Probe when reference timer is at 3000ms
                if (_refTimer.TimeRemaining == 3000)
                {
                    if (oneShot)
                    {
                        oneShot = false;
                        
                        // Assert
                        _stubDisplay.Received().ShowTime(
                            Arg.Is<int>(expectedMinutes),
                            Arg.Is<int>(expectedSeconds));
                    }
                }
            }
        }

        // TC-03
        [Test]
        public void StartCooking_TimerExpires_DisplayShowCorrectTime()
        {
            // Arrange
            int timerSetting = 5000;

            // Act
            _sut.StartCooking(350, timerSetting);
            // ref timer has 100ms extra to ensure the sut timer expires first
            _refTimer.Start(timerSetting+100);

            while (_refTimer.TimeRemaining > 0)
            {
                // Wait until reference timer expires
            }

            // Assert time is 00:00
            _stubDisplay.Received().ShowTime(
                Arg.Is<int>(0),
                Arg.Is<int>(0));
        }

        // TC-04
        [Test]
        public void StartCooking_TimerExpires_PowerTubeTurnsOff()
        {
            // Arrange
            int timerSetting = 2000;

            // Act
            _sut.StartCooking(350, timerSetting);
            // ref timer has 100ms extra to ensure the sut timer expires first
            _refTimer.Start(timerSetting+100);

            while (_refTimer.TimeRemaining > 0)
            {
                // Wait until timer expires
            }

            // Assert power tube is set off when timer expires
            _stubPowerTube.Received().TurnOff();
        }

        // TC-05
        [Test]
        public void StartCooking_TimerExpires_UICookingIsDone()
        {
            // Arrange
            int timerSetting = 2000;

            // Act
            _sut.StartCooking(350, timerSetting);
            // ref timer has 100ms extra to ensure the sut timer expires first
            _refTimer.Start(timerSetting + 100);

            while (_refTimer.TimeRemaining > 0)
            {
                // Wait until timer expires
            }

            // Assert power tube is set off when timer expires
            _stubUi.Received().CookingIsDone();
        }
    }
}