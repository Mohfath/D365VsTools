using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CrmCodeGenerator.Tests
{

    [TestClass]
    public class UpgradeConnection_ConnectedTests : ConnectedTestsBase
    {
        //[TestInitialize] public override void TestInit() => base.TestInit();
        //[TestCleanup] public override void TestFinish() => base.TestFinish();

        [TestMethod]
        public void CheckConnection()
        {

        }

        // A GetInstances() helper using a global-discovery-service OAuth flow used to live here, calling an
        // Instance type and GetAccessToken method that no longer exist anywhere in this codebase and were
        // never called by any test in this class. Removed as dead, uncompilable code rather than reconstructed
        // without a spec for the intended OAuth behavior.
    }
}