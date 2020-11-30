using System;
using NUnit.Framework;
using NSubstitute;
using Microwave.Classes.Boundary;
using Microwave.Classes.Controllers;
using Microwave.Classes.Interfaces;

namespace Microwave.Test.Integration
{
    [TestFixture]
    class IT6_UserInterface
    {

        private UserInterface _ui;
        private CookController _cookController;
        private PowerTube _powerTube;
        private Display _display;
        private Light _light;

        private IButton _fakeStartCancelButton;
        private IButton _fakePowerButton;
        private IButton _fakeTimeButton;

        private IDoor _fakeDoor;
        private ITimer _faketimer;
        private IOutput _fakeoutput;

        [SetUp]
        public void SetUp()
        {
            _fakeStartCancelButton = Substitute.For<IButton>();
            _fakePowerButton = Substitute.For<IButton>();
            _fakeTimeButton = Substitute.For<IButton>();
            _fakeDoor = Substitute.For<IDoor>();
            _fakeoutput = Substitute.For<IOutput>();
            _faketimer = Substitute.For<ITimer>();

            _powerTube = new PowerTube(_fakeoutput);
            _display = new Display(_fakeoutput);
            _light = new Light(_fakeoutput);
            _cookController = new CookController(_faketimer, _display, _powerTube);
            _ui = new UserInterface(_fakePowerButton, _fakeTimeButton, _fakeStartCancelButton, _fakeDoor, _display, _light, _cookController);
            _cookController.UI = _ui;
        }


        #region UserInterface <> CookController

        // TC01-03
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(10)]
        public void OnStartCancelPressed_StartsCooking_TimerReceivedCorrectTime(int min)
        {
            _fakePowerButton.Pressed += Raise.Event();
            for (int i = 0; i < min; i++)
            {
                _fakeTimeButton.Pressed += Raise.Event();
            }
            _fakeStartCancelButton.Pressed += Raise.Event();

            _faketimer.Received().Start(Arg.Is(60*min));
        }

        // TC04-06
        [TestCase(1)]
        [TestCase(14)]
        [TestCase(15)]
        public void OnStartCancelPressed_StartsCooking_PowerTubeLogsCorrectPower(int power)
        {
            int percentagePower = ((power*50 * 100) / 700) % 100;

            for (int i = 0; i < power; i++)
            {
                _fakePowerButton.Pressed += Raise.Event();
            }
            _fakeTimeButton.Pressed += Raise.Event();

            _fakeoutput.ClearReceivedCalls();
            _fakeStartCancelButton.Pressed += Raise.Event();

            _fakeoutput.Received().OutputLine(Arg.Is<string>(str =>
                str.ToLower().Contains("powertube works") &&
                str.ToLower().Contains(percentagePower.ToString())
            ));
        }
        
        //TC07
        [Test]
        public void OnStartCancelPressed_StartsCooking_PowerTubeTurnsOff()
        {
            _fakePowerButton.Pressed += Raise.Event();
            _fakeTimeButton.Pressed += Raise.Event();

            _fakeStartCancelButton.Pressed += Raise.Event();

            _fakeoutput.ClearReceivedCalls();
            _faketimer.Expired += Raise.Event();

            _fakeoutput.Received().OutputLine(Arg.Is<string>(str =>
                str.ToLower().Contains("powertube turned off")
            ));
        }

        #endregion

        #region UserInterface <> Display
            
        // TC08
        [Test]
        public void OnPowerPressed_PressedOnce_DisplayShowsCorrectPower()
        {
            _fakePowerButton.Pressed += Raise.Event();

            _fakeoutput.Received().OutputLine(Arg.Is<string>(str =>
                str.ToLower().Contains("display") &&
                str.ToLower().Contains("50") &&
                str.ToLower().Contains("w")
            ));
        }

        // TC08
        [Test]
        public void OnPowerPressed_PressedTwice_DisplayShowsCorrectPower()
        {
            _fakePowerButton.Pressed += Raise.Event();

            _fakeoutput.ClearReceivedCalls();

            _fakePowerButton.Pressed += Raise.Event();

            _fakeoutput.Received().OutputLine(Arg.Is<string>(str =>
                str.ToLower().Contains("display") &&
                str.ToLower().Contains("100") &&
                str.ToLower().Contains("w")
            ));
        }

        //TC09
        [Test]
        public void OnPowerPressed_PressedFourteenTimes_DisplayShowsCorrectPower()
        {
            for (int i = 0; i < 13; i++)
            {
                _fakePowerButton.Pressed += Raise.Event();
            }
            _fakeoutput.ClearReceivedCalls();

            _fakePowerButton.Pressed += Raise.Event();

            _fakeoutput.Received().OutputLine(Arg.Is<string>(str =>
                str.ToLower().Contains("display") &&
                str.ToLower().Contains("700") &&
                str.ToLower().Contains("w")
            ));
        }

        //TC10
        [Test]
        public void OnPowerPressed_PressedFifteenTimes_PowerRollsOverDisplayShows()
        {
            for (int i = 0; i < 14; i++)
            {
                _fakePowerButton.Pressed += Raise.Event();
            }

            _fakeoutput.ClearReceivedCalls();
            _fakePowerButton.Pressed += Raise.Event();

            _fakeoutput.Received().OutputLine(Arg.Is<string>(str =>
                str.ToLower().Contains("display") &&
                str.ToLower().Contains("50") &&
                str.ToLower().Contains("w")
            ));
        }

        //TC11
        [Test]
        public void OnTimePressed_PressedOnce_DisplayShowsCorrectTime()
        {

            _fakePowerButton.Pressed += Raise.Event();
            _fakeTimeButton.Pressed += Raise.Event();

            _fakeoutput.Received().OutputLine(Arg.Is<string>(str =>
                str.ToLower().Contains("display") &&
                str.ToLower().Contains("01:00")
            ));
        }

        //TC12
        [Test]
        public void OnTimePressed_PressedTwice_DisplayShowsCorrectTime()
        {

            _fakePowerButton.Pressed += Raise.Event();
            _fakeTimeButton.Pressed += Raise.Event();

            _fakeoutput.ClearReceivedCalls();
            _fakeTimeButton.Pressed += Raise.Event();

            _fakeoutput.Received().OutputLine(Arg.Is<string>(str =>
                str.ToLower().Contains("display") &&
                str.ToLower().Contains("02:00")
            ));
        }

        //TC13
        [Test]
        public void OnStartCancelPressed_StartsCooking_DisplayClears()
        {
            _fakePowerButton.Pressed += Raise.Event();
            _fakeTimeButton.Pressed += Raise.Event();
            _fakeStartCancelButton.Pressed += Raise.Event();

            _faketimer.Expired += Raise.Event(); 

            _fakeoutput.Received().OutputLine(Arg.Is<string>(str =>
                str.ToLower().Contains("display cleared")
            ));

        }

        // Extensions behavior
        //TC14
        [Test]
        public void OnDoorOpened_WhileCooking_DisplayClears()
        {
            _fakePowerButton.Pressed += Raise.Event();
            _fakeTimeButton.Pressed += Raise.Event();
            _fakeStartCancelButton.Pressed += Raise.Event();

            _fakeDoor.Opened += Raise.Event();

            _fakeoutput.Received().OutputLine(Arg.Is<string>(str =>
                str.ToLower().Contains("display cleared")
            ));

        }

        //TC15
        [Test]
        public void OnStartCancelPressed_WhileCooking_DisplayClears()
        {
            _fakePowerButton.Pressed += Raise.Event();
            _fakeTimeButton.Pressed += Raise.Event();
            _fakeStartCancelButton.Pressed += Raise.Event();

            _fakeStartCancelButton.Pressed += Raise.Event();

            _fakeoutput.Received().OutputLine(Arg.Is<string>(str =>
                str.ToLower().Contains("display cleared")
            ));
        }

        //TC16
        [Test]
        public void OnDoorOpened_WhileSetupTime_DisplayClears()
        {
            _fakePowerButton.Pressed += Raise.Event();
            _fakeTimeButton.Pressed += Raise.Event();

            _fakeoutput.ClearReceivedCalls();
            _fakeDoor.Opened += Raise.Event();

            _fakeoutput.Received().OutputLine(Arg.Is<string>(str =>
                str.ToLower().Contains("display cleared")
            ));
        }

        //TC17
        [Test]
        public void OnStartCancelPressed_WhileSetupPower_DisplayClears()
        {
            _fakePowerButton.Pressed += Raise.Event();

            _fakeoutput.ClearReceivedCalls();
            _fakeStartCancelButton.Pressed += Raise.Event();

            _fakeoutput.Received().OutputLine(Arg.Is<string>(str =>
                str.ToLower().Contains("display cleared")
            ));
        }

        //TC18
        [Test]
        public void OnDoorOpens_WhileSetupPower_DisplayClears()
        {
            _fakePowerButton.Pressed += Raise.Event();

            _fakeoutput.ClearReceivedCalls();
            _fakeDoor.Opened += Raise.Event();

            _fakeoutput.Received().OutputLine(Arg.Is<string>(str =>
                str.ToLower().Contains("display cleared")
            ));
        }



        #endregion

        #region UserInterface <> Light
        //TC19
        [Test]
        public void OnDoorOpened_BeforeSetup_LightLogsTurnOn()
        {
            _fakeDoor.Opened += Raise.Event();

            _fakeoutput.Received().OutputLine(Arg.Is<string>(str =>
                str.ToLower().Contains("light is turned on")
            ));
        }

        //TC20
        [Test]
        public void OnDoorOpened_WhilePowerSetup_LightLogsTurnOn()
        {
            _fakePowerButton.Pressed += Raise.Event();
            _fakeoutput.ClearReceivedCalls();

            _fakeDoor.Opened += Raise.Event();

            _fakeoutput.Received().OutputLine(Arg.Is<string>(str =>
                str.ToLower().Contains("light is turned on")
            ));
        }
        
        //TC21
        [Test]
        public void OnDoorOpened_WhileTimeSetup_LightLogsTurnOn()
        {
            _fakePowerButton.Pressed += Raise.Event();
            _fakeTimeButton.Pressed += Raise.Event();
            _fakeoutput.ClearReceivedCalls();

            _fakeDoor.Opened += Raise.Event();

            _fakeoutput.Received().OutputLine(Arg.Is<string>(str =>
                str.ToLower().Contains("light is turned on")
            ));
        }

        //TC22
        [Test]
        public void OnDoorOpened_AfterCooking_LightLogsTurnOn()
        {
            _fakePowerButton.Pressed += Raise.Event();
            _fakeTimeButton.Pressed += Raise.Event();
            _fakeStartCancelButton.Pressed += Raise.Event();
            _faketimer.Expired += Raise.Event();
            _fakeoutput.ClearReceivedCalls();

            _fakeDoor.Opened += Raise.Event();

            _fakeoutput.Received().OutputLine(Arg.Is<string>(str =>
                str.ToLower().Contains("light is turned on")
            ));
        }

        //TC23
        [Test]
        public void OnStartCancelPressed_StartsCooking_LightLogsTurnOn()
        {
            _fakePowerButton.Pressed += Raise.Event();
            _fakeTimeButton.Pressed += Raise.Event();
            _fakeoutput.ClearReceivedCalls();

            _fakeStartCancelButton.Pressed += Raise.Event();

            _fakeoutput.Received().OutputLine(Arg.Is<string>(str =>
                str.ToLower().Contains("light is turned on")
            ));
        }

        //TC24
        [Test]
        public void OnDoorClosed_NothingElse_LightLogsTurnOff()
        {
            _fakeDoor.Opened += Raise.Event();
            _fakeoutput.ClearReceivedCalls();

            _fakeDoor.Closed += Raise.Event();

            _fakeoutput.Received().OutputLine(Arg.Is<string>(str =>
                str.ToLower().Contains("light is turned off")
            ));
        }

        //TC25
        [Test]
        public void OnStartCancelPressed_CookingIsDone_LightLogsTurnOff()
        {
            _fakePowerButton.Pressed += Raise.Event();
            _fakeTimeButton.Pressed += Raise.Event();
            _fakeStartCancelButton.Pressed += Raise.Event();
            _fakeoutput.ClearReceivedCalls();

            _faketimer.Expired += Raise.Event();

            _fakeoutput.Received().OutputLine(Arg.Is<string>(str =>
                str.ToLower().Contains("light is turned off")
            ));
        }

        //TC26
        [Test]
        public void OnStartCancelPressed_WhileCooking_LightLogsTurnOff()
        {
            _fakePowerButton.Pressed += Raise.Event();
            _fakeTimeButton.Pressed += Raise.Event();
            _fakeStartCancelButton.Pressed += Raise.Event();
            _fakeoutput.ClearReceivedCalls();

            _fakeStartCancelButton.Pressed += Raise.Event();

            _fakeoutput.Received().OutputLine(Arg.Is<string>(str =>
                str.ToLower().Contains("light is turned off")
            ));
        }

        #endregion

    }
}
