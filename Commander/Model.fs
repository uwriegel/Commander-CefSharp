module Model

type ColumnsType = String = 0 | Size = 1 | Date = 2

type Column = {
    Name: string
    IsSortable: bool
    ColumnsType: ColumnsType
}

[<NoComparison>]
type Columns = {
    Name: string 
    Values: seq<Column>
}

type ViewType = Root = 0 | Directory = 1


