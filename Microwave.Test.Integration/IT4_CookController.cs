using NUnit.Framework;
using NSubstitute;
using Microwave.Classes.Controllers;
using Microwave.Classes.Interfaces;
using NSubstitute.Core.Arguments;
using System;
using System.Threading;
using Timer = Microwave.Classes.Boundary.Timer;

namespace Microwave.Test.Integration
{
    [TestFixture]
    public class IT4_CookController
    {
        
        // System Under Test
        private CookController _sut;

        // Real objects
        private Timer _timer;
        private Timer _refTimer;

        // Fakes
        private IUserInterface _stubUi;
        private IPowerTube _stubPowerTube;
        private IDisplay _stubDisplay;
        private ILight _stubLight;

        [SetUp]
        public void Setup()
        {
            _stubUi = Substitute.For<IUserInterface>();
            _stubPowerTube = Substitute.For<IPowerTube>();
            _stubDisplay = Substitute.For<IDisplay>();
            _stubLight = Substitute.For<ILight>();
            
            _timer = new Timer();
            _refTimer = new Timer();

            _sut = new CookController(_timer, _stubDisplay, _stubPowerTube, _stubUi);
        }

        [Test]
        public void StartCooking_ValidParameters_DisplayShowCorrectTime()
        {
            // Arrange
            int expMinutes = 0;
            int expSeconds = 3;
            // Ensure test only executes once
            bool oneShot = true;

            // Act
            _sut.StartCooking(50, 5000);
            _refTimer.Start(5000);

            // Assert
            while (_refTimer.TimeRemaining > 0)
            {
                if (_refTimer.TimeRemaining == 3000)
                {
                    if (oneShot)
                    {
                        oneShot = false;
                        //_stubDisplay.ReceivedWithAnyArgs().ShowTime(minutes, seconds);
                        _stubDisplay.Received().ShowTime(Arg.Is<int>(expMinutes), Arg.Is<int>(expSeconds));
                    }
                }
            }
            // Timer has expired time is 00:00
            _stubDisplay.Received().ShowTime(Arg.Is<int>(0), Arg.Is<int>(0));
        }

        [Test]
        public void StartCooking_TimerExpires_DisplayShowCorrectTime()
        {
            // Arrange

            // Act
            _sut.StartCooking(50, 5000);
            // Make ref timer 100ms longer to ensure the sut timer has expired
            _refTimer.Start(5100);

            // Assert
            while (_refTimer.TimeRemaining > 0)
            {
                // Wait until timer expires
            }
            // Timer has expired time is 00:00
            _stubDisplay.Received().ShowTime(Arg.Is<int>(0), Arg.Is<int>(0));
        }

        [Test]
        public void StartCooking_TimerStarts_PowerTubeTurnsOn()
        {
            // Arrange
            // Ensure test only executes once
            bool oneShot = true;

            // Act
            _sut.StartCooking(50, 2000);
            // Make ref timer 100ms longer to ensure the sut timer has expired
            _refTimer.Start(2100);

            // Assert
            while (_refTimer.TimeRemaining > 0)
            {
                if (oneShot)
                { 
                    oneShot = false;
                    // Power tube is on while timer counts
                    _stubPowerTube.Received().TurnOn(50);
                }
            }
        }

        [Test]
        public void StartCooking_TimerExpires_PowerTubeTurnsOff()
        {
            // Arrange

            // Act
            _sut.StartCooking(50, 2000);
            // Make ref timer 100ms longer to ensure the sut timer has expired
            _refTimer.Start(2100);

            while (_refTimer.TimeRemaining > 0)
            {
                // Wait until timer expires
            }

            // Assert
            // Timer expired power tube is set off
            _stubPowerTube.Received().TurnOff();
        }
    }
}