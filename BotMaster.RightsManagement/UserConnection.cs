using BotMaster.Database;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotMaster.RightsManagement;
public class UserConnection : IdEntity<int>
{
    public string ConnectionCode { get; set; }
    public DateTime ValidUntil { get; set; }
    public bool Connected { get; set; }


    public virtual PlattformUser PlattformUser { get; set; }
}
