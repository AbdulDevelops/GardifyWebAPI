import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import {
  HomepageComponent, TodoPageComponent, PlantsSearchComponent, AddNewDiaryComponent, AddNewTodoComponent,
  WarnungenComponent, ShopComponent, NewslistComponent, NewspageComponent, PageNotFoundComponent,
  PostViewComponent, ScannerComponent, ScansHistoryComponent,
  PlantsDocComponent, ForecastComponent, LoginComponent, CartComponent, ArticleDetailsComponent, NewsletterComponent, PlantsDocDetailsComponent,
  PlantSearchDetailsComponent, PremiumComponent, BonusComponent, GardeningAZComponent, ProfileComponent, MyDevicesComponent,
  RegisterComponent, SettingsComponent, UnauthorizedComponent, MygardenDetailsComponent, EcoElementDetailComponent, MyPostsComponent
} from '../components/';
import { ReactiveFormsModule } from '@angular/forms';
import { ScansResultsComponent } from '../components/scans-results/scans-results.component';
import { AuthGuard } from '../guards/auth.guard';
import { DemoGuard } from '../guards/demo.guard';
import { RolesGuard } from '../guards/roles.guard';
import { ResetPasswordComponent } from '../components/reset-password/reset-password.component';
import { TodoDetailsComponent } from '../components/todo-details/todo-details.component';
import { EditTodoComponent } from '../components/edit-todo/edit-todo.component';
import { EditDiaryComponent } from '../components/edit-diary/edit-diary.component';
import { ContactComponent } from '../components/contact/contact.component';
import { CheckoutComponent } from '../components/checkout/checkout.component';
import { EcolistComponent } from '../components/ecolist/ecolist.component';
import { AgbComponent } from '../components/agb/agb.component';
import { PrivacyPolicyComponent } from '../components/privacy-policy/privacy-policy.component';
import { ImpressumComponent } from '../components/impressum/impressum.component';
import { BonusInfoComponent } from '../components/bonus-info/bonus-info.component';
import { VideosComponent } from '../components/videos/videos.component';
import { FaqComponent } from '../components/faq/faq.component';
import { GlossarComponent } from '../components/glossar/glossar.component';
import { NewThemaComponent } from '../components/new-thema/new-thema.component';
import { OrdersComponent } from '../components/orders/orders.component';
import { PurchaseComponent } from '../components/purchase/purchase.component';
import { PurchaseFailComponent } from '../components/purchase-fail/purchase-fail.component';
import { PurchaseSuccessComponent } from '../components/purchase-success/purchase-success.component';
import { NewsletterConfirmComponent } from '../components/newsletter-confirm/newsletter-confirm.component';
import { QuickStartComponent } from '../components/quick-start/quick-start.component';
import { CancellationComponent } from '../components/cancellation/cancellation.component';
import { DeleteConfirmComponent } from '../components/delete-confirm/delete-confirm.component'
import { OekoscanComponent } from '../components/oekoscan/oekoscan.component'
import { OekoscanResultComponent } from '../components/oekoscan-result/oekoscan-result.component'
import { PageNotReadyComponent } from '../components/page-not-ready/page-not-ready.component'
import { LatestPlantsComponent } from '../components/latest-plants/latest-plants.component';
import { Role } from '../models/roles';
import { TeamComponent } from '../components/team/team.component'
import { SocialComponent } from '../components/social/social.component';
import { GardenarchivComponent } from '../components/gardenarchiv/gardenarchiv.component';
import { UploadGartenArchivFotoComponent } from '../components/upload-garten-archiv-foto/upload-garten-archiv-foto.component';
import { ImageOverviewComponent } from '../components/image-overview/image-overview.component';
import { NewPresentationComponent } from '../components/new-presentation/new-presentation.component';
import { ImagesPresentationComponent } from '../components/images-presentation/images-presentation.component';
import { CommunityPageNewComponent } from '../components/community-page-new/community-page-new.component';
import { EditAbumComponent } from '../components/edit-abum/edit-abum.component';
import { FindMemberComponent } from '../components/find-member/find-member.component';
import { AskQuestionComponent } from '../components/community-page-new/ask-question/ask-question.component';
import { CommunityGardenComponent } from '../components/community-page-new/community-garden/community-garden.component';
import { CommunityPicturesComponent } from '../components/community-page-new/community-pictures/community-pictures.component';
import { SearchInGardenArchivComponent } from '../components/search-in-garden-archiv/search-in-garden-archiv.component';
import { OtherPresentationComponent } from '../components/other-presentation/other-presentation.component';
import { MessageListComponent } from '../components/community-page-new/message-list/message-list.component';

const routes: Routes = [
  { path: '', component: HomepageComponent },
  { path: 'home/:registermode', component: HomepageComponent },
  { path: 'home', component: HomepageComponent },
  { path: 'start', component: QuickStartComponent },
  { path: 'kontakt', component: ContactComponent },
  { path: 'todo', component: TodoPageComponent, canActivate: [AuthGuard] },

  { path: 'todo/:year/:month', component: TodoPageComponent, canActivate: [AuthGuard] },
  { path: 'todo/:id', component: TodoDetailsComponent, canActivate: [AuthGuard] },
  { path: 'edit/todo/:id', component: EditTodoComponent, canActivate: [AuthGuard] },
  { path: 'edit/diary/:id', component: EditDiaryComponent, canActivate: [AuthGuard] },
  { path: 'newdiary', component: AddNewDiaryComponent, canActivate: [AuthGuard] },
  { path: 'newtodo', component: AddNewTodoComponent, canActivate: [AuthGuard] },
  { path: 'newtodo/:id', component: AddNewTodoComponent, canActivate: [AuthGuard] },
  { path: 'warnungen', component: WarnungenComponent, canActivate: [AuthGuard] },
  { path: 'shop', component: ShopComponent },
  { path: 'suche', component: PlantsSearchComponent },
  { path: 'neue-pflanzen', component: LatestPlantsComponent },
  { path: 'news', component: NewslistComponent },
  { path: 'news/:id/:name', component: NewspageComponent },
  { path: 'thread/:id', component: PostViewComponent, canActivate: [AuthGuard] },
  { path: 'datenschutz', component: PrivacyPolicyComponent },
  {
    path: 'pflanzendoc', component: PlantsDocComponent, canActivate: [AuthGuard],
    children: [
      { path: '', redirectTo: 'pflanzendoc', pathMatch: 'full' },
      { path: 'my-posts', component: MyPostsComponent },
    ]
  },
  { path: 'pflanzendoc/:newPost', component: PlantsDocComponent, canActivate: [AuthGuard] },
  { path: 'wetter', component: ForecastComponent, canActivate: [AuthGuard] },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'social/register/danke', component: RegisterComponent },
  { path: 'gartenflora/register/danke', component: RegisterComponent },

  { path: 'register/danke', component: RegisterComponent },

  { path: 'register/:testen', component: RegisterComponent },
  { path: 'scanner', component: ScannerComponent },
  { path: 'scanresults', component: ScansResultsComponent },
  { path: 'scanverlauf', component: ScansHistoryComponent, canActivate: [AuthGuard] },
  { path: 'warenkorb', component: CartComponent, canActivate: [AuthGuard] },
  { path: 'kasse', component: CheckoutComponent, canActivate: [AuthGuard] },
  { path: 'bestellung/fehler/:err/:id', component: PurchaseFailComponent, canActivate: [AuthGuard] },
  { path: 'bestellung/erfolgreich/:id', component: PurchaseSuccessComponent, canActivate: [AuthGuard] },
  { path: 'bestellung/:id', component: PurchaseComponent, canActivate: [AuthGuard] },
  { path: 'artikel/:id/:test', component: ArticleDetailsComponent },
  { path: 'newsletter', component: NewsletterComponent, canActivate: [AuthGuard] },
  { path: 'newsletter-bestaetigt', component: NewsletterConfirmComponent },
  { path: 'thread', component: PlantsDocDetailsComponent, canActivate: [AuthGuard] },
  { path: 'pflanze/:id/:name', component: PlantSearchDetailsComponent },
  { path: 'premium', component: PremiumComponent, canActivate: [AuthGuard] },
  { path: 'profil', component: ProfileComponent, canActivate: [AuthGuard] },
  //{ path: 'bonus', component: BonusComponent, canActivate: [AuthGuard]},
  { path: 'info', component: BonusInfoComponent },
  { path: 'agb', component: AgbComponent },
  { path: 'widerruf', component: CancellationComponent },
  { path: 'vonAbisZ', component: GardeningAZComponent },
  { path: 'impressum', component: ImpressumComponent },
  { path: 'neuesthema', component: NewThemaComponent, canActivate: [DemoGuard] },
  { path: 'einstellungen', component: SettingsComponent, canActivate: [AuthGuard] },
  {
    path: 'meingarten', component: MygardenDetailsComponent, canActivate: [AuthGuard],
    children: [
      { path: 'ecoelement/:name', component: EcoElementDetailComponent },
      { path: '', redirectTo: 'meingarten', pathMatch: 'full' },
      { path: 'ecolist', component: EcolistComponent },
      { path: 'myDevices/:id', component: MyDevicesComponent },
    ]
  },
  {
    path: 'demo/meingarten', component: MygardenDetailsComponent, canActivate: [AuthGuard],
  },
  { path: 'meingarten/:id/:name', component: MygardenDetailsComponent, canActivate: [AuthGuard] },
  { path: 'new', component: UnauthorizedComponent },
  { path: 'resetpassword', component: ResetPasswordComponent },
  { path: 'videos', component: VideosComponent, canActivate: [AuthGuard] },
  { path: 'faq', component: FaqComponent },
  { path: 'glossar', component: GlossarComponent },
  { path: 'bestellungen', component: OrdersComponent, canActivate: [AuthGuard] },
  { path: 'oekoscan', component: OekoscanComponent, canActivate: [AuthGuard] },
  { path: 'oekoscan-result', component: OekoscanResultComponent },
  { path: 'deletionconfirmation', component: DeleteConfirmComponent },
  { path: 'not-ready', component: PageNotReadyComponent },
  { path: 'team', component: TeamComponent },
  { path: 'social', component: SocialComponent },
  { path: 'gartenflora', component: SocialComponent },
  {
    path: 'gartenarchiv', component: GardenarchivComponent, canActivate: [AuthGuard],
    children: [
      { path: 'uploadfoto', component: UploadGartenArchivFotoComponent },
      { path: 'image/detail/:id', component: ImageOverviewComponent },
      { path: 'new/presentation', component: NewPresentationComponent },
      { path: 'images/presentation', component: ImagesPresentationComponent },
      { path: 'images/edit-album/:id', component: EditAbumComponent },
      
    ]
  },
  { path: 'images/search', component: SearchInGardenArchivComponent,canActivate: [AuthGuard] },
  { path: 'images/carousel', component: OtherPresentationComponent,canActivate: [AuthGuard] },
   {
    path: 'community', component: CommunityPageNewComponent, canActivate: [AuthGuard],
    children: [
      {path: '', redirectTo: 'communityGarden', pathMatch: 'full'},
      { path: 'communityGarden', component: CommunityGardenComponent },
      { path: 'communityPicture', component: CommunityPicturesComponent },
      { path: 'askQuestion', component: AskQuestionComponent },
      { path: 'messageList', component: MessageListComponent },

    ]
  },
  { path: '**', component: PageNotFoundComponent },

];

@NgModule({
  imports: [RouterModule.forRoot(routes),
    ReactiveFormsModule],
  exports: [RouterModule]
})
export class AppRoutingModule { }
