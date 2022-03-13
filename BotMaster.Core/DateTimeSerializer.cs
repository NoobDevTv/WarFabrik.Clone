using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotMaster.Core;
public static class DateTimeSerializer
{
    public static void DateTimeToBinary(BinaryWriter bw, DateTime dt)
    {
        bw.Write(dt.ToBinary());
    }
    public static DateTime DateTimeFromBinary(BinaryReader br)
    {
        return DateTime.FromBinary(br.ReadInt64());
    }
}
