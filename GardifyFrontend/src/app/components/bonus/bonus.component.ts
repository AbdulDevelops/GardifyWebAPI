import { Component, OnInit } from '@angular/core';
import { UtilityService } from 'src/app/services/utility.service';
import { ThemeProviderService } from 'src/app/services/themeProvider.service';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-bonus',
  templateUrl: './bonus.component.html',
  styleUrls: ['./bonus.component.css']
})
export class BonusComponent implements OnInit {
  userId: string;
  mode: string; earnedPts = []; spentPts = [];

  constructor(private util: UtilityService, private auth: AuthService, private tp: ThemeProviderService) { }

  ngOnInit() {
    this.tp.getTheme().subscribe(t => this.mode = t);
    this.userId = this.auth.getCurrentUser().UserId;
    this.util.getEarnedPoints(this.userId).subscribe( p => this.earnedPts = p );
    this.util.getspentPoints(this.userId).subscribe( p => this.spentPts = p );
  }

  get totalEarned() { return this.earnedPts.reduce((acc, g) => acc + g.Points, 0) || 0 }
  get totalSpent() { return this.spentPts.reduce((acc, g) => acc + g.Points, 0) || 0 }
}
