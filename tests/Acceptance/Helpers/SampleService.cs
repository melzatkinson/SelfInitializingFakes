namespace SelfInitializingFakes.Tests.Acceptance.Helpers
{
    using System;
    using System.Collections.Generic;

    public class SampleService : ISampleService
    {
        public void VoidMethod(string s, out int i, ref DateTime dt)
        {
            i = 17;
            dt = new DateTime(2017, 1, 24);
        }

        public Guid GuidReturningMethod() => new Guid("5b61d48f-e9e5-49ad-9c51-a9aae056aa84");

        public IDictionary<string, Guid> DictionaryReturningMethod() => new Dictionary<string, Guid>
            {
                ["key1"] = new Guid("6c7d8912-802a-43c0-82a2-cb811058a9bd"),
            };
    }
}