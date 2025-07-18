using System;
using LibLogica.Gates;
using NUnit.Framework;

namespace TestLibLogica.Gates;

public class TestOrGate : BinaryGateTestBase<OrGate>
{
    public static Object[] UpdateTestCases =
    [
        new Object[] { false, false, false },
        new Object[] { false, true, true },
        new Object[] { true, false, true },
        new Object[] { true, true, true }
    ];

    [TestCaseSource(nameof(UpdateTestCases))]
    public void Update_SetsO_ToLogicalOrOfAAndB(Boolean a, Boolean b, Boolean expectedO)
    {
        TestLogicOperation(a, b, expectedO);
    }
}
