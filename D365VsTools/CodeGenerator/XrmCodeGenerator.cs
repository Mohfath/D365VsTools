using System;
using System.Collections.Generic;
using System.IO;
using System.Media;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using D365VsTools.CodeGenerator.Model;
using D365VsTools.CodeGenerator.T4;
using D365VsTools.Common;
using D365VsTools.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TextTemplating;
using Microsoft.VisualStudio.TextTemplating.VSHost;
using Microsoft.Xrm.Sdk;
using Context = D365VsTools.CodeGenerator.Model.Context;

namespace D365VsTools.CodeGenerator
{
    // http://blogs.msdn.com/b/vsx/archive/2013/11/27/building-a-vsix-deployable-single-file-generator.aspx
    [Guid(ProjectGuids.CodeGenerator)]
    public class XrmCodeGenerator
    {
        public const string Name = nameof(XrmCodeGenerator);
        public const string Description = "Microsoft Dataverse Code Generator for Visual Studio";

        private readonly CommandExecutor executor = new CommandExecutor();
        private string extension = "cs";
        

        public void GenerateCode(string inputFileName)
        {
            Logger.WriteLine("Loading Template File... ");
            string inputFileContent = File.ReadAllText(inputFileName);

            // Runs the mapping/CRM work off the UI thread so Visual Studio stays responsive; BuildCode
            // still switches back to the UI thread for the part that needs it (the T4 templating engine).
            executor.Execute(service => Task.Run(() =>
            {
                try
                {
                    Logger.WriteLine("Loading Entities & Attributes Mapping File... ");
                    using (var mapper = CreateMapper(inputFileName))
                    {
                        Logger.WriteLine("Creating Mapping Context... ");
                        Context context = CreateContext(service, mapper, out var error);

                        string generateCode;
                        if (error == null)
                        {
                            Logger.WriteLine("Generating code from template... ");
                            generateCode = BuildCode(context, inputFileName, inputFileContent);
                        }
                        else
                            generateCode = error;

                        if (generateCode != null)
                        {
                            var outputFileName = Path.ChangeExtension(inputFileName, extension);
                            File.WriteAllText(outputFileName, generateCode, Encoding.UTF8);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.WriteLine("[ERROR] " + ex.Message);
                    Logger.WriteLine(ex.StackTrace);
                }
                finally
                {
                    Logger.WriteLine("Executing Generate Code - End");
                    SystemSounds.Beep.Play();
                }
            }));
        }

        private Context CreateContext(IOrganizationService service, Mapper mapper, out string resultCode)
        {
            Context context = null;
            resultCode = null;
            try
            {
                context = mapper.CreateContext(service);
            }
            catch (Exception ex)
            {
                var error = "[ERROR] " + ex.Message + (ex.InnerException != null ? "\n" + "[ERROR] " + ex.InnerException.Message : "");
                Logger.WriteLine(error);
                Logger.WriteLine(ex.StackTrace);
                Logger.WriteLine("Unable to map entities, see error above.");
                resultCode = error + Environment.NewLine + ex.StackTrace;
            }
            return context;
        }

        public string BuildCode(Context context, string inputFileName, string inputFileContent)
        {
            try
            {
                if (inputFileContent == null)
                    throw new ArgumentNullException(nameof(inputFileContent));

                if (context == null)
                    throw new ArgumentNullException(nameof(context));

                // The T4 templating engine is a VS service and must be used from the UI thread, even when
                // BuildCode itself is being called from a backgrounded Generate Code command.
                string content = null;
                string sessionHostError = null;
                Callback cb = null;

                ProjectHelper.RunOnUIThread(() =>
                {
                    var t4 = Package.GetGlobalService(typeof(STextTemplating)) as ITextTemplating;
                    var sessionHost = t4 as ITextTemplatingSessionHost;
                    if (sessionHost == null)
                    {
                        sessionHostError = "Unexpected Error occur by Initializing the SessionHost. Abort";
                        return;
                    }

                    sessionHost.Session = sessionHost.CreateSession();
                    sessionHost.Session["Context"] = context;

                    cb = new Callback();
                    t4.BeginErrorSession();
                    content = t4.ProcessTemplate(inputFileName, inputFileContent, cb);
                    t4.EndErrorSession();
                });

                if (sessionHostError != null)
                {
                    Logger.WriteLine(sessionHostError);
                    return sessionHostError;
                }

                // If there was an output directive in the TemplateFile, then cb.SetFileExtension() will have been called.
                if (!string.IsNullOrWhiteSpace(cb.FileExtension))
                    extension = cb.FileExtension;

                if (cb.ErrorMessages.Count > 0)
                {
                    var errors = new StringBuilder();
                    //Configuration.Instance.DTE.ExecuteCommand("View.ErrorList");
                    // Append any error/warning to output window
                    foreach (var err in cb.ErrorMessages)
                    {
                        // The templating system (eg t4.ProcessTemplate) will automatically add error/warning to the ErrorList 
                        var errorLine = "[" + (err.Warning ? "WARN" : "ERROR") + "] " + err.Message + " " + err.Line + "," + err.Column;
                        errors.AppendLine(errorLine);
                        Logger.WriteLine(errorLine);
                    }
                    var error = errors.ToString();
                    Logger.WriteLine(error);
                    return error;
                }

                Logger.WriteLine("Writing code to disk... ");
                Logger.WriteLine("Done!");
                return content;
            }
            catch (Exception ex)
            {
                var error = "[ERROR] " + ex.Message + (ex.InnerException != null ? "\n" + "[ERROR] " + ex.InnerException.Message : "");
                Logger.WriteLine(error);
                Logger.WriteLine(ex.StackTrace);
                Logger.WriteLine("Unable to map entities, see error above.");
                return error + Environment.NewLine + ex.StackTrace;
            }
        }

        private Mapper CreateMapper(string inputFileName)
        {
            var mappingFile = Path.ChangeExtension(inputFileName, "mapping.json");
            Logger.WriteLine($"Mapping File: {mappingFile ?? "null"}");
            if (string.IsNullOrWhiteSpace(mappingFile))
            {
                Logger.WriteLine("Mapping File not found. Abort");
                return null;
            }
            var mappingSettings = LoadMappingFromFile(mappingFile);

            var entities = mappingSettings?.Entities?.Keys;
            if (entities == null)
            {
                Logger.WriteLine("Mapping File not found. Abort"); // Todo: consider to generate all entities (I don't thing so)
                return null;
            }

            var validationErrors = ValidateMapping(mappingSettings);
            if (validationErrors.Count > 0)
            {
                Logger.WriteLine($"Mapping File is invalid ({validationErrors.Count} issue(s)):");
                foreach (var error in validationErrors)
                    Logger.WriteLine("  - " + error);
                Logger.WriteLine("Abort");
                return null;
            }

            var logicalNames = string.Join("\n\t", entities);
            Logger.WriteLine($"Entities Mapping:\n\t{logicalNames}");

            return new Mapper(mappingSettings);
        }

        /// <summary>
        /// Validates the mapping file's logical names and CodeNames before generation starts:
        /// logical names (entity/attribute keys) must be lowercase to match Dataverse metadata lookups,
        /// which are case-sensitive; CodeNames must be valid, PascalCase C# identifiers, and unique per entity.
        /// </summary>
        public static List<string> ValidateMapping(MappingSettings mappingSettings)
        {
            var errors = new List<string>();
            if (mappingSettings?.Entities == null)
                return errors;

            bool IsValidIdentifier(string name) =>
                !string.IsNullOrEmpty(name) && System.CodeDom.Compiler.CodeGenerator.IsValidLanguageIndependentIdentifier(name);

            void ValidateCodeName(string codeName, string context)
            {
                if (!IsValidIdentifier(codeName))
                    errors.Add($"{context}: CodeName '{codeName}' is not a valid C# identifier.");
                else if (!char.IsUpper(codeName[0]))
                    errors.Add($"{context}: CodeName '{codeName}' should start with an uppercase letter (PascalCase), e.g. '{char.ToUpperInvariant(codeName[0]) + codeName.Substring(1)}'.");
            }

            foreach (var entityEntry in mappingSettings.Entities)
            {
                var entityLogicalName = entityEntry.Key ?? string.Empty;
                var mapping = entityEntry.Value;

                if (entityLogicalName != entityLogicalName.ToLowerInvariant())
                    errors.Add($"Entity '{entityLogicalName}': logical name must be lowercase, e.g. '{entityLogicalName.ToLowerInvariant()}'.");

                if (!string.IsNullOrWhiteSpace(mapping?.CodeName))
                    ValidateCodeName(mapping.CodeName, $"Entity '{entityLogicalName}'");

                if (mapping?.Attributes == null)
                    continue;

                var seenCodeNames = new HashSet<string>(StringComparer.Ordinal);
                foreach (var attributeEntry in mapping.Attributes)
                {
                    var attributeLogicalName = attributeEntry.Key ?? string.Empty;
                    var codeName = attributeEntry.Value;
                    var context = $"Entity '{entityLogicalName}', attribute '{attributeLogicalName}'";

                    if (attributeLogicalName != attributeLogicalName.ToLowerInvariant())
                        errors.Add($"{context}: logical name must be lowercase, e.g. '{attributeLogicalName.ToLowerInvariant()}'.");

                    if (string.IsNullOrWhiteSpace(codeName))
                    {
                        errors.Add($"{context}: CodeName is missing or empty.");
                        continue;
                    }

                    ValidateCodeName(codeName, context);

                    if (!seenCodeNames.Add(codeName))
                        errors.Add($"Entity '{entityLogicalName}': duplicate CodeName '{codeName}' used for more than one attribute.");
                }
            }

            return errors;
        }

        public static MappingSettings LoadMappingFromFile(string mappingFile)
        {
            Logger.WriteLine("Loading Mapping from File ...");
            bool exist = File.Exists(mappingFile);
            if (exist == false)
                return null;

            var json = File.ReadAllText(mappingFile);
            if (string.IsNullOrWhiteSpace(json))
                return null;

            var mapping = Deserialize<MappingSettings>(json);
            return mapping;
        }

        public static T Deserialize<T>(string json) where T : class
        {
            if (string.IsNullOrEmpty(json))
                return default;

            var settings = new DataContractJsonSerializerSettings { UseSimpleDictionaryFormat = true };
            using (var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                var serializer = new DataContractJsonSerializer(typeof(T), settings);
                return serializer.ReadObject(memoryStream) as T;
            }
        }
    }
}
