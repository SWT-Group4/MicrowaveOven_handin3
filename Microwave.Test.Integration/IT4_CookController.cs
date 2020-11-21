using NUnit.Framework;
using NSubstitute;
using Microwave.Classes.Boundary;
using Microwave.Classes.Controllers;
using Microwave.Classes.Interfaces;

namespace Microwave.Test.Integration
{
    [TestFixture]
    public class Integration4CookControllerTests
    {   
        // System Under Test
        private CookController _sut;

        // Real objects
        private Timer _timer;

        // Fakes
        private IUserInterface _stubUi;
        private IPowerTube _stubPowerTube;
        private IDisplay _stubDisplay;
        private ILight _stubLight;

        // Constants
        public const int TwoKiloWatt = 2000;
        public const int FiveSeconds = 5;

        [SetUp]
        public void Setup()
        {
            _stubUi = Substitute.For<IUserInterface>();
            _stubPowerTube = Substitute.For<IPowerTube>();
            _stubDisplay = Substitute.For<IDisplay>();
            _stubLight = Substitute.For<ILight>();

            _timer = new Timer();

            _sut = new CookController(_timer, _stubDisplay, _stubPowerTube, _stubUi);

            
        }

        [Test]
        public void StartCooking_ValidParameter_PowerTubeIsOn()
        {
            // Act
            _sut.StartCooking(TwoKiloWatt, FiveSeconds);
            
            // Assert
            //_stubPowerTube.Received()
        }


        [Test]
        public void Test1()
        {
            Assert.Pass();
        }
    }
}