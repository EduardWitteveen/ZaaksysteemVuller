using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericUTurn.POCO
{
    public class Bericht
    {
        public long Berichtid { get; set; }
        public string Kenmerk { get; set; }
        public string Action { get; set; }
        public int ResponseCode { get; set; }
        public string RequestBody { get; set; }
        public string ResponseBody { get; set; }
        public string Url { get; set; }
    }
}