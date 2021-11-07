class BaseModel {
    Id?: number;
    createdBy?: string; // not optional in prod
    createdDate?: any;
    editedDate?: any;
    editedBy?: string;
    Deleted?: boolean;
}

export class Todo extends BaseModel {
    Description: string;
    Title: string;
    DateStart: any;
    DateEnd: any;
    CyclicId?:number;
    Finished?: boolean;
    Ignored?: boolean;
    ReferenceId?:number;
    ReferenceType: number;
    Precision: number;
    Notes?: string;
    EntryImages?: any[];
    RelatedPlantName:string;
    RelatedPlantId:number;
}
export class Post extends BaseModel{
    
AdminAllowsPublishment?: false
Description: string;
Headline?: string;
Images?: any[]; 
IsOwnFoto?: Boolean
PublishDate?: any;
QuestionAuthorId?: number
QuestionId?: number
QuestionText: string
Thema: string
TotalAnswers?: number
UserAllowsPublishment?:boolean
}
export enum ReferenceToModelClass
        {
            NotSet = 0, UserPlant = 1, UserTool = 2, Garden = 3, Plant = 4, Tool = 5, PlantTag, PlantCharacteristicCategory, DiaryEntry, FaqEntry, NewsEntry, Article, Todo, GeneralInternalComment, TaxonomicTree = 14, LexiconTerm = 15, UserDevice, ScanImage,AdminDevice,Device, Suggestion,EcoElement, Event, PlantDocQuestion,PlantDocAnswer, BioScan = 20, UserProfil
        }

export enum TodoCycleType {
    Once = 0, Weekly = 1, Monthly = 2, Yearly = 3, BiYearly = 4, Daily = 5, TriYearly = 6, QuadYearly = 7, PentYearly = 8
}

export enum NotifyType {
    Entry = 0, EntryHighlight = 1, Push = 2, Email = 3
}

export class NewsEntry extends BaseModel {
    Title: string;
    Text: string;
    EntryImages?: any[];
    Date?: any;
    ValidFrom?: any;
    ValidTo?: any;
    IsVisibleOnPage?: boolean;
}
export class InstaPost extends BaseModel {
    Text: any;
    MediaEntry: any;
    Media_Type:any;
    Date: any;
    Expanded?:boolean;
}
export class Article extends BaseModel {
    name: string;
    description: string;
    img?: string;
    Label?: string;
    ProductLink?: string;
    Thumbnail?: string;
    PhotoLink?: string;
    ArticleNr?: string;
    price?: number;
    isAvailable?: boolean;
    pricePercentagePayableWithPoints?: number;
    // articleReferences: any;
}

export class Garden extends BaseModel {
    CardinalDirection?: number;
    ShadowStrength?: number;
    Inside?: boolean;
    Temperature?: number;
    Light?: number;
    Name: string;
    Description?: string;
    GroundType?: number;
    PhType?: number;
    Wetness?: number;
    IsPrivate?: boolean;
    Images?: any[];
    Plants?: any;
    TodoList?: any[];
}

export class Plant extends BaseModel {
    NameLatin: string;
    NameGerman?: string;
    Description: string;
    Published?: boolean;
    Herkunft?: string;
    GardenCategoryId?: number;
    Synonym?: string;
    Familie?: string;
    PlantCharacteristics?: any;
    IsInUserGarden: boolean;
    Images?: any;
    Show?: boolean;
    Todos:any;
    TakeTodos?: boolean; // not sent to server
    PlantUrl: string;
}

// duplicate of Plant
export class PlantView extends BaseModel {
    NameLatin:string;
    NameGerman:string;
    Description:string;
    Image:Image;
}

export class Image {
    SrcAttr: string;
    TitleAttr: string;
}

export class UserPlant extends BaseModel {
    Name: string;
    Count?: number;
    Description: string;
    IsInPot?: boolean;
    InitialAgeInDays?: number;
    Images?: any[];
    Age?: string;
    Gardenin?: number;
    PlantId?: number;
    Diaries?: DiaryEntry[];
    UserListId?: number;
    Todos?: any[];
    showMenu?: boolean;
    Notes: string;
}

export class UserWarning {
    Id: number;
    Title: string;
    Body: string;
    ConditionDate: any;
    ConditionValue: number;
    AlertType: AlertType;
    NotificationType: NotificationType;
    Dismissed: boolean;
}

export enum NotificationType {
    Calendar = 0, CalendarHighlight = 1, Push = 2, Email = 3
}

export enum AlertType {
    Frost = 0, Storm = 1
}

export class UserSettings {
    UserId: string;
    ActiveStormAlert: boolean;
    ActiveFrostAlert: boolean;
    ActiveNewPlantAlert: boolean;
    AlertByEmail: boolean;
    AlertByPush: boolean;
    FrostDegreeBuffer: number;
}

export class InternalComment extends BaseModel {
    finished?: boolean;
    text: string;
    referenceId?: number;
    referenceType?: number;
    userId?: string;
}

export class FaqEntry extends BaseModel {
    Date?: any;
    QuestionText: string;
    answerText?: string;
    isOpen?: boolean;
    questionAuthorId?: string;
    answerAuthorId?: string;
    userAllowsPublishment?: boolean;
    adminAllowsPublishment?: boolean;
    referenceId?: number;
    referenceType?: number;
    Answers: any[];
}

export class DiaryEntry extends BaseModel {
    Title: string;
    Description: string;
    /**
     * holds bio scan data after decryption
     */
    DescriptionStr: string;
    Date: any;
    EntryOf: ObjectType;
    EntryObjectId: number;
    UserId: string;
    ImageId?: number;
    EntryImages?: any[]
}

export class ShopItem extends BaseModel{
    image: string;
    Name: string;
    ExpertTip: string;
    Price: number;
    prozentbezahlbar?: number;
    punkte?: number;
    esparnis?: number;
    restZahlung?: number;
    Description:string;
    ArticleImages?: any[];
    PricePercentagePayableWithPoints?: any;
    PricePartAfterPoints?: any;
    NormalPrice?: any;
}

export class ShopOrder extends BaseModel{
    PaymentConfirmed: boolean;
    OrderConfirmed: boolean;
    TCResponseCode: string;
    TransactionId: string;
    Status: OrderStatus;
    CustomerName: string;
    Street: string;
    HouseNr: string;
    City: string;
    Zip: string;
    Comment: string;
    PaidWith: string;
    OrderedArticles: any[];
}

export class NewsLetter extends BaseModel{
    LastName: string;
    FirstName: string;
    Email: string;
    Gender: number;
    Birthday: Date;
}
export class UserDevices extends BaseModel{
  Name: string;
    Date: Date;
    notifyForWind: boolean;
    notifyForFrost:boolean;
   isActive: boolean;
   Note: string;
   showMenu?: boolean;
   UserDevListId:number;
   AdminDevId?:number
}
export class UserDevList extends BaseModel{
    Name: string;
    Description: string;
    GardenId: number;
    UserDevices:UserDevices[];
}
export class ScanResult {
    PnResults: PnResults;
    GImages: any[];     // url
    GPlants: GPlantEntry[];
}

export class GPlantEntry {
    NameGerman: string;
    NameLatin: string;
    Links: any;
    Score: number;
    Description: string;
    Id: number;
}

export class PnResults {
    results: PnPlantEntry[];
    InDb: Plant[];
}

export class PnPlantEntry {
    species: PnSpecies;
    score: number;
    images: GSImage[];
}

export class GSImage {
    link: string; // original image
    image: any; // has thumbnailLink and contextLink
}

export class PnSpecies {
    commonNames: string[];
    scientificNameWithoutAuthor: string;
    family: any; // has scientificNameWithoutAuthor
    genus: any; // has scientificNameWithoutAuthor
}

export class PlantSearch {
    Plants: Plant[];
}
export class User{
    $id: any
    City: string
    Country: string
    FirstName: string
    HouseNr: any
    LastName: string
    PropertyModel: null
    RegistrationDate: any
    ResetTodo: any
    Street: string
    UserId: string
    UserName: string
    Zip: string
    Selected:boolean
}

export interface Dataset{
    rain:any;
    minTemp:any;
    maxTemp:any;
    day:any
}

export interface linesetMinTemp{
    minTemp:number;
    day:string
}

export interface linesetMaxTem{
    day:string
}

export class Group {
    Id: number;
    Name: string;
}

export enum OrderStatus {
    New = 0, Shipped = 1, Returned = 2, Completed = 3
}

export enum ObjectType {
    NotSet = 0, UserPlant = 1, UserTool = 2, Garden = 3, Plant = 4, Tool = 5, PlantTag, PlantCharacteristicCategory, DiaryEntry, FaqEntry, NewsEntry, Article, Todo, GeneralInternalComment, TaxonomicTree = 14, BioScan = 20
}

export class Alert {
    type: AlertType;
    message: string;
}

export class UserList extends BaseModel {
    Name: string;
    Description: string;
    GardenId: number;
    UserPlants: UserPlant[];
}
export class UserPlantTrigger extends BaseModel{
    PlantId:number;
    UserListId:number;
    GardenId:number;
}
export enum AlertType {
    Success,
    Error
}

export enum StatisticEventTypes {
    Login = 1, 
    Register = 2, 
    SuggestPlant = 3, 
    OrderConfirmed = 4,
    OrderClicked = 5,
    Pageview = 6,
    NewGarden = 7, 
    EmailNotify = 8,
    SubmitQuestion = 9, 
    SubmitAnswer = 10,
    AddToCart = 11,
    AddGardenPlant = 12,
    SaveBioScan = 13,
    GuidedTour = 14
}


export const WeatherIcons = {
    base: '/assets/weather-icons/',
    SunRain1: 'Sonne_mit_Regen_Stufe_1.svg',
    MoonRain1: 'Mond_mit_Regen_Stufe_1.svg',
    SunRain2: 'Sonne_mit_Regen_Stufe_2.svg',
    MoonRain2: 'Mond_mit_Regen_Stufe_2.svg',
    Sun: 'Sonne.svg',
    Moon: 'Mond.svg',
    Clouds: 'Wolken.svg',
    SunClouds: 'Wolke_mit_Sonne.svg',
    MoonClouds: 'Wolke_mit_Mond.svg',
    Rain1: 'Regen_Stufe_1.svg',
    Rain2: 'Regen_Stufe_2.svg',
    Fog: 'Nebel.svg',
    Storm1: 'Gewitter_mit_Regen_Stufe_1.svg',
    Storm2: 'Gewitter_mit_Regen_Stufe_2.svg',
    StormyWind1: 'Wind_mit_Gewitter_mit_Regen_Stufe_1.svg',
    RainWind1: 'Wind_mit_Regen_Stufe_1.svg',
    SunWind1: 'Wind_mit_Sonne_Stufe_1.svg',
    MoonWind1: 'Wind_mit_Mond_Stufe_1.svg',
    StormyWind2: 'Wind_mit_Gewitter_mit_Regen_Stufe_2.svg',
    RainWind2: 'Wind_mit_Regen_Stufe_2.svg',
    SunWind2: 'Wind_mit_Sonne_Stufe_2.svg',
    MoonWind2: 'Wind_mit_Mond_Stufe_2.svg',
    HailWind1: 'Wind_mit_Hagel_Stufe_1.svg',
    HailWind2: 'Wind_mit_Hagel_Stufe_2.svg',
    SunHail1: 'Sonne_mit_Hagel_Stufe_1.svg',
    MoonHail1: 'Mond_mit_Hagel_Stufe_1.svg',
    SunSnow1: 'Sonne_mit_Schnee_Stufe_1.svg',
    MoonSnow1: 'Mond_mit_Schnee_Stufe_1.svg',
    SnowWind1: 'Wind_mit_Schnee_Stufe_1.svg',
    SnowWind2: 'Wind_mit_Schnee_Stufe_2.svg'
};

export const TCErrCodes = {
    E001: 'Es wurden keine Daten übergeben.',
    E010: 'Die Authentifizierung ist fehlgeschlagen.',
    E101: 'Kein user-Parameter übergeben.',
    E102: 'Kein key-Parameter übergeben.',
    E103: 'Kein oder ungültiger amount-Parameter übergeben.',
    E104: 'Kein hash-Parameter übergeben.',
    E105: 'Hash-Parameter ist ungültig.',
    E106: 'Kein orderID-Parameter übergeben',
    E107: 'orderID ist nicht plausibel.',
    E108: 'Kein success-URL übergeben.',
    E109: 'Kein abort-URL übergeben.',
    E110: 'Ungültige Währung angegeben.',
    E111: 'Keine Bezahlmethode übergeben.',
    E112: 'Ungültige Bezahlmethode übergeben.',
    E113: 'Ungültige Sprache übergeben.'
};

export enum ColorSort {
    weiß = 0, gelb = 1, rot = 2, purpur = 3, lachsfarben = 4, gelbgrün = 5, hellblau = 6, 
    silbrigweiß = 7, orange = 8, dunkelrot = 9, violett = 10, rosa = 11, grün = 12, blau = 13, 
    creme = 14, apricot = 15, rotbraun = 16, braun = 17, pink = 18, blaugrün = 19, blauviolett = 20, schwarz = 21, 
    fliederfarben = 22
}

export enum PlaceHolder {
    Img = 'https://gardify.de/intern/Images/gardify_Pflanzenbild_Platzhalter.svg',
    Profile = './assets/images/gardify_v14_Pflanzendoc_Persona_Desktop.png'
}

export interface UploadedImageResponse {
    file: any;
    title: string;
    err: string;
    src: string | ArrayBuffer;
    dateCreated: any;
}
export class Ad {
    constructor(
        public adClient: string,
        public adSlot: number,
        public adFormat: string,
        public fullWidthResponsive: boolean,
        public isVertical: boolean,
        public data_ad_layout_key:string) {
            this.adClient = adClient;
            this.adSlot = adSlot;
            this.adFormat = adFormat || 'auto';
            this.fullWidthResponsive = fullWidthResponsive || true;
            this.isVertical = isVertical || false;
            this.data_ad_layout_key=data_ad_layout_key;
    }
}
export enum Position{
    Research=1,Graphic=2,Projectmanagement=3,Press=4,Accounting=5,Social_Media=6,Programming=7,ExecutiveDirector=8,SoilAnalysis=9
}
export const StatutIcons = {
    Research:'./assets/main-icons/Gardify_Team_Wissenschaftliche_Mitarbeiter.png',
    Graphic:'./assets/main-icons/Gardify_Team_Grafik.png',
    Projectmanagement:'./assets/main-icons/Gardify_Team_Projektmanagement.png',
    Press:'./assets/main-icons/Gardify_Team_Presse.png',
    Accounting:'./assets/main-icons/Gardify_Team_Buchhaltung.png',
    Social_Media:'./assets/main-icons/Gardify_Team_Social_Media.png',
    Programming:'./assets/main-icons/Gardify_Team_Programmierung.png',
    ExecutiveDirector:'./assets/main-icons/Gardify_Team_Geschäftsführer.png',
    SoilAnalysis:'./assets/main-icons/Gardify_Team_Maulwurf.png'
}
export const EmployeesImages={
    Employee1:'./assets/EmployeeImages/gardify_Markus_Philippen_x.png',
    Employee2:'./assets/EmployeeImages/gardify_Jasmin_Obholzer_x.png',
    Employee3:'./assets/EmployeeImages/gardify_Melanie_Bayo_x.png',
    Employee4:'./assets/EmployeeImages/gardify_Therese_Liouville_x.png',
    Employee5:'./assets/EmployeeImages/gardify_Janine_Sommer_x.png',
    Employee6:'./assets/EmployeeImages/gardify_Jana_Adam_x.png',
    Employee7:'./assets/EmployeeImages/gardify_Martha_Agnes_x.png',
    Employee8:'./assets/EmployeeImages/gardify_Vanessa_Schmolke_x.png',
    Employee9:'./assets/EmployeeImages/gardify_Karin_Becker_x.png',
    Employee10:'./assets/EmployeeImages/gardify_Justyna_Schwertner_x.png',
    Employee11:'./assets/EmployeeImages/gardify_Caroline_Mohr_x.png',
    Employee12:'./assets/EmployeeImages/gardify_Ellen_Schlüter_x.png',
    Employee13:'./assets/EmployeeImages/gardify_Maja_Sauvant_x.png',
    Employee14:'./assets/EmployeeImages/gardify_Philine_Anastasopoulos_x.png',
    Employee15:'./assets/EmployeeImages/gardify_Valerie_Mayer_x.png',
    Employee16:'./assets/EmployeeImages/gardify_Katerina_Stegemann_x.png',
    Employee17:'./assets/EmployeeImages/gardify_Annika_Steinacker_x.png',
    Employee18:'./assets/EmployeeImages/gardify_Karl_Joest_x.png',
    Employee19:'./assets/EmployeeImages/gardify_Karin_Kokoschka_x.png',
    Employee20:'./assets/EmployeeImages/gardify_Johanna_Hänichen_x.png',
    Employee21:'./assets/EmployeeImages/gardify_Andreas_Grewe_x.png',
    Employee22:'./assets/EmployeeImages/gardify_Ralf_Joest_x.png',
    Employee23:'./assets/EmployeeImages/gardify_Bob_the_digger_x.png',







}
export class Employee {
    EmployeeId:number;
    Title:string;
    Firstname:string;
    Lastname:string;
    PositionId:any;
    Position:string;
    FavouritPlantLatin:string;
    FavouritPlantGerman:string
    FavouriteCitation:string;
    profilBildSrc:string
}
export class UserActionsFrom{
    IsIos :boolean=false;
    IsAndroid :boolean=false;
    IsWebPage :boolean=true;
}
export class GardenAlbumFileToModuleViewModel
{
    Headline:string;
    Location :string;
    UserCreatedDate :any
    AlternativeDate :any
    ImgOwner :boolean
    Rating :any
    Source :string
    Tags:string
}


export interface Community {
    $id: string;
    QuestionId: number;
    QuestionAuthorId: string;
    QuestionAuthorName?: any;
    QuestionText: string;
    Thema: string;
    PublishDate: Date;
    Images: Image[];
    IsOnlyExpert: boolean;
} 

export interface CommunityQuestion {
    $id: string;
    communityPostViewModel: Post;
    communityAnswerList: CommunityAnswerList[];
    NewAnswer: any;
    Thema: string;
} 

export interface CommunityPost {
    $id: string;
    QuestionId: number;
    QuestionAuthorId: string;
    QuestionAuthorName?: any;
    QuestionText: string;
    Thema: string;
    PublishDate: Date;
    Images: Image[];
    IsOnlyExpert: boolean;
    AutorName?: any;
}

export interface CommunityAnswerList {
    $id: string;
    AnswerText: string;
    AutorName: string;
    Date: Date;
    AnswerImages?: any;
    ProfilUrl: Detail[];
    AnswerId: number;
    IsFromAdmin: boolean;
}

export interface CommunityWithAnswers {
    $id: string;
    AutorProfilUrl: Detail[];
    Post: CommunityPost;
    CommunityAnswerList: CommunityAnswerList[];
}

export interface Detail {
    $id: string;
    Id: number;
    Author: string;
    License?: any;
    FullTitle: string;
    FullDescription: string;
    TitleAttr: string;
    AltAttr: string;
    SrcAttr: string;
    SrcAttrBackend: string;
    Sort: number;
    InsertDate: Date;
    TakenDate: Date;
    Comments: string;
    Tags: string;
    Note?: any;
    Rating: number;
    IsOwnImage: boolean;
    IsMainImg: boolean;
    Albums: any[];
    highlighted: boolean;
}

