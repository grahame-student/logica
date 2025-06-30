using LibLogica.Gates;

namespace TestLibLogica.Gates;

internal class TestBufferGate
{
    //
    // A --|>-- O
    //     ^
    //     Enable
    // 
    // | A | Enable | O    |
    // +---+--------+------+
    // | 0 | 1      | 0    |
    // | 1 | 1      | 1    |
    // | x | 0      | null |

    private BufferGate _gate;

    [SetUp]
    public void Setup()
    {
        _gate = new BufferGate();
    }

    [Test]
    public void A_Initially_False()
    {
        Assert.That(_gate.A.Value, Is.EqualTo(false));
    }

    [Test]
    public void Enable_Initially_False()
    {
        Assert.That(_gate.Enable.Value, Is.EqualTo(false));
    }

    [Test]
    public void O_Initially_Null()
    {
        Assert.That(_gate.O.Value, Is.EqualTo(false));
    }

    [Test]
    public void O_Initially_Disabled()
    {
        Assert.That(_gate.O.IsEnabled.Value, Is.EqualTo(false));
    }

    [Test]
    public void O_IsEnabled_WhenEnableTrue()
    {
        _gate.Enable.Value = true;

        Assert.That(_gate.O.IsEnabled.Value, Is.EqualTo(true));
    }

    public static Object[] UpdateTestCases =
    [
        new Object[] { false, false },
        new Object[] { true, true },
    ];

    [TestCaseSource((nameof(UpdateTestCases)))]
    public void Update_SetsO_ToAWhenEnabled(Boolean a, Boolean expectedO)
    {
        _gate.A.Value = a;
        _gate.Enable.Value = true;

        _gate.Update();

        Assert.That(_gate.O.Value, Is.EqualTo(expectedO));
    }

    [Test]
    public void GetIdsAndGetValues_ContainSameNumberOfElements()
    {
        Assert.That(_gate.GetIds().Count(), Is.EqualTo(_gate.GetValues().Count()));
    }
}
