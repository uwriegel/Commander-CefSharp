import { Component, OnInit, ViewChild, NgZone } from '@angular/core'
import { IColumnSortEvent } from '../../columns/columns.component'
import { Item, Columns, Response } from '../../model/model'
import { ThemesService } from 'src/app/services/themes.service'
import { TableViewComponent as TableView } from '../../table-view/table-view.component'
import { IProcessor, ICommanderView } from 'src/app/interfaces/commander-view'

@Component({
    selector: 'app-test-table-view',
    templateUrl: './table-view.component.html',
    styleUrls: ['./table-view.component.css']
})
export class TableViewComponent implements OnInit, ICommanderView {

    setColumns(columns: Columns) { 
        this.columns = columns
    }

    itemsChanged() {
        this.zone.run(() => {
            const response: Response = JSON.parse(this.commander.getItems())
            this.items = response.items
        })
    }

    setCurrentItem(item: string) { }
    
    onCurrentIndexChanged(index: number) {
        this.commander.setIndex(this.items[index].index)
    }

    itemType = "item"
    //itemType = "testItem"

    columns: Columns
    items: Item[] = []

    @ViewChild(TableView) tableView: TableView

    constructor(public themes: ThemesService, private zone: NgZone) { 
        commanderViewLeft = this
    }

    ngOnInit() { 
        this.commander.ready()
        this.tableView.focus()
    }

    onRoot() { this.get("root") }
    onC() { this.get("c:\\windows\\..") }
    onSystem32() { this.get("c:\\windows\\system32") }
    onPics() { this.get("c:\\04 - Brayka Bay") }

    get(path: string) {
        this.commander.changePath(path)
        this.tableView.focus()
    }

    onSort(sortEvent: IColumnSortEvent) {
        this.commander.sort(sortEvent.index, sortEvent.ascending)
    }

    private commander = CommanderLeft
}

declare var CommanderLeft : IProcessor
declare var commanderViewLeft : ICommanderView
