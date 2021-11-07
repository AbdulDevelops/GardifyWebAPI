import { Component, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { UtilityService } from 'src/app/services/utility.service';
import { ThemeProviderService } from 'src/app/services/themeProvider.service';
import { Ad } from 'src/app/models/models';

@Component({
  selector: 'app-my-posts',
  templateUrl: './my-posts.component.html',
  styleUrls: ['./my-posts.component.css']
})
export class MyPostsComponent implements OnInit {
  subs = new Subscription();
  currentUserPosts: any;
  mode:string;
  selectedPost:any
  postItem: any;
  adBetween:Ad;
  constructor(private util:UtilityService, private tp:ThemeProviderService) { 
    this.adBetween = new Ad(
      'ca-pub-3562132666777902',
      3041043183,
      'fluid',
      null,
      false,
      "-fb+5w+4e-db+86"
    ) 
  }

  ngOnInit(): void {
    this.subs.add(this.tp.getTheme().subscribe(t=>this.mode=t))
    this.getCurrrentUserPosts()
  }
  getCurrrentUserPosts(){
    this.subs.add(this.util.getCurrentUserPost().subscribe(p=>{
      this.currentUserPosts=p
    }))
  }
  updatePost(post:any){
    if(post!=null){
      this.util.updatePost(post.QuestionId, post).subscribe()
    }
  }
  getPostById(id:number){
    this.subs.add(this.util.getPostById(id).subscribe(data=>this.postItem=data))
  }
  toUrl(url: string) {
    return this.util.toUrl(url);
  }
}
