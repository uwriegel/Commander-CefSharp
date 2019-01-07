import { Component, OnInit, Input } from '@angular/core'

@Component({
    selector: 'app-frame-viewer',
    templateUrl: './frame-viewer.component.html',
    styleUrls: ['./frame-viewer.component.css']
})
export class FrameViewerComponent implements OnInit {

    @Input()
    file: string

    constructor() { }

    ngOnInit() { }
}
