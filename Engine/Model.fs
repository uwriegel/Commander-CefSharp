module Model

type ItemType = Undefined = 0 | Parent = 1 | Directory = 2 | File = 3 

type Column = {
    name: string
    isSortable: bool
}

type Columns = {
    name: string
    values: Column[]
}

type ResponseItem = {
    itemType: ItemType
    index: int
    items: string[]
    icon: string
    isCurrent: bool
    isHidden: bool
}

type Response = {
    itemToSelect: string
    path: string
    items: ResponseItem[]
}