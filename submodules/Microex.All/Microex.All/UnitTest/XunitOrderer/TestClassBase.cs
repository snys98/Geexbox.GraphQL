using Xunit;

namespace Microex.All.UnitTest.XunitOrderer {
    [CollectionDefinition("Non-Parallel Collection", DisableParallelization = true)]
    [TestCaseOrderer("Microex.All.UnitTest.XunitOrderer.TestCaseOrderer", "Microex.All")]
    public abstract class TestClassBase {
    }
}
