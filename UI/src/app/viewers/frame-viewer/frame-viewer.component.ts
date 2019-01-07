import { Component, OnInit, Input, ElementRef, ViewChild } from '@angular/core'

@Component({
    selector: 'app-frame-viewer',
    templateUrl: './frame-viewer.component.html',
    styleUrls: ['./frame-viewer.component.css']
})
export class FrameViewerComponent implements OnInit {

    @ViewChild("frame")
    frame: ElementRef

    @Input()
    set file(value: string) {
        (this.frame.nativeElement as HTMLFrameElement).src = value
    }

    constructor() { }

    ngOnInit() { }
}
