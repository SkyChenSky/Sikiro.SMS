using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using Sikiro.Nosql.Mongo.Base;

namespace Sikiro.Model
{
    [Mongo(MongoKey.SmsDataBase, MongoKey.SmsCollection)]
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

        public int SendCount { get; set; }
    }
}
