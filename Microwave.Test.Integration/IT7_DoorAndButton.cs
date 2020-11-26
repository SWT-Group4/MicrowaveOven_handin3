using System;
using NUnit.Framework;
using NSubstitute;
using Microwave.Classes.Boundary;
using Microwave.Classes.Controllers;
using Microwave.Classes.Interfaces;

namespace Microwave.Test.Integration
{
    [TestFixture]
    class IT7_DoorAndButton
    {

        private UserInterface _ui;
        private CookController _cookController;
        private PowerTube _powerTube;
        private Display _display;
        private Light _light;

        private IButton _StartCancelButton;
        private IButton _PowerButton;
        private IButton _TimeButton;
        private IDoor _Door;

        private ITimer _faketimer;
        private IOutput _fakeoutput;

        [SetUp]
        public void SetUp()
        {
            _fakeoutput = Substitute.For<IOutput>();
            _faketimer = Substitute.For<ITimer>();

            _StartCancelButton = new Button();
            _PowerButton = new Button();
            _TimeButton = new Button();
            _Door = new Door();

            _powerTube = new PowerTube(_fakeoutput);
            _display = new Display(_fakeoutput);
            _light = new Light(_fakeoutput);
            _cookController = new CookController(_faketimer, _display, _powerTube);
            _ui = new UserInterface(_PowerButton, _TimeButton, _StartCancelButton, _Door, _display, _light,
                _cookController);
            _cookController.UI = _ui;
        }

        #region Door <> UserInterface
        [Test]
        public void DoorOpen_BeforeSetup_LightLogsTurnOn()
        {
            _Door.Open();

            _fakeoutput.Received().OutputLine(Arg.Is<string>(str =>
                str.ToLower().Contains("light is turned on")
            ));
        }

        [Test]
        public void DoorOpenClose_BeforeSetup_LightLogsTurnOff()
        {
            _Door.Open();

            _fakeoutput.ClearReceivedCalls();

            _Door.Close();

            _fakeoutput.Received().OutputLine(Arg.Is<string>(str =>
                str.ToLower().Contains("light is turned off")
            ));
        }


        [Test]
        public void DoorOpenClose_WhileCooking_PowertubeTurnsOffAndDisplayClears()
        {
            _PowerButton.Press();
            _TimeButton.Press();
            _StartCancelButton.Press();

            _fakeoutput.ClearReceivedCalls();

            _Door.Open();

            _fakeoutput.Received().OutputLine(Arg.Is<string>(str =>
                str.ToLower().Contains("powertube turned off") 
            ));

            _fakeoutput.Received().OutputLine(Arg.Is<string>(str =>
                str.ToLower().Contains("display cleared")
            ));
        }
        #endregion

        // As we thoroughly tested all the possible scenarios in the previous step IT6,
        // we don't need to test all possible combinations of button presses.
        #region Button <> UserInterface
        [Test]
        public void PowerButtonPress_PressedOnce_DisplayShowsCorrectPower()
        {
            _PowerButton.Press();

            _fakeoutput.Received().OutputLine(Arg.Is<string>(str =>
                str.ToLower().Contains("display") &&
                str.ToLower().Contains("50") &&
                str.ToLower().Contains("w")
            ));
        }

        [Test]
        public void TimeButtonPress_PressedOnce_DisplayShowsCorrectTime()
        {
            _PowerButton.Press();
            _fakeoutput.ClearReceivedCalls();

            _TimeButton.Press();

            _fakeoutput.Received().OutputLine(Arg.Is<string>(str =>
                str.ToLower().Contains("display") &&
                str.ToLower().Contains("01:00")
            ));
        }

        [Test]
        public void StartCancelButtonPress_StartsCooking_PowertubeAndLightLogsTurnsOn()
        {
            _PowerButton.Press();
            _TimeButton.Press();
            _fakeoutput.ClearReceivedCalls();

            _StartCancelButton.Press();

            _fakeoutput.Received().OutputLine(Arg.Is<string>(str =>
                str.ToLower().Contains("powertube works with 7")
            ));

            _fakeoutput.Received().OutputLine(Arg.Is<string>(str =>
                str.ToLower().Contains("light is turned on")
            ));
        }
        #endregion


    }
}


