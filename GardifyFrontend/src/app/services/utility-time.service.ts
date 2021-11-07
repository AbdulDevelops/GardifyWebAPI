export class UtilityTimeService {


    private intervals = [
        { label: 'Jahr', seconds: 31536000 },
        { label: 'Monat', seconds: 2592000 },
        { label: 'Tag', seconds: 86400 },
        { label: 'Stunde', seconds: 3600 },
        { label: 'Minute', seconds: 60 },
        { label: 'Sekunde', seconds: 1 }
      ];
      
    relativeTimeFromNow(date) {
        const seconds = Math.floor((Date.now() - date.getTime()) / 1000);
        const interval = this.intervals.find(i => i.seconds < seconds);
        const count = Math.floor(seconds / interval.seconds);
        const plural = interval.label.endsWith('e') ? 'n' : 'en'
        return `vor ${count} ${interval.label}${count !== 1 ? plural : ''}`;
      }
}