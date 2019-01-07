import { Component, OnInit } from '@angular/core'
import { IProcessor, ICommanderView } from 'src/app/interfaces/commander-view'
import { Columns } from 'src/app/model/model'

const callerId = "1"

@Component({
    selector: 'app-test-connection',
    templateUrl: './connection.component.html',
    styleUrls: ['./connection.component.css']
})
export class ConnectionComponent implements OnInit, ICommanderView {
    constructor() {
        commanderViewLeft = this
        CommanderLeft.ready()
    }

    ngOnInit() { }

    withColumns: any

    setColumns(columns: Columns) {
        console.log("New Columns", columns)
    }

    itemsChanged() { 
        console.log(CommanderLeft.getItems())
    }

    setCurrentItem(item: string) { 
        console.log("current item", item)
    }

    onGet(url: string) {
        CommanderLeft.changePath(url)
    }
}

declare var CommanderLeft : IProcessor
declare var CommanderRight : IProcessor

declare var commanderViewLeft : ICommanderView
declare var commanderViewRight : ICommanderView