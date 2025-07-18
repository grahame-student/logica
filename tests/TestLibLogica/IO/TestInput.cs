using System;
using LibLogica.IO;
using NUnit.Framework;

namespace TestLibLogica.IO;

internal class TestInput
{
    private Input _input;

    [SetUp]
    public void Setup()
    {
        _input = new Input();
    }

    [Test]
    public void Connect_UpdatesValue_WhenOutputSignalChanges()
    {
        var output = new Output();
        _input.Connect(output);

        output.Value = true;

        Assert.That(_input.Value, Is.EqualTo(output.Value));
    }
}
