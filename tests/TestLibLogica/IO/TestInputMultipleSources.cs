using LibLogica.IO;

using System;
using NUnit.Framework;

namespace TestLibLogica.IO;

internal class TestInputMultipleSources
{
    private Input _input;
    private Output _output1;
    private Output _output2;

    [SetUp]
    public void Setup()
    {
        _input = new Input();
        _output1 = new Output();
        _output2 = new Output();
    }

    [Test]
    public void SingleActiveSource_SetsInputValue()
    {
        _output1.Value = true;
        _output2.IsHighImpedance = true;

        _input.Connect(_output1);
        _input.Connect(_output2);

        Assert.That(_input.Value, Is.EqualTo(true));
    }

    [Test]
    public void AllSourcesHighImpedance_KeepsCurrentValue()
    {
        _output1.IsHighImpedance = false;
        _output1.Value = true;
        _output2.IsHighImpedance = true;

        _input.Connect(_output1);
        _input.Connect(_output2);

        Assert.That(_input.Value, Is.EqualTo(true));

        // Now make output1 high impedance too - input should keep current value
        _output1.IsHighImpedance = true;
        Assert.That(_input.Value, Is.EqualTo(true)); // Keeps the previous value
    }

    [Test]
    public void MultipleActiveSources_ThrowsException()
    {
        _output1.Value = true;
        _output1.IsHighImpedance = false;
        _output2.Value = false;
        _output2.IsHighImpedance = false;

        _input.Connect(_output1);

        Assert.Throws<InvalidOperationException>(() => _input.Connect(_output2));
    }

    [Test]
    public void SourceBecomesHighImpedance_InputUpdates()
    {
        _output1.Value = true;
        _output1.IsHighImpedance = false;
        _output2.Value = false;
        _output2.IsHighImpedance = true;

        _input.Connect(_output1);
        _input.Connect(_output2);

        Assert.That(_input.Value, Is.EqualTo(true));

        // First make output1 high impedance, then activate output2
        _output1.IsHighImpedance = true;
        _output2.IsHighImpedance = false;

        Assert.That(_input.Value, Is.EqualTo(false));
    }
}
