import { Injectable, NgZone } from '@angular/core'
import { Observable, Subject } from 'rxjs'
import { IThemes } from '../interfaces/themes'

@Injectable({
    providedIn: 'root'
})
export class ThemesService implements IThemes {
    constructor(private zone: NgZone) { themes = this }

    initialize(app: HTMLElement) { this.app = app}

    columnHeight = 0

    itemHeightChanged: Observable<boolean> = new Subject<boolean>();

    get theme() { return this._theme }
    set theme(theme: string) {
        this.zone.run(() => {
            console.log("Theme changed", theme)
            this._theme = theme
            setTimeout(() => {
                const bodyStyles = window.getComputedStyle(this.app)
                this.itemHeight = <any>bodyStyles.getPropertyValue('--itemHeight')
                this.testItemHeight = <any>bodyStyles.getPropertyValue('--testItemHeight')
                this.columnHeight = <any>bodyStyles.getPropertyValue('--itemColumnHeight')
            })        
        })
    }
    private _theme = "blue"

    get itemHeight() { return this._itemHeight }
    set itemHeight(value: number) { 
        this._itemHeight = value 
        setTimeout(() => (this.itemHeightChanged as Subject<boolean>).next(true))
        console.log("New itemHeight:", this.itemHeight)
    }
    private _itemHeight = 0

    testItemHeight = 0

    private app: HTMLElement
}

declare var themes: IThemes
