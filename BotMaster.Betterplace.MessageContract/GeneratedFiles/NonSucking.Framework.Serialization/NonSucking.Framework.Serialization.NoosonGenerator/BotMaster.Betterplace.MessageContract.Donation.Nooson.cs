namespace BotMaster.Betterplace.MessageContract
{
    public partial class Donation
    {
        public void Serialize(System.IO.BinaryWriter writer)
        {
            writer.Write(TypeId);
            writer.Write(Id);
            writer.Write(Donated_amount_in_cents);
            writer.Write(Matched_amount_in_cents);
            writer.Write(Matched);
            writer.Write(Score);
            writer.Write(Author);
            writer.Write(Message);
            writer.Write(Links.Length);
            foreach (var item__Links__b in Links)
            {
                writer.Write(item__Links__b);
            }
        }

        public static Donation Deserialize(System.IO.BinaryReader reader)
        {
            var TypeId__ret__c = reader.ReadInt32();
            var Id__ret__d = reader.ReadString();
            System.DateTime Created_at__ret__f;
            Created_at__ret__f = new System.DateTime();
            System.DateTime Updated_at__ret__h;
            Updated_at__ret__h = new System.DateTime();
            var Donated_amount_in_cents__ret__i = reader.ReadInt32();
            var Matched_amount_in_cents__ret__j = reader.ReadInt32();
            var Matched__ret__k = reader.ReadBoolean();
            var Score__ret__l = reader.ReadString();
            var Author__ret__m = reader.ReadString();
            var Message__ret__n = reader.ReadString();
            System.DateTime Confirmed_at__ret__p;
            Confirmed_at__ret__p = new System.DateTime();
            var count__Links__t = reader.ReadInt32();
            var Links__ret__s = new System.Collections.Generic.List<string>(count__Links__t);
            for (int i____u = 0; i____u < count__Links__t; i____u++)
            {
                var String____r = reader.ReadString();
                Links__ret__s.Add(String____r);
            }

            var ret____ = new BotMaster.Betterplace.MessageContract.Donation(Id__ret__d, TypeId__ret__c);
            ret____.Created_at = Created_at__ret__f;
            ret____.Updated_at = Updated_at__ret__h;
            ret____.Donated_amount_in_cents = Donated_amount_in_cents__ret__i;
            ret____.Matched_amount_in_cents = Matched_amount_in_cents__ret__j;
            ret____.Matched = Matched__ret__k;
            ret____.Score = Score__ret__l;
            ret____.Author = Author__ret__m;
            ret____.Message = Message__ret__n;
            ret____.Confirmed_at = Confirmed_at__ret__p;
            ret____.Links = Links__ret__s.ToArray();
            return ret____;
        }
    }
}