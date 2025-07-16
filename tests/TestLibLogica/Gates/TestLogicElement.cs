using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LibLogica.Gates;
using NUnit.Framework;

namespace TestLibLogica.Gates;

public class TestLogicElement
{
    // Concrete implementation for testing purposes
    private class TestableLogicElement : LogicElement
    {
        public override void Update() { }
        public override IEnumerable<string> GetIds() => new[] { IdPrefix() + "test" };
        protected override IEnumerable<string> GetLocalIds() => new[] { "test" };
        public override IEnumerable<bool> GetValues() => new[] { true };
        protected override IEnumerable<bool> GetLocalValues() => new[] { true };
    }

    [Test]
    public void GetNextGateCount_ThreadSafety_GeneratesUniqueIds()
    {
        const int threadCount = 10;
        const int elementsPerThread = 100;
        var allIds = new HashSet<string>();
        var lockObject = new object();
        var tasks = new Task[threadCount];

        for (int i = 0; i < threadCount; i++)
        {
            tasks[i] = Task.Run(() =>
            {
                var localIds = new List<string>();
                for (int j = 0; j < elementsPerThread; j++)
                {
                    var element = new TestableLogicElement();
                    localIds.AddRange(element.GetIds());
                }

                lock (lockObject)
                {
                    foreach (var id in localIds)
                    {
                        allIds.Add(id);
                    }
                }
            });
        }

        Task.WaitAll(tasks);

        // Verify that all IDs are unique (no duplicates due to race conditions)
        var expectedCount = threadCount * elementsPerThread;
        Assert.That(allIds.Count, Is.EqualTo(expectedCount));
    }

    [Test]
    public void GetNextGateCount_SequentialCreation_GeneratesUniqueIds()
    {
        var ids = new HashSet<string>();

        for (int i = 0; i < 100; i++)
        {
            var element = new TestableLogicElement();
            var elementIds = element.GetIds();

            foreach (var id in elementIds)
            {
                Assert.That(ids.Add(id), Is.True, $"Duplicate ID found: {id}");
            }
        }

        Assert.That(ids.Count, Is.EqualTo(100));
    }
}
