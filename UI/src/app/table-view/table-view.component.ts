import { Component, ViewChild, ElementRef, Output, EventEmitter, Input } from '@angular/core'
import { ScrollbarComponent as Scrollbar } from '../scrollbar/scrollbar.component'
import { ColumnsComponent as IColumnSortEvent } from '../columns/columns.component'
import { Item, Columns } from '../model/model'
import { getBodyNode } from '@angular/animations/browser/src/render/shared';
import { repeatKey } from '../functional/scrolling';

@Component({
    selector: 'app-table-view',
    templateUrl: './table-view.component.html',
    styleUrls: ['./table-view.component.css']
})
export class TableViewComponent {

    @Input() id = ""
    @Input() columnHeight = 0
    @Input() itemHeight = 0
    @Input() itemType: string
    @Output() private onSort: EventEmitter<IColumnSortEvent> = new EventEmitter()    
    @Output() onCurrentIndexChanged: EventEmitter<Number> = new EventEmitter()    
    @ViewChild("table") table: ElementRef
    @ViewChild(Scrollbar) private scrollbar: Scrollbar
    @ViewChild("tbody")
    
    @Input()
    set columns(value: Columns) {
        if (value) {
            console.log("Spalten", value)
            this._columns = value
        }
    }
    get columns() { return this._columns }
    _columns: Columns

    columnsName = ""

    @Input() 
    set items(value: Item[]) {
        if (value) {
            console.log("Items", value)
            this._items = value
        }
    }
    get items() { return this._items }
    _items: Item[] = []
    
    onColumnsChanged(name: string) {
        this.columnsName = name
    }

    onFocusIn(evt: Event) {
        console.log("Hab ihn bekommen")
    }

    focus() { 
        const index = this.getCurrentIndex()
        // var index = this.currentItemIndex - this.startPosition
        // if (index >= 0 && index < this.tableCapacity)
        // {
        //     var trs = this.tbody.querySelectorAll('tr')
        //     if (index < trs.length)
        //     {
        //         trs[index].focus()
        //         return true
        //     }
        // }
        // this.tableView.focus()
        // return false
    
        this.table.nativeElement.focus() 
    }

    getCurrentItemIndex() { return this.getCurrentIndex() }

    getCurrentItem() {
        const index = this.getCurrentIndex()
        if (index != -1)
            return this.items[index]
        else
            return null
    }

    getAllItems() { return this.items }

    onResize() { this.scrollbar.onResize() }

    onKeyDown(evt: KeyboardEvent) {
        switch (evt.which) {
            case 33:
                this.pageUp(evt.repeat)
                break
            case 34:
                this.pageDown(evt.repeat)
                break
            case 35: // End
                if (!evt.shiftKey)
                    this.end()
                break
            case 36: //Pos1
                if (!evt.shiftKey)
                    this.pos1()
                break
            case 38:
                this.upOne(evt.repeat)
                break
            case 40:
                this.downOne(evt.repeat)
                break
            default:
                return // exit this handler for other keys
        }
        evt.preventDefault() // prevent the default action (scroll / move caret)
    }

    onMouseDown(evt: MouseEvent) {
        const tr = <HTMLTableRowElement>(<HTMLElement>evt.target).closest("tbody tr")
        if (tr) {
            const currentIndex = Array.from(this.table.nativeElement.querySelectorAll("tr"))
                .findIndex(n => n == tr) + this.scrollbar.getPosition() - 1
            if (currentIndex != -1)
                this.setCurrentIndex(currentIndex)
        }
    }

    onColumnSort(sortEvent: IColumnSortEvent) {
        this.onSort.emit(sortEvent)
    }

    downOne(repeated = false) {
        repeatKey(repeated, () => {
            const index = this.getCurrentIndex(0)
            const nextIndex = index < this.items.length - 1 ? index + 1 : index
            this.setCurrentIndex(nextIndex, index)
        })
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
        this.scrollbar.scrollIntoView(index)
        this.onCurrentIndexChanged.emit(index)
    }

    private upOne(repeated: boolean) {
        repeatKey(repeated, () => {
            const index = this.getCurrentIndex(0)
            const nextIndex = index > 0 ? index - 1 : index
            this.setCurrentIndex(nextIndex, index)
        })
    }

    private pageDown(repeated: boolean) {
        repeatKey(repeated, () => {
            const index = this.getCurrentIndex(0)
            const nextIndex = index < this.items.length - this.scrollbar.itemsCapacity + 1 ? index + this.scrollbar.itemsCapacity - 1: this.items.length - 1
            this.setCurrentIndex(nextIndex, index)
        })
    }

    private pageUp(repeated: boolean) {
        repeatKey(repeated, () => {
            const index = this.getCurrentIndex(0)
            const nextIndex = index > this.scrollbar.itemsCapacity - 1? index - this.scrollbar.itemsCapacity + 1: 0
            this.setCurrentIndex(nextIndex, index)
        })
    }

    private end() { this.setCurrentIndex(this.items.length - 1) } 
    
    private pos1() { this.setCurrentIndex(0) } 

    // private repeatKey(repeated: boolean, process: () => void) {
    //     let isLooping = false

    //     let processLoop = () => {
    //         if (isLooping) {
    //             process()
    //             setTimeout(() => processLoop())
    //         }
    //     }

    //     if (!repeated) 
    //         process()
    //     else {
    //         let onkeyDown = function (evt: KeyboardEvent) {
    //             evt.stopPropagation()
    //             evt.preventDefault()
    //         }

    //         let onkeyUp = function () {
    //             isLooping = false
    //             document.removeEventListener("keydown", onkeyDown, true)
    //             document.removeEventListener("keyup", onkeyUp, true)
    //         }

    //         document.addEventListener("keydown", onkeyDown, true)
    //         document.addEventListener("keyup", onkeyUp, true)
    //         isLooping = true
    //         setTimeout(() => processLoop())
    //     }
    // }
}
