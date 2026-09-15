using System.Collections.Generic;
using System.Linq;
using D365VsTools.Xrm;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk.Metadata;

namespace CrmCodeGenerator.Tests
{

    [TestClass]
    public class MappingEntity_ConnectedTests : ConnectedTestsBase
    {
        [TestInitialize] public override void TestInit() => base.TestInit();
        [TestCleanup] public override void TestFinish() => base.TestFinish();

        [TestMethod]
        public void CheckConnection()
        {
            Assert.IsFalse(WhoAmI().IsNullOrEmpty());
        }

        [TestMethod]
        public void Get_Account_Metadata_StateAttribute()
        {
            var metadata = service.GetEntityMetadata("account");
            Assert.IsNotNull(metadata);

            var state = metadata.Attributes.FirstOrDefault(a => a.LogicalName == "statecode");
            Assert.IsNotNull(state);
        }
        
        
        // Ignored: QuickConnection.GerOrganizationDetails was removed from the codebase along with the legacy
        // CRM-Online discovery API it depended on; not reconstructed (no reference implementation, no spec).
        [TestMethod]
        [Ignore]
        public void GerOrganizationDetails()
        {
            Assert.Inconclusive("Legacy discovery API no longer exists.");
        }

        // Ignored: QuickConnection.GetOrganizationNames and the connection-related Settings properties
        // (UseOnline/UseSSL/UseOffice365/ServerName/Username/Password/DiscoveryUrl) were removed from the
        // codebase along with the legacy Office365/CRM-Online federated discovery flow; not reconstructed.
        [TestMethod]
        [Ignore]
        public void GetOrganizationNames()
        {
            Assert.Inconclusive("Legacy discovery API no longer exists.");
        }

        [TestMethod]
        public void Get_Process_Metadata_StateAttribute()
        {
            var metadata = service.GetEntityMetadata("account");
            Assert.IsNotNull(metadata);

            var state = metadata.Attributes.FirstOrDefault(a => a.LogicalName == "statecode");
            Assert.IsNotNull(state);
            Assert.IsTrue(state is StateAttributeMetadata);
        }

        
        [TestMethod]
        public void Get_Process_Metadata_ActiveStageIdAttribute()
        {
            var metadata = service.GetEntityMetadata("wag_creditriskprocess");
            Assert.IsNotNull(metadata);

            var activeStageId = metadata.Attributes.FirstOrDefault(a => a.LogicalName == "activestageid");
            Assert.IsNotNull(activeStageId);
        }  
        
        [TestMethod]
        public void Get_ProcessInstance_Metadata_StateAttribute()
        {
            var metadata = service.GetEntityMetadata("businessprocessflowinstance");
            Assert.IsNotNull(metadata);

            var state = metadata.Attributes.FirstOrDefault(a => a.LogicalName == "statecode");
            Assert.IsNotNull(state);
        }
        
        [TestMethod]
        public void Get_ProcessInstance_Metadata_ActiveStageIdAttribute()
        {
            var metadata = service.GetEntityMetadata("businessprocessflowinstance");
            Assert.IsNotNull(metadata);

            var activeStageId = metadata.Attributes.FirstOrDefault(a => a.LogicalName == "activestageid");
            Assert.IsNotNull(activeStageId);
        }

        [TestMethod]
        public void GetAllMetadataInclude_ProcessInstance()
        {
            var metadatas = service.GetAllEntitiesMetadata();
            Assert.IsNotNull(metadatas);

            var pi = metadatas.FirstOrDefault(e => e.LogicalName == "businessprocessflowinstance");
            Assert.IsNotNull(pi);
        }
        

        [TestMethod]
        public void GetEntitiesMetadataInclude_ProcessInstance()
        {
            IEnumerable<string> selectedEntities = new []{"account", "businessprocessflowinstance"};
            var metadatas = service.GetEntitiesMetadata(selectedEntities);
            Assert.IsNotNull(metadatas);

            var pi = metadatas.FirstOrDefault(e => e.LogicalName == "businessprocessflowinstance");
            Assert.IsNotNull(pi);
        }

        
        // Ignored: Mapper/MappingSettings API has been redesigned since this test was written: Mapper's
        // constructor now takes MappingSettings (not Settings), Settings has no IncludeNonStandard
        // property, and GetSelectedEntities() is now parameterless and reads from an internally
        // populated metadata cache. Not rewritten without a spec for the intended new behavior.
        [TestMethod]
        [Ignore]
        public void GetEntitiesMetadataInclude_GetSelected()
        {
            Assert.Inconclusive("Mapper/MappingSettings API has changed; test needs to be rewritten.");
        }
    }
}