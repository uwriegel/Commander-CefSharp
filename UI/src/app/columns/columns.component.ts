import { Component, ViewChildren, ViewChild, ElementRef, QueryList, Renderer2, Output, EventEmitter, Input } from '@angular/core'
import { Columns } from '../model/model'

export interface IColumnSortEvent {
    index: number,
    ascending: boolean
}

@Component({
    selector: '[app-columns]',
    templateUrl: './columns.component.html',
    styleUrls: ['./columns.component.css']
})
export class ColumnsComponent {
    constructor(private renderer: Renderer2) {}

    @Input() id = ""
    @Output() onSort: EventEmitter<IColumnSortEvent> = new EventEmitter()    
    @ViewChild("columnsRow") columnsRow: ElementRef
    @ViewChildren("th") 
    get ths() { return this._ths }
    set ths(value) {
        this._ths = value
        this.restoreWidths()
    }
    private _ths: QueryList<ElementRef>

    @Input()
    get columns() { return this._columns }
    set columns(value) {
        if (value) {
            this._columns = value
            if (this.columnsName != value.name) {
                this.columnsName = value.name
            }
        }
    }
    private _columns: Columns = {
        name: "nil",
        values: []
    }

    readonly height = 16

    onMouseMove(evt: MouseEvent) {
        const th = <HTMLElement>evt.target
        if (th.localName == "th" && (th.offsetLeft > 0 || evt.pageX - th.getBoundingClientRect().left > 10)
            && (th.offsetLeft + th.offsetWidth < this.columnsRow.nativeElement.offsetWidth || evt.pageX - th.getBoundingClientRect().left < 4)
            && (th.getBoundingClientRect().left + th.offsetWidth - evt.pageX < 4 || evt.pageX - th.getBoundingClientRect().left < 4)) {
            this.renderer.addClass(th, "pointer-ew")
            this.grippingReady = true
            this.previous = evt.pageX - th.getBoundingClientRect().left < 4
        }
        else {
            this.renderer.removeClass(th, "pointer-ew")
            this.grippingReady = false
        }
    }

    private onClick(evt: Event) {
        if (!this.grippingReady) {
            const th = evt.target as HTMLElement
            const columnResult = 
                this.columns.values
                .map((n, i) => { return { column: n, index: i }})
                .find(n => n.column.name == th.innerText.trim())
            if (columnResult && columnResult.column.isSortable) {
                const ascending = th.classList.contains("sortAscending")
                this.ths.forEach(th => {
                    this.renderer.removeClass(th.nativeElement, "sortAscending")
                    this.renderer.removeClass(th.nativeElement, "sortDescending")
                })
                this.renderer.addClass(th, ascending ? "sortDescending" : "sortAscending")
                this.onSort.emit({ascending: !ascending, index: columnResult.index })
            }
        }
    }

    private onMouseDown(evt: MouseEvent) {
        const column = <HTMLElement>evt.target
        if (this.grippingReady) 
            this.beginColumnDragging(evt.pageX, column)
    }

    private beginColumnDragging(startGripPosition: number, targetColumn: HTMLElement) {
        document.body.style.cursor = 'ew-resize'

        let currentHeader: HTMLElement
        if (!this.previous)
            currentHeader = targetColumn
        else
            currentHeader = <HTMLElement>targetColumn.previousElementSibling
        const nextHeader = <HTMLElement>currentHeader.nextElementSibling
        const currentLeftWidth = currentHeader.offsetWidth
        const sumWidth = currentLeftWidth + nextHeader.offsetWidth

        const onmove = (evt: MouseEvent) => {
            this.renderer.setStyle(document.body, "cursor", 'ew-resize')
            var diff = evt.pageX - startGripPosition

            if (currentLeftWidth + diff < 15)
                diff = 15 - currentLeftWidth
            else if (diff > sumWidth - currentLeftWidth - 15)
                diff = sumWidth - currentLeftWidth - 15

            const combinedWidth = this.getCombinedWidth(currentHeader, nextHeader)

            let leftWidth = currentLeftWidth + diff
            let rightWidth = sumWidth - currentLeftWidth - diff
            const factor = combinedWidth / sumWidth
            leftWidth = leftWidth * factor
            rightWidth = rightWidth * factor

            currentHeader.style.width = leftWidth + '%'
            nextHeader.style.width = rightWidth + '%'
            evt.preventDefault()
        }

        const onup = (evt: MouseEvent) => {
            const columnsWidths = this.getWidths()
            localStorage[this.getColumnsId()] = JSON.stringify(columnsWidths)
            document.body.style.cursor = null
            window.removeEventListener('mousemove', onmove)
            window.removeEventListener('mouseup', onup)
        }

        window.addEventListener('mousemove', onmove)
        window.addEventListener('mouseup', onup)
    }

    private getWidths() {
        let widths = new Array()
        this.ths.forEach((th, i) => {
            widths[i] = th.nativeElement.style.width
            if (!widths[i])
                widths[i] = (100 / this.columns.values.length) + '%'
        })
        return widths
    }

    private setWidths(widths: string[]) {
        this.ths.forEach((th, i) => this.renderer.setStyle(th.nativeElement, "width", widths[i]))
    }
    
    private restoreWidths() {
        const json = localStorage[this.getColumnsId()]
        if (json && this.ths) {
            const columnWidth = JSON.parse(json)
            this.setWidths(columnWidth)
        }
    }

    private getColumnsId() { return this.id + "-" + this.columns.name }

    private getCombinedWidth(column: HTMLElement, nextColumn: HTMLElement) {
        const firstWidth = column.style.width
            ? parseFloat(column.style.width.substr(0, column.style.width.length - 1))
            : 100 / this.columns.values.length
        const secondWidth = nextColumn.style.width
            ? parseFloat(nextColumn.style.width.substr(0, nextColumn.style.width.length - 1))
            : 100 / this.columns.values.length
        return firstWidth + secondWidth
    }

    private previous = false
    private grippingReady = false
    private columnsName = ""
}
