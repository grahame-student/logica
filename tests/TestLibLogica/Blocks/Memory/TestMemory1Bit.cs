using LibLogica.Blocks.Memory;
using NUnit.Framework;

namespace TestLibLogica.Blocks.Memory;
internal class TestMemory1Bit : LogicElementTestBase<Memory1Bit>
{
    [Test]
    public void InputDataIn_Initially_False()
    {
        Assert.That(_element.DataIn.Value, Is.False);
    }

    [Test]
    public void InputWrite_Initially_False()
    {
        Assert.That(_element.Write.Value, Is.False);
    }

    [Test]
    public void OutputDataOut_Initially_False()
    {
        Assert.That(_element.DataOut.Value, Is.False);
    }

    [Test]
    public void Update_WhenWriteIsTrue_SetsDataOutToDataIn()
    {
        _element.DataIn.Value = true;

        _element.Write.Value = true;
        _element.Update();

        Assert.That(_element.DataOut.Value, Is.True);
    }

    [Test]
    public void Update_WhenWriteIsFalse_DoesNotChangeDataOut()
    {
        _element.DataIn.Value = true;

        _element.Write.Value = false;
        _element.Update();

        Assert.That(_element.DataOut.Value, Is.False);
    }

    [Test]
    public void Update_WhenWriteIsTrueAndDataInIsFalse_SetsDataOutToFalse()
    {
        _element.DataIn.Value = true;
        _element.Write.Value = true;
        _element.Update();

        _element.DataIn.Value = false;
        _element.Write.Value = true;
        _element.Update();
        Assert.That(_element.DataOut.Value, Is.False);
    }
}
