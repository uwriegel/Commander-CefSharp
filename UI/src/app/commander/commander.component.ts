import { Component, ViewChild, OnInit, NgZone, HostListener, AfterViewInit, Input, ElementRef } from '@angular/core'
import { CommanderViewComponent } from '../commander-view/commander-view.component'
import { DialogComponent } from '../dialog/dialog.component'
import { IProcessor } from '../interfaces/commander-view'
import { ICommander } from '../interfaces/commander'
import { ViewerComponent } from '../viewers/viewer/viewer.component';

@Component({
    selector: 'app-commander',
    templateUrl: './commander.component.html',
    styleUrls: ['./commander.component.css']
})
export class CommanderComponent implements OnInit, AfterViewInit, ICommander {

    @ViewChild("leftView") leftView: CommanderViewComponent
    @ViewChild("rightView") rightView: CommanderViewComponent
    @ViewChild("viewer") viewer: ViewerComponent
    @ViewChild("status") status: ElementRef

    @Input() 
    dialog: DialogComponent

    viewerRatio = 0

    focusedView: CommanderViewComponent

    isViewVisible = false

    commanderViewLeft = CommanderLeft
    commanderViewRight = CommanderRight

    setViewer(on: boolean) {
        this.zone.run(() => this.isViewVisible = on)
    }

    onResize() {
        this.viewerRatio = (this.viewer.appElement.nativeElement as HTMLElement).clientHeight / document.body.clientHeight
    }

    constructor(private zone: NgZone) { commander = this }

    ngOnInit() { }

    ngAfterViewInit() { 
        setTimeout(() => this.leftView.focus()) 
        this.viewer.statusRatio = (this.status.nativeElement as HTMLElement).clientHeight / document.body.clientHeight
    }

    @HostListener('keydown', ['$event']) 
    private onKeydown(evt: KeyboardEvent) {
        switch (evt.which) {
            case 9: // tab
                if (!evt.shiftKey) {
                    if (this.focusedView == this.leftView) 
                        this.rightView.focus()
                    else
                        this.leftView.focus()
                } 
                evt.stopPropagation()
                evt.preventDefault()
                break
        }
    }

    gotFocus(view: CommanderViewComponent) { 
        this.focusedView = view 
        console.log(this.focusedView.id)
    }

    onRatioChanged() {
        this.leftView.onResize()
        this.rightView.onResize()
        this.viewerRatio = (this.viewer.appElement.nativeElement as HTMLElement).clientHeight / document.body.clientHeight
    }
}

declare var CommanderLeft : IProcessor
declare var CommanderRight : IProcessor
declare var commander : ICommander
