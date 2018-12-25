module Model

type Column = {
    name: string
    isSortable: bool
}

type Columns = {
    name: string
    values: Column[]
}

