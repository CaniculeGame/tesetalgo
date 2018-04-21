using System;
using System.Collections.Generic;
using System.Text;

namespace test
{

    class Message
    {
        private int headerId = 0;
        private int headerMessageLength = 0;
        private Byte[] data = null;

        public int HeaderID { get { return headerId; }  set { headerId = value; } }
        public int MessageLength { get { return headerMessageLength; } set { headerMessageLength = value; } }

        public Byte[] Data
        {
            set
            {
                if (headerMessageLength <= 0)
                    return;
                

                data = new Byte[headerMessageLength];
                data = value;
                
            }

            get { return data; }
        }

    }
}
