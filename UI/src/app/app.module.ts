import { BrowserModule } from '@angular/platform-browser'
import { BrowserAnimationsModule } from '@angular/platform-browser/animations'
import { NgModule } from '@angular/core'

import { AppComponent } from './app.component'
import { ScrollbarComponent as TestScrollbarComponent } from './test/scrollbar/scrollbar.component'
import { ScrollbarComponent } from './scrollbar/scrollbar.component'
import { TestColumnsComponent } from './test/columns/columns.component'
import { ColumnsComponent } from './columns/columns.component'
import { IconViewComponent } from './test/icon-view/icon-view.component'
import { TableViewComponent as TestTableViewComponent } from './test/table-view/table-view.component'
import { TableViewComponent } from './table-view/table-view.component'
import { TableViewItemComponent } from './datatemplates/table-view-item/table-view-item.component'
import { CommanderViewComponent } from './commander-view/commander-view.component'
import { CommanderViewComponent as TestCommanderViewComponent } from './test/commander-view/commander-view.component'
import { FolderComponent } from './svgs/folder/folder.component'
import { DriveComponent } from './svgs/drive/drive.component'
import { NetworkdriveComponent } from './svgs/networkdrive/networkdrive.component'
import { CdromComponent } from './svgs/cdrom/cdrom.component'
import { UsbComponent } from './svgs/usb/usb.component'
import { VirtualListPipe } from './pipes/virtual-list.pipe'
import { ClipHeightPipe } from './pipes/clip-height.pipe'
import { RestrictorComponent as TestRestrictor} from './test/restrictor/restrictor.component'
import { GridComponent } from './test/grid/grid.component'
import { GridSplitterComponent } from './grid-splitter/grid-splitter.component'
import { CommanderComponent } from './commander/commander.component'
import { DialogComponent as TestDialogComponent } from './test/dialog/dialog.component'
import { DialogComponent } from './dialog/dialog.component'
import { SelectAllDirective } from './directives/select-all.directive'
import { DefaultButtonDirective } from './directives/default-button.directive'
import { ConnectionComponent } from './test/connection/connection.component'
import { TableViewTestItemComponent } from './test/table-view-test-item/table-view-test-item.component';
import { ImageViewerComponent } from './viewers/image-viewer/image-viewer.component';
import { ViewerComponent } from './viewers/viewer/viewer.component';
import { FrameViewerComponent } from './viewers/frame-viewer/frame-viewer.component';
import { SanitizePipe } from './pipes/sanitize.pipe'

@NgModule({
    declarations: [
        AppComponent,
        TestScrollbarComponent,
        TestColumnsComponent,
        ScrollbarComponent,
        ColumnsComponent,
        IconViewComponent,
        TestTableViewComponent,
        TableViewComponent,
        TableViewItemComponent,
        CommanderViewComponent,
        TestCommanderViewComponent,
        FolderComponent,
        DriveComponent,
        NetworkdriveComponent,
        CdromComponent,
        UsbComponent,
        VirtualListPipe,
        ClipHeightPipe,
        TestRestrictor,
        GridComponent,
        GridSplitterComponent,
        CommanderComponent,
        TestDialogComponent,
        DialogComponent,
        SelectAllDirective,
        DefaultButtonDirective,
        ConnectionComponent,
        TableViewTestItemComponent,
        ImageViewerComponent,
        ViewerComponent,
        FrameViewerComponent,
        SanitizePipe,
    ],
    imports: [
        BrowserModule,
        BrowserAnimationsModule
    ],
    providers: [],
    bootstrap: [AppComponent]
})
export class AppModule { }
