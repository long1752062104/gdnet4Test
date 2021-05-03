using Net.Client;
using Net.Share;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            NetConvertBinary.AddNetworkType<OperationList>();
            NetConvertBinary.AddNetworkType<Operation>();
            NetConvertBinary.AddNetworkType<Operation[]>();
            //Task.Run(() =>
            //{
            //    while (true)
            //    {
            //        Thread.Sleep(1000);
            //        var t = UdpClient.Testing("127.0.0.1", 6678, 1000, 1024, (cli) =>
            //        {

            //        });
            //        Thread.Sleep(5000000);
            //        t.Cancel(true);
            //        t.Dispose();
            //    }
            //});
            Thread.Sleep(1000);
            ClientBase client = new UdpClient();//创建压测流畅查看客户端
            client.Log += Console.WriteLine;//监听log
            client.MTU = 1300;
            client.OnPingCallback += (ms) => { Console.Title = ms.ToString(); };
            client.OnSerializeRpcHandle = (model) => { return NetConvertBinary.Serialize(model.func, model.pars); };
            client.OnDeserializeRpcHandle = (buffer, index, count) => { return NetConvertBinary.Deserialize(buffer, index, count); };
            client.OnSerializeOptHandle = (list) => { return NetConvertBinary.SerializeObject(list); };
            client.OnDeserializeOptHandle = (buffer, index, count) => { return NetConvertBinary.DeserializeObject<OperationList>(buffer, index, count); };
            client.Connect("127.0.0.1", 6678).Wait();//连接
            client.Send(new byte[1]);
            var p = new Program();
            client.AddRpcHandle(p);//添加rpc
            client.RTOMode = RTOMode.Variable;
            client.RTO = 50;
            client.MTPS = 1024 * 1024 * 10;
            client.SendOperationReliable = true;
            int i = 0;
            Thread.Sleep(1000);
            Task.Run(()=> {
                while (true)
                {
                    Thread.Sleep(1000);
                    Console.WriteLine("帧数:" + num11);
                    num11 = 0;
                }
            });
            while (true) 
            {
                Thread.Sleep(31);
                client.SendRT(NetCmd.LocalRT, "Test", i.ToString()); //请求服务器转发给自己 可靠传输
                i++;
            }
        }

        static int iiii=0;
        static int num11 = 0;

        [rpc]
        void Test(string str) //服务器回调方法 客户端收到回调的方法  用户方法
        {
            num11++;
            int ii = int.Parse(str);
            if (iiii == ii)
            {
                iiii++;
            }
            else
            {
                //Console.WriteLine(iiii);//当可靠错乱, 就会在这里断下
                iiii = ii + 1;
            }

            //Console.WriteLine(str);//看看能不能正常回调, 正常情况是可以回调, 如果服务器压力过高, 则收不到数据了
        }
    }
}
