import { Component, } from '@angular/core'

@Component({
  selector: 'test-grid',
  templateUrl: './grid.component.html',
  styleUrls: ['./grid.component.css']
})
export class GridComponent {

    isLastVisible = true

    onClick() {
        this.isLastVisible = !this.isLastVisible
    }

    onRatioChanged() {
        console.log("onRatioChanged")
    }
}
