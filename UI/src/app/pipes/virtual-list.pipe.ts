import { Pipe, PipeTransform } from '@angular/core'
import { Observable, Subscriber } from 'rxjs'
import { ScrollbarComponent } from '../scrollbar/scrollbar.component'
import { Item } from '../model/model'

@Pipe({
    name: 'virtualList'
})
export class VirtualListPipe implements PipeTransform {
    transform(items: Item[], scrollbar: ScrollbarComponent) {
        this.items = items || [] 
        if (!this.scrollbar) {
            this.scrollbar = scrollbar
            this.scrollbar.positionChanged.subscribe((position, _) => {
                this.displayObserver.next(this.getViewItems(position))
            })
        }
        return new Observable<Item[]>(displayObserver => {
            this.displayObserver = displayObserver
            let index = this.items.findIndex(n => (<Item>n).isCurrent) 
            if (index == -1)
                index = 0
            this.scrollbar.setPosition(index)
            this.scrollbar.itemsChanged(this.items.length, null, index)
            this.displayObserver.next(this.getViewItems(this.scrollbar.getPosition()))
        })
    }

    private getViewItems(position: number) {        
        return this.items.filter((_, i) => i >= position && i < this.scrollbar.maxItemsToDisplay + 1 + position)
    }

    private displayObserver: Subscriber<Item[]>
    private items: Item[]
    private scrollbar: ScrollbarComponent
}
