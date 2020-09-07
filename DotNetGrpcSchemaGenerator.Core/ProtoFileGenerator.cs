using ProtoBuf.Grpc.Reflection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.ServiceModel;

namespace DotNetGrpcSchemaGenerator.Core
{
    public class ProtoFileGenerator
    {
        public void SaveProtoFile(string assemblyFilePath, string destinationPath)
        {
            var assembly = Assembly.LoadFrom(assemblyFilePath);
            var files = GetTypesWithServiceContractAttribute(assembly);

            foreach (var file in files)
            {
                Console.WriteLine("Found: " + file.Name);

                var generator = new SchemaGenerator();
                var schema = generator.GetSchema(file);

                Directory.CreateDirectory(destinationPath);
                File.WriteAllText($@"{destinationPath}\{file.Name.ToLower()}.proto", schema);
            }
        }

        private IEnumerable<Type> GetTypesWithServiceContractAttribute(Assembly assembly)
        {
            foreach (Type type in assembly.GetTypes())
            {
                if (type.GetCustomAttributes(typeof(ServiceContractAttribute), true).Length > 0)
                {
                    yield return type;
                }
            }
        }
    }
}
