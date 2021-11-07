import { BrowserModule } from '@angular/platform-browser';
import { NgModule, LOCALE_ID } from '@angular/core';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { AppRoutingModule } from './app-routing/app-routing.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { Ng5SliderModule } from 'ng5-slider';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { NgMultiSelectDropDownModule } from 'ng-multiselect-dropdown';
import { AppComponent } from './app.component';
import {ProgressBarModule} from "angular-progress-bar";
import { ChartsModule } from 'ng2-charts';
import { MatSliderModule } from '@angular/material/slider';
import {MatTooltipModule} from '@angular/material/tooltip'; 
import {DragDropModule} from '@angular/cdk/drag-drop';
import { MatDividerModule } from '@angular/material/divider';
import{MatDatepickerInput, MatDatepickerModule} from '@angular/material/datepicker';
import{MatInputModule} from '@angular/material/input'
import {
    HeaderComponent, FooterComponent, HomepageComponent, MainCarouselComponent,  PlantsDocComponent,
    TodoPageComponent, WarnungenComponent, ShopComponent, PlantsSearchComponent, NewslistComponent,
    NewspageComponent, PageNotFoundComponent, PostViewComponent, 
    ForecastComponent, LoginComponent,ScannerComponent,CartComponent,ArticleDetailsComponent,NewsletterComponent,PlantsDocDetailsComponent
    ,PlantSearchDetailsComponent, PremiumComponent, ScansHistoryComponent, BonusComponent,MygardenDetailsComponent,
} from './components/';
import { HttpMainInterceptor } from './interceptor/http-main.interceptor';
import { AuthGuard } from './guards/auth.guard';

import { CalendarModule, DateAdapter } from 'angular-calendar';
import { adapterFactory } from 'angular-calendar/date-adapters/date-fns';
import { registerLocaleData } from '@angular/common';
import localeDe from '@angular/common/locales/de';

import { GardeningAZComponent } from './components/gardening-az/gardening-az.component';
import { ProfileComponent } from './components/profile/profile.component';
import { SearchArticlePipe } from './pipes/search-article.pipe';
import { MyDevicesComponent } from './components/my-devices/my-devices.component';
import { ScansResultsComponent } from './components/scans-results/scans-results.component';
import { RegisterComponent } from './components/register/register.component';
import { AuthService } from './services/auth.service';

import { SettingsComponent } from './components/settings/settings.component';
import { UnauthorizedComponent } from './components/unauthorized/unauthorized.component';
import { AlertComponent } from './components/alert/alert.component';
import { AlertService } from './services/alert.service';
import { AddNewTodoComponent } from './components/add-new-todo/add-new-todo.component';
import { AddNewDiaryComponent } from './components/add-new-diary/add-new-diary.component';
import { ResetPasswordComponent } from './components/reset-password/reset-password.component';
import { TodoDetailsComponent } from './components/todo-details/todo-details.component';
import { EditTodoComponent } from './components/edit-todo/edit-todo.component';
import { EditDiaryComponent } from './components/edit-diary/edit-diary.component';
import { ServiceWorkerModule } from '@angular/service-worker';
import { environment } from '../environments/environment';
import { ContactComponent } from './components/contact/contact.component';
import { CheckoutComponent } from './components/checkout/checkout.component';
import { EcolistComponent } from './components/ecolist/ecolist.component';
import { AgbComponent } from './components/agb/agb.component';
import { PrivacyPolicyComponent } from './components/privacy-policy/privacy-policy.component';
import { ImpressumComponent } from './components/impressum/impressum.component';
import { BonusInfoComponent } from './components/bonus-info/bonus-info.component';
import { VideosComponent } from './components/videos/videos.component';
import { GlossarComponent } from './components/glossar/glossar.component';
import { FaqComponent } from './components/faq/faq.component';
import { NewThemaComponent } from './components/new-thema/new-thema.component';
import { OrdersComponent } from './components/orders/orders.component';
import { PurchaseComponent } from './components/purchase/purchase.component';
import { PurchaseSuccessComponent } from './components/purchase-success/purchase-success.component';
import { PurchaseFailComponent } from './components/purchase-fail/purchase-fail.component';
import { NewsletterConfirmComponent } from './components/newsletter-confirm/newsletter-confirm.component';
import { QuickStartComponent } from './components/quick-start/quick-start.component';
import { CookieConsentComponent } from './components/cookie-consent/cookie-consent.component';
import { StatisticsComponent } from './components/statistics/statistics.component';
import { EcoElementDetailComponent } from './components/eco-element-detail/eco-element-detail.component';
import { MyPostsComponent } from './components/my-posts/my-posts.component';
import { CancellationComponent } from './components/cancellation/cancellation.component';
import { DeleteConfirmComponent } from './components/delete-confirm/delete-confirm.component';
import { OekoscanComponent } from './components/oekoscan/oekoscan.component';
import { OekoscanResultComponent } from './components/oekoscan-result/oekoscan-result.component';
import { PageNotReadyComponent } from './components/page-not-ready/page-not-ready.component';
import { TourComponent } from './components/tour/tour.component';
import { AppPasswordDirective } from './directives/app-password.directive';
import { LatestPlantsComponent } from './components/latest-plants/latest-plants.component';
import { IfRolesDirective } from './directives/if-roles.directive';
import { CookieService } from 'ngx-cookie-service';
import { AdComponent } from './components/ad/ad.component';
import { TeamComponent } from './components/team/team.component';
import { SafeHtmlPipe } from './pipes/SafeHtmlPipe';
import { SocialComponent } from './components/social/social.component';
import {QuillModule} from 'ngx-quill';
import { GardenarchivComponent } from './components/gardenarchiv/gardenarchiv.component';
import { UploadGartenArchivFotoComponent } from './components/upload-garten-archiv-foto/upload-garten-archiv-foto.component'
import { MatNativeDateModule } from '@angular/material/core';
import { MatFormFieldModule, MatLabel } from '@angular/material/form-field';
import{ MatRadioModule}from '@angular/material/radio';
import { ImageOverviewComponent } from './components/image-overview/image-overview.component';
import { NewPresentationComponent } from './components/new-presentation/new-presentation.component'
import {MatSlideToggleModule} from '@angular/material/slide-toggle';
import {MatTableModule} from '@angular/material/table';
import {MatListModule} from '@angular/material/list';
import { MoveplantToListComponent } from './components/moveplant-to-list/moveplant-to-list.component';
import { ImagesPresentationComponent } from './components/images-presentation/images-presentation.component';
import { CommunityPageNewComponent } from './components/community-page-new/community-page-new.component';
import { CommunityHeaderSearchComponent } from './components/community-page-new/community-header-search/community-header-search.component';
import { CommunityPicturesComponent } from './components/community-page-new/community-pictures/community-pictures.component';
import { CommunityGardenComponent } from './components/community-page-new/community-garden/community-garden.component';
import { FindMemberComponent } from './components/find-member/find-member.component';
import { EditAbumComponent } from './components/edit-abum/edit-abum.component';
import { AskQuestionComponent } from './components/community-page-new/ask-question/ask-question.component';
import { FilterOptionsComponent } from './components/filter-options/filter-options.component';
import { SearchInGardenArchivComponent } from './components/search-in-garden-archiv/search-in-garden-archiv.component';
import { HeaderGardenArchivComponent } from './components/header-garden-archiv/header-garden-archiv.component';
import { OtherPresentationComponent } from './components/other-presentation/other-presentation.component';
import { MessageListComponent } from './components/community-page-new/message-list/message-list.component';
registerLocaleData(localeDe);

@NgModule({
  declarations: [
    AppComponent,
    HeaderComponent,
    FooterComponent,
    HomepageComponent,
    MainCarouselComponent,
    TodoPageComponent,
    WarnungenComponent,
    ShopComponent,
    PlantsSearchComponent,
    NewslistComponent,
    NewspageComponent,
    PageNotFoundComponent,
    PostViewComponent,
    PlantsDocComponent,
    ForecastComponent,
    LoginComponent,
    ScannerComponent,
    CartComponent,
    ArticleDetailsComponent,
    NewsletterComponent,
    ScansHistoryComponent,
    PlantsDocDetailsComponent,
    PlantSearchDetailsComponent,
    PremiumComponent,
    BonusComponent,
    GardeningAZComponent,
    ProfileComponent,
    SearchArticlePipe,
    MyDevicesComponent,
    ScansResultsComponent,
    RegisterComponent,
    SettingsComponent,
    UnauthorizedComponent,
    MygardenDetailsComponent,
    AlertComponent,
    AddNewTodoComponent,
    AddNewDiaryComponent,
    ResetPasswordComponent,
    TodoDetailsComponent,
    EditTodoComponent,
    EditDiaryComponent,
    ContactComponent,
    CheckoutComponent,
    EcolistComponent,
    AgbComponent,
    PrivacyPolicyComponent,
    BonusInfoComponent,
    ImpressumComponent,
    VideosComponent,
    GlossarComponent,
    FaqComponent,
    NewThemaComponent,
    OrdersComponent,
    PurchaseComponent,
    PurchaseSuccessComponent,
    PurchaseFailComponent,
    NewsletterConfirmComponent,
    QuickStartComponent,
    CookieConsentComponent,
    StatisticsComponent,
    EcoElementDetailComponent,
    MyPostsComponent,
    CancellationComponent,
    DeleteConfirmComponent,
    OekoscanComponent,
    OekoscanResultComponent,
    PageNotReadyComponent,
    TourComponent,
    AppPasswordDirective,
    LatestPlantsComponent,
    IfRolesDirective,
    AdComponent,
    TeamComponent,
    SocialComponent,
    SafeHtmlPipe,
    GardenarchivComponent,
    UploadGartenArchivFotoComponent,
    ImageOverviewComponent,
    NewPresentationComponent,
    MoveplantToListComponent,
    ImagesPresentationComponent,
    CommunityPageNewComponent,
    CommunityHeaderSearchComponent,
    CommunityGardenComponent,
    CommunityPicturesComponent,
    FindMemberComponent,
    EditAbumComponent,
    AskQuestionComponent,
    FilterOptionsComponent,
    SearchInGardenArchivComponent,
    HeaderGardenArchivComponent,
    OtherPresentationComponent,
    MessageListComponent,
  ],
  imports: [
    NgMultiSelectDropDownModule.forRoot(),
    BrowserModule,
    HttpClientModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    Ng5SliderModule,
    ProgressBarModule,
    FontAwesomeModule,
    ChartsModule,
    MatSliderModule,
    MatTooltipModule,
    DragDropModule,
    FormsModule, ReactiveFormsModule,
    MatDividerModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatFormFieldModule,
    MatInputModule,
    MatRadioModule,
    MatSlideToggleModule,
    MatTableModule,
    MatListModule,
    ServiceWorkerModule.register('/ngsw-worker.js', { enabled: environment.production }),
    CalendarModule.forRoot({
      provide: DateAdapter,
      useFactory: adapterFactory
    }),
    QuillModule.forRoot()
  ],
  providers: [
    AuthGuard, AlertService,
    { provide: HTTP_INTERCEPTORS, useClass: HttpMainInterceptor, multi: true },
    AuthService, CookieService,
    { provide: LOCALE_ID, useValue: 'de' },
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
