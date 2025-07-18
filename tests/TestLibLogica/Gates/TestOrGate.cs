using System;
using NUnit.Framework;
using LibLogica.Gates;

namespace TestLibLogica.Gates;

public class TestOrGate : BinaryGateTestBase<OrGate>
{
    [Test]
    public void Update_SetsO_ToLogicalOrOfAAndB_FalseFalse()
    {
        TestLogicOperation(false, false, false);
    }

    [Test]
    public void Update_SetsO_ToLogicalOrOfAAndB_FalseTrue()
    {
        TestLogicOperation(false, true, true);
    }

    [Test]
    public void Update_SetsO_ToLogicalOrOfAAndB_TrueFalse()
    {
        TestLogicOperation(true, false, true);
    }

    [Test]
    public void Update_SetsO_ToLogicalOrOfAAndB_TrueTrue()
    {
        TestLogicOperation(true, true, true);
    }
}
