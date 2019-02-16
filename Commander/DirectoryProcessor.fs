module DirectoryProcessor

open Model

[<Literal>]
let name = "directory"

let columns = [ { Name = Resources.Resources.DirectoryName; IsSortable = false; ColumnsType = ColumnsType.String };
                   { Name = Resources.Resources.DirectoryName; IsSortable = false; ColumnsType = ColumnsType.String };  
                   { Name = Resources.Resources.DirectoryExtension; IsSortable = false; ColumnsType = ColumnsType.String };  
                   { Name = Resources.Resources.DirectoryDate; IsSortable = true; ColumnsType = ColumnsType.Date };  
                   { Name = Resources.Resources.DirectorySize; IsSortable = true; ColumnsType = ColumnsType.Size };  
                   { Name = Resources.Resources.DirectoryVersion; IsSortable = false; ColumnsType = ColumnsType.String } 
               ] 