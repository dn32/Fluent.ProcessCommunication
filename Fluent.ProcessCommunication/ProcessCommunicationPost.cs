using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Fluent.ProcessCommunication
{
    public partial class ProcessCommunicationPost
    {
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
            new ProcessCommunicationServer("callback-" + package.TransportKey, callback).Init();
            new ProcessCommunicationClient("post").Post(json);
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
            new ProcessCommunicationClient("callback-" + transportKey).Post(json);
        }

        public T Post<T>(object obj, int timeOut = 1000, bool useJson = true)
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
                var objRet = package.UseJson ? JsonConvert.DeserializeObject<T>((string)package.Content) : Util.FromByteArray<T>((byte[])package.Content);
                return objRet;
            }
            else
            {
                return default(T);
            }
        }
    }
}
