using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using QD.Framework.NoSql;

namespace Sikiro.Model
{
    public class SmsModel : MongoEntity
    {
        public string Content { get; set; }

        public SmsEnums.SmsType Type { get; set; }

        public SmsEnums.SmsStatus Status { get; set; }

        public List<string> Mobiles { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CreateDateTime { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime? TimeSendDateTime { get; set; }
       
    }
}
