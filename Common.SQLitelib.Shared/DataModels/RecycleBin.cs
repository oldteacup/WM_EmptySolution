using Clipboard.Data.Enums;
using Clipboard.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.SQLitelib.DataModels
{
    [Table("RecycleBin")]
    public class RecycleBin : IClipboardData
    {
        [Key]
        public string ID { get; set; } = Guid.NewGuid().ToString("N");

        public ClipboardDataTypes DataType { get; set; } = ClipboardDataTypes.Unknown;

        public string Data { get; set; }

        public bool IsFavorite { get; set; } = false;

        public bool IsDeleted { get; set; } = true;

        public DateTime CreateTime { get; set; } = DateTime.Now;

        public DateTime LastTime { get; set; } = DateTime.Now;

        public DateTime DestructionTime { get; set; }

        public int UsageCounter { get; set; } = 0;

        public RecycleBin() { }

        public RecycleBin(IClipboardData data)
        {
            ID = data.ID;
            DataType = data.DataType;
            Data = data.Data;
            IsFavorite = data.IsFavorite;
            IsDeleted = data.IsDeleted;
            CreateTime = data.CreateTime;
            LastTime = data.LastTime;
            DestructionTime = DateTime.Now;
            UsageCounter = data.UsageCounter;
        }
    }
}
