using ProtoBuf.Grpc.Reflection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.ServiceModel;

namespace DotNetGrpcSchemaGenerator.Debugger
{
    class Program
    {
        static void Main(string[] args)
        {
            var assembly = Assembly.LoadFrom(@"..\CodeFirstGrpc\bin\Debug\netcoreapp3.1\CodeFirstGrpc.dll");
            var files = GetTypesWithServiceContractAttribute(assembly);

            foreach (var file in files)
            {
                Console.WriteLine("Found: " + file.Name);

                var generator = new SchemaGenerator();
                var schema = generator.GetSchema(file);

                Directory.CreateDirectory(@"..\CodeFirstGrpc\Protos\");
                File.WriteAllText($@"..\CodeFirstGrpc\Protos\{file.Name.ToLower()}.proto", schema);
            }

            Console.ReadLine();
        }

        static IEnumerable<Type> GetTypesWithServiceContractAttribute(Assembly assembly)
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
