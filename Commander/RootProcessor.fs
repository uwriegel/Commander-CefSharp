module RootProcessor

open Model

[<Literal>]
let name = "root"

let columns = [ { Name = Resources.Resources.RootName; IsSortable = false; ColumnsType = ColumnsType.String }; 
    { Name = Resources.Resources.RootLabel; IsSortable = false; ColumnsType = ColumnsType.String }; 
    { Name = Resources.Resources.RootSize; IsSortable = false; ColumnsType = ColumnsType.Size } 
] 