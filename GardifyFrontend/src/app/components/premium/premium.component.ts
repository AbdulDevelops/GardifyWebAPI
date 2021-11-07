import { Component, OnInit, OnDestroy } from '@angular/core';
import { UtilityService } from 'src/app/services/utility.service';
import { ThemeProviderService } from 'src/app/services/themeProvider.service';
import { AuthService } from 'src/app/services/auth.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-premium',
  templateUrl: './premium.component.html',
  styleUrls: ['./premium.component.css']
})
export class PremiumComponent implements OnInit,OnDestroy {
  userId: string;
  mode: string; points: number;
  subs= new Subscription()
  constructor(private util: UtilityService, private tp: ThemeProviderService, private auth: AuthService) { }
  ngOnDestroy(): void {
    this.subs.unsubscribe()
  }

  ngOnInit() {
    this.userId = this.auth.getCurrentUser().UserId;
   this.subs.add(this.tp.getTheme().subscribe(t => this.mode = t));
  }

}
