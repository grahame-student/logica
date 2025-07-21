using LibLogica.Blocks.Width8Bit;
using NUnit.Framework;

namespace TestLibLogica.Blocks.Width8Bit;

internal class TestTristateBuffer8Bit : LogicElementTestBase<TristateBuffer8Bit>
{
    [Test]
    public void Inputs_Initially_Zero()
    {
        Assert.That(LogicElementTestHelper.GetArrayValue(_element.Inputs), Is.Zero);
    }

    [Test]
    public void Enable_Initially_False()
    {
        Assert.That(_element.Enable.Value, Is.False);
    }

    [Test]
    public void Outputs_Initially_Zero()
    {
        Assert.That(LogicElementTestHelper.GetArrayValue(_element.Outputs), Is.Zero);
    }
}
