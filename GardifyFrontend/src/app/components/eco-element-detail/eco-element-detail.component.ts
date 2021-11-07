import { Component, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { ThemeProviderService } from 'src/app/services/themeProvider.service';
import { ActivatedRoute } from '@angular/router';
@Component({
  selector: 'app-eco-element-detail',
  templateUrl: './eco-element-detail.component.html',
  styleUrls: ['./eco-element-detail.component.css']
})
export class EcoElementDetailComponent implements OnInit {
  subs=new Subscription();
  mode:string;
  ecoName: any;
  constructor(private tp:ThemeProviderService, private activatedRoute:ActivatedRoute) 
  {
    this.subs.add(this.activatedRoute.params.subscribe(params =>{
      this.ecoName= params.name
      }
      ))
   }

  ngOnInit(): void {
    this.subs.add(this.tp.getTheme().subscribe(t => this.mode = t));
  }

}
