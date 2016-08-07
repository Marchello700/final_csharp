using SQLiteClassLibrary;
using System;

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
