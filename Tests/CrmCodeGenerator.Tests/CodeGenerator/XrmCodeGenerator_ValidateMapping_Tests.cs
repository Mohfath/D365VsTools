using System.Collections.Generic;
using System.Linq;
using D365VsTools.CodeGenerator;
using D365VsTools.CodeGenerator.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CrmCodeGenerator.Tests
{
    [TestClass]
    public class XrmCodeGenerator_ValidateMapping_Tests
    {
        private static MappingSettings SingleEntity(string entityLogicalName, EntityMappingSetting mapping) =>
            new MappingSettings
            {
                Entities = new Dictionary<string, EntityMappingSetting> { { entityLogicalName, mapping } }
            };

        [TestMethod]
        public void NullEntities_ReturnsNoErrors()
        {
            var errors = XrmCodeGenerator.ValidateMapping(new MappingSettings { Entities = null });
            Assert.AreEqual(0, errors.Count);
        }

        [TestMethod]
        public void ValidMapping_ReturnsNoErrors()
        {
            var settings = SingleEntity("account", new EntityMappingSetting
            {
                CodeName = "Account",
                Attributes = new Dictionary<string, string>
                {
                    { "accountid", "Id" },
                    { "name", "Name" }
                }
            });

            var errors = XrmCodeGenerator.ValidateMapping(settings);

            Assert.AreEqual(0, errors.Count);
        }

        [TestMethod]
        public void EntityWithNoCodeNameOrAttributes_ReturnsBothErrors()
        {
            var settings = SingleEntity("account", new EntityMappingSetting());

            var errors = XrmCodeGenerator.ValidateMapping(settings);

            Assert.AreEqual(2, errors.Count);
            Assert.IsTrue(errors.Any(e => e.Contains("CodeName is missing or empty")));
            Assert.IsTrue(errors.Any(e => e.Contains("Attributes is missing or empty")));
        }

        [TestMethod]
        public void EntityMissingCodeName_ReturnsError()
        {
            var settings = SingleEntity("account", new EntityMappingSetting
            {
                Attributes = new Dictionary<string, string> { { "accountid", "Id" } }
            });

            var errors = XrmCodeGenerator.ValidateMapping(settings);

            Assert.AreEqual(1, errors.Count);
            StringAssert.Contains(errors[0], "CodeName is missing or empty");
        }

        [TestMethod]
        public void EntityMissingAttributes_ReturnsError()
        {
            var settings = SingleEntity("account", new EntityMappingSetting { CodeName = "Account" });

            var errors = XrmCodeGenerator.ValidateMapping(settings);

            Assert.AreEqual(1, errors.Count);
            StringAssert.Contains(errors[0], "Attributes is missing or empty");
        }

        [TestMethod]
        public void EntityWithEmptyAttributesDictionary_ReturnsError()
        {
            var settings = SingleEntity("account", new EntityMappingSetting
            {
                CodeName = "Account",
                Attributes = new Dictionary<string, string>()
            });

            var errors = XrmCodeGenerator.ValidateMapping(settings);

            Assert.AreEqual(1, errors.Count);
            StringAssert.Contains(errors[0], "Attributes is missing or empty");
        }

        [TestMethod]
        public void UppercaseEntityLogicalName_ReturnsError()
        {
            var settings = SingleEntity("Account", new EntityMappingSetting
            {
                CodeName = "Account",
                Attributes = new Dictionary<string, string> { { "accountid", "Id" } }
            });

            var errors = XrmCodeGenerator.ValidateMapping(settings);

            Assert.AreEqual(1, errors.Count);
            StringAssert.Contains(errors[0], "Account");
            StringAssert.Contains(errors[0], "lowercase");
        }

        [TestMethod]
        public void UppercaseAttributeLogicalName_ReturnsError()
        {
            var settings = SingleEntity("account", new EntityMappingSetting
            {
                CodeName = "Account",
                Attributes = new Dictionary<string, string> { { "AccountId", "Id" } }
            });

            var errors = XrmCodeGenerator.ValidateMapping(settings);

            Assert.AreEqual(1, errors.Count);
            StringAssert.Contains(errors[0], "AccountId");
            StringAssert.Contains(errors[0], "lowercase");
        }

        [TestMethod]
        public void EntityCodeName_InvalidIdentifier_ReturnsError()
        {
            var settings = SingleEntity("account", new EntityMappingSetting
            {
                CodeName = "My Account",
                Attributes = new Dictionary<string, string> { { "accountid", "Id" } }
            });

            var errors = XrmCodeGenerator.ValidateMapping(settings);

            Assert.AreEqual(1, errors.Count);
            StringAssert.Contains(errors[0], "not a valid C# identifier");
        }

        [TestMethod]
        public void EntityCodeName_StartsWithLowercase_ReturnsError()
        {
            var settings = SingleEntity("account", new EntityMappingSetting
            {
                CodeName = "account",
                Attributes = new Dictionary<string, string> { { "accountid", "Id" } }
            });

            var errors = XrmCodeGenerator.ValidateMapping(settings);

            Assert.AreEqual(1, errors.Count);
            StringAssert.Contains(errors[0], "PascalCase");
        }

        [TestMethod]
        public void EntityCodeName_StartsWithDigit_ReturnsError()
        {
            var settings = SingleEntity("account", new EntityMappingSetting
            {
                CodeName = "1Account",
                Attributes = new Dictionary<string, string> { { "accountid", "Id" } }
            });

            var errors = XrmCodeGenerator.ValidateMapping(settings);

            Assert.AreEqual(1, errors.Count);
            StringAssert.Contains(errors[0], "not a valid C# identifier");
        }

        [TestMethod]
        public void AttributeCodeName_Empty_ReturnsError()
        {
            var settings = SingleEntity("account", new EntityMappingSetting
            {
                CodeName = "Account",
                Attributes = new Dictionary<string, string> { { "accountid", "" } }
            });

            var errors = XrmCodeGenerator.ValidateMapping(settings);

            Assert.AreEqual(1, errors.Count);
            StringAssert.Contains(errors[0], "missing or empty");
        }

        [TestMethod]
        public void AttributeCodeName_StartsWithLowercase_ReturnsError()
        {
            var settings = SingleEntity("account", new EntityMappingSetting
            {
                CodeName = "Account",
                Attributes = new Dictionary<string, string> { { "accountid", "id" } }
            });

            var errors = XrmCodeGenerator.ValidateMapping(settings);

            Assert.AreEqual(1, errors.Count);
            StringAssert.Contains(errors[0], "PascalCase");
        }

        [TestMethod]
        public void AttributeCodeName_InvalidIdentifier_ReturnsError()
        {
            var settings = SingleEntity("account", new EntityMappingSetting
            {
                CodeName = "Account",
                Attributes = new Dictionary<string, string> { { "accountid", "Account-Id" } }
            });

            var errors = XrmCodeGenerator.ValidateMapping(settings);

            Assert.AreEqual(1, errors.Count);
            StringAssert.Contains(errors[0], "not a valid C# identifier");
        }

        [TestMethod]
        public void DuplicateAttributeCodeNames_ReturnsError()
        {
            var settings = SingleEntity("account", new EntityMappingSetting
            {
                CodeName = "Account",
                Attributes = new Dictionary<string, string>
                {
                    { "accountid", "Id" },
                    { "accountnumber", "Id" }
                }
            });

            var errors = XrmCodeGenerator.ValidateMapping(settings);

            Assert.AreEqual(1, errors.Count);
            StringAssert.Contains(errors[0], "duplicate CodeName");
        }

        [TestMethod]
        public void MultipleIssuesInSameEntity_ReturnsOneErrorPerIssue()
        {
            var settings = SingleEntity("Account", new EntityMappingSetting
            {
                CodeName = "account",
                Attributes = new Dictionary<string, string>
                {
                    { "AccountId", "id" },
                    { "name", "" }
                }
            });

            var errors = XrmCodeGenerator.ValidateMapping(settings);

            // Entity logical name casing, entity CodeName casing, attribute logical name casing,
            // attribute CodeName casing, and the empty attribute CodeName: 5 distinct issues.
            Assert.AreEqual(5, errors.Count);
        }

        [TestMethod]
        public void IssuesAcrossMultipleEntities_AreAllReported()
        {
            var settings = new MappingSettings
            {
                Entities = new Dictionary<string, EntityMappingSetting>
                {
                    { "Account", new EntityMappingSetting { CodeName = "Account", Attributes = new Dictionary<string, string> { { "accountid", "Id" } } } },
                    { "contact", new EntityMappingSetting { CodeName = "contact", Attributes = new Dictionary<string, string> { { "contactid", "Id" } } } }
                }
            };

            var errors = XrmCodeGenerator.ValidateMapping(settings);

            Assert.AreEqual(2, errors.Count);
            Assert.IsTrue(errors.Any(e => e.Contains("Account") && e.Contains("lowercase")));
            Assert.IsTrue(errors.Any(e => e.Contains("contact") && e.Contains("PascalCase")));
        }
    }
}
