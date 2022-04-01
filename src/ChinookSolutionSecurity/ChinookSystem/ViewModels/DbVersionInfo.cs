using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChinookSystem.ViewModels
{
        //The view that is used by the "outside world"
        //access must match the method where the class  is used (typically public)
        //purpose: used to simply carry data
        //      create data fields as auto-implemented properties
        //      consist of the "raw" data of the query


    public class DbVersionInfo
    {
        public int Major { get; set; }
        public int Minor { get; set; }
        public int Build { get; set; }
        public DateTime ReleaseDate { get; set; }

    }
}
