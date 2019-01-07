import { Component, AfterViewInit, ViewChild, ElementRef, Renderer2, Output, EventEmitter, Input } from '@angular/core'
import { trigger, state, style, transition, animate } from '@angular/animations'
import { ThemesService } from '../services/themes.service';

@Component({
    selector: 'app-scrollbar',
    templateUrl: './scrollbar.component.html',
    styleUrls: ['./scrollbar.component.css'],
    animations: [
        trigger('visibility', [
            state('invisible', style({
                width: '0px',
                height: '0px',
                opacity: '0',
            })),
            state('visible', style({
                width: '16px',
                height: 'calc(100% - {{offset}}px)',
                opacity: '1',
            }),
        {
            params: {offset: 20}
        }),
            transition('invisible => visible', animate('500ms ease-in')),
            transition('visible => invisible', animate('500ms ease-out'))
        ])
    ]    
})
export class ScrollbarComponent implements AfterViewInit {
    @ViewChild("scrollbar") scrollbar: ElementRef
    @ViewChild("grip") grip: ElementRef
    @Input() list: HTMLElement
    @Input() columnsHeight = 0
    @Input() itemHeight: number
    @Output() positionChanged: EventEmitter<number> = new EventEmitter()    

    visibility = 'invisible'

    getPosition() { return this.position}
    setPosition(value: number) { this.position = value }

    get maxItemsToDisplay() { return this._maxItemsToDisplay }
    private _maxItemsToDisplay = 0
    
    get itemsCapacity() { return this._itemsCapacity }

    constructor(private renderer: Renderer2, private themes: ThemesService) {}

    ngAfterViewInit() {
        this.themes.itemHeightChanged.subscribe(val => {
            console.log("Nächstes", val)
            console.log("Höhe", this.itemHeight)
            this.onResize(true)
        })
        this.onResize()
    }

    /**
     * Has to be called when scrollableContent has changed item count
     * @param numberOfItems new complete number of items
     * @param numberOfItemsDisplayed number of Items which can be displayed without scrolling
     * @param newScrollPos first item displayed
     */
    itemsChanged(numberOfItems: number, itemsCapacity?: number, newScrollPos?: number) {
        if (itemsCapacity)
            this._itemsCapacity = itemsCapacity
        else
            itemsCapacity = this._itemsCapacity
        if (itemsCapacity > numberOfItems)
            itemsCapacity = numberOfItems
        this.parentHeight = this.scrollbar.nativeElement.parentElement.parentElement.clientHeight - this.offsetTop
        if (numberOfItems)
            this.itemsCountAbsolute = numberOfItems
        if (itemsCapacity)
            this._maxItemsToDisplay = itemsCapacity

        if (!this.itemsCountAbsolute)
            return
        if (this.itemsCountAbsolute <= this.maxItemsToDisplay) {
            this.visibility = 'invisible'
            this.position = 0
            this.positionChanged.emit(this.position)
        }
        else {
            this.visibility = 'visible'
            var gripHeight = (this.parentHeight - 32 - this.columnsHeight) * (this.maxItemsToDisplay / this.itemsCountAbsolute)
            if (gripHeight < 5)
                gripHeight = 5
            this.steps = this.itemsCountAbsolute - this.maxItemsToDisplay        
            this.step = (this.parentHeight - 32 - this.columnsHeight - gripHeight) / this.steps
            this.renderer.setStyle(this.grip.nativeElement, "height", gripHeight + 'px')
            if (this.position > this.steps) 
                this.position = this.steps
            this.positionChanged.emit(this.position)
        }
        if (newScrollPos != undefined) {
            this.position = newScrollPos
            if (this.position < 0)
                this.position = 0
            else if (this.position >= numberOfItems - itemsCapacity)
                this.position = numberOfItems - itemsCapacity
            this.positionChanged.emit(this.position)
        }
        this.positionGrip()
    }

    scrollIntoView(index: number) {
        if (index < this.position)
            this.itemsChanged(this.itemsCountAbsolute, this._itemsCapacity, index)
        if (index > this.position + this._itemsCapacity - 1)
            this.itemsChanged(this.itemsCountAbsolute, this._itemsCapacity, index - this._itemsCapacity + 1)
    }

    onResize(force = false) {
        if (force || (this.list && this.list.parentElement.clientHeight != this.recentHeight)) {
            this.recentHeight = this.list.parentElement.clientHeight
            this._itemsCapacity = this.calculateCapacity()
            this.itemsChanged(this.itemsCountAbsolute, this._itemsCapacity)
        }
    }

    private onMouseWheel(evt: WheelEvent) {
        var delta = evt.wheelDelta / Math.abs(evt.wheelDelta) * 3
        this.itemsChanged(this.itemsCountAbsolute, this._itemsCapacity, this.position - delta)
    }

    private calculateCapacity() {
        let capacity = Math.floor((this.list.parentElement.clientHeight - this.columnsHeight) / this.itemHeight)
        if (capacity < 0)
            capacity = 0
    
        return capacity
    }

    scrollbarMouseDown(evt: MouseEvent) {
        if (!(<HTMLElement>evt.target).classList.contains("scrollbar"))
            return

        this.pageMousePosition = evt.layerY
        const isPageUp = evt.layerY < this.grip.nativeElement.offsetTop

        clearTimeout(this.timer)
        clearInterval(this.interval)
        if (isPageUp)
            this.pageUp()
        else
            this.pageDown()

        this.timer = setTimeout(() => this.interval = setInterval((
            isPageUp ? () => this.pageUp() : () => this.pageDown()), 10), 600)
    }

    gripMouseDown(evt: MouseEvent) {
        if (evt.which != 1)
            return
        this.gripping = true
        evt.preventDefault()

        this.gripTopDelta = this.grip.nativeElement.offsetTop + this.scrollbar.nativeElement.offsetTop - evt.pageY
        var gripperMove = (evt: MouseEvent) => {
            if (!this.gripping || evt.which != 1) {
                window.removeEventListener('mousemove', gripperMove)
                return
            }

            var top = evt.pageY + this.gripTopDelta - this.scrollbar.nativeElement.offsetTop
            if (top < 15)
                top = 15
            else if (top + this.grip.nativeElement.offsetHeight - 15 > (this.parentHeight - 32 - this.columnsHeight))
                top = this.parentHeight - 32 - this.columnsHeight - this.grip.nativeElement.offsetHeight + 15
            this.renderer.setStyle(this.grip.nativeElement, "top", top + 'px')

            var currentPosition = Math.floor((top - 15) / this.step + 0.5)
            if (currentPosition != this.position) {
                this.position = currentPosition
                    this.positionChanged.emit(this.position)
            }
        }

        window.addEventListener('mousemove', gripperMove)
    }

    upMouseDown() {
        clearTimeout(this.timer)
        clearInterval(this.interval)
        this.mouseUp()

        this.timer = setTimeout(() => this.interval = setInterval(() => this.mouseUp(), 10), 600)
    }

    downMouseDown() {
        clearTimeout(this.timer)
        clearInterval(this.interval)
        this.mouseDown()

        this.timer = setTimeout(() => this.interval = setInterval(() => this.mouseDown(), 10), 600)
    }

    mouseup() {
        clearTimeout(this.timer)
        clearInterval(this.interval)
        this.gripping = false
        this.setFocus()
    }

    onClick(evt: Event) {
        evt.stopPropagation()
    }

    onMouseLeave() {
        clearTimeout(this.timer)
        clearInterval(this.interval)
    }

    mouseUp() {
        this.position -= 1
        if (this.position < 0) {
            this.position = 0
            clearTimeout(this.timer)
            clearInterval(this.interval)
            return
        }

        this.positionGrip()
        this.positionChanged.emit(this.position)
    }

    mouseDown() {
        this.position += 1
        if (this.position > this.steps) {
            this.position = this.steps
            clearTimeout(this.timer)
            clearInterval(this.interval)
            return
        }
        this.positionGrip()
        this.positionChanged.emit(this.position)
    }

    private pageUp() {
        if (this.grip.nativeElement.offsetTop < this.pageMousePosition) {
            clearTimeout(this.timer)
            clearInterval(this.interval)
            return
        }

        this.position -= this.maxItemsToDisplay - 1
        if (this.position < 0) {
            const lastTime = this.position != 0
            this.position = 0
            clearTimeout(this.timer)
            clearInterval(this.interval)
            if (lastTime) {
                this.positionGrip()
                this.positionChanged.emit(this.position)
            }
            return
        }
        this.positionGrip()
        this.positionChanged.emit(this.position)
    }

    private pageDown() {
        if (this.grip.nativeElement.offsetTop + this.grip.nativeElement.offsetHeight > this.pageMousePosition) {
            clearTimeout(this.timer)
            clearInterval(this.interval)
            return
        }

        this.position += this.maxItemsToDisplay - 1
        if (this.position > this.steps) {
            const lastTime = this.position != 0
            this.position = this.steps
            clearTimeout(this.timer)
            clearInterval(this.interval)
            if (lastTime) {
                this.positionGrip()
                this.positionChanged.emit(this.position)
            }
            return
        }

        this.positionGrip()
        this.positionChanged.emit(this.position)
    }

    private positionGrip() {
        const top = 15 + this.position * this.step
        this.renderer.setStyle(this.grip.nativeElement, "top", top + 'px')
    }

    private _itemsCapacity = 0
    private position = 0
    private setFocus = () => { }
    private gripTopDelta = -1
    private gripping = false
    private recentHeight = 0
    private parentHeight = 0
    private offsetTop = 0

    private timer: any
    private interval: any
    private pageMousePosition = 0
    private step = 0
    private steps = 0
    private itemsCountAbsolute = 0
}
