namespace TestLibLogica.Gates;

using LibLogica.Gates;

public class TestNorGate
{
    private NorGate _gate;

    [SetUp]
    public void Setup()
    {
        _gate = new NorGate();
        Console.WriteLine(String.Join(",", _gate.GetIds()));
    }

    [Test]
    public void InputAInitiallyFalse()
    {
        Assert.That(_gate.A.Value, Is.EqualTo(false));
    }

    [Test]
    public void InputBInitiallyFalse()
    {
        Assert.That(_gate.B.Value, Is.EqualTo(false));
    }

    public static Object[] UpdateTestCases =
    [
        new Object[] { false, false, true },
        new Object[] { false, true, false },
        new Object[] { true, false, false },
        new Object[] { true, true, false }
    ];

    [TestCaseSource((nameof(UpdateTestCases)))]
    public void Update_SetsO_ToLogicalNorOfAAndB(Boolean a, Boolean b, Boolean expectedO)
    {
        _gate.A.Value = a;
        _gate.B.Value = b;

        _gate.Update();

        Assert.That(_gate.O.Value, Is.EqualTo(expectedO));
    }

    [Test]
    public void GetIdsAndGetValues_ContainSameNumberOfElements()
    {
        /*
        for (var i = 0; i < _gate.GetIds().Count(); i++)
        {
            Console.WriteLine($"{_gate.GetIds().ToArray()[i]} - {_gate.GetValues().ToArray()[i]}"  );
        }
        */

        Assert.That(_gate.GetIds().Count(), Is.EqualTo(_gate.GetValues().Count()));
    }
}
