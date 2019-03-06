module ItemIndex

open Model

let create (itemType: ItemType) arrayIndex = 
    int ((uint32 itemType) <<< 24) ||| arrayIndex

let getDefault viewType = 
    match viewType with
    | ViewType.Root -> create ItemType.Directory 0
    | ViewType.Directory | _ -> create ItemType.Parent 0

let getArrayIndex index = int ((uint32 index) &&& (uint32 0xFFFFFF))
let getItemType index = enum<ItemType>(int (byte (index >>> 24)))

let isSelected index arrayIndex itemType =
    
    let mistkerl = create itemType arrayIndex
    
    let affe = (create itemType arrayIndex) = index

    affe