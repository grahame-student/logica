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
        public override IEnumerable<String> GetIds() => new[] { IdPrefix() + "test" };
        public override IEnumerable<Boolean> GetValues() => new[] { true };
    }

    private HashSet<String> RunConcurrentIdGenerationTest(Int32 threadCount, Int32 elementsPerThread)
    {
        var allIds = new HashSet<String>();
        var lockObject = new Object();
        var tasks = new Task[threadCount];

        for (Int32 i = 0; i < threadCount; i++)
        {
            tasks[i] = Task.Run(() =>
            {
                var localIds = new List<String>();
                for (Int32 j = 0; j < elementsPerThread; j++)
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
        return allIds;
    }

    [TestCase(2, 10)]
    [TestCase(5, 20)]
    [TestCase(10, 100)]
    public void GetNextGateCount_ConcurrentThreads_GeneratesExpectedNumberOfIds(Int32 threadCount, Int32 elementsPerThread)
    {
        var allIds = RunConcurrentIdGenerationTest(threadCount, elementsPerThread);
        var expectedCount = threadCount * elementsPerThread;
        Assert.That(allIds.Count, Is.EqualTo(expectedCount));
    }

    [Test]
    public void GetNextGateCount_ConcurrentCreation_GeneratesUniqueIds()
    {
        const Int32 threadCount = 5;
        const Int32 elementsPerThread = 50;
        var allIds = RunConcurrentIdGenerationTest(threadCount, elementsPerThread);
        Assert.That(allIds.Count, Is.EqualTo(threadCount * elementsPerThread));
    }

    [Test]
    public void GetNextGateCount_ThreadSafety_CompletesWithoutExceptions()
    {
        const Int32 threadCount = 10;
        const Int32 elementsPerThread = 100;
        var tasks = new Task[threadCount];
        var exceptions = new List<Exception>();
        var lockObject = new Object();

        for (Int32 i = 0; i < threadCount; i++)
        {
            tasks[i] = Task.Run(() =>
            {
                try
                {
                    for (Int32 j = 0; j < elementsPerThread; j++)
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

        Assert.That(exceptions.Count, Is.Zero);
    }

    [Test]
    public void GetNextGateCount_SequentialCreation_GeneratesUniqueIds()
    {
        var ids = new HashSet<String>();

        for (Int32 i = 0; i < 100; i++)
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
