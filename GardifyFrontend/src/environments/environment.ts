// This file can be replaced during build by using the `fileReplacements` array.
// `ng build --prod` replaces `environment.ts` with `environment.prod.ts`.
// The list of file replacements can be found in `angular.json`.
var localBaseUrl = 'https://localhost:44328/';
var gardifyUrl = 'https://gardifybackend.sslbeta.de/';
export const environment = {
  production: false,
  VAPID_KEY: 'BDhYJhVQLL5aCJ0H0wO3vGq9DjKx_u8hOxEW3jiRlUyFaRTfE-gqNMhZ57LTJecHvtDEhbbrnRbPdOfa__4r1Wo',
  localBaseUrl: localBaseUrl,
  fcDailyURL: localBaseUrl + 'api/WeatherAPI/',
  forecastURL: localBaseUrl + 'api/WeatherAPI/',
  todosURL: localBaseUrl + 'api/todoesAPI/',
  threadsURL: localBaseUrl + 'api/forumapi/',
  newsURL: localBaseUrl + 'api/newsentriesapi/',
  gardenURL: localBaseUrl + 'api/gardenapi/',
  faqURL: localBaseUrl + 'api/faqentriesapi/',
  searchURL: localBaseUrl + 'api/plantsearchapi/',
  searchSiblingURL: localBaseUrl + 'api/plantsearchapi/findSiblingById/',
  scanURL: localBaseUrl + 'api/plantsearchimageapi',
  newsletterURL: localBaseUrl + 'api/newsletterapi',
  plantIdSearchURL:localBaseUrl + 'api/plantsearchapi/',
  articleURL: localBaseUrl + 'api/articlesapi/',
  shopCartURL: localBaseUrl + 'api/shopcartapi/',
  scanHistoryURL: localBaseUrl + 'api/plantsearchimageapi/history',
  groupURL: localBaseUrl + 'api/groupapi',
  familyURL: localBaseUrl + 'api/plantsearchapi/families',
  tagCatsURL: localBaseUrl + 'api/planttagsapi/cats',
  tagURL: localBaseUrl + 'api/planttagsapi/',
  userPlantsURL: localBaseUrl + 'api/userplantsapi/',
  pointsURL: localBaseUrl + 'api/pointsapi/',
  lexiconURL: localBaseUrl + 'api/lexiconapi/',
  diaryURL: localBaseUrl + 'api/diaryapi',
  devicesURL: localBaseUrl + 'api/DeviceAPI/',
  loginURL: localBaseUrl + 'api/AccountAPI/login',
  registerURL: localBaseUrl + 'api/AccountAPI/register',
  userURL: localBaseUrl + 'api/AccountAPI/',
  updateUserURL: localBaseUrl + 'api/AccountAPI/update/',
  deleteUserURL: localBaseUrl + 'api/AccountAPI/delete/',
  userListsURL: localBaseUrl + 'api/UserListAPI/',
  userDevicesListsURL: localBaseUrl + 'api/UserDevicesListAPI/',
  adminDevicesListURL: localBaseUrl + 'api/AdminDeviceAPI/',
  warningUrl: localBaseUrl + 'api/WarningAPI',
  plantDocURL:localBaseUrl + 'api/PlantDocAPI',
  communityURL:localBaseUrl + 'api/communityAPI',
  searchQueriesURL:localBaseUrl + 'api/SearchQueryAPI/',
  gardifyBaseUrl: 'https://gardifybackend.sslbeta.de/',
  gpBaseUrl: 'https://gardify.de/intern/',
  forgotUrl: localBaseUrl + 'api/AccountAPI/forgot',
  resendconfEmailUrl: localBaseUrl + 'api/AccountAPI/resendConfEmail',
  ecolistUrl: localBaseUrl + 'api/gardenapi/',
  ordersUrl: localBaseUrl + 'api/OrdersAPI/',
  eventsUrl: localBaseUrl + 'api/EventsAPI/',
  videosURL: localBaseUrl + 'api/VideosAPI/',
  statsURL: localBaseUrl + 'api/statsAPI/',
  citiesURL: localBaseUrl + 'api/citiesAPI/',
  plzURL: localBaseUrl + 'api/citiesAPI/verifyPLZ',
  subscriptionURL: localBaseUrl + 'api/SubscriptionAPI',
  imageApiUrl:localBaseUrl+'api/ImageAPI',
  adsense: {
    adClient: 'ca-pub-3562132666777902',
    show: true
 }
};

/*
 * For easier debugging in development mode, you can import the following file
 * to ignore zone related error stack frames such as `zone.run`, `zoneDelegate.invokeTask`.
 *
 * This import should be commented out in production mode because it will have a negative impact
 * on performance if an error is thrown.
 */
// import 'zone.js/dist/zone-error';  // Included with Angular CLI.
