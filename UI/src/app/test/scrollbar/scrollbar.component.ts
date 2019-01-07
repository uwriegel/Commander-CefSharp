import { Component, ViewChild, OnInit, NgZone } from '@angular/core'
import { Subscriber } from 'rxjs'
import { ScrollbarComponent as ScrollBar  } from "../../scrollbar/scrollbar.component"
import { Item, Columns, Response } from 'src/app/model/model'
import { ICommanderView, IProcessor } from 'src/app/interfaces/commander-view'

@Component({
    selector: 'app-test-scrollbar',
    templateUrl: './scrollbar.component.html',
    styleUrls: ['./scrollbar.component.css']
})
export class ScrollbarComponent implements OnInit, ICommanderView {

    @ViewChild(ScrollBar) private scrollBar: ScrollBar
    items: Item[] = []
    
    ngOnInit() { 
        this.get(this.dirs[1]) 
    }

    constructor(private zone: NgZone) {
        commanderViewLeft = this
    }

    setColumns(columns: Columns){}

    itemsChanged() {
        this.zone.run(() => {
            const response: Response = JSON.parse(CommanderLeft.getItems())
            this.items = response.items
        })
    }

    setCurrentItem(item: string) { }

    onNew() {
        const index = this.seed++ % 3
        const dir = this.dirs[index]
        this.get(dir)
    }

    get(path: string) { 
        CommanderLeft.changePath(path)
    }

    onKeyDown(evt: KeyboardEvent) {
        switch (evt.which) {
            case 33:
                this.pageUp()
                break
            case 34:
                this.pageDown()
                break
            case 35: // End
                this.end()
                break
            case 36: //Pos1
                this.pos1()
                break
            case 38:
                this.upOne()
                break
            case 40:
                this.downOne()
                break
            default:
                return // exit this handler for other keys
        }
        evt.preventDefault() // prevent the default action (scroll / move caret)
    }

    private getCurrentIndex(defaultValue?: number) { 
        const index = this.items.findIndex(n => n.isCurrent) 
        if (index != -1 || defaultValue == null)
            return index
        else
            return defaultValue
    }

    private setCurrentIndex(index: number, lastIndex?: number) {
        if (lastIndex == null) 
            lastIndex = this.getCurrentIndex(0)
        this.items[lastIndex].isCurrent = false
        this.items[index].isCurrent = true
        this.scrollBar.scrollIntoView(index)
    }

    private downOne() {
        const index = this.getCurrentIndex(0)
        const nextIndex = index < this.items.length - 1 ? index + 1 : index
        this.setCurrentIndex(nextIndex, index)
    }

    private upOne() {
        const index = this.getCurrentIndex(0)
        const nextIndex = index > 0 ? index - 1 : index
        this.setCurrentIndex(nextIndex, index)
    }    

    private pageDown() {
        const index = this.getCurrentIndex(0)
        const nextIndex = index < this.items.length - this.scrollBar.itemsCapacity + 1 ? index + this.scrollBar.itemsCapacity - 1: this.items.length - 1
        this.setCurrentIndex(nextIndex, index)
    }

    private pageUp() {
        const index = this.getCurrentIndex(0)
        const nextIndex = index > this.scrollBar.itemsCapacity - 1? index - this.scrollBar.itemsCapacity + 1: 0
        this.setCurrentIndex(nextIndex, index)
    }

    private end() { this.setCurrentIndex(this.items.length - 1) } 
    
    private pos1() { this.setCurrentIndex(0) } 

    private seed = 0
    private dirs = [ "c:\\", "c:\\windows", "c:\\windows\\system32"]
    //private dirs = [ "/", "/usr/share", "/opt"]
    private displayObserver: Subscriber<Item[]>
}

declare var CommanderLeft : IProcessor
declare var commanderViewLeft : ICommanderView
