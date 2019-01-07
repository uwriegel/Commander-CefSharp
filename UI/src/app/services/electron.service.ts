import { Injectable, NgZone, ElementRef } from '@angular/core'
import { ThemesService } from './themes.service';
import { Subject, Observable } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class ElectronService {

    readonly themeChanged: Observable<string> = new Subject<string>()
    readonly showHiddenChanged: Observable<boolean> = new Subject<boolean>()
    readonly onCreateFolder: Observable<void> = new Subject<void>()

    showHidden = false

    constructor(private themes: ThemesService, private zone: NgZone) {
        // ipcRenderer.on("setTheme", (_: any, theme: string) => {
        //     this.zone.run(() => {
        //         (this.themeChanged as Subject<string>).next(theme)
        //     })
        // })
        // ipcRenderer.on("showHidden", (_: any, show: boolean) => {
        //     this.showHidden = show
        //     this.zone.run(() => {
        //          (this.showHiddenChanged as Subject<boolean>).next(show)
        //     })
        // })
        // ipcRenderer.on("onCreateFolder", () => {
        //     this.zone.run(() => {
        //          (this.onCreateFolder as Subject<void>).next()
        //     })
        // })
        // ipcRenderer.send("initialized")
    }   
}
