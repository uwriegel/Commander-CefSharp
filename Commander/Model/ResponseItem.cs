using System;
using System.Collections.Generic;
using System.Linq;
using Commander.Enums;

namespace Commander.Model
{
    struct ResponseItem
    {
        public ResponseItem(ItemType itemType, int index, IEnumerable<string> items, string icon, bool isCurrent, bool isHidden = false, bool isExif = false)
        {
            ItemType = itemType;
            Index = index;
            Items = items.ToArray();
            Icon = icon;
            IsCurrent = isCurrent;
            IsHidden = isHidden;
            IsExif = isExif;
        }

        public ItemType ItemType { get; }
        public int Index { get; }
        public string[] Items { get; }
        public string Icon { get; }
        public bool IsCurrent { get; }
        public bool IsHidden { get; }
        public bool IsExif { get; }
    }
}
