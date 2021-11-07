import { Component, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { ThemeProviderService } from 'src/app/services/themeProvider.service';

@Component({
  selector: 'app-delete-confirm',
  templateUrl: './delete-confirm.component.html',
  styleUrls: ['./delete-confirm.component.css']
})
export class DeleteConfirmComponent implements OnInit {

  mode: string;
  subs= new Subscription()
  constructor(private tp: ThemeProviderService) { }
  ngOnDestroy(): void {
   this.subs.unsubscribe()
  }

  ngOnInit() {
    this.subs.add(this.tp.getTheme().subscribe(t => this.mode = t));
  }
}
