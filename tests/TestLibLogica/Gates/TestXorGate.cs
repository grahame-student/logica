using System;
using LibLogica.Gates;
using NUnit.Framework;

namespace TestLibLogica.Gates;

public class TestXorGate : BinaryGateTestBase<XorGate>
{
    public static Object[] UpdateTestCases =
    [
        new Object[] { false, false, false },
        new Object[] { false, true, true },
        new Object[] { true, false, true },
        new Object[] { true, true, false }
    ];

    [TestCaseSource(nameof(UpdateTestCases))]
    public void Update_SetsO_ToLogicalXorOfAAndB(Boolean a, Boolean b, Boolean expectedO)
    {
        TestLogicOperation(a, b, expectedO);
    }
}
