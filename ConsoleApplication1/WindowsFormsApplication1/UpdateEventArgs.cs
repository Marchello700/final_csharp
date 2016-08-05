using SQLiteClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication1
{
    public class UpdateEventArgs : EventArgs
    {
        private readonly ComputerDetail _computerDetail;

        public UpdateEventArgs(ComputerDetail computerDetail)
        {
            _computerDetail = computerDetail;
        }

        public ComputerDetail GetComputerDetail()
        {
            return _computerDetail;
        }
    }
}
