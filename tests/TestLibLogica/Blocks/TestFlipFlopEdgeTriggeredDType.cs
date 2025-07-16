using System;
using System.Linq;
using LibLogica.Blocks;
using NUnit.Framework;

namespace TestLibLogica.Blocks
{
    public class TestFlipFlopEdgeTriggeredDType
    {
        private FlipFlopEdgeTriggeredDType _block;

        [SetUp]
        public void Setup()
        {
            _block = new FlipFlopEdgeTriggeredDType();
        }

        [Test]
        public void InputD_InitiallyFalse()
        {
            Assert.That(_block.D.Value, Is.EqualTo(false));
        }

        [Test]
        public void InputClock_InitiallyFalse()
        {
            Assert.That(_block.Clock.Value, Is.EqualTo(false));
        }

        [Test]
        public void OutputQ_InitiallyFalse()
        {
            Assert.That(_block.Q.Value, Is.EqualTo(false));
        }

        [Test]
        public void OutputNQ_InitiallyTrue()
        {
            Assert.That(_block.NQ.Value, Is.EqualTo(true));
        }

        public static Object[] UpdateQTestCases =
        [
            //             d,     isRising, expectedQ
            new Object[] { false, false, false },
            new Object[] { false, true, false },
            new Object[] { true, false, false },
            new Object[] { true, true, true },
        ];

        [TestCaseSource(nameof(UpdateQTestCases))]
        public void Update_SetsQ(Boolean d, Boolean isRisingEdge, Boolean expectedQ)
        {
            _block.D.Value = d;
            _block.Clock.Value = !isRisingEdge;
            _block.Update();

            _block.Clock.Value = isRisingEdge;
            _block.Update();

            Assert.That(_block.Q.Value, Is.EqualTo(expectedQ));
        }

        public static Object[] UpdateNQTestCases =
        [
            //             d,     isRising, expectedNq
            new Object[] { false, false, true },
            new Object[] { false, true, true },
            new Object[] { true, false, true },
            new Object[] { true, true, false },
        ];

        [TestCaseSource(nameof(UpdateNQTestCases))]
        public void Update_SetsNQ(Boolean d, Boolean isRisingEdge, Boolean expectedNQ)
        {
            _block.D.Value = d;
            _block.Clock.Value = !isRisingEdge;
            _block.Update();

            _block.Clock.Value = isRisingEdge;
            _block.Update();

            Assert.That(_block.NQ.Value, Is.EqualTo(expectedNQ));
        }


        [Test]
        public void GetIdsAndGetValues_ContainSameNumberOfElements()
        {
            Assert.That(_block.GetIds().Count(), Is.EqualTo(_block.GetValues().Count()));
        }
    }
}
