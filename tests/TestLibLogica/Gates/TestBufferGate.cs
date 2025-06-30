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
}
