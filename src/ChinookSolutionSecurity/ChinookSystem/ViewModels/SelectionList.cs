#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#region


#endregion

namespace ChinookSystem.ViewModels
{
    public class SelectionList
    {
        //coomon class used to hold 2 values for use in a select list senario
        // such as a drop down list

        public int ValueId { get; set; }
        public string DisplayText { get; set; }

    }
}
