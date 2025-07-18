using System;
using NUnit.Framework;
using LibLogica.Gates;

namespace TestLibLogica.Gates;

public class TestXorGate : BinaryGateTestBase<XorGate>
{
    [Test]
    public void Update_SetsO_ToLogicalXorOfAAndB_FalseFalse()
    {
        TestLogicOperation(false, false, false);
    }

    [Test]
    public void Update_SetsO_ToLogicalXorOfAAndB_FalseTrue()
    {
        TestLogicOperation(false, true, true);
    }

    [Test]
    public void Update_SetsO_ToLogicalXorOfAAndB_TrueFalse()
    {
        TestLogicOperation(true, false, true);
    }

    [Test]
    public void Update_SetsO_ToLogicalXorOfAAndB_TrueTrue()
    {
        TestLogicOperation(true, true, false);
    }
}
