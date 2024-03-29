﻿using BotMaster.Database;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BotMaster.RightsManagement;

[Table("PlattformUsers")]
public class PlattformUser : IdEntity<int>
{
    

    [Required]
    public string Platform { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    public string PlattformUserId { get; set; }

    public virtual User? User { get; set; }
    public virtual ICollection<Right> Rights { get; set; } = new List<Right>();
    public virtual ICollection<Group> Groups { get; set; } = new List<Group>();
}
