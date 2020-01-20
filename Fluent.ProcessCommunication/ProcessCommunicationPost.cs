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

        private void PostAsync(object content, Action<Package> callback)
        {
            var package = new Package
            {
                Content = content,
                TransportKey = Guid.NewGuid().ToString(),
                UseResponse = true,
                AditionalInformation = AditionalInformation
            };

            new ProcessCommunicationServer($"callback-{Cliente}-{package.TransportKey}", Cliente, callback).Init();
            new ProcessCommunicationClient($"post-{Cliente}").Post(package);
        }

        public void PostAsync(object content)
        {
            var package = new Package
            {
                Content = content,
                TransportKey = Guid.NewGuid().ToString(),
                UseResponse = false,
                AditionalInformation = AditionalInformation
            };

            new ProcessCommunicationClient($"post-{Cliente}").Post(package);
        }

        public void Response(string transportKey, object content)
        {
            var package = new Package
            {
                Content = content,
                TransportKey = Guid.NewGuid().ToString()
            };

            try
            {
                new ProcessCommunicationClient($"callback-{Cliente}-{transportKey}").Post(package);
            }
            catch (TimeoutException) { }
        }

        internal object PostInternal(object obj, int timeOut = 10000)
        {
            bool rxOk = false;
            Package package = null;

            void callback(Package _package)
            {
                rxOk = true;
                package = _package;
            }

            PostAsync(obj, callback);

            var task = Task.Run(() => { while (!rxOk) ; });

            task.Wait(timeOut);
            if (task.IsCompleted)
            {
                return package.Content;
            }
            else
            {
                throw new TimeoutException();
            }
        }

        public Package Post(object obj, int timeOut = 10000)
        {
            var ret = PostInternal(obj, timeOut);
            if (ret == null)
            {
                return default;
            }

            return (Package)ret;
        }
    }
}
