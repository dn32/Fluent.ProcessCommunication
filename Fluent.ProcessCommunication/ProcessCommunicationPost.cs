using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Fluent.ProcessCommunication
{
    public class ProcessCommunicationPost
    {
        public string Cliente { get; set; }
        public string AditionalInformation { get; set; }

        public ProcessCommunicationPost(string cliente)
        {
            Cliente = cliente;
        }

        public ProcessCommunicationPost(string cliente, string aditionalInformation)
        {
            Cliente = cliente;
            AditionalInformation = aditionalInformation;
        }

        private void PostAsync(object content, Action<Package> callback, bool useJson = true)
        {
            var bytes = useJson ? JsonConvert.SerializeObject(content) : (object)Util.ToByteArray(content);
            var package = new Package
            {
                Content = bytes,
                TransportKey = Guid.NewGuid().ToString(),
                UseJson = useJson,
                UseResponse = true,
                AditionalInformation = AditionalInformation
            };

            var json = JsonConvert.SerializeObject(package);
            new ProcessCommunicationServer($"callback-{Cliente}-{package.TransportKey}", Cliente, callback).Init();
            new ProcessCommunicationClient($"post-{Cliente}").Post(json);
        }

        public void PostAsync(object content, bool useJson = true)
        {
            var bytes = useJson ? JsonConvert.SerializeObject(content) : (object)Util.ToByteArray(content);
            var package = new Package
            {
                Content = bytes,
                TransportKey = Guid.NewGuid().ToString(),
                UseJson = useJson,
                UseResponse = false,
                AditionalInformation = AditionalInformation
            };

            var json = JsonConvert.SerializeObject(package);
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
            try
            {
                new ProcessCommunicationClient($"callback-{Cliente}-{transportKey}").Post(json);
            }
            catch (TimeoutException) { }
        }

        public object Post(Type returnType, object obj, int timeOut = 10000, bool useJson = true)
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
                throw new TimeoutException();
            }
        }

        public T Post<T>(object obj, int timeOut = 10000, bool useJson = true)
        {
            var ret = Post(typeof(T), obj, timeOut, useJson);
            if (ret == null)
            {
                return default(T);
            }

            return (T)ret;
        }
    }
}
