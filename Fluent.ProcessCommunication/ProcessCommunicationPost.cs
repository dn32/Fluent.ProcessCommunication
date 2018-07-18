using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Fluent.ProcessCommunication
{
    public class ProcessCommunicationPost
    {
        public string Cliente { get; set; }

        public ProcessCommunicationPost(string cliente)
        {
            Cliente = cliente;
        }

        public void PostAsync(object content, Action<Package> callback, bool useJson = true)
        {
            var bytes = useJson ? JsonConvert.SerializeObject(content) : (object)Util.ToByteArray(content);
            var package = new Package
            {
                Content = bytes,
                TransportKey = Guid.NewGuid().ToString(),
                UseJson = useJson
            };

            var json = JsonConvert.SerializeObject(package);
            new ProcessCommunicationServer($"callback-{Cliente}-{package.TransportKey}", Cliente, callback).Init();
            new ProcessCommunicationClient($"post-{Cliente}").Post(json);
        }

        public void Response(string transportKey, object content, bool useJson = true)
        {
            var bytes = useJson ? JsonConvert.SerializeObject(content) : (object)Util.ToByteArray(content);
            var package = new Package
            {
                Content = bytes,
                TransportKey = Guid.NewGuid().ToString(),
                UseJson = useJson
            };

            var json = JsonConvert.SerializeObject(package);
            new ProcessCommunicationClient($"callback-{Cliente}-{transportKey}").Post(json);
        }

        public object Post(Type returnType, object obj, int timeOut = 1000, bool useJson = true)
        {
            bool rxOk = false;
            Package package = null;

            void callback(Package _package)
            {
                rxOk = true;
                package = _package;
            }

            PostAsync(obj, callback, useJson);

            var task = Task.Run(() => { while (!rxOk) ; });

            task.Wait(timeOut);
            if (task.IsCompleted)
            {
                var objRet = package.UseJson ? JsonConvert.DeserializeObject((string)package.Content, returnType) : Util.FromByteArray((byte[])package.Content);
                return objRet;
            }
            else
            {
                return null;// new OperationReturn(eOperationReturn.TIMEOUT);
            }
        }

        public T Post<T>(object obj, int timeOut = 1000, bool useJson = true) where T : class
        {
            return Post(typeof(T), obj, timeOut, useJson) as T;
        }
    }
}
