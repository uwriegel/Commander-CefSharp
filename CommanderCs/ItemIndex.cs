using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Commander.Enums;

namespace Commander
{
    static class ItemIndex
    {
#pragma warning disable CS0675 // Bitwise-or operator used on a sign-extended operand
        public static int Create(ItemType itemType, int arrayIndex) => (int)(((uint)itemType << 24) | arrayIndex);
#pragma warning restore CS0675 // Bitwise-or operator used on a sign-extended operand
        public static int GetArrayIndex(this int index) => (int)((uint)index & 0xFFFFFF);
        public static ItemType GetItemType(this int index) => (ItemType)(byte)(index >> 24);

        public static int GetDefault(ViewType viewType)
        {   
            switch (viewType)
            {
                case ViewType.Root:
                    return Create(ItemType.Directory, 0);
                default:
                    return Create(ItemType.Parent, 0);
            }
        }

        public static bool IsSelected(this int index, int arrayIndex, ItemType itemType)
            => Create(itemType, arrayIndex) == index;
    }
}
