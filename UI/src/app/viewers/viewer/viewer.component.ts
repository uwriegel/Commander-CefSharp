import { Component, OnInit, Input, ElementRef } from '@angular/core'
import { IViewer } from 'src/app/interfaces/commander'

@Component({
    selector: 'app-viewer',
    templateUrl: './viewer.component.html',
    styleUrls: ['./viewer.component.css']
})
export class ViewerComponent implements OnInit {

    isImage = false
    isFrame = false
    file = ""

    @Input()
    set isViewVisible(value: boolean) {
        this._isViewVisible = value
        this.setItem()
    }
    get isViewVisible() { return this._isViewVisible}
    private _isViewVisible = false
       
    @Input()
    set item(value: string) {
        this._item = value
        this.setItem()
    }
    get item() { return this._item }
    private _item = ""

    @Input()
    set statusRatio(ratio: number) { Viewer.setStatusRatio(ratio) }

    @Input()
    set viewerRatio(ratio: number) {
        if (ratio)
            Viewer.setViewerRatio(ratio)
    }
    
    constructor(public appElement: ElementRef) { }

    ngOnInit() { }

    private setItem() {
        if (this.item && this.isViewVisible) {
            this.isImage = this.item.toLowerCase().endsWith(".jpg")
                || this.item.toLowerCase().endsWith(".png")
                || this.item.toLowerCase().endsWith(".jpeg")


            // TODO: PDF test in another browser
            this.isFrame = this.item.toLowerCase().endsWith(".pdf")

            // TODO: not shown when serving with Angular
            this.file = "file?path=" + this.item
            //this.file = "file:///" + this.item

            // TODO: PDF test in another browser
            //if (this.item.toLowerCase().endsWith(".pdf"))
            //    Viewer.setFile(this.item)
            //else
            //    Viewer.setFile(null)
        }
        else {
            this.isImage = null
            this.isFrame = null
            this.file = null
        }
    }
}

declare var Viewer : IViewer
