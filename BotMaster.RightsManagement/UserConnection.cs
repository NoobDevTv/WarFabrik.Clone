using BotMaster.Database;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotMaster.RightsManagement;

[Table("UserConnections")]
public class UserConnection : IdEntity<int>, ICloneableGeneric<UserConnection>
{
    public string ConnectionCode { get; set; }
    public DateTime ValidUntil { get; set; }
    public bool Connected { get; set; }

    public int PlattformUserId { get; set; }

    [ForeignKey(nameof(PlattformUserId))]
    public virtual PlattformUser? PlattformUser { get; set; }

    public UserConnection Clone()
    {
        return new()
        {
            Connected = Connected,
            ConnectionCode = ConnectionCode,
            ValidUntil = ValidUntil,
            PlattformUserId = PlattformUserId,
            PlattformUser = PlattformUser?.Clone()
        };
    }
}
