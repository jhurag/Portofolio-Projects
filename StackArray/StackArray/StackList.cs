using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackArray
{
    class StackList
    {
        class ListNode
        {
            private int content;
            private ListNode _next;

            public int Number
            {
                get { return content; }
                set { content = value; }
            }

            public ListNode PointerToNextOne
            {
                get { return _next; }
                set { _next = value; }
            }
            public ListNode()
            {
            }

            public ListNode(int content)
            {
                Number = content;
                PointerToNextOne = null;
            }
        }

        private ListNode _head;

        public bool IsEmpty
        {
            get { return _head == null; }
        }

        public void Push(int content)
        {
            ListNode newNode = new ListNode(content);
            newNode.PointerToNextOne = _head;
            _head = newNode;
        }


        public int Pop()
        {
            int x = 0;
            if (!IsEmpty)
            {
                x = _head.Number;
                _head = _head.PointerToNextOne;
            }
            return x;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            ListNode node = _head;
            while (node != null)
            {
                sb.Append(node.Number);
                sb.Append(" -> ");
                node = node.PointerToNextOne;
            }
            sb.Append("*");

            return sb.ToString();
        }
    }
}
