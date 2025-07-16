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

    [TestCase(2, 10)]
    [TestCase(5, 20)]
    [TestCase(10, 100)]
    public void GetNextGateCount_ConcurrentThreads_GeneratesExpectedNumberOfIds(int threadCount, int elementsPerThread)
    {
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

        var expectedCount = threadCount * elementsPerThread;
        Assert.That(allIds.Count, Is.EqualTo(expectedCount));
    }

    [Test]
    public void GetNextGateCount_ConcurrentCreation_GeneratesUniqueIds()
    {
        const int threadCount = 5;
        const int elementsPerThread = 50;
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

        Assert.That(allIds.Count, Is.EqualTo(threadCount * elementsPerThread));
    }

    [Test]
    public void GetNextGateCount_ThreadSafety_CompletesWithoutExceptions()
    {
        const int threadCount = 10;
        const int elementsPerThread = 100;
        var tasks = new Task[threadCount];
        var exceptions = new List<Exception>();
        var lockObject = new object();

        for (int i = 0; i < threadCount; i++)
        {
            tasks[i] = Task.Run(() =>
            {
                try
                {
                    for (int j = 0; j < elementsPerThread; j++)
                    {
                        var element = new TestableLogicElement();
                        element.GetIds();
                    }
                }
                catch (InvalidOperationException ex)
                {
                    lock (lockObject)
                    {
                        exceptions.Add(ex);
                    }
                }
                catch (ArgumentException ex)
                {
                    lock (lockObject)
                    {
                        exceptions.Add(ex);
                    }
                }
            });
        }

        Task.WaitAll(tasks);

        Assert.That(exceptions.Count, Is.EqualTo(0));
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
