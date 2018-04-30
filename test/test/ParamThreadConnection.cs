using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace test
{
    class ParamThreadConnection
    {

        private Socket socket_ = null;
        private int id_ = -1;

        public Socket SocketClient { get { return socket_; } set { socket_ = value; } }
        public int Id { get { return id_; } set { if (value < 0) id_ = -1; else id_ = value; } }
        
    }
}
