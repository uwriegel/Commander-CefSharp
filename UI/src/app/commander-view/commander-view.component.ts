import { Component, ViewChild, ElementRef, Input, AfterViewInit, Output, EventEmitter, NgZone } from '@angular/core'
import { trigger, state, style, transition, animate } from '@angular/animations'
import { Observable, fromEvent } from 'rxjs'
import { filter, map } from 'rxjs/operators'
import { IColumnSortEvent } from '../columns/columns.component'
import { TableViewComponent } from '../table-view/table-view.component'
import { DialogComponent } from '../dialog/dialog.component'
import { Buttons } from '../enums/buttons.enum'
import { DialogResultValue } from '../enums/dialog-result-value.enum'
import { Item, Response, ItemType, ColumnsType, Columns } from '../model/model'
import { ThemesService } from '../services/themes.service'
import { ICommanderView, IProcessor, ProcessItemType } from '../interfaces/commander-view'
import { repeatKey } from '../functional/scrolling';

@Component({
    selector: 'app-commander-view',
    templateUrl: './commander-view.component.html',
    styleUrls: ['./commander-view.component.css'],
    animations: [
        trigger('flyInOut', [
            state('in', style({
                opacity: 0,
                width: '0%',
                height: '0px'    
            })),
            transition(":enter", [
                style({
                    opacity: 0,
                    width: '0%',
                    height: '0px'    
                }),
                animate("0.3s ease-out", style({
                    opacity: 1,
                    width: '70%',
                    height: '15px'
                }))
            ]),
            transition(':leave', [
                animate("0.3s ease-in", style({
                    opacity: 0,
                    width: '0%',
                    height: '0px'    
                }))
            ])
        ])    
    ]
})
export class CommanderViewComponent implements AfterViewInit, ICommanderView {

    @ViewChild(TableViewComponent) private tableView: TableViewComponent
    @ViewChild("input") private input: ElementRef
    @Output() private gotFocus: EventEmitter<CommanderViewComponent> = new EventEmitter()    
    @Input() commander: IProcessor
    @Input() 
    set id(value: string) {
        this._id = value
        if (value == 'left') 
            commanderViewLeft = this
        else if (value == 'right') 
            commanderViewRight = this
    }
    get id() { return this._id }
    private _id = ""

    @Input()
    dialog: DialogComponent

    setColumns(columns: Columns) { 
        this.columns = columns
    }

    itemsChanged() {
        this.zone.run(() => {
            const response: Response = JSON.parse(this.commander.getItems())
            this.items = response.items
            this.currentPath = response.path
        })
    }

    setCurrentItem(item: string) { 
        this.zone.run(() => this.currentItem = item)
    }
    
    currentPath = ""
    currentItem = ""

    columns: Columns
    
    get items() { return this._items}
    set items(value: Item[]) { 
        this.undoRestriction()
        this._items = value
    }
    private _items: Item[] = []

    onCurrentIndexChanged(index: number) {
        this.commander.setIndex(this.items[index].index)
    }
    
    restrictValue = ""

    undoRestriction = () => {}

    ngOnInit() { 
        this.commander.ready()
    }

    ngAfterViewInit() { 
        this.keyDownEvents = fromEvent(this.tableView.table.nativeElement, "keydown") 
        this.undoRestriction = this.initializeRestrict() 
    }

    constructor(public themes: ThemesService, private zone: NgZone) {}

    focus() { this.tableView.focus() }

    onMouseUp() {
        setTimeout(() => this.input.nativeElement.select())
    }

    onResize() { this.tableView.onResize() }

    createFolder(text: string) {
        this.zone.run(() => {
            this.dialog.buttons = Buttons.OkCancel
            this.dialog.text = text
            this.dialog.withInput = true
            this.dialog.inputText = ""
            const subscription = this.dialog.show().subscribe(result => {
                subscription.unsubscribe()
                if (result.result == DialogResultValue.Ok) {
                    this.commander.createFolder(result.text)
                    // TODO: dialog.text = "Der Ordner existiert bereits!"
                    // TODO: dialog.text = "Die Syntax für den Dateinamen, Verzeichnisnamen oder die Datenträgerbezeichnung ist falsch!"
                }
                else
                    this.focus()
            })
        })
    }

    copy(targetPath: string, text: string) {
        this.zone.run(() => {
            this.dialog.buttons = Buttons.OkCancel
            this.dialog.text = text
            const subscription = this.dialog.show().subscribe(result => {
                subscription.unsubscribe()
                if (result.result == DialogResultValue.Ok)
                    this.commander.copy(targetPath)
                else
                    this.focus()
            })
        })
    }

    delete(dialog: DialogComponent) {
        // if (this.itemProcessor.canDelete()) {
        //     var items = this.getSelectedItems()
        //     if (items.length == 0)
        //         return
        //     dialog.buttons = Buttons.OkCancel
        //     dialog.text = "Möchtest Du die selektierten Elemente löschen?"
        //     const subscription = dialog.show().subscribe(result => {
        //         subscription.unsubscribe()
        //         if (result.result == DialogResultValue.Ok) {
        //             const subscription = this.itemProcessor.deleteItems(items.map(n => `${this.path}\\${n.name}`))
        //                 .subscribe(obs => {
        //                     subscription.unsubscribe()
        //                     this.refresh()
        //                     this.focus()
        //                 }, err => {
        //                     subscription.unsubscribe()
        //                     switch (err) {
        //                         default:
        //                             dialog.text = `Fehler: ${err}`
        //                             break
        //                     }
                        
        //                     const subscriptionDialog = dialog.show().subscribe(result => {
        //                         subscriptionDialog.unsubscribe()
        //                         this.focus()
        //                     })
        //                 })
        //         }
        //         else
        //             this.focus()
        //     })
        // }
        // else {
        //     dialog.text = "Du kannst hier keine Elemente löschen!"
        //     const subscription = dialog.show().subscribe(() => {
        //         subscription.unsubscribe()
        //         this.focus()
        //     })
        // }
    }

    onFocus() { this.focus() }

    onFocusIn() { this.gotFocus.emit(this) }

    onInputChange() {
        this.commander.changePath(this.input.nativeElement.value)
        this.tableView.focus()
    }

    onInputKeydown(evt: KeyboardEvent) {
        switch (evt.which) {
            case 9: // TAB
                this.tableView.focus()
                evt.stopPropagation()
                evt.preventDefault()
                break
        }
    }

    onKeydown(evt: KeyboardEvent) {
        switch (evt.which) {
            case 9: // TAB
                if (evt.shiftKey) {
                    this.input.nativeElement.focus()
                    evt.stopPropagation()
                    evt.preventDefault()
                }    
                break
            case 13: // Return
                this.processItem(evt.altKey ? ProcessItemType.Properties : (evt.ctrlKey ? ProcessItemType.StartAs : ProcessItemType.Show))
                break
            case 32: // _                
                this.toggleSelection(this.tableView.getCurrentItem())
                break
            case 35: // End
                if (evt.shiftKey) 
                    this.selectAllItems(this.tableView.getCurrentItemIndex(), false)
                break
            case 36: // Pos1
                if (evt.shiftKey) 
                    this.selectAllItems(this.tableView.getCurrentItemIndex(), true)
                break                
            case 45: // Einfg
                repeatKey(evt.repeat, () => {
                    if (this.toggleSelection(this.tableView.getCurrentItem()))
                        this.tableView.downOne()
                })
                break;
            case 107: // NUM +
                this.selectAllItems(0, false)
                break
            case 109: // NUM -
                this.selectAllItems(0, true)
                break                
        }
    }

    private selectAllItems(currentItemIndex: number, above: boolean) {
        this.tableView.getAllItems().forEach((item, index) => {
            item.isSelected = this.isItemSelectable(item) 
                ? above ? index <= currentItemIndex : index >= currentItemIndex ? this.isItemSelectable(item) : false
                : false
        })
        this.selectionChanged()
    }

    private toggleSelection(item: Item) {
        let result = false
        if (this.isItemSelectable(item)) {
            item.isSelected = !item.isSelected
            result = true
        }
        this.selectionChanged()
        return result
    }

    private isItemSelectable(item: Item) {
        switch (item.itemType) {
            case ItemType.Parent:
            case ItemType.Drive:
                return false
            default:
                return true
        }
    }

    onClick(evt: MouseEvent) { 
        if (evt.ctrlKey && (evt.target as HTMLElement).closest("td"))  
            this.toggleSelection(this.tableView.getCurrentItem())
    }

    onDblClick(evt: MouseEvent) { 
        if ((evt.target as HTMLElement).closest("td")) 
            this.processItem(evt.altKey ? ProcessItemType.Properties : (evt.ctrlKey ? ProcessItemType.StartAs : ProcessItemType.Show))
    }

    onColumnSort(evt: IColumnSortEvent) {
        this.commander.sort(evt.index, evt.ascending)
    }

    private processItem(processItemType: ProcessItemType) { this.commander.processItem(processItemType) }

    private selectionChanged() {
        const items = this.tableView.getAllItems().filter(n => n.isSelected).map(n => n.index) 
        this.commander.setSelected(items)
    }

    private initializeRestrict() {
        const inputChars = this.keyDownEvents.pipe(filter(n => !n.altKey && !n.ctrlKey && !n.shiftKey && n.key.length > 0 && n.key.length < 2))
        const backSpaces = this.keyDownEvents.pipe(filter(n => n.which == 8))
        const escapes = this.keyDownEvents.pipe(filter(n => n.which == 27))
        let originalItems: Item[]
        
        inputChars.subscribe(evt => {
            const items = this.items.filter(n => n.items[0].toLowerCase().startsWith(this.restrictValue + evt.key))
            if (items.length > 0) {
                this.restrictValue += evt.key
                if (!originalItems)
                    originalItems = this.items
                this.items.forEach(n => n.isCurrent = false)    
                this._items = items
                items[0].isCurrent = true
                this.onCurrentIndexChanged(0)
                this.tableView.focus()
            }
        })
        backSpaces.subscribe(() => {
            if (this.restrictValue.length > 0) {
                this.restrictValue = this.restrictValue.substr(0, this.restrictValue.length - 1);
                const items = originalItems.filter(n => n.items[0].toLowerCase().startsWith(this.restrictValue));
                this._items = items;
            }
        })

        const undoRestriction = () => {
            if (originalItems) {
                this._items = originalItems
                originalItems = null
                this.restrictValue = ""
            }
        }

        escapes.subscribe(() => undoRestriction())
        return undoRestriction
    }

    private keyDownEvents: Observable<KeyboardEvent>
}

declare var commanderViewLeft : ICommanderView
declare var commanderViewRight : ICommanderView
