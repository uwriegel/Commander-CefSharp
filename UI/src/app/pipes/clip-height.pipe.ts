import { Pipe, PipeTransform } from '@angular/core'
import { DomSanitizer } from '@angular/platform-browser'

@Pipe({
    name: 'clipHeight'
})
export class ClipHeightPipe implements PipeTransform {

    constructor(private sanitizer: DomSanitizer) {}

    transform(height: number) {
        return this.sanitizer.bypassSecurityTrustStyle(`rect(0px, auto, ${height}px, 0px)`)
    }
}
