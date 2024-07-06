namespace TestLibLogica.IO;

using LibLogica.IO;

internal class TestOutput
{
    private Output _output;

    [SetUp]
    public void Setup()
    {
        _output = new Output();
    }

    [Test]
    public void Value_RaisesSignalChangedEvent_WhenValueChanges()
    {
        Boolean wasRaised = false;
        _output.SignalChanged += (sender, args) => wasRaised = true;

        _output.Value = !_output.Value;

        Assert.That(wasRaised, Is.EqualTo(true));
    }

    [Test]
    public void Value_PassesNewValueInSignalChangedEvent_WhenValueChanges()
    {
        Boolean newValue = false;
        _output.SignalChanged += (sender, args) => newValue = args.Value;

        _output.Value = true;

        Assert.That(newValue, Is.EqualTo(true));
    }
}
