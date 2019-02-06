using System;
using System.Reflection;

namespace WebCSharpConsole.Services.ConsoleEmulator
{
    [Serializable]
    public class ConsoleExecutor : MarshalByRefObject
    {
        private Assembly assembly;

        public void SetAssembly(string assemblyLocation)
        {
            if (assemblyLocation == null)
            {
                throw new ArgumentNullException(nameof(assemblyLocation));
            }

            if (this.assembly != null)
            {
                throw new InvalidOperationException($"{nameof(this.assembly)} is already set.");
            }

            this.assembly = Assembly.LoadFrom(assemblyLocation);
        }

        public string Execute()
        {
            this.assembly.EntryPoint.Invoke(null, new object[0]);
            var result = DummyConsole.Console.ReadOutputStreamToEnd();
            DummyConsole.Console.Clear();
            return result;
        }
    }
}
