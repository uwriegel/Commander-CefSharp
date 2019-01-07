import { Component, NgZone } from '@angular/core'
import { IColumnSortEvent } from '../../columns/columns.component'
import { Columns } from 'src/app/model/model'
import { ICommanderView, IProcessor } from 'src/app/interfaces/commander-view'

@Component({
    selector: 'app-test-columns',
    templateUrl: './columns.component.html',
    styleUrls: ['./columns.component.css']
})
export class TestColumnsComponent implements ICommanderView {

    constructor(private zone: NgZone) { 
        commanderViewLeft = this
        CommanderLeft.ready()
    }

    columns: Columns

    setColumns(columns: Columns) {
        console.log("New Columns", columns)
        this.zone.run(() => this.columns = columns)
    }

    itemsChanged() { console.log("Items changed")}
    
    setCurrentItem(item: string) { }
    
    onSort(sortEvent: IColumnSortEvent) {
        console.log(`Sorting: ${sortEvent.index} ascending: ${sortEvent.ascending}`)
    }

    onChange(path: string) {
        CommanderLeft.changePath(path)
    }
}

declare var CommanderLeft : IProcessor
declare var commanderViewLeft : ICommanderView