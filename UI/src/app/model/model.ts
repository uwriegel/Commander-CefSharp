
export enum ItemType {
    Undefiend,
    Parent,
    Directory,
    File,
    Drive
}

export interface Item {
    itemType: ItemType
    index: number
    items: string[]
    icon: string
    isSelected?: boolean
    isCurrent?: boolean
    isHidden?: boolean
    isExif?: boolean
}

export interface Response {
    path: string
    items: Item[]
}

export interface Columns {
    name: string
    values: Column[]
}

export enum ColumnsType {
    String,
    Size,
    Date
}

export interface Column {
    name: string
    isSortable?: boolean
    columnsType: ColumnsType
}


