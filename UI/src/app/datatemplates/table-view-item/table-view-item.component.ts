import { Component, Input } from '@angular/core'
import { Item, Columns } from '../../model/model'

@Component({
    selector: '[app-table-view-item]',
    templateUrl: './table-view-item.component.html',
    styleUrls: ['./table-view-item.component.css']
})
export class TableViewItemComponent {
    @Input()
    item: Item
    @Input()
    columns: Columns
}
