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
        }

        // Integration tests that test the interface between UI

        #region UserInterface <> CookController

        [Test]
        public void OnStartCancelPressed_PowerAndTimeSet_StartsCooking()
        {
            _fakePowerButton.Pressed +=
                Raise.EventWith(EventArgs.Empty);
            _fakeTimeButton.Pressed +=
                Raise.EventWith(EventArgs.Empty);
            _fakeStartCancelButton.Pressed +=
                Raise.EventWith(EventArgs.Empty);
            
            //Assert.That(() => _);

        }

        #endregion

        #region UserInterface <> Display
        [Test]
        public void OnPowerPressed_PressedOnce_DisplayShowsCorrectPower()
        {
            _fakePowerButton.Pressed +=
                Raise.EventWith(EventArgs.Empty);

            _fakeoutput.Received().OutputLine(Arg.Is<string>(str =>
                str.ToLower().Contains("display") &&
                str.ToLower().Contains("50") &&
                str.ToLower().Contains("w")
            ));
        }

        [Test]
        public void OnPowerPressed_PressedTwice_DisplayShowsCorrectPower()
        {
            _fakePowerButton.Pressed +=
                Raise.EventWith(EventArgs.Empty);

            _fakeoutput.ClearReceivedCalls();

            _fakePowerButton.Pressed +=
                Raise.EventWith(EventArgs.Empty);

            _fakeoutput.Received().OutputLine(Arg.Is<string>(str =>
                str.ToLower().Contains("display") &&
                str.ToLower().Contains("100") &&
                str.ToLower().Contains("w")
            ));
        }

        [Test]
        public void OnPowerPressed_PressedFourteenTimes_DisplayShowsCorrectPower()
        {
            for (int i = 0; i < 13; i++)
            {
                _fakePowerButton.Pressed +=
                    Raise.EventWith(EventArgs.Empty);
            }
            _fakeoutput.ClearReceivedCalls();

            _fakePowerButton.Pressed +=
                Raise.EventWith(EventArgs.Empty);

            _fakeoutput.Received().OutputLine(Arg.Is<string>(str =>
                str.ToLower().Contains("display") &&
                str.ToLower().Contains("700") &&
                str.ToLower().Contains("w")
            ));
        }

        [Test]
        public void OnPowerPressed_PressedFifteenTimes_PowerRollsOverDisplayShows()
        {
            for (int i = 0; i < 14; i++)
            {
                _fakePowerButton.Pressed +=
                    Raise.EventWith(EventArgs.Empty);
            }

            _fakeoutput.ClearReceivedCalls();
            _fakePowerButton.Pressed +=
                Raise.EventWith(EventArgs.Empty);

            _fakeoutput.Received().OutputLine(Arg.Is<string>(str =>
                str.ToLower().Contains("display") &&
                str.ToLower().Contains("50") &&
                str.ToLower().Contains("w")
            ));
        }


        //[Test]
        public void OnStartCancelPressed_StartsCooking_DisplayClears()
        {
            _fakePowerButton.Pressed +=
                Raise.EventWith(EventArgs.Empty);
            _fakeTimeButton.Pressed +=
                Raise.EventWith(EventArgs.Empty);
            _fakeStartCancelButton.Pressed +=
                Raise.EventWith(EventArgs.Empty);

            _faketimer.Expired += Raise.EventWith(EventArgs.Empty); // Issues right here

            _fakeoutput.Received().OutputLine(Arg.Is<string>(str =>
                str.ToLower().Contains("display cleared")
            ));

        }

        [Test]
        public void OnDoorOpened_WhileCooking_DisplayClears()
        {
            _fakePowerButton.Pressed +=
                Raise.EventWith(EventArgs.Empty);
            _fakeTimeButton.Pressed +=
                Raise.EventWith(EventArgs.Empty);
            _fakeStartCancelButton.Pressed +=
                Raise.EventWith(EventArgs.Empty);

            _fakeDoor.Opened += Raise.EventWith(EventArgs.Empty);

            _fakeoutput.Received().OutputLine(Arg.Is<string>(str =>
                str.ToLower().Contains("display cleared")
            ));

        }

        [Test]
        public void OnStartCancelPressed_WhileCooking_DisplayClears()
        {
            _fakePowerButton.Pressed +=
                Raise.EventWith(EventArgs.Empty);
            _fakeTimeButton.Pressed +=
                Raise.EventWith(EventArgs.Empty);
            _fakeStartCancelButton.Pressed +=
                Raise.EventWith(EventArgs.Empty);

            _fakeStartCancelButton.Pressed +=
                Raise.EventWith(EventArgs.Empty);

            _fakeoutput.Received().OutputLine(Arg.Is<string>(str =>
                str.ToLower().Contains("display cleared")
            ));

        }



        #endregion

        #region UserInterface <> Light


        #endregion

        #region Maybe UserInterface <> Door/Button

        #endregion

    }
}
