using Net.Server;
using Net.Share;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace ConsoleApp2
{
    class Program
    {
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }

    public class p : KcpPlayer
    {
        public override void OnUpdate()
        {
            if (Scene == null)
                return;
            (Scene as NetScene<p>).AddOperation(new Operation(5, new byte[50]));//压测并发, 每个客户端一次70多个byte数据
        }
    }

    public class s : NetScene<p> 
    {
    }

    public class Server : UdpServer<p, s>//dotnetty服务器
    {
        public int num;
        public int sceneID = 10000;
        protected override void OnStarting()
        {
            base.OnStarting();
            BufferPool.Size = 1024 * 1024;
            NetConvertBinary.AddNetworkType<OperationList>();
            NetConvertBinary.AddNetworkType<Operation>();
            NetConvertBinary.AddNetworkType<Operation[]>();
        }
        protected override void OnStartupCompleted()
        {
            var s = CreateScene(sceneID.ToString());
            s.Count = 10;
            sceneID++;
        }
        protected override void OnAddPlayerToScene(p client)
        {
            num++;
            if (num >= 10)
            {
                var s = CreateScene(client, sceneID.ToString());
                if (s != null)
                {
                    s.Count = 10;
                    //s.SendOperationReliable = true;
                }
                sceneID++;
                num = 0;
            }
            else
            {
                JoinScene(client, (sceneID - 1).ToString());
            }
        }
        protected override byte[] OnSerializeRpc(RPCModel model)
        {
            return NetConvertBinary.Serialize(model.func, model.pars);
        }
        protected override FuncData OnDeserializeRpc(byte[] buffer, int index, int count)
        {
            return NetConvertBinary.Deserialize(buffer, index, count);
        }
        protected override Segment OnSerializeOpt(OperationList list)
        {
            return NetConvertBinary.SerializeObject(list);
        }
        protected override OperationList OnDeserializeOpt(byte[] buffer, int index, int count)
        {
            return NetConvertBinary.DeserializeObject<OperationList>(buffer, index, count);
        }
    }
}
