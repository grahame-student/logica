using LibLogica.IO;

namespace TestLibLogica.IO;

internal class TestNullableOutput
{
    private NullableOutput _output;

    [SetUp]
    public void Setup()
    {
        _output = new NullableOutput();
    }

    [Test]
    public void Value_RaisesSignalChangedEvent_WhenValueChangesAndOutputEnabled()
    {
        Boolean wasRaised = false;
        _output.SignalChanged += (sender, args) => wasRaised = true;

        _output.IsEnabled.Value = true;
        _output.Value = !_output.Value;

        Assert.That(wasRaised, Is.EqualTo(true));
    }

    [Test]
    public void Value_PassesNewValueInSignalChangedEvent_WhenValueChangesAndOutputEnabled()
    {
        Boolean newValue = false;
        _output.SignalChanged += (sender, args) => newValue = args.Value;

        _output.IsEnabled.Value = true;
        _output.Value = true;

        Assert.That(newValue, Is.EqualTo(true));
    }

    [Test]
    public void Value_DoesNotRaiseSignalChangedEvent_WhenValueChangesAndOutputDisabled()
    {
        Boolean wasRaised = false;
        _output.SignalChanged += (sender, args) => wasRaised = true;

        _output.IsEnabled.Value = false;
        _output.Value = !_output.Value;

        Assert.That(wasRaised, Is.EqualTo(false));
    }
}
