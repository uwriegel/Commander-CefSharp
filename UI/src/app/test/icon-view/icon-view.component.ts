import { Component, OnInit } from '@angular/core'
import { IProcessor } from '../../interfaces/commander-view'

@Component({
    selector: 'app-icon-view',
    templateUrl: './icon-view.component.html',
    styleUrls: ['./icon-view.component.css']
})
export class IconViewComponent {
    items: string[] = []

    constructor() {
        this.items = CommanderLeft.getTestItems()
    }
}

declare var CommanderLeft: IProcessor

