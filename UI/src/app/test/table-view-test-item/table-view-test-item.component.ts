import { Component, Input } from '@angular/core'
import { Item } from 'src/app/model/model'

@Component({
    selector: '[app-table-view-test-item]',
    templateUrl: './table-view-test-item.component.html',
    styleUrls: ['./table-view-test-item.component.css']
})
export class TableViewTestItemComponent {
    @Input()
    item: Item
}
