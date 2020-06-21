namespace SelfInitializingFakes.Tests.Acceptance
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using FluentAssertions;
    using SelfInitializingFakes.Tests.Acceptance.Helpers;
    using Xbehave;

    public abstract class TypeSerializationTestBase
    {
        [Scenario]
        public void SerializeCommonCalls(
            IRecordedCallRepository repository,
            int voidMethodOutInteger,
            DateTime voidMethodRefDateTime,
            Lazy<int> lazyIntMethodResult,
            Lazy<string> lazyStringMethodResult,
            Lazy<int> lazyOutResult,
            Task taskResult,
            Task<int> taskIntResult)
        {
            "Given a recorded call repository"
                .x(() => repository = this.CreateRepository());

            "When I use a self-initializing fake in recording mode"
                .x(() =>
                {
                    using (var fakeService = SelfInitializingFake<ISampleService>.For(() => new SampleService(), repository))
                    {
                        DateTime discardDateTime = DateTime.MaxValue;

                        var fake = fakeService.Object;

                        fake.VoidMethod("firstCallKey", out _, ref discardDateTime);
                        _ = fake.LazyIntReturningMethod();
                        _ = fake.LazyStringReturningMethod();
                        fake.MethodWithLazyOut(out _);
                        _ = fake.TaskReturningMethod();
                        _ = fake.TaskIntReturningMethod();
                    }
                });

            "And I use a self-initializing fake in playback mode"
                .x(() =>
                {
                    using (var playbackFakeService = SelfInitializingFake<ISampleService>.For(UnusedFactory, repository))
                    {
                        var fake = playbackFakeService.Object;

                        fake.VoidMethod("firstCallKey", out voidMethodOutInteger, ref voidMethodRefDateTime);
                        lazyIntMethodResult = fake.LazyIntReturningMethod();
                        lazyStringMethodResult = fake.LazyStringReturningMethod();
                        fake.MethodWithLazyOut(out lazyOutResult);
                        taskResult = fake.TaskReturningMethod();
                        taskIntResult = fake.TaskIntReturningMethod();
                    }
                });

            "Then the playback fake returns the recorded out and ref parameters and results"
                .x(() =>
                {
                    voidMethodOutInteger.Should().Be(17);
                    voidMethodRefDateTime.Should().Be(new DateTime(2017, 1, 24));

                    lazyIntMethodResult.IsValueCreated.Should().BeFalse();
                    lazyIntMethodResult.Value.Should().Be(3);

                    lazyStringMethodResult.IsValueCreated.Should().BeFalse();
                    lazyStringMethodResult.Value.Should().Be("three");

                    lazyOutResult.IsValueCreated.Should().BeFalse();
                    lazyOutResult.Value.Should().Be(-14);

                    taskResult.IsCompleted.Should().BeTrue();

                    taskIntResult.IsCompleted.Should().BeTrue();
                    taskIntResult.Result.Should().Be(5);
                });
        }

        protected static ISampleService UnusedFactory() => null!;

        protected abstract IRecordedCallRepository CreateRepository();
    }
}