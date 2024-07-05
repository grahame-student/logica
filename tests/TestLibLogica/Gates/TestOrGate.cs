namespace TestLibLogica.Gates;

using LibLogica.Gates;

public class TestOrGate
{
    private OrGate _gate;

    [SetUp]
    public void Setup()
    {
        _gate = new OrGate();
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
        new Object[] { false, false, false },
        new Object[] { false, true, true },
        new Object[] { true, false, true },
        new Object[] { true, true, true }
    ];

    [TestCaseSource((nameof(UpdateTestCases)))]
    public void Update_SetsO_ToLogicalOrOfAAndB(Boolean a, Boolean b, Boolean expectedO)
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
