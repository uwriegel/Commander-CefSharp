import { Component, OnInit, Input, ElementRef } from '@angular/core'
import { IProgram } from 'src/app/interfaces/commander'

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
    set item(item: string) {
        if (item) {
            this.isImage = item.toLowerCase().endsWith(".jpg") || item.toLowerCase().endsWith(".png") || item.toLowerCase().endsWith(".jpeg")
            //this.isFrame = item.toLowerCase().endsWith(".pdf") 
            this.file = "file?path=" + item
            if (item.toLowerCase().endsWith(".pdf"))
                Program.setFile(item)
            else
                Program.setFile(null)
        }
    }

    @Input()
    set statusRatio(ratio: number) { Program.setStatusRatio(ratio) }

    @Input()
    set viewerRatio(ratio: number) {
        if (ratio)
            Program.setViewerRatio(ratio)
    }
    
    constructor(public appElement: ElementRef) { }

    ngOnInit() { }

}

declare var Program : IProgram