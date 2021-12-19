using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace WarFabrik.Clone
{
    public readonly record struct FollowInformation(string UserName, string UserId, DateTime Since);
}
